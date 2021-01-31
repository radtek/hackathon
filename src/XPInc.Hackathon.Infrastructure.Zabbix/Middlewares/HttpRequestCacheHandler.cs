using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Application.Services;
using XPInc.Hackathon.Infrastructure.Configuration;

namespace XPInc.Hackathon.Infrastructure.Middlewares
{
    public class HttpRequestCacheHandler<TOptions> : DelegatingHandler
        where TOptions : ProviderOptions
    {
        private readonly ILogger<HttpRequestCacheHandler<TOptions>> _logger;
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheOptions;
        private readonly MediaTypeHeaderValue _mediaType = new MediaTypeHeaderValue("application/json");
        private readonly SemaphoreSlim _semaphore;

        public HttpRequestCacheHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        { }

        public HttpRequestCacheHandler([FromServices] IOptionsMonitor<TOptions> options,
                                       [FromServices] ILogger<HttpRequestCacheHandler<TOptions>> logger,
                                       [FromServices] IDistributedCache cache)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (logger == default)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (cache == default)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            this._logger = logger;
            this._cache = cache;

            this._semaphore = new SemaphoreSlim(options.CurrentValue.Setting.HasSimultaneousRequestsLimit
                                                    ? options.CurrentValue.Setting.SimultaneousRequestsLimit.Value
                                                    : int.MaxValue);

            if (options.CurrentValue.Setting.IsCachingEnabled)
            {
                this._cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = options.CurrentValue.Setting.CacheDuration
                };
            }
        }

        public static string CacheKey { get; } = "positionapi-http-request-cache";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == default)
            {
                throw new ArgumentNullException(nameof(request));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return this.SendInternalAsync(request, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendInternalAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string cacheKey = this.CreateCacheKey(request);
            string cacheResult = await this._cache.GetStringAsync(cacheKey, cancellationToken)
                                                    .ConfigureAwait(false);

            if (cacheResult == default)
            {
                await this._semaphore.WaitAsync()
                                        .ConfigureAwait(false);

                var response = await base.SendAsync(request, cancellationToken)
                                            .ConfigureAwait(false);

                this._semaphore.Release();

                if (response.IsSuccessStatusCode &&
                        response.Content.Headers.ContentType.MediaType == _mediaType.MediaType)
                {
                    var json = await response.Content.ReadAsStringAsync()
                                                        .ConfigureAwait(false);

                    _ = this._cache.SetStringAsync(cacheKey, json, this._cacheOptions, cancellationToken)
                                        .ContinueWith(x =>
                                        {
                                            if (x.IsFaulted)
                                            {
                                                this._logger.LogError(x.Exception, "Could not create cache entry {cacheKey}.", new[] { cacheKey });
                                            }
                                        }, cancellationToken)
                                        .ConfigureAwait(false);
                }

                return response;
            }
            else
            {
                this._logger.LogInformation("Response fetched from memory cache; origin: {path}.", request.RequestUri);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(cacheResult)
            };
        }

        private string CreateCacheKey(HttpRequestMessage request)
        {
            var builder = new StringBuilder(CacheKey);
            builder.Append("-");
            builder.Append(request.Method.Method);
            builder.Append("-");
            builder.Append(request.RequestUri.PathAndQuery);

            return NonCryptographicHash.Create(builder.ToString());
        }
    }
}
