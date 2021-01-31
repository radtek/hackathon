using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Infrastructure.Configuration
{
    public class ApplicationOptions
    {
        public static string SectionPath { get; } = "Application";

        [Required(ErrorMessage = "An application name must be supplied.")]
        public string Name { get; set; }
    }
}
