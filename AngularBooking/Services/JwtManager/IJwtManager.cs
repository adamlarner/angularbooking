using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AngularBooking.Services.JwtManager
{
    public interface IJwtManager
    {
        Task<JwtSecurityToken> GenerateJwtAsync(string username, List<Claim> additionalClaims);
        Task<string> GenerateJwtStringAsync(string username, List<Claim> additionalClaims);
        bool IsRevoked(string token);
        bool IsRevoked(JwtSecurityToken token);
        bool RevokeToken(string token);
        bool RevokeToken(JwtSecurityToken token);
        TimeSpan TokenDuration { get; }
    }
}
