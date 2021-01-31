using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace XPInc.Hackathon.Core.Application.Services
{
    public interface IJwtAuthentication
    {
        string Issuer { get; }

        IEnumerable<string> Audiences { get; }

        string CreateToken(TimeSpan duration);

        string CreateToken(IEnumerable<Claim> claims, TimeSpan duration);

        bool ValidateToken(string payload);

        bool ValidateToken(string payload, out SecurityToken token);

        bool ValidateToken(string payload, out (ClaimsPrincipal principal, SecurityToken token) result);
    }
}
