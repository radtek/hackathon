using System;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XPInc.Hackathon.Core.Application;
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
            services.AddOptions<RedisStreamingConfiguration>()
                .Bind(configuration.GetSection("Infrastructure:Streaming"));

            // configure streaming broker
            services.TryAddSingleton<IStreamingConfiguration, RedisStreamingConfiguration>();
            services.TryAddTransient<IStreamingBroker, RedisStreamingBroker>();

            // configure CQRS stack
            services.AddMediatR(typeof(Module));

            // configure mapper and mapper profiles
            services.AddAutoMapper(typeof(Module)).RegisterProfiles();

            // configure application ports & adapters
            services.AddHttpClient<IIncidentService, ZabbixIncidentService>(cfg =>
            {
                cfg.BaseAddress = new Uri("");
            });

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
