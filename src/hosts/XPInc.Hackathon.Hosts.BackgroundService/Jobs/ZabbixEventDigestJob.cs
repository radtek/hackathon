using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using XPInc.Hackathon.Core.Application.Configuration;
using XPInc.Hackathon.Core.Application.Services;
using XPInc.Hackathon.Hosts.BackgroundService.Middlewares;

namespace XPInc.Hackathon.Hosts.BackgroundService.Jobs
{
    public class ZabbixEventDigestJob : IJob
    {
        private const string ZabbixEventDigestProfileKey = "ZabbixEvents";

        private readonly ILogger<ZabbixEventDigestJob> _logger;
        private readonly IEventService _eventService;
        private readonly BackgroundServiceOptions _options;
        private readonly IMediator _mediator;
        private readonly ReadinessProbe _probe;

        public ZabbixEventDigestJob([FromServices] ILogger<ZabbixEventDigestJob> logger,
                                    [FromServices] IOptions<BackgroundServiceOptions> options,
                                    [FromServices] IEventService eventService,
                                    [FromServices] IMediator mediator,
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

            if (eventService == default)
            {
                throw new ArgumentNullException(nameof(eventService));
            }

            if (mediator == default)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            if (probe == default)
            {
                throw new ArgumentNullException(nameof(probe));
            }

            this._logger = logger;
            this._eventService = eventService;
            this._options = options.Value;
            this._mediator = mediator;
            this._probe = probe;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            this._logger.LogInformation("Hosted service 'ZabbixEventDigestJob' started.");

            var duration = this._options.BackgroundService.HasCacheSection
                                    ? this._options.BackgroundService.Cache.ProfileMap[ZabbixEventDigestProfileKey]
                                    : throw new InvalidOperationException($"Could not execute '{nameof(ZabbixEventDigestJob)}' background service.");

            this._logger.LogInformation("Hosted service 'ZabbixEventDigestJob' is getting all teams from Zabbix...");

            // Getting teams from Zabbix
            var events = await this._eventService.GetEventsAsync(context.CancellationToken);

            // foreach (var item in events)
            // {
            //     var command = new CreateEventCommand(item.)
            //     {
            //         EventId = item.EventId,
            //         Host = item.Host,

            //     };

            //     var @event = Event.Create(command);
            //     var result = this._mediator.Send(new EventCreated(@event));
            // };

            // Tell any /ready listeners that our job has completed and the application can how handles requests.
            this._probe.SignalTaskCompleted();

            this._logger.LogInformation("Hosted service 'ZabbixEventDigestJob' data fetching complete.");
            this._logger.LogInformation("Hosted service 'ZabbixEventDigestJob' finished (Cache options: {duration}).", new { duration });
        }
    }
}
