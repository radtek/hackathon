using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;
using System;
using System.Net;
using System.Net.Mime;

namespace XPInc.Hackathon.Hosts.Api
{
    public class Startup
    {
        public Startup([FromServices] IConfiguration configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // this.Configuration = (new XPCryptorRSA()).Decrypt(configuration);
            this.Configuration = configuration;
            this.ApplicationName = this.Configuration.GetSection("ApplicationName").Value;
        }

        public string ApplicationName { get; }

        public IConfiguration Configuration { get; }

        public void ConfigureServices([FromServices] IServiceCollection services)
        {
            // services
            //     .CreateOptions(this.Configuration)
            //     .CreateMvc()
            //     .CreateHealthChecks()
            //     .CreateOpenApiSpecificationMiddleware(this.ApplicationName)
            //     .CreateAuthentication()
            //     .CreateCacheMiddleware();

            // Project dependencies.
            // services.AddApplication();
            // Soft referencing (injecting configurations on runtime).
            // services.AddInfrastructure(this.Configuration);
            // Background hosted service.
            // services.AddBackgroundService(this.Configuration);

            // services.TryAddScoped<ExceptionMiddleware>();
            // services.TryAddScoped<IJwtAuthentication, JwtAuthenticationService>();
            // services.TryAddScoped<IDatabaseConnectionChecker, DatabaseConnectionChecker>();
            services.TryAddTransient<IMediator>(factory => factory.GetService<IMediator>());

            services.TryAddSingleton(factory =>
            {
                var configuration = new MapperConfiguration(configure =>
                { });

                return configuration.CreateMapper();
            });
        }

        public void Configure([FromServices] IApplicationBuilder app,
                              [FromServices] IWebHostEnvironment env,
                              [FromServices] ILogger<Startup> logger,
                            //   [FromServices] IOptions<ApiHealthOptions> apiOptions,
                            //   [FromServices] IOptions<HostsOptions> hostOptions,
                              [FromServices] IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation("API started with development configuration...");
            }
            else
            {
                app.UseHsts();

                logger.LogInformation("API started with production configuration...");
            }

            // Middleware injection order is important for performance.
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(configure =>
            {
                configure.AllowAnyOrigin();
                configure.AllowAnyMethod();
                configure.AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseResponseCompression();

            // this.ConfigureOpenApiUi(app, env, hostOptions, provider);

            // app.UseMiddleware<ExceptionMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                // TODO: refactor appsettings.json and frameworks health check structure.
                // const string ReadinessPath = "/ready";

                // endpoints.MapHealthChecks(ReadinessPath, CreateHealthCheckOptions("readiness", logger));
                // endpoints.MapHealthChecks(apiOptions.Value.Path.OriginalString, CreateHealthCheckOptions("liveness", logger));
                endpoints.MapControllers();
            });

            logger.LogInformation("API is now running.");

            // static HealthCheckOptions CreateHealthCheckOptions(string tag, ILogger<Startup> logger)
            // {
            //     return new HealthCheckOptions()
            //     {
            //         Predicate = (check) => check.Tags.Contains(tag),
            //         ResponseWriter = async (context, result) =>
            //         {
            //             const string VerboseKey = "verbose";

            //             bool enableVerboseOutput = false;

            //             if (context.Request.QueryString.HasValue)
            //             {
            //                 enableVerboseOutput = context.Request.Query.ContainsKey(VerboseKey);
            //             }

            //             context.Response.ContentType = MediaTypeNames.Application.Json;

            //             await context.Response.BodyWriter.WriteAsync(HealthReportSerializer.AsMemory(result, enableVerboseOutput), context.RequestAborted);

            //             LogLevel level;

            //             switch (result.Status)
            //             {
            //                 case HealthStatus.Degraded:
            //                     {
            //                         level = LogLevel.Warning;

            //                         break;
            //                     }
            //                 case HealthStatus.Unhealthy:
            //                     {
            //                         level = LogLevel.Error;

            //                         break;
            //                     }
            //                 case HealthStatus.Healthy:
            //                 default:
            //                     {
            //                         level = LogLevel.Information;

            //                         break;
            //                     }
            //             }

            //             logger.Log(level, "API health status is {Status}.", result.Status);
            //         }
            //     };
            // }
        }

        private void ConfigureOpenApiUi(IApplicationBuilder app,
                                        IWebHostEnvironment env,
                                        // IOptions<HostsOptions> hostOptions,
                                        IApiVersionDescriptionProvider provider)
        {
            app.UseOpenApi(configure =>
                        {
                            configure.PostProcess = (document, request) =>
                            {
                                // Requests to the server are always different from "localhost".
                                // if (!IPAddress.IsLoopback(request.HttpContext.Connection.RemoteIpAddress))
                                // {
                                //     document.BasePath = $"/{hostOptions.Value.Api.Swagger.VirtualDirectory}";
                                // }
                            };
                        });

            app.UseSwaggerUi3(configure =>
            {
                configure.SwaggerRoutes.Clear();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    configure.SwaggerRoutes.Add(new SwaggerUi3Route(description.GroupName, $"/swagger/{description.GroupName}/swagger.json"));
                }

                configure.TransformToExternalPath = (internalUiRoute, request) =>
                {
                    // Requests to the server are always different from "localhost".
                    // if (internalUiRoute.StartsWith("/") && !IPAddress.IsLoopback(request.HttpContext.Connection.RemoteIpAddress))
                    // {
                    //     return $"/{hostOptions.Value.Api.Swagger.VirtualDirectory}{internalUiRoute}";
                    // }

                    return internalUiRoute;
                };
            });

            if (env.IsDevelopment())
            {
                // Reason it's development only:
                // NSwag does not support multiple specifications with ReDoc. We'll need to implement ourselves some kind of merging pipeline.
                app.UseReDoc(configure =>
                {
                    configure.DocumentPath = $"/swagger/v1/swagger.json";
                    configure.Path = "/redoc";
                });
            }
        }
    }
}
