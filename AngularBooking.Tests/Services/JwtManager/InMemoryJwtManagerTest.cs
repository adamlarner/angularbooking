using AngularBooking.Models;
using AngularBooking.Services.JwtManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AngularBooking.Tests.Services.JwtManager
{
    public class InMemoryJwtManagerTest
    {
        private readonly IConfiguration _configuration;

        public InMemoryJwtManagerTest()
        {
            // configuration to match SIT
            string appsettingsPath = Path.GetFullPath(Path.Combine(@"../../../../AngularBooking/appsettings.json"));
            var builder = new ConfigurationBuilder()
            .AddJsonFile(appsettingsPath,
                         optional: false, reloadOnChange: true);
            _configuration = builder.Build();

            // override with test value
            _configuration["JWT_KEY"] = "$$Secret_Test_Key_Here$$";
            builder.AddConfiguration(_configuration);
            _configuration = builder.Build();
        }

        [Fact]
        public async void Should_CreateJwtToken()
        {
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);

            JwtSecurityToken token = await jwtManager.GenerateJwtAsync("test@test.com", new List<Claim>());
            if(token != null)
                Assert.Equal("test@test.com", token.Subject);
            else
                Assert.True(false);
        }

        [Fact]
        public void Should_RevokeToken()
        {
            // mock user manager
            Mock<IUserStore<User>> userStore = new Mock<IUserStore<User>>();
            Mock<UserManager<User>> userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);

            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);

            // create test token
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "test@test.com"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var token = new JwtSecurityToken
            (
                issuer: _configuration["JWT_ISSUER"],
                audience: _configuration["JWT_AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"])),
                    SecurityAlgorithms.HmacSha256)
            );


            Assert.True(jwtManager.RevokeToken(token));
        }

        [Fact]
        public void Should_RevokeStringToken()
        {
            // mock user manager
            Mock<IUserStore<User>> userStore = new Mock<IUserStore<User>>();

            // create test token
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "test@test.com"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var token = new JwtSecurityToken
            (
                issuer: _configuration["JWT_ISSUER"],
                audience: _configuration["JWT_AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"])),
                    SecurityAlgorithms.HmacSha256)
            );
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string stringToken = handler.WriteToken(token);

            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);

            Assert.True(jwtManager.RevokeToken(stringToken));

        }

        [Fact]
        public void Should_CheckTokenIsRevoked_ReturnTrue()
        {
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);

            // create test token
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "test@test.com"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var token = new JwtSecurityToken
            (
                issuer: _configuration["JWT_ISSUER"],
                audience: _configuration["JWT_AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"])),
                    SecurityAlgorithms.HmacSha256)
            );

            // add mocked token
            jwtManager.RevokeToken(token);

            Assert.True(jwtManager.IsRevoked(token));
        }

        [Fact]
        public void Should_CheckTokenIsRevoked_ReturnFalse()
        {
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);

            // create test token
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "test@test.com"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var token = new JwtSecurityToken
            (
                issuer: _configuration["JWT_ISSUER"],
                audience: _configuration["JWT_AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"])),
                    SecurityAlgorithms.HmacSha256)
            );

            // token not yet added, and so should return false
            Assert.False(jwtManager.IsRevoked(token));
        }

        [Fact]
        public void Should_ReturnDefaultTokenDuration()
        {
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);
            Assert.Equal(TimeSpan.FromMinutes(10), jwtManager.TokenDuration);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(1)]
        [InlineData(100)]
        public void Should_ReturnSpecifiedTokenDuration(int minutes)
        {
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration, TimeSpan.FromMinutes(minutes));
            Assert.Equal(TimeSpan.FromMinutes(minutes), jwtManager.TokenDuration);
        }

    }
}
