using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XPInc.Hackathon.Framework.Streaming;
using XPInc.Hackathon.Infrastructure.Streaming;

namespace XPInc.Hackathon.Infrastructure.DependencyInjection
{
    public static class StreamingInfrastructureModule
    {
        public static IServiceCollection AddStreamingInfrastructure(this IServiceCollection services,
                                                                 IConfiguration configuration)
        {
            services
                .CreateOptions(configuration)
                .CreateDependencies(configuration);

            return services;
        }

        private static IServiceCollection CreateOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<RedisStreamingOptions>()
                .Configure(configure =>
                {
                    string connectionString = configuration.GetConnectionString(RedisStreamingOptions.SectionPath);

                    configure.Endpoint = connectionString;
                    configure.Database = 1;
                })
                .ValidateDataAnnotations();

            return services;
        }

        private static IServiceCollection CreateDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

#pragma warning disable S1172  // DI container injects IConfiguration on runtime.
        private static IServiceCollection CreateDependencies(this IServiceCollection services, IConfiguration configuration)
#pragma warning restore S1172  // DI container injects IConfiguration on runtime.
        {
            // Inverted control.
            services.AddSingleton<IStreamingConfiguration, RedisStreamingOptions>();
            services.AddTransient<IStreamingBroker, RedisStreamingBroker>();

            return services;
        }
    }
}
