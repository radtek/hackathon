using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Core.Domain.Commands
{
    public sealed class CreateActionCommand : IDomainCommand
    {
        public string Username { get; set; }

        public ActionType Type { get; set; }

        public string Message { get; set; }

        public ActionStatus Status { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}
