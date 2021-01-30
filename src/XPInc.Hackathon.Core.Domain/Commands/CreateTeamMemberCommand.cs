using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Core.Domain.Commands
{
    public sealed class CreateTeamMemberCommand : IDomainCommand
    {
        public string Name { get; init; }

        public string Username { get; init; }

        public string Email { get; init; }

        public string Phone { get; init; }

        public TeamMember Manager { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}
