using System;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using XPInc.Hackathon.Core.Application;
using XPInc.Hackathon.Framework.Extensions;
using XPInc.Hackathon.Framework.Streaming;
using XPInc.Hackathon.Infrastructure.Streaming;
using XPInc.Hackathon.Infrastructure.Zabbix;
using XPInc.Hackathon.Infrastructure.Zabbix.Models.Profiles;

namespace XPInc.Hackathon.Infrastructure
{
    public static class Module
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // configure options
            services.AddOptions<RedisStreamingConfiguration>()
                .Bind(configuration.GetSection("Infrastructure:Streaming"));

            services.AddOptions<ZabbixOptions>()
                .Bind(configuration.GetSection($"Infrastructure:Services:{ZabbixOptions.Section}"));

            // configure streaming broker
            services.TryAddSingleton<IStreamingConfiguration, RedisStreamingConfiguration>();
            services.TryAddTransient<IStreamingBroker, RedisStreamingBroker>();

            // configure CQRS stack
            services.AddMediatR(typeof(Module));

            // configure mapper and mapper profiles
            services.AddAutoMapper(typeof(Module)).RegisterProfiles();

            // configure application ports & adapters
            var zabbixOptions = services.GetOptions<ZabbixOptions>().Value; // get zabbix service options

            services
                .AddHttpClient<IEventService, ZabbixEventService>(cfg =>
                {
                    cfg.BaseAddress = new Uri(zabbixOptions.Endpoint);
                })
                // configure service resilience strategy
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(
                    zabbixOptions.RetryCount ?? 3, // max number of retries
                    retryAtt => TimeSpan.FromSeconds(Math.Pow(zabbixOptions.SleepSecondsPow ?? 2, retryAtt)) // sleep between each retry
                ))
                .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: zabbixOptions.ExceptionsBeforeBreak ?? 3, // exceptions allowed before circuit open
                    durationOfBreak: zabbixOptions.DurationOfBreak ?? TimeSpan.FromSeconds(30) // time that circuit stays opened
                ));

            return services;
        }

        private static void RegisterProfiles(this IServiceCollection services)
        {
            services.TryAddSingleton(provider =>
            {
                return new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new IncidentProfile());
                });
            });
        }
    }
}
