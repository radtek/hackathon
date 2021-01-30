using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Core.Domain.Commands
{
    public sealed class CreateIncidentCommand : IDomainCommand
    {
        public string Username { get; init; }

        public string EventId { get; init; }

        public Level Severity { get; init; }

        public string Trigger { get; init; }

        public string Host { get; init; }

        public string ProblemDescription { get; init; }

        public IEnumerable<string> Tags { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}
