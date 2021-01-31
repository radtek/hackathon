using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace XPInc.Hackathon.Core.Application.Models
{
    // Token exchange as https://tools.ietf.org/html/rfc8693#section-2.3.
    public class AuthenticationResponse
    {
        public AuthenticationResponse(string token, TimeSpan expiration)
        {
            if (token == default)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token == string.Empty)
            {
                throw new ArgumentException("Token can not be empty.", nameof(token));
            }

            if (expiration == default)
            {
                throw new ArgumentException("Expiration can not be zero.", nameof(expiration));
            }

            this.Token = token;
            this.ExpiresIn = expiration.TotalSeconds;
        }

        public AuthenticationResponse(string token, TimeSpan expiration, IEnumerable<Claim> claims)
            : this(token, expiration)
        {
            if (claims == default)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            if (claims.Any())
            {
                this.Claims = claims;
            }
        }

        [JsonPropertyName("accessToken")]
        public string Token { get; }

        [JsonPropertyName("expiresIn")]
        public double ExpiresIn { get; }

        [JsonPropertyName("issuedTokenType")]
        public string IssuedTokenType { get; } = RfcTokenIdentifier;

        [JsonIgnore]
        [JsonPropertyName("claims")]
        public IEnumerable<Claim> Claims { get; }

        [JsonIgnore]
        public TimeSpan Expires { get => this.ExpiresIn != default ? TimeSpan.FromSeconds(this.ExpiresIn) : default; }

        private static string RfcTokenIdentifier { get; } = "urn:ietf:params:oauth:token-type:jwt";
    }
}
