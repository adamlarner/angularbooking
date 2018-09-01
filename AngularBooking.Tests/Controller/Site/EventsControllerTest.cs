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
    public class EventsControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IConfiguration _configuration;

        public EventsControllerTest(WebApplicationFactory<Startup> factory)
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
        public void Should_GetEvents()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.Get()).Returns(new List<Event>
            {
                new Event { Id = 1, Name = "Event1", Duration = 120, AgeRating = AgeRatingType.PEGI_12, Image = "", Description = "Event" },
                new Event { Id = 2, Name = "Event2", Duration = 130, AgeRating = AgeRatingType.PEGI_12A, Image = "", Description = "Event" },
                new Event { Id = 3, Name = "Event3", Duration = 140, AgeRating = AgeRatingType.PEGI_15, Image = "", Description = "Event" },
                new Event { Id = 4, Name = "Event4", Duration = 150, AgeRating = AgeRatingType.PEGI_18, Image = "", Description = "Event" },
                new Event { Id = 5, Name = "Event5", Duration = 160, AgeRating = AgeRatingType.PEGI_PG, Image = "", Description = "Event" },
            }
            .AsQueryable());

            EventsController controller = new EventsController(mock.Object);
            var events = controller.GetEvents();
            Assert.True(events.Count() == 5);
        }

        [Fact]
        public void Should_GetEvent()
        {
            Event testEvent = new Event { Id = 1, Name = "Event1", Duration = 120, AgeRating = AgeRatingType.PEGI_12, Image = "", Description = "Event" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.GetById(1)).Returns(testEvent);

            EventsController controller = new EventsController(mock.Object);
            var @event = controller.GetEvent(1);
            Assert.IsType<OkObjectResult>(@event);
        }

        [Fact]
        public void Should_PutEvent()
        {
            Event testEvent = new Event { Id = 1, Name = "Event1", Duration = 120, AgeRating = AgeRatingType.PEGI_12, Image = "", Description = "Event" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.Update(testEvent)).Returns(true);

            EventsController controller = new EventsController(mock.Object);
            var events = controller.PutEvent(1, testEvent);
            Assert.IsType<NoContentResult>(events);
        }

        [Fact]
        public void ShouldNot_PutEvent_ModelStateError()
        {
            Event testEvent = new Event { Id = 1, Name = "Event1", Duration = 120, AgeRating = AgeRatingType.PEGI_12, Image = "", Description = "Event" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.Update(testEvent)).Returns(true);

            EventsController controller = new EventsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var events = controller.PutEvent(1, testEvent);
            Assert.IsType<BadRequestObjectResult>(events);
        }

        [Fact]
        public void ShouldNot_PutEvent_IdMismatch()
        {
            Event testEvent = new Event { Id = 1, Name = "Event1", Duration = 120, AgeRating = AgeRatingType.PEGI_12, Image = "", Description = "Event" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.Update(testEvent)).Returns(true);

            EventsController controller = new EventsController(mock.Object);
            var events = controller.PutEvent(2, testEvent);
            Assert.IsType<BadRequestResult>(events);
        }

        [Fact]
        public void Should_PostEvent()
        {
            Event testEvent = new Event { Id = 1, Name = "Event1", Duration = 120, AgeRating = AgeRatingType.PEGI_12, Image = "", Description = "Event" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.Create(testEvent)).Returns(true);

            EventsController controller = new EventsController(mock.Object);
            var events = controller.PostEvent(testEvent);
            Assert.IsType<CreatedAtActionResult>(events);

        }

        [Fact]
        public void ShouldNot_PostEvent_ModelStateError()
        {
            Event testEvent = new Event { Id = 1, Name = "Event1", Duration = 120, AgeRating = AgeRatingType.PEGI_12, Image = "", Description = "Event" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.Create(testEvent)).Returns(true);
            mock.Setup(f => f.Events.GetById(1)).Returns(testEvent);

            EventsController controller = new EventsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var events = controller.PostEvent(testEvent);
            Assert.IsType<BadRequestObjectResult>(events);
        }

        [Fact]
        public void Should_DeleteEvent()
        {
            Event testEvent = new Event { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.GetById(1)).Returns(testEvent);
            mock.Setup(f => f.Events.Delete(testEvent)).Returns(true);

            EventsController controller = new EventsController(mock.Object);
            var result = controller.DeleteEvent(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteEvent_ModelStateError()
        {
            Event testEvent = new Event { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.Delete(testEvent)).Returns(true);

            EventsController controller = new EventsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeleteEvent(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteEvent_NotFound()
        {
            Event testEvent = new Event { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Events.GetById(10)).Returns((Event)null);

            EventsController controller = new EventsController(mock.Object);
            var result = controller.DeleteEvent(10);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Should_GetEvents_Top5()
        {
            // add seed data for events
            var client = _factory.CreateClient();

            List<Event> events = new List<Event>
            {
                new Event { Name = "test a", Description = "desc", Image = "", Duration = 100, AgeRating = AgeRatingType.BBFC_12A },
                new Event { Name = "test b", Description = "desc", Image = "", Duration = 90, AgeRating = AgeRatingType.BBFC_PG },
                new Event { Name = "test c", Description = "desc", Image = "", Duration = 130, AgeRating = AgeRatingType.BBFC_U },
                new Event { Name = "test d", Description = "desc", Image = "", Duration = 240, AgeRating = AgeRatingType.PEGI_12A },
                new Event { Name = "test e", Description = "desc", Image = "", Duration = 1, AgeRating = AgeRatingType.PEGI_PG },
                new Event { Name = "test f", Description = "desc", Image = "", Duration = 1000, AgeRating = AgeRatingType.PEGI_R18 },
                new Event { Name = "test g", Description = "desc", Image = "", Duration = 3, AgeRating = AgeRatingType.PEGI_Universal },
                new Event { Name = "test h", Description = "desc", Image = "", Duration = 2451000, AgeRating = AgeRatingType.BBFC_UC }
            };

            // spoof admin access
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);
            string testToken = await jwtManager.GenerateJwtStringAsync("test@test.com", new List<Claim> { new Claim(ClaimTypes.Role, "admin") });
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            foreach (Event @event in events)
            {
                string json = JsonConvert.SerializeObject(@event);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("api/events", content);
            }

            // ignore response
            var filterResponse = await client.GetAsync("/api/events?$top=5");
            string data = await filterResponse.Content.ReadAsStringAsync();
            List<Event> getEvents = await filterResponse.Content.ReadAsAsync<List<Event>>();

            if(getEvents != null)
            {
                Assert.Equal(5, getEvents.Count);
            }
            else
            {
                Assert.NotNull(getEvents);
            }

        }

        [Fact]
        public async void Should_GetEvents_Select()
        {
            // add seed data for events
            var client = _factory.CreateClient();

            List<Event> events = new List<Event>
            {
                new Event { Name = "test a", Description = "desc", Image = "", Duration = 100, AgeRating = AgeRatingType.BBFC_12A },
                new Event { Name = "test b", Description = "desc", Image = "", Duration = 90, AgeRating = AgeRatingType.BBFC_PG },
                new Event { Name = "test c", Description = "desc", Image = "", Duration = 130, AgeRating = AgeRatingType.BBFC_U },
                new Event { Name = "test d", Description = "desc", Image = "", Duration = 240, AgeRating = AgeRatingType.PEGI_12A }
            };

            // spoof admin access
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);
            string testToken = await jwtManager.GenerateJwtStringAsync("test@test.com", new List<Claim> { new Claim(ClaimTypes.Role, "admin") });
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            foreach (Event @event in events)
            {
                string json = JsonConvert.SerializeObject(@event);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("api/events", content);
            }

            // ignore response
            var filterResponse = await client.GetAsync("/api/events?$select=name");
            string data = await filterResponse.Content.ReadAsStringAsync();
            List<Event> getEvents = await filterResponse.Content.ReadAsAsync<List<Event>>();

            if (getEvents != null)
            {
                Assert.Equal(4, getEvents.Count((value) => { return value.Name != null && value.Description == null; }));
            }
            else
            {
                Assert.NotNull(getEvents);
            }

        }

        [Theory]
        [InlineData("test a")]
        [InlineData("test b")]
        [InlineData("test c")]
        [InlineData("test d")]
        public async void Should_GetEvents_FilterName(string filter)
        {
            // add seed data for events
            var client = _factory.CreateClient();

            List<Event> events = new List<Event>
            {
                new Event { Name = "test a", Description = "desc", Image = "", Duration = 100, AgeRating = AgeRatingType.BBFC_12A },
                new Event { Name = "test b", Description = "desc", Image = "", Duration = 90, AgeRating = AgeRatingType.BBFC_PG },
                new Event { Name = "test c", Description = "desc", Image = "", Duration = 130, AgeRating = AgeRatingType.BBFC_U },
                new Event { Name = "test d", Description = "desc", Image = "", Duration = 240, AgeRating = AgeRatingType.PEGI_12A }
            };

            // spoof admin access
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);
            string testToken = await jwtManager.GenerateJwtStringAsync("test@test.com", new List<Claim> { new Claim(ClaimTypes.Role, "admin") });
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            foreach (Event @event in events)
            {
                string json = JsonConvert.SerializeObject(@event);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("api/events", content);
            }

            // ignore response
            var filterResponse = await client.GetAsync($"/api/events?$filter=Name eq '{filter}'");
            string data = await filterResponse.Content.ReadAsStringAsync();
            List<Event> getEvents = await filterResponse.Content.ReadAsAsync<List<Event>>();

            if (getEvents != null)
            {
                Assert.Single(getEvents);
            }
            else
            {
                Assert.NotNull(getEvents);
            }
        }


    }
}
