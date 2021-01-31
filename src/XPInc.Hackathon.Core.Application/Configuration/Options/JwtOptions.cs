using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using XPInc.Hackathon.Framework.Serialization;

namespace XPInc.Hackathon.Core.Application.Configuration
{
    /// <summary>Authentication settings.</summary>
    /// <remarks>By default, it assumes these settings are in hosts:api hierarchy.</remarks>
    [Serializable]
    public sealed class JwtOptions
    {
        public static string DefaultSectionPath { get; } = "Hosts:Api:Authentication";

        [Required(ErrorMessage = "A secret must be supplied.")]
        [MinLength(16, ErrorMessage = "A secret containing at least 128 bits must be supplied.")]
        public string Secret { get; set; }

        [Required(ErrorMessage = "At least one audience must be supplied. Usually, it's the application name or address.")]
        public IEnumerable<string> AcceptedAudiences { get; set; }

        public IssuerOptions Issuer { get; set; }

        [JsonConverter(typeof(NullableTimeSpanConverter))]
        [Required(ErrorMessage = "Token TTL must be supplied.")]
        public TimeSpan? TokenDuration { get; set; }

        public bool IsExpiringEnabled { get => this.TokenDuration.HasValue && (this.TokenDuration.Value != TimeSpan.Zero); }

        [Serializable]
        public sealed class IssuerOptions
        {
            public string Name { get; set; }

            public string Audience { get; set; }
        }
    }
}
