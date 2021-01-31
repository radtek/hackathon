using System;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Infrastructure.Configuration
{
    public class ProviderOptions
    {
        public static string ZabbixSectionPath { get; } = "Providers:Zabbix";

        [Required(ErrorMessage = "A URL must be specified.")]
        public Uri Url { get; set; }

        [Required(ErrorMessage = "A resource configuration must be specified.")]
        public ResourceOptions Resource { get; set; }

        public SettingOptions Setting { get; set; }

        public bool HasSettingsSection { get => this.Setting != default; }

        public class ResourceOptions
        {
            public Uri Uri { get; set; }
        }

        public class SettingOptions
        {
            [Range(10, int.MaxValue, ErrorMessage = "Simultaneous requests limit can not be less than 10.")]
            public int? SimultaneousRequestsLimit { get; set; }

            public bool HasSimultaneousRequestsLimit { get => this.SimultaneousRequestsLimit.HasValue && (this.SimultaneousRequestsLimit.Value != default); }

            public TimeSpan? CacheDuration { get; set; }

            public bool IsCachingEnabled { get => this.CacheDuration.HasValue && (this.CacheDuration.Value != TimeSpan.Zero); }

            public ResilienceOptions Resilience { get; set; }

            public class ResilienceOptions
            {
                public int? RetryCount { get; set; }

                public int? SleepSecondsPow { get; set; }

                public int? ExceptionsBeforeBreak { get; set; }

                public TimeSpan? DurationOfBreak { get; set; }
            }
        }
    }
}
