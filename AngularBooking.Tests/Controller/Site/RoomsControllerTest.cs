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
    public class RoomsControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IConfiguration _configuration;

        public RoomsControllerTest(WebApplicationFactory<Startup> factory)
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
        public void Should_GetRooms()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.Get()).Returns(new List<Room>
            {
                new Room { Id = 1, Name = "Test1", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" },
                new Room { Id = 2, Name = "Test2", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" },
                new Room { Id = 3, Name = "Test3", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" },
                new Room { Id = 4, Name = "Test4", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" },
                new Room { Id = 5, Name = "Test5", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" }
            }
            .AsQueryable());

            RoomsController controller = new RoomsController(mock.Object);
            var rooms = controller.GetRooms();
            Assert.True(rooms.Count() == 5);
        }

        [Fact]
        public void Should_GetRoom()
        {
            Room testRoom = new Room { Id = 1, Name = "Test5", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.GetById(1)).Returns(testRoom);

            RoomsController controller = new RoomsController(mock.Object);
            var room = controller.GetRoom(1);
            Assert.IsType<OkObjectResult>(room);
        }

        [Fact]
        public void Should_PutRoom()
        {
            Room testRoom = new Room { Id = 1, Name = "Test5", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.Update(testRoom)).Returns(true);

            RoomsController controller = new RoomsController(mock.Object);
            var rooms = controller.PutRoom(1, testRoom);
            Assert.IsType<NoContentResult>(rooms);
        }

        [Fact]
        public void ShouldNot_PutRoom_ModelStateError()
        {
            Room testRoom = new Room { Id = 1, Name = "Test5", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.Update(testRoom)).Returns(true);

            RoomsController controller = new RoomsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var rooms = controller.PutRoom(1, testRoom);
            Assert.IsType<BadRequestObjectResult>(rooms);
        }

        [Fact]
        public void ShouldNot_PutRoom_IdMismatch()
        {
            Room testRoom = new Room { Id = 1, Name = "Test5", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.Update(testRoom)).Returns(true);

            RoomsController controller = new RoomsController(mock.Object);
            var rooms = controller.PutRoom(2, testRoom);
            Assert.IsType<BadRequestResult>(rooms);
        }

        [Fact]
        public void Should_PostRoom()
        {
            Room testRoom = new Room { Id = 1, Name = "Test5", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.Create(testRoom)).Returns(true);

            RoomsController controller = new RoomsController(mock.Object);
            var rooms = controller.PostRoom(testRoom);
            Assert.IsType<CreatedAtActionResult>(rooms);

        }

        [Fact]
        public void ShouldNot_PostRoom_ModelStateError()
        {
            Room testRoom = new Room { Id = 1, Name = "Test5", Description = "Test", VenueId = 1, Columns = 10, Rows = 10, Isles = "{}" };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.Create(testRoom)).Returns(true);
            mock.Setup(f => f.Rooms.GetById(1)).Returns(testRoom);

            RoomsController controller = new RoomsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var rooms = controller.PostRoom(testRoom);
            Assert.IsType<BadRequestObjectResult>(rooms);
        }

        [Fact]
        public void Should_DeleteRoom()
        {
            Room testRoom = new Room { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.GetById(1)).Returns(testRoom);
            mock.Setup(f => f.Rooms.Delete(testRoom)).Returns(true);

            RoomsController controller = new RoomsController(mock.Object);
            var result = controller.DeleteRoom(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteRoom_ModelStateError()
        {
            Room testRoom = new Room { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.Delete(testRoom)).Returns(true);

            RoomsController controller = new RoomsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeleteRoom(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteRoom_NotFound()
        {
            Room testRoom = new Room { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Rooms.GetById(10)).Returns((Room)null);

            RoomsController controller = new RoomsController(mock.Object);
            var result = controller.DeleteRoom(10);
            Assert.IsType<NotFoundResult>(result);
        }

        /* integration testing (expand) */
        /* fetch room, including showing, for specified date, and specified event (by id) */
        [Fact]
        public async void Should_GetRoom_WithShowingOnSpecifiedDate()
        {
            var client = _factory.CreateClient();

            // spoof admin access
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);
            string testToken = await jwtManager.GenerateJwtStringAsync("test@test.com", new List<Claim> { new Claim(ClaimTypes.Role, "admin") });
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            // add seed data for dependencies
            Event @event = new Event
            {
                Name = "Test Event",
                Description = "Event Desc",
                Image = "",
                Duration = 120,
                AgeRating = AgeRatingType.BBFC_PG
            };

            string json = JsonConvert.SerializeObject(@event);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var postResponse = await client.PostAsync("api/events", content);

            Venue venue = new Venue
            {
                Name = "Test Venue",
                Description = "Venue Desc",
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                ContactPhone = "",
                Image = "",
                Website = "",
                Instagram = "",
                Facebook = "",
                Twitter = "",
                Facilities = FacilityFlags.Bar | FacilityFlags.GuideDogsPermitted,
                LatLong = ""
            };

            json = JsonConvert.SerializeObject(venue);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            postResponse = await client.PostAsync("api/venues", content);

            Room room = new Room
            {
                Name = "Test Room",
                Description = "Room Desc",
                Columns = 10,
                Rows = 10,
                Isles = "",
                VenueId = 1
            };

            json = JsonConvert.SerializeObject(room);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            postResponse = await client.PostAsync("api/rooms", content);

            PricingStrategy strategy = new PricingStrategy
            {
                Name = "Test Strategy",
                Description = "Strategy Desc"
            };

            json = JsonConvert.SerializeObject(strategy);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            postResponse = await client.PostAsync("api/pricingstrategies", content);

            Showing showing = new Showing
            {
                Id = 0,
                StartTime = new DateTime(2018, 12, 1),
                EndTime = new DateTime(2018, 12, 2),
                PricingStrategyId = 1,
                EventId = 1,
                RoomId = 1
            };

            json = JsonConvert.SerializeObject(showing);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            postResponse = await client.PostAsync("api/showings", content);

            var filterResponse = await client.GetAsync($"api/rooms?$expand=Showings&$filter=Showings/any(s : date(s/StartTime) ge 2018-12-1 and date(s/StartTime) lt 2018-12-2 and s/EventId eq 1)&$select=Id,Name,Showings");
            string data = await filterResponse.Content.ReadAsStringAsync();
            List<Room> getRooms = await filterResponse.Content.ReadAsAsync<List<Room>>();

            if (getRooms != null && getRooms.Count() > 0)
            {
                Assert.True(getRooms[0].Showings != null);
            }
            else
            {
                if (getRooms.Count == 0)
                    Assert.NotEmpty(getRooms);

                Assert.NotNull(getRooms);
            }

        }

    }
}
