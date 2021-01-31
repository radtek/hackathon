using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using XPInc.Hackathon.Core.Application.Configuration;
using XPInc.Hackathon.Hosts.Api.Configuration;
using XPInc.Hackathon.Hosts.Api.Middlewares;
using XPInc.Hackathon.Hosts.BackgroundService.Middlewares;

namespace XPInc.Hackathon.Hosts.Api.DependencyInjection
{
    public static class ApiModule
    {
        private static string HttpCacheControlProfile { get; } = "HttpCacheControl";

        public static IServiceCollection CreateOptions(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuring and validating options.
            services
                .AddOptions<HostsOptions>()
                .PostConfigure(configure =>
                {
                    if (configure.Api.HasCacheSection && configure.Api.Cache.HasAnyProfile)
                    {
                        configure.Api.Cache.ProfileMap = configure.Api.Cache.Profiles?.ToDictionary(x => x.Name, y => y.Duration.Value);
                    }
                })
                .Bind(configuration.GetSection(HostsOptions.SectionPath))
                .ValidateDataAnnotations();

            services
                .AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.DefaultSectionPath))
                .ValidateDataAnnotations();

            services
                .AddOptions<ApiHealthOptions>()
                .Bind(configuration.GetSection(ApiHealthOptions.SectionPath))
                .ValidateDataAnnotations();

            return services;
        }

        public static IServiceCollection CreateMvc(this IServiceCollection services)
        {
            services
                .AddControllers(configure =>
                {
                    configure.OutputFormatters.Add(new StringOutputFormatter());
                })
                .AddMvcOptions(setup =>
                {
                    using var serviceProvider = services.BuildServiceProvider();
                    var options = serviceProvider.GetRequiredService<IOptions<HostsOptions>>().Value;

                    CacheProfile cacheProfile;

#pragma warning disable S3240  // Makes no sense using ternary operators here (make it more difficult to read).
                    if ((options.Api.Cache?.HasAnyProfile).GetValueOrDefault() && options.Api.Cache.ProfileMap.ContainsKey(HttpCacheControlProfile))
#pragma warning restore S3240  // Makes no sense using ternary operators here (make it more difficult to read).
                    {
                        cacheProfile = new CacheProfile
                        {
                            NoStore = false,
                            Duration = (int)options.Api.Cache.ProfileMap[HttpCacheControlProfile].TotalSeconds,
                            // In case we need it to be by user-agent basis, add VaryByHeader = "User-Agent".
                        };
                    }
                    else
                    {
                        cacheProfile = new CacheProfile
                        {
                            NoStore = true
                        };
                    }

                    setup.CacheProfiles.Add(HttpCacheControlProfile, cacheProfile);
                })
                .ConfigureApiBehaviorOptions(setup =>
                {
                    setup.SuppressInferBindingSourcesForParameters = true;

                    setup.ClientErrorMapping[StatusCodes.Status400BadRequest].Link = "https://httpstatuses.com/400";
                    setup.ClientErrorMapping[StatusCodes.Status401Unauthorized].Link = "https://httpstatuses.com/401";
                    setup.ClientErrorMapping[StatusCodes.Status403Forbidden].Link = "https://httpstatuses.com/403";
                    setup.ClientErrorMapping[StatusCodes.Status404NotFound].Link = "https://httpstatuses.com/404";
                    setup.ClientErrorMapping[StatusCodes.Status406NotAcceptable].Link = "https://httpstatuses.com/406";
                    setup.ClientErrorMapping[StatusCodes.Status500InternalServerError].Link = "https://httpstatuses.com/500";
                })
                .AddJsonOptions(configure =>
                {
                    configure.JsonSerializerOptions.IgnoreNullValues = true;
                    configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                });

            return services;
        }

        public static IServiceCollection CreateHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddCheck<ReadinessProbe>("zabbixEventDigestion", tags: new[] { "readiness", "hosted service" })
                .AddCheck<MemoryHealthCheck>("memoryUsage", tags: new[] { "memory", "liveness" });

            services.AddSingleton<ReadinessProbe>();

            return services;
        }

        public static IServiceCollection CreateOpenApiSpecificationMiddleware(this IServiceCollection services, string applicationName)
        {
            services
                .AddApiVersioning(setup =>
                {
                    setup.ApiVersionReader = new UrlSegmentApiVersionReader();
                    setup.DefaultApiVersion = new ApiVersion(1, 0);
                    setup.AssumeDefaultVersionWhenUnspecified = true;
                    setup.ReportApiVersions = true;
                })
                .AddVersionedApiExplorer(setup =>
                {
                    setup.GroupNameFormat = "'v'V";
                    setup.SubstituteApiVersionInUrl = true;
                })
                .AddOpenApiDocument(configure => OpenApiSetup.Create(configure, applicationName, "v1"))
                .AddOpenApiDocument(configure => OpenApiSetup.Create(configure, applicationName, "v2", hasAuthenticationScheme: true));

            return services;
        }

        public static IServiceCollection CreateAuthentication(this IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            var authenticationOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    var tokenParameters = new TokenValidationParameters
                    {
                        // Obtain from a key vault later.
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationOptions.Secret)),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true
                    };

                    bool hasIssuer = authenticationOptions.Issuer != default;
                    bool hasAudiences = authenticationOptions.AcceptedAudiences != default;

                    if (hasIssuer)
                    {
                        tokenParameters.ValidateIssuer = hasIssuer;
                        tokenParameters.ValidIssuer = authenticationOptions.Issuer.Name;
                    }

                    if (hasAudiences)
                    {
                        tokenParameters.ValidateAudience = hasAudiences;
                        tokenParameters.ValidAudiences = authenticationOptions.AcceptedAudiences;
                    }

                    options.TokenValidationParameters = tokenParameters;
                });

            return services;
        }

        public static IServiceCollection CreateCacheMiddleware(this IServiceCollection services)
        {
            services
                .AddResponseCaching()
                .AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                });

            return services;
        }
    }
}
