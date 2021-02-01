using System;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Polly;
using RestSharp;
using XPInc.Hackathon.Infrastructure.Configuration;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Services;

namespace XPInc.Hackathon.Infrastructure.DependencyInjection
{
    public static class ZabbixInfrastructureModule
    {
        public static string StreamingDatabasePath { get; } = "Streaming";

        public static IServiceCollection AddZabbixInfrastructure(this IServiceCollection services,
                                                                 IConfiguration configuration)
        {
            services
                .CreateOptions(configuration)
                .CreateHttpClientsAndPolicies(configuration)
                .CreateProfiles()
                .CreateDependencies(configuration);

            return services;
        }

        private static IServiceCollection CreateOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<ZabbixOptions>()
                .Bind(configuration.GetSection(ZabbixOptions.ZabbixSectionPath))
                .ValidateDataAnnotations();

            return services;
        }

#pragma warning disable S1172 // DI container injects IConfiguration on runtime.
        private static IServiceCollection CreateHttpClientsAndPolicies(this IServiceCollection services, IConfiguration configuration)
#pragma warning restore S1172 // DI container injects IConfiguration on runtime.
        {
            // Http resquest configuration
            using var serviceProvider = services.BuildServiceProvider();

            var zabbixOptions = serviceProvider.GetRequiredService<IOptions<ZabbixOptions>>();

            var zabbixHttpBuilder = services
                .AddHttpClient<IRestClient, RestClient>((service, configure) =>
                {
                    configure.BaseAddress = zabbixOptions.Value.Url;
                })
                .AddTransientHttpErrorPolicy(builder =>
                {
                    var jitterer = new Random();

                    return builder.WaitAndRetryAsync(
                        zabbixOptions.Value.Setting.Resilience.RetryCount ?? 3,
                        attemp => TimeSpan.FromSeconds(Math.Pow(zabbixOptions.Value.Setting.Resilience.SleepSecondsPow ?? 2, attemp)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                    );
                })
                .AddTransientHttpErrorPolicy(builder =>
                    builder.CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: zabbixOptions.Value.Setting.Resilience.ExceptionsBeforeBreak ?? 3,
                        durationOfBreak: zabbixOptions.Value.Setting.Resilience.DurationOfBreak ?? TimeSpan.FromSeconds(30)
                    )
                );

            return services;
        }

        private static IServiceCollection CreateProfiles(this IServiceCollection services)
        {
            //services.TryAddSingleton(provider =>
            //{
            //    return new MapperConfiguration(configure =>
            //    {
            //        configure.AddProfile(new IncidentProfile());
            //    });
            //});

            return services;
        }

#pragma warning disable S1172  // DI container injects IConfiguration on runtime.
        private static IServiceCollection CreateDependencies(this IServiceCollection services, IConfiguration configuration)
#pragma warning restore S1172  // DI container injects IConfiguration on runtime.
        {
            // Inverted control.
            services.TryAddTransient<IZabbixService, ZabbixService>();

            return services;
        }
    }
}
