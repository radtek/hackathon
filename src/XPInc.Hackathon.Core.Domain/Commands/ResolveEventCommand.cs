using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Core.Domain.Commands
{
    public sealed class ResolveEventCommand : IDomainCommand
    {
        public string Username { get; set; }

        public string Message { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}
