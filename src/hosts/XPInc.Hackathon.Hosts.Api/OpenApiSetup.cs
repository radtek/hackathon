using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using System;

namespace XPInc.Hackathon.Hosts.Api
{
    public static class OpenApiSetup
    {
        public static AspNetCoreOpenApiDocumentGeneratorSettings Create(AspNetCoreOpenApiDocumentGeneratorSettings configure,
                                                                        string applicationName,
                                                                        string version)
            => Create(configure, applicationName, version, false);

        public static AspNetCoreOpenApiDocumentGeneratorSettings Create(AspNetCoreOpenApiDocumentGeneratorSettings configure,
                                                                        string applicationName,
                                                                        string version,
                                                                        bool hasAuthenticationScheme)
        {
            if (configure == default)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            if (applicationName == default)
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            if (version == default)
            {
                throw new ArgumentNullException(nameof(version));
            }

            configure.DocumentName = version;
            configure.ApiGroupNames = new[] { version };

            configure.Title = applicationName;
            configure.GenerateExamples = true;

            if (hasAuthenticationScheme)
            {
                configure.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT",
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the textbox: Bearer {your JWT token}."
                    })
                );

                configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            }

            configure.PostProcess = configure =>
            {
                configure.Info.Version = version;
                configure.Info.Title = "NOCify API";
                configure.Info.Description = "API responsible to offer event monitoring from Zabbix services.";
                configure.Info.TermsOfService = "https://www.xpi.com.br/assets/documents/politica-seguranca-cibernetica-06-19.pdf";

                configure.Info.Contact = new OpenApiContact
                {
                    Name = "Squad COE",
                    Email = "squad-coe@xpi.com.br",
                    Url = "https://xpi.com.br"
                };

                configure.Info.License = new OpenApiLicense
                {
                    Name = "Copyright © XP Inc."
                };
            };

            return configure;
        }
    }
}
