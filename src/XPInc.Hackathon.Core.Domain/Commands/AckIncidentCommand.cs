using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Core.Domain.Commands
{
    public sealed class AckIncidentCommand : IDomainCommand
    {
        public string Username { get; init; }

        public string Message => "Mensagem lida.";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}