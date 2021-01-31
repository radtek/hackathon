using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Core.Application.Models
{
    public class AuthenticationRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Client secret must be informed.")]
        public string ClientSecret { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // E.g.: create a RegEx to validate in the future.
            yield return ValidationResult.Success;
        }
    }
}
