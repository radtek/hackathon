using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XPInc.Hackathon.Framework.Streaming;
using XPInc.Hackathon.Infrastructure.Streaming;

namespace XPInc.Hackathon.Infrastructure
{
    public static class Module
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<RedisStreamingConfiguration>()
                .Bind(configuration.GetSection("Infrastructure:Streaming"));

            // configure streaming broker
            services.AddSingleton<IStreamingConfiguration, RedisStreamingConfiguration>();
            services.AddTransient<IStreamingBroker, RedisStreamingBroker>();

            // configure CQRS stack
            services.AddMediatR(typeof(Module));

            return services;
        }
    }
}
