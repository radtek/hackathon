using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using XPInc.Hackathon.Core.Application.Configuration;
using XPInc.Hackathon.Hosts.BackgroundService.Middlewares;

namespace XPInc.Hackathon.Hosts.BackgroundService.Jobs
{
    public class ZabbixEventDigestJob : IJob
    {
        private const string ZabbixEventDigestProfileKey = "ZabbixEvents";

        private readonly ILogger<ZabbixEventDigestJob> _logger;
        private readonly BackgroundServiceOptions _options;
        private readonly ReadinessProbe _probe;

        public ZabbixEventDigestJob([FromServices] ILogger<ZabbixEventDigestJob> logger,
                                    [FromServices] IOptions<BackgroundServiceOptions> options,
                                    [FromServices] ReadinessProbe probe)
        {
            if (logger == default)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (probe == default)
            {
                throw new ArgumentNullException(nameof(probe));
            }

            this._logger = logger;
            this._options = options.Value;
            this._probe = probe;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            this._logger.LogInformation("Hosted service 'AssetsCacheJob' started.");

            var duration = this._options.BackgroundService.HasCacheSection
                                    ? this._options.BackgroundService.Cache.ProfileMap[ZabbixEventDigestProfileKey]
                                    : throw new InvalidOperationException($"Could not execute '{nameof(ZabbixEventDigestJob)}' background service.");

            this._logger.LogInformation("Hosted service 'AssetsCacheJob' is fetching new data...");

            // TODO: logic here.
            await Task.Delay(1);

            // Tell any /ready listeners that our job has completed and the application can how handles requests.
            this._probe.SignalTaskCompleted();

            this._logger.LogInformation("Hosted service 'AssetsCacheJob' data fetching complete.");
            this._logger.LogInformation("Hosted service 'AssetsCacheJob' finished (Cache options: {duration}).", new { duration });
        }
    }
}
