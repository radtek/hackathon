using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace XPInc.Hackathon.Hosts.Api.Configuration
{
    public class HostsOptions
    {
        public static string SectionPath { get; } = "Hosts";

        [Required(ErrorMessage = "A host configuration must be specified.")]
        public ApiOptions Api { get; set; }

        public bool HasApiSection { get => this.Api != default; }

        public class ApiOptions
        {
            public CacheOptions Cache { get; set; }

            public bool HasCacheSection { get => this.Cache != default; }

            [Required(ErrorMessage = "An application authentication section must be supplied.")]
            public AuthenticationOptions Authentication { get; set; }

            [Required(ErrorMessage = "Swagger section must be supplied.")]
            public SwaggerOptions Swagger { get; set; }

            public class CacheOptions
            {
                public IEnumerable<CacheProfileOptions> Profiles { get; set; }

                public bool HasAnyProfile { get => (this.Profiles?.Any()).GetValueOrDefault(); }

                public IDictionary<string, TimeSpan> ProfileMap { get; set; }

                public class CacheProfileOptions
                {
                    [Required(ErrorMessage = "Cache profile name can not be empty.")]
                    public string Name { get; set; }

                    [Required(ErrorMessage = "A cache profile duration must be provided.")]
                    public TimeSpan? Duration { get; set; }
                }
            }

            public class AuthenticationOptions
            {
                [Required(ErrorMessage = "Token TTL must be supplied.")]
                public TimeSpan? TokenDuration { get; set; }

                public bool IsExpiringEnabled { get => this.TokenDuration.HasValue && (this.TokenDuration.Value != TimeSpan.Zero); }
            }

            public class SwaggerOptions
            {
                [Required(ErrorMessage = "A virtual directory must be informed.")]
                public string VirtualDirectory { get; set; }
            }
        }
    }
}
