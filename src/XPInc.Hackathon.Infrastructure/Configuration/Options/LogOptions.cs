using System;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Infrastructure.Configuration
{
    public class LogOptions
    {
        public static string SectionPath { get; } = "Logging:Serilog";

        [Required(ErrorMessage = "A URL must be specified.")]
        public Uri Host { get; set; }

        [Required(ErrorMessage = "Port number must be specified.")]
        [Range(1, ushort.MaxValue, ErrorMessage = "Port number out of valid range.")]
        public int? Port { get; set; }
    }
}
