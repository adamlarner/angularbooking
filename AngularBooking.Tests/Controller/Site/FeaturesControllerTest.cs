using AngularBooking.Controllers.Site;
using AngularBooking.Data;
using AngularBooking.Models;
using AngularBooking.Services.JwtManager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Controller.Site
{
    public class FeaturesControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IConfiguration _configuration;

        public FeaturesControllerTest(WebApplicationFactory<Startup> factory)
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
        public void Should_GetFeatures()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.Get()).Returns(new List<Feature>
            {
                new Feature { Id = 1, Title = "Title", Name = "Feature1", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1},
                new Feature { Id = 2, Title = "Title", Name = "Feature2", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 2},
                new Feature { Id = 3, Title = "Title", Name = "Feature3", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 3},
            }
            .AsQueryable());

            FeaturesController controller = new FeaturesController(mock.Object);
            var features = controller.GetFeatures();
            Assert.True(features.Count() == 3);
        }

        [Fact]
        public void Should_GetFeature()
        {
            Feature testFeature = new Feature { Id = 1, Title = "Title", Name = "Feature1", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.GetById(1)).Returns(testFeature);

            FeaturesController controller = new FeaturesController(mock.Object);
            var feature = controller.GetFeature(1);
            Assert.IsType<OkObjectResult>(feature);
        }

        [Fact]
        public void Should_PutFeature()
        {
            Feature testFeature = new Feature { Id = 1, Title = "Title", Name = "Feature1", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.Update(testFeature)).Returns(true);

            FeaturesController controller = new FeaturesController(mock.Object);
            var features = controller.PutFeature(1, testFeature);
            Assert.IsType<NoContentResult>(features);
        }

        [Fact]
        public void ShouldNot_PutFeature_ModelStateError()
        {
            Feature testFeature = new Feature { Id = 1, Title = "Title", Name = "Feature1", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.Update(testFeature)).Returns(true);

            FeaturesController controller = new FeaturesController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var features = controller.PutFeature(1, testFeature);
            Assert.IsType<BadRequestObjectResult>(features);
        }

        [Fact]
        public void ShouldNot_PutFeature_IdMismatch()
        {
            Feature testFeature = new Feature { Id = 1, Title = "Title", Name = "Feature1", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.Update(testFeature)).Returns(true);

            FeaturesController controller = new FeaturesController(mock.Object);
            var features = controller.PutFeature(2, testFeature);
            Assert.IsType<BadRequestResult>(features);
        }

        [Fact]
        public void Should_PostFeature()
        {
            Feature testFeature = new Feature { Id = 1, Title = "Title", Name = "Feature1", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.Create(testFeature)).Returns(true);

            FeaturesController controller = new FeaturesController(mock.Object);
            var features = controller.PostFeature(testFeature);
            Assert.IsType<CreatedAtActionResult>(features);

        }

        [Fact]
        public void ShouldNot_PostFeature_ModelStateError()
        {
            Feature testFeature = new Feature { Id = 1, Title = "Title", Name = "Feature1", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.Create(testFeature)).Returns(true);
            mock.Setup(f => f.Features.GetById(1)).Returns(testFeature);

            FeaturesController controller = new FeaturesController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var features = controller.PostFeature(testFeature);
            Assert.IsType<BadRequestObjectResult>(features);
        }

        [Fact]
        public void Should_DeleteFeature()
        {
            Feature testFeature = new Feature { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.GetById(1)).Returns(testFeature);
            mock.Setup(f => f.Features.Delete(testFeature)).Returns(true);

            FeaturesController controller = new FeaturesController(mock.Object);
            var result = controller.DeleteFeature(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteFeature_ModelStateError()
        {
            Feature testFeature = new Feature { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.Delete(testFeature)).Returns(true);

            FeaturesController controller = new FeaturesController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeleteFeature(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteFeature_NotFound()
        {
            Feature testFeature = new Feature { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Features.GetById(10)).Returns((Feature)null);

            FeaturesController controller = new FeaturesController(mock.Object);
            var result = controller.DeleteFeature(10);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Should_GetFeatures_Top5()
        {
            // add seed data for features
            var client = _factory.CreateClient();

            List<Feature> features = new List<Feature>
            {
                   new Feature { Id = 1, Title = "Title", Name = "Feature1", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 },
                   new Feature { Id = 2, Title = "Title", Name = "Feature2", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 2 },
                   new Feature { Id = 3, Title = "Title", Name = "Feature3", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 3 },
                   new Feature { Id = 4, Title = "Title", Name = "Feature4", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 4 },
                   new Feature { Id = 5, Title = "Title", Name = "Feature5", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 5 },
                   new Feature { Id = 6, Title = "Title", Name = "Feature6", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 6 },
                   new Feature { Id = 7, Title = "Title", Name = "Feature7", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 7 },
                   new Feature { Id = 8, Title = "Title", Name = "Feature8", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 8 },
                   new Feature { Id = 9, Title = "Title", Name = "Feature9", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 9 },
                   new Feature { Id = 10, Title = "Title", Name = "Feature10", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 10 },
            };

            // spoof admin access
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);
            string testToken = await jwtManager.GenerateJwtStringAsync("test@test.com", new List<Claim> { new Claim(ClaimTypes.Role, "admin") });
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            foreach (Feature feature in features)
            {
                string json = JsonConvert.SerializeObject(feature);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("api/features", content);
            }

            // ignore response
            var filterResponse = await client.GetAsync("/api/features?$top=5");
            string data = await filterResponse.Content.ReadAsStringAsync();
            List<Feature> getFeatures = await filterResponse.Content.ReadAsAsync<List<Feature>>();

            if (getFeatures != null)
            {
                Assert.Equal(5, getFeatures.Count);
            }
            else
            {
                Assert.NotNull(getFeatures);
            }

        }

        [Fact]
        public async void Should_GetFeatures_Select()
        {
            // add seed data for features
            var client = _factory.CreateClient();

            List<Feature> features = new List<Feature>
            {
                new Feature { Id = 1, Title = "Title", Name = "Feature1", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 },
                new Feature { Id = 2, Title = "Title", Name = "Feature2", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 },
                new Feature { Id = 3, Title = "Title", Name = "Feature3", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 },
                new Feature { Id = 4, Title = "Title", Name = "Feature4", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 }
            };

            // spoof admin access
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);
            string testToken = await jwtManager.GenerateJwtStringAsync("test@test.com", new List<Claim> { new Claim(ClaimTypes.Role, "admin") });
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            foreach (Feature feature in features)
            {
                string json = JsonConvert.SerializeObject(feature);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("api/features", content);
            }

            // ignore response
            var filterResponse = await client.GetAsync("/api/features?$select=name");
            string data = await filterResponse.Content.ReadAsStringAsync();
            List<Feature> getFeatures = await filterResponse.Content.ReadAsAsync<List<Feature>>();

            if (getFeatures != null)
            {
                Assert.Equal(4, getFeatures.Count((value) => { return value.Name != null && value.Image == null; }));
            }
            else
            {
                Assert.NotNull(getFeatures);
            }

        }

        [Theory]
        [InlineData("test a")]
        [InlineData("test b")]
        [InlineData("test c")]
        [InlineData("test d")]
        public async void Should_GetFeatures_FilterName(string filter)
        {
            // add seed data for features
            var client = _factory.CreateClient();

            List<Feature> features = new List<Feature>
            {
               new Feature { Id = 1, Title = "Title", Name = "test a", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 1 },
               new Feature { Id = 2, Title = "Title", Name = "test b", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 2 },
               new Feature { Id = 3, Title = "Title", Name = "test c", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 3 },
               new Feature { Id = 4, Title = "Title", Name = "test d", Detail = "Feature Details", Link = "www.something.com", Image = "Image Data", Order = 4 }
            };

            // spoof admin access
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);
            string testToken = await jwtManager.GenerateJwtStringAsync("test@test.com", new List<Claim> { new Claim(ClaimTypes.Role, "admin") });
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            foreach (Feature feature in features)
            {
                string json = JsonConvert.SerializeObject(feature);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("api/features", content);
            }

            // ignore response
            var filterResponse = await client.GetAsync($"/api/features?$filter=Name eq '{filter}'");
            string data = await filterResponse.Content.ReadAsStringAsync();
            List<Feature> getFeatures = await filterResponse.Content.ReadAsAsync<List<Feature>>();

            if (getFeatures != null)
            {
                Assert.Single(getFeatures);
            }
            else
            {
                Assert.NotNull(getFeatures);
            }
        }


    }
}
