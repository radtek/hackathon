using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using XPInc.Hackathon.Core.Domain.Strategies;

namespace XPInc.Hackathon.Core.Domain.Commands
{
    public sealed class CreateEventCommand : IDomainCommand
    {
        public string Username { get; set; }

        public string EventId { get; set; }

        public EventLevel Severity { get; set; }

        public string Trigger { get; set; }

        public string Host { get; set; }

        public string ProblemDescription { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<Team> Teams { get; set; }

        public CreateEventCommand()
        { }

        public CreateEventCommand(int severity) => Severity = severity switch
        {
            1 => new DisasterLevel(),
            2 => new HighLevel(),
            3 => new AverageLevel(),
            4 => new WarningLevel(),
            5 => new InformationLevel(),
            _ => new UnknowLevel(),
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}
