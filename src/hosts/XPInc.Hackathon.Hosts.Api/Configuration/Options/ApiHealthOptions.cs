using System;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Hosts.Api.Configuration
{
    public sealed class ApiHealthOptions
    {
        public static string SectionPath { get; } = "HealthCheck";

        [Required(ErrorMessage = "An endpoint must be provided.")]
        public Uri Path { get; set; }

        [Required(ErrorMessage = "An endpoint must be provided.")]
        public DatabaseHealthOptions Database { get; set; }

        [Required(ErrorMessage = "An endpoint must be provided.")]
        public MemoryHealthOptions Memory { get; set; }

        public sealed class DatabaseHealthOptions
        {
            [Required(ErrorMessage = "A provider must be specified.")]
            public string Provider { get; set; }

            [Required(ErrorMessage = "A threshold must be specified.")]
            [Range(1, byte.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
            public byte? Threshold { get; set; }
        }

        public sealed class MemoryHealthOptions
        {
            [Required(ErrorMessage = "A threshold must be specified.")]
            [Range(1, long.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
            public long? Threshold { get; set; }
        }
    }
}
