using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Core.Domain.Commands
{
    public sealed class CreateIncidentCommand : IDomainCommand
    {
        public string Username { get; set; }

        public string EventId { get; set; }

        public Level Severity { get; set; }

        public string Trigger { get; set; }

        public string Host { get; set; }

        public string ProblemDescription { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}
