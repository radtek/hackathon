using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using XPInc.Hackathon.Core.Application.Configuration;
using XPInc.Hackathon.Hosts.BackgroundService.Jobs;

namespace XPInc.Hackathon.Hosts.BackgroundService.DependencyInjection
{
    public static class BackgroundServiceModule
    {
        private const byte MaxNumbersOfThreads = 5;

        private static IServiceCollection CreateOptions(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuring and validating options.
            services
                .AddOptions<BackgroundServiceOptions>()
                .PostConfigure(configure =>
                {
                    configure.BackgroundService.Cache.ProfileMap = configure.BackgroundService.Cache.Profiles?.ToDictionary(x => x.Name, y => y.Duration.Value);
                })
                .Bind(configuration.GetSection(BackgroundServiceOptions.SectionPath))
                .ValidateDataAnnotations();

            return services;
        }

        public static IServiceCollection AddBackgroundService(this IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<BackgroundServiceOptions>>().Value;

            if (options.BackgroundService.HasJobs)
            {
                services
                    .AddQuartz(configure =>
                    {
                        const string ZabbixEventDigestJobKey = "zabbix-event-digesting";

                        var jobsMap = options.BackgroundService.Jobs.ToDictionary(x => x.Name, y => y.Cron);

                        configure.UseMicrosoftDependencyInjectionJobFactory(options =>
                        {
                            options.AllowDefaultConstructor = true;
                        });

                        // We only need a few threads.
                        configure.UseDefaultThreadPool(configure => configure.MaxConcurrency = MaxNumbersOfThreads);

                        configure.AddJob<EventCollectorJob>(job => job.WithIdentity(ZabbixEventDigestJobKey));

                        // Run it after 0 seconds and by Cron schedule.
                        configure
                            .AddTrigger(trigger =>
                            {
                                const float Seconds = 0;

                                trigger
                                    .ForJob(ZabbixEventDigestJobKey)
                                    .StartAt(DateTimeOffset.Now.AddSeconds(Seconds));
                            })
                            .AddTrigger(trigger =>
                            {
                                trigger
                                    .ForJob(ZabbixEventDigestJobKey)
                                    .WithCronSchedule(jobsMap[ZabbixEventDigestJobKey]);
                            });
                    })
                    .AddQuartzHostedService();

                services.AddTransient<EventCollectorJob>();
            }

            return services;
        }

        public static IServiceCollection AddBackgroundService(this IServiceCollection services,
                                                           IConfiguration configuration)
        {
            services
                .CreateOptions(configuration);

            BackgroundServiceModule.AddBackgroundService(services);

            return services;
        }
    }
}
