using AngularBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AngularBooking.Services.JwtManager
{
    public class InMemoryJwtManager : IJwtManager
    {
        private IConfiguration _configuration;

        private ConcurrentDictionary<string, JwtSecurityToken> _tokenStore = new ConcurrentDictionary<string, JwtSecurityToken>();
        private TimeSpan _tokenDuration = TimeSpan.FromMinutes(10);

        // used to create unique signature, setting as '*' will auto-generate
        private readonly string _jwtKey;

        // keeps track of when next token purge is needed
        private long _nextPurgeTicks;

        public InMemoryJwtManager(IConfiguration configuration, string jwtKey = null)
        {
            _configuration = configuration;
            _jwtKey = jwtKey ?? _configuration["JWT_KEY"];
            Interlocked.Exchange(ref _nextPurgeTicks, DateTime.UtcNow.Add(_tokenDuration).ToBinary());
        }

        public InMemoryJwtManager(IConfiguration configuration, TimeSpan tokenDuration, string jwtKey = null)
        {
            _configuration = configuration;
            _tokenDuration = tokenDuration;
            _jwtKey = jwtKey ?? _configuration["JWT_KEY"];
            Interlocked.Exchange(ref _nextPurgeTicks, DateTime.UtcNow.Add(_tokenDuration).ToBinary());
        }

        public bool RevokeToken(JwtSecurityToken token)
        {
            if (token != null)
            {
                string jti = token.Id;
                _tokenStore.GetOrAdd(jti, token);

                // check next purge time, and purge if required
                DateTime nextPurge = DateTime.FromBinary(_nextPurgeTicks);
                if(nextPurge < DateTime.UtcNow)
                {
                    lock(_tokenStore)
                    {
                        List<string> keys = _tokenStore.Keys.ToList();
                        for(int i = 0; i < keys.Count; i++)
                        {
                            if(_tokenStore[keys[i]].ValidTo < DateTime.UtcNow)
                            {
                                // remove token, discard output
                                _tokenStore.Remove(keys[i], out _);
                            }
                        }
                    }

                    Interlocked.Exchange(ref _nextPurgeTicks, DateTime.UtcNow.Add(_tokenDuration).ToBinary());
                }

                return true;
            }

            return false;
        }

        public bool RevokeToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt = handler.ReadJwtToken(token);
            if(jwt != null)
                return RevokeToken(jwt);

            return false;
        }

        public async Task<JwtSecurityToken> GenerateJwtAsync(string username, List<Claim> additionalClaims)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            claims.AddRange(additionalClaims.ToList());

            var token = new JwtSecurityToken
            (
                issuer: _configuration["JWT_ISSUER"],
                audience: _configuration["JWT_AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.Add(_tokenDuration),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                    SecurityAlgorithms.HmacSha256)
            );

            return await Task.FromResult(token);
        }

        public async Task<string> GenerateJwtStringAsync(string username, List<Claim> additionalClaims)
        {
            JwtSecurityToken token = await GenerateJwtAsync(username, additionalClaims);

            if (token == null)
                return await Task.FromResult<string>(null);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public bool IsRevoked(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return false;

            JwtSecurityToken jwt = handler.ReadJwtToken(token);

            return IsRevoked(jwt);
        }

        public bool IsRevoked(JwtSecurityToken token)
        {
            return _tokenStore.ContainsKey(token.Id);
        }

        public TimeSpan TokenDuration
        {
            get
            {
                return _tokenDuration;
            }
        }
    }
}
