using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Network;
using XPInc.Hackathon.Core.Application.DependencyInjection;
using XPInc.Hackathon.Framework.Data.Contexts;
using XPInc.Hackathon.Framework.Serialization;
using XPInc.Hackathon.Infrastructure.Configuration;
using XPInc.Hackathon.Infrastructure.Data.Contexts;

namespace XPInc.Hackathon.Infrastructure.DependencyInjection
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                           IConfiguration configuration)
        {
            services
                .CreateOptions(configuration)
                .CreateDatabases(configuration)
                .CreateDependencies(configuration)
                .CreateConfiguration(configuration);

            services
                .AddApplication()
                .AddStreamingInfrastructure(configuration)
                .AddZabbixInfrastructure(configuration);

            return services;
        }

        private static IServiceCollection CreateOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<ApplicationOptions>()
                .Bind(configuration.GetSection(ApplicationOptions.SectionPath))
                .ValidateDataAnnotations();

            services
                .AddOptions<LogOptions>()
                .Bind(configuration.GetSection(LogOptions.SectionPath))
                .ValidateDataAnnotations();

            return services;
        }

        private static IServiceCollection CreateDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.SetupNoctifyCollections(configuration);

            // Setting up ADO databases
            services.TryAddScoped<IDataContextCollection>(factory =>
            {
                var dataContexts = new DataContextCollection();

                dataContexts.AddContext(factory.GetService<NoctifyCollectionsDataContext>());

                return dataContexts;
            });

            return services;
        }

        private static IServiceCollection SetupNoctifyCollections(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                var url = configuration["ConnectionStrings:NoctifyCollections"];
                var mongoUrl = new MongoUrl(url);

                var client = new MongoClient(mongoUrl);
                var database = client.GetDatabase(mongoUrl.DatabaseName);

                var collectionNames = string.Join(", ", database.ListCollectionNames().ToList());
                Log.Information("Successfuly connected to database. Collections sample: {collectionNames}.", collectionNames);

                BsonSerializer.RegisterSerializer(DateTimeSerializer.LocalInstance);

                var pack = new ConventionPack
                {
                    new IgnoreExtraElementsConvention(true)
                };

                ConventionRegistry.Register("Basic", pack, a => true);

                //Connection
                services.TryAddSingleton<IMongoClient>(client);
                services.TryAddSingleton(database);

                services.TryAddScoped<NoctifyCollectionsDataContext>();

                return services;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error ocurred.");
                throw;
            }
        }

#pragma warning disable S1172  // DI container injects IConfiguration on runtime.
        private static IServiceCollection CreateDependencies(this IServiceCollection services, IConfiguration configuration)
#pragma warning restore S1172  // DI container injects IConfiguration on runtime.
        {
            // Inverted control.
            // services.TryAddTransient<ICacheManager, CacheManager>();
            // services.TryAddTransient<IBinaryCacheManager, BinaryCacheManager>();
            // services.TryAddTransient<IAsyncBinarySerializer, BinarySerializer>();
            services.TryAddTransient<ICacheSerializerAsync<string>, CacheSerialization>();
            services.TryAddScoped<ICacheSerializerAsync<Stream>, CacheSerialization>();

            return services;
        }

#pragma warning disable S1172  // DI container injects IConfiguration on runtime.
        private static IServiceCollection CreateConfiguration(this IServiceCollection services, IConfiguration configuration)
#pragma warning restore S1172  // DI container injects IConfiguration on runtime.
        {
            const string Application = "Application";

            // Getting log configuration.
            using var serviceProvider = services.BuildServiceProvider();
            var applicationOptions = serviceProvider.GetRequiredService<IOptions<ApplicationOptions>>();
            var logOptions = serviceProvider.GetRequiredService<IOptions<LogOptions>>();

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty(Application, applicationOptions.Value.Name)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.UDPSink(logOptions.Value.Host.Host, logOptions.Value.Port.Value)
                .CreateLogger();

            return services;
        }
    }
}
