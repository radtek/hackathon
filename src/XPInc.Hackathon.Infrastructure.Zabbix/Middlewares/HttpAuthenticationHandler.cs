using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XPInc.Hackathon.Core.Application.Models;
using XPInc.Hackathon.Framework.Serialization;
using XPInc.Hackathon.Infrastructure.Configuration;

namespace XPInc.Hackathon.Infrastructure.Middlewares
{
    public class HttpAuthenticationHandler : DelegatingHandler
    {
        private static readonly MediaTypeHeaderValue _mediaType = new MediaTypeHeaderValue("application/json");

        private static HttpRequestMessage _authenticationRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = new StringContent($"{{\"clientSecret\":\"coe\"}}", Encoding.UTF8, _mediaType.MediaType)
        };

        private readonly ILogger<HttpAuthenticationHandler> _logger;
        private readonly ICacheSerializerAsync<Stream> _serializer;
        private readonly IDistributedCache _cache;
        private readonly SemaphoreSlim _semaphore;

        public HttpAuthenticationHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        { }

        public HttpAuthenticationHandler([FromServices] IOptionsMonitor<ZabbixOptions> options,
                                         [FromServices] ILogger<HttpAuthenticationHandler> logger,
                                         [FromServices] ICacheSerializerAsync<Stream> serializer,
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

            if (serializer == default)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (cache == default)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            this._logger = logger;
            this._serializer = serializer;
            this._cache = cache;
            this._semaphore = new SemaphoreSlim(options.CurrentValue.Setting.HasSimultaneousRequestsLimit
                                                    ? options.CurrentValue.Setting.SimultaneousRequestsLimit.Value
                                                    : int.MaxValue);

            _authenticationRequestMessage.RequestUri = new Uri($"{options.CurrentValue.Url.AbsoluteUri}/api/v1/token");
        }

        public static string Secret { get; } = "coe";

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
            string token = await this._cache.GetStringAsync(_authenticationRequestMessage.RequestUri.AbsoluteUri, cancellationToken)
                                                .ConfigureAwait(false);

            if (token == default)
            {
                await this._semaphore.WaitAsync()
                                        .ConfigureAwait(false);

                var authenticationResponse = await base.SendAsync(_authenticationRequestMessage, cancellationToken)
                                                            .ConfigureAwait(false);

                this._semaphore.Release();

                authenticationResponse.EnsureSuccessStatusCode();

                var content = await authenticationResponse.Content.ReadAsStreamAsync()
                                                                    .ConfigureAwait(false);

                var authentication = await this._serializer.DeserializeAsync<AuthenticationResponse>(content)
                                                                .ConfigureAwait(false);

                token = authentication.Token;

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = authentication.Expires
                };

                _ = this._cache.SetStringAsync(_authenticationRequestMessage.RequestUri.AbsoluteUri, token, cacheOptions, cancellationToken)
                                    .ContinueWith(x =>
                                    {
                                        if (x.IsFaulted)
                                        {
                                            this._logger.LogError(x.Exception, "Could not create cache entry {cacheKey}.", new[] { _authenticationRequestMessage.RequestUri.AbsoluteUri });
                                        }
                                    }, cancellationToken)
                                    .ConfigureAwait(false);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken)
                                .ConfigureAwait(false);
        }
    }
}
