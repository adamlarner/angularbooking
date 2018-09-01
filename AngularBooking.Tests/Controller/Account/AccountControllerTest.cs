using AngularBooking.Models;
using AngularBooking.Services.Email;
using AngularBooking.Services.JwtManager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace AngularBooking.Tests.Controller.Account
{
    public class AccountControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IConfiguration _configuration;

        public AccountControllerTest(WebApplicationFactory<Startup> factory)
        {
            string appsettingsPath = Path.GetFullPath(Path.Combine(@"../../../../AngularBooking/appsettings.json"));
            var builder = new ConfigurationBuilder()
            .AddJsonFile(appsettingsPath,
                         optional: false, reloadOnChange: false);
            _configuration = builder.Build();

            // override with test value
            _configuration["JWT_KEY"] = "$$Secret_Test_Key_Here$$";

            builder.AddConfiguration(_configuration);
            _configuration = builder.Build();

            // override environment to avoid launching angular app
            _factory = factory.WithWebHostBuilder(f =>
           {
               f.UseEnvironment("Development_WebAPI_Test");
               f.UseConfiguration(_configuration);
               f.ConfigureAppConfiguration(g => g.AddConfiguration(_configuration));
           });
        }

        [Fact]
        public async Task Should_CreateNewUser()
        {
            var client = _factory.CreateClient();

            var jsonData = new
            {
                firstName = "testy",
                lastName = "testo",
                email = "test@test.com",
                password = "Password.01",
                confirmPassword = "Password.01"
            };

            string json = JsonConvert.SerializeObject(jsonData);
            var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            // get antiforgery token
            var aftResponse = await client.GetAsync("api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            var response = await client.PostAsync("/api/account/register", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Login_AndReturnToken()
        {
            var client = _factory.CreateClient();

            // register user first
            var jsonDataRegister = new
            {
                firstName = "testy",
                lastName = "testo",
                email = "test@test.com",
                password = "Password.01",
                confirmPassword = "Password.01"
            };

            string json = JsonConvert.SerializeObject(jsonDataRegister);
            var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            // clear existing email data
            ClearEmailData();

            // ignore response
            var response = await client.PostAsync("/api/account/register", content);

            // retrieve confirmation email and send confirmation
            string emailData = ReadEmailData();
            string confirmLink = emailData.Replace("Please click the following link to confirm registration: ", "");
            var emailConfirmResponse = client.GetAsync(confirmLink).Result;

            // attempt login
            var jsonDataLogin = new
            {
                email = "test@test.com",
                password = "Password.01"
            };

            json = JsonConvert.SerializeObject(jsonDataLogin);
            content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            response = await client.PostAsync("/api/account/login", content);

            // refresh antiforgery token, and add to header
            aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Remove(tokenData.TokenName);
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            // get customer/profile information
            var response2 = await client.GetAsync("/api/customer");

            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        }

        [Fact]
        public async Task ShouldNot_Login_IncorrectCredentials()
        {
            var client = _factory.CreateClient();

            // register user first
            var jsonDataRegister = new
            {
                firstName = "testy",
                lastName = "testo",
                email = "test2@test.com",
                password = "Password.01",
                confirmPassword = "Password.01"
            };

            string json = JsonConvert.SerializeObject(jsonDataRegister);
            var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            // clear existing email data
            ClearEmailData();

            // ignore response, since user may already be created by previous test
            var response = await client.PostAsync("/api/account/register", content);

            // retrieve confirmation email and send confirmation
            string emailData = ReadEmailData();
            string confirmLink = emailData.Replace("Please click the following link to confirm registration: ", "");
            var emailConfirmResponse = client.GetAsync(confirmLink).Result;

            // attempt login
            var jsonDataLogin = new
            {
                email = "test2@test.com",
                password = "Password.02"
            };

            json = JsonConvert.SerializeObject(jsonDataLogin);
            content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            response = await client.PostAsync("/api/account/login", content);
            string contentResponse = await response.Content.ReadAsStringAsync();
            bool isValid = Regex.IsMatch(contentResponse, "Incorrect username/password");

            Assert.Contains("Incorrect username/password", contentResponse);
        }

        [Fact]
        public async Task ShouldNot_Login_EmailNotConfirmed()
        {
            var client = _factory.CreateClient();

            // register user first
            var jsonDataRegister = new
            {
                firstName = "testy",
                lastName = "testo",
                email = "test2@test.com",
                password = "Password.01",
                confirmPassword = "Password.01"
            };

            string json = JsonConvert.SerializeObject(jsonDataRegister);
            var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            // clear existing email data
            ClearEmailData();

            // ignore response, since user may already be created by previous test
            var response = await client.PostAsync("/api/account/register", content);

            // retrieve confirmation email, and confirm subject is as expected
            string emailSubject = ReadEmailSubject();
            Assert.Equal("Subject: Confirm Account Registration", emailSubject);

            // attempt login
            var jsonDataLogin = new
            {
                email = "test2@test.com",
                password = "Password.02"
            };

            json = JsonConvert.SerializeObject(jsonDataLogin);
            content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            response = await client.PostAsync("/api/account/login", content);
            string contentResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains("Account access is not allowed", contentResponse);
        }

        [Fact]
        public async Task Should_Logout_AndPreventTokenReuse()
        {
            var client = _factory.CreateClient();

            // register user first
            var jsonDataRegister = new
            {
                firstName = "testy",
                lastName = "testo",
                email = "test@test.com",
                password = "Password.01",
                confirmPassword = "Password.01"
            };

            string json = JsonConvert.SerializeObject(jsonDataRegister);
            var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            // clear existing email data
            ClearEmailData();

            // ignore response, since user may already be created by previous test
            var response = await client.PostAsync("/api/account/register", content);

            // retrieve confirmation email and send confirmation
            string emailData = ReadEmailData();
            string confirmLink = emailData.Replace("Please click the following link to confirm registration: ", "");
            var emailConfirmResponse = client.GetAsync(confirmLink).Result;

            // attempt login
            var jsonDataLogin = new
            {
                email = "test@test.com",
                password = "Password.01"
            };

            json = JsonConvert.SerializeObject(jsonDataLogin);
            content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            response = await client.PostAsync("/api/account/login", content);

            // refresh antiforgery token, and add to header
            aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Remove(tokenData.TokenName);
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            var responseTest = await client.GetAsync("/api/customer");

            // logout, and attempt to access test endpoint again
            var responseLogout = await client.PostAsync("/api/account/logout", content);

            // get customer/profile information
            var responseRetry = await client.GetAsync("/api/customer");

            if (responseTest.StatusCode == HttpStatusCode.OK)
            {
                Assert.NotEqual(HttpStatusCode.OK, responseRetry.StatusCode);
            }
            else
            {
                Assert.True(false);
            }

        }

        [Fact]
        public async Task Should_RefreshAuthToken()
        {
            var client = _factory.CreateClient();

            // register user first
            var jsonDataRegister = new
            {
                firstName = "testy",
                lastName = "testo",
                email = "test@test.com",
                password = "Password.01",
                confirmPassword = "Password.01"
            };

            string json = JsonConvert.SerializeObject(jsonDataRegister);
            var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            // clear existing email data
            ClearEmailData();

            // ignore response, since user may already be created by previous test
            var response = await client.PostAsync("/api/account/register", content);

            // retrieve confirmation email and send confirmation
            string emailData = ReadEmailData();
            string confirmLink = emailData.Replace("Please click the following link to confirm registration: ", "");
            var emailConfirmResponse = client.GetAsync(confirmLink).Result;

            // attempt login
            var jsonDataLogin = new
            {
                email = "test@test.com",
                password = "Password.01"
            };

            json = JsonConvert.SerializeObject(jsonDataLogin);
            content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            response = await client.PostAsync("/api/account/login", content);

            // refresh antiforgery token, and add to header
            aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Remove(tokenData.TokenName);
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            var responseTest = await client.GetAsync("/api/customer");

            // refresh auth token
            var authRefreshResponse = await client.PostAsync("/api/account/refreshAuth", null);

            // attempt to access test endpoint again
            var responseRetry = await client.GetAsync("/api/customer");

            if (responseTest.StatusCode == HttpStatusCode.OK && authRefreshResponse.StatusCode == HttpStatusCode.OK)
            {
                Assert.Equal(HttpStatusCode.OK, responseRetry.StatusCode);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async Task ShouldNot_RefreshAuthToken_Unauthorized()
        {
            var client = _factory.CreateClient();

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            // attempt refresh
            var response = await client.PostAsync("/api/account/refreshAuth", null);

            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Should_ConfirmUserIsAuthorized()
        {
            var client = _factory.CreateClient();

            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);

            string testToken = await jwtManager.GenerateJwtStringAsync("test@test.com", new List<Claim>());

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token
            var aftResponse = await client.GetAsync("api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            var response = await client.GetAsync("/api/account/isAuth");

            string content = await response.Content.ReadAsStringAsync();

            Assert.Equal("true", content);

        }

        [Fact]
        public async Task ShouldNot_ConfirmUserIsAuthorized_InvalidToken()
        {
            var client = _factory.CreateClient();

            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);

            JwtSecurityToken token = await jwtManager.GenerateJwtAsync("test@test.com", new List<Claim>());
            token.Payload["exp"] = DateTimeOffset.Now.ToUnixTimeSeconds();
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            
            string testToken = handler.WriteToken(token);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token
            var aftResponse = await client.GetAsync("api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            var response = await client.GetAsync("/api/account/isAuth");

            string content = await response.Content.ReadAsStringAsync();

            Assert.Equal("false", content);

        }

        // -- common helper methods --

        // clear email file pickup
        private bool ClearEmailData()
        {
            try
            {
                if (Directory.Exists(BasicEmailService.TestPickupDirectory))
                    Directory.Delete(BasicEmailService.TestPickupDirectory, true);

                Directory.CreateDirectory(BasicEmailService.TestPickupDirectory);
            }
            catch(Exception e)
            {
                // catch-all, return false;
                return false;
            }

            return true;
        }

        private string ReadEmailData()
        {
            if(Directory.Exists(BasicEmailService.TestPickupDirectory))
            {
                string[] files = Directory.GetFiles(BasicEmailService.TestPickupDirectory);
                if (files.Length == 0)
                    return string.Empty;

                string[] emailData = File.ReadAllLines(files[0]);

                // 11th line onwards is content start
                string emailContent = string.Join("", emailData.Skip(10).ToList());
                emailContent = Base64UrlEncoder.Decode(emailContent);

                return emailContent;

            }

            return string.Empty;
        }

        private string ReadEmailSubject()
        {
            if (Directory.Exists(BasicEmailService.TestPickupDirectory))
            {
                string[] files = Directory.GetFiles(BasicEmailService.TestPickupDirectory);
                if (files.Length == 0)
                    return string.Empty;

                string[] emailSubject = File.ReadAllLines(files[0]);

                // 7th line is subject
                return emailSubject[6];

            }

            return string.Empty;
        }

    }
}
