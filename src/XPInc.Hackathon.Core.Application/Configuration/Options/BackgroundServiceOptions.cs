using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace XPInc.Hackathon.Core.Application.Configuration
{
    public class BackgroundServiceOptions
    {
        public static string SectionPath { get; } = "Hosts";

        public ServiceOptions BackgroundService { get; set; }

        public bool HasBackgroundServiceSection { get => this.BackgroundService != default; }

        public class ServiceOptions
        {
            public IEnumerable<JobOptions> Jobs { get; set; }

            public bool HasJobs { get => (this.Jobs?.Any()).GetValueOrDefault(); }

            public CacheOptions Cache { get; set; }

            public bool HasCacheSection { get => this.Cache != default; }

            public class JobOptions
            {
                [Required(ErrorMessage = "Job name is required.")]
                public string Name { get; set; }

                [Required(ErrorMessage = "CRON expression is required.")]
                public string Cron { get; set; }
            }

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
        }
    }
}
