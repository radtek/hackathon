using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace XPInc.Hackathon.Core.Application.DependencyInjection
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ApplicationModule));
            services.AddAutoMapper(typeof(ApplicationModule));

            services.TryAddTransient<IMediator>(factory => factory.GetService<IMediator>());

            return services;
        }
    }
}
