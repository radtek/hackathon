using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using XPInc.Hackathon.Core.Application.Configuration;

namespace XPInc.Hackathon.Core.Application.Services
{
    public class JwtAuthenticationService : IJwtAuthentication
    {
        private readonly string _issuerAudience;
        private readonly SigningCredentials _credentials;
        private readonly SymmetricSecurityKey _securityKey;

        public string Issuer { get; private set; }

        public IEnumerable<string> Audiences { get; private set; }

        // We only expect DI problems when using this overload (JwtOptions is validatable).
        // That's why there is no property validations.
        public JwtAuthenticationService([FromServices] IOptions<JwtOptions> options)
        {
            if (options == default)
            {
                throw new ArgumentException("Did not find instance from DI container.", nameof(options));
            }

            this._issuerAudience = options.Value.Issuer.Audience;
            this._securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Secret));
            this._credentials = new SigningCredentials(this._securityKey, SecurityAlgorithms.HmacSha256Signature);

            this.Issuer = options.Value.Issuer.Name;
            this.Audiences = options.Value.AcceptedAudiences;
        }

        protected JwtAuthenticationService(string secret, string issuer)
        {
            if (secret == default)
            {
                throw new ArgumentNullException(nameof(secret));
            }

            if (issuer == default)
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            if (secret.Length < 16)
            {
                throw new ArgumentException("Can not create token with a secret length less than 128 bits.", nameof(secret));
            }

            if (issuer.Length == 0)
            {
                throw new ArgumentException("Can not create token an empty issuer.", nameof(issuer));
            }

            this._securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            this._credentials = new SigningCredentials(this._securityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        protected JwtAuthenticationService(string secret, string issuer, string audience)
            : this(secret, issuer)
        {
            if (audience == default)
            {
                throw new ArgumentNullException(nameof(audience));
            }

            if (audience.Length == 0)
            {
                throw new ArgumentException("Can not create token an empty audience.", nameof(audience));
            }

            this._issuerAudience = audience;

            this.Issuer = issuer;
            this.Audiences = new[] { audience };
        }

        public static JwtAuthenticationService Create(string secret, string issuer)
            => new JwtAuthenticationService(secret, issuer);

        public static JwtAuthenticationService Create(string secret, string issuer, string audience)
            => new JwtAuthenticationService(secret, issuer, audience);

        public static TokenValidationParameters CreateValidationParameters(JwtOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var parameters = CreateValidationParameters();

            if (options.Issuer != default)
            {
                parameters.ValidateIssuer = true;
                parameters.ValidIssuer = options.Issuer?.Name;
            }

            if (options.AcceptedAudiences != default)
            {
                parameters.ValidateAudience = true;
                parameters.ValidAudiences = options.AcceptedAudiences;
            }

            return parameters;
        }

        public static TokenValidationParameters CreateValidationParameters()
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true
            };

            return parameters;
        }

        public string CreateToken(TimeSpan duration)
            => this.CreateToken(Enumerable.Empty<Claim>(), duration);

        public string CreateToken(IEnumerable<Claim> claims, TimeSpan duration)
        {
            if (claims == default)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            if (duration == default)
            {
                throw new ArgumentException("Could not create a JWT token without expiring information.", nameof(duration));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = now,
                Expires = now.Add(duration),
                SigningCredentials = this._credentials
            };

            if (claims.Any())
            {
                tokenDescriptor.Subject = new ClaimsIdentity(claims);
            }

            if (this.Issuer != default)
            {
                tokenDescriptor.Issuer = this.Issuer;
            }

            if (this.Audiences != default)
            {
                tokenDescriptor.Audience = this._issuerAudience;
            }

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public bool ValidateToken(string payload) => this.ValidateToken(payload, out SecurityToken token);

        public bool ValidateToken(string payload, out SecurityToken token)
        {
            bool isValid = this.ValidateToken(payload, out (ClaimsPrincipal principal, SecurityToken innerToken) result);

            token = result.innerToken;

            return isValid;
        }

        public bool ValidateToken(string payload, out (ClaimsPrincipal principal, SecurityToken token) result)
        {
            if (payload == default)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            result = default;

            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(payload))
            {
                return false;
            }

            var validationParameters = CreateValidationParameters();
            validationParameters.IssuerSigningKey = this._securityKey;

            if (this.Issuer != default)
            {
                validationParameters.ValidateIssuer = true;
                validationParameters.ValidIssuer = this.Issuer;
            }

            if (this.Audiences != default)
            {
                validationParameters.ValidateAudience = true;
                validationParameters.ValidAudiences = this.Audiences;
            }

            result.principal = tokenHandler.ValidateToken(payload, validationParameters, out result.token);

            return result.principal != default && result.token != default;
        }
    }
}
