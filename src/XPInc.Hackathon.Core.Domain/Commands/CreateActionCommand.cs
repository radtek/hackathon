using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Core.Domain.Commands
{
    public sealed class CreateActionCommand : IDomainCommand
    {
        public string Username { get; init; }

        public ActionType Type { get; init; }

        public string Message { get; init; }

        public ActionStatus Status { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}