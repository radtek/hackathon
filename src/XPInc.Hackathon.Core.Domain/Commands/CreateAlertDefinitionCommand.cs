﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Core.Domain.Commands
{
    public sealed class CreateAlertDefinitionCommand : IDomainCommand
    {
        public Incident Incident { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}
