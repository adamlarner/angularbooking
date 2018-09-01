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
    public class VenuesControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IConfiguration _configuration;

        public VenuesControllerTest(WebApplicationFactory<Startup> factory)
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
        public void Should_GetVenues()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.Get()).Returns(new List<Venue>
            {
                new Venue
                {
                    Id = 1,
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    Description = "Test",
                    ContactPhone = "01234567898",
                    Image = "",
                    LatLong = "0,0",
                    Name = "Test",
                    Instagram = "inst",
                    Facebook = "fb",
                    Twitter = "tw"
                },
                new Venue
                {
                    Id = 1,
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    Description = "Test",
                    ContactPhone = "01234567898",
                    Image = "",
                    LatLong = "0,0",
                    Name = "Test",
                    Instagram = "inst",
                    Facebook = "fb",
                    Twitter = "tw"
                },
                new Venue
                {
                    Id = 1,
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    Description = "Test",
                    ContactPhone = "01234567898",
                    Image = "",
                    LatLong = "0,0",
                    Name = "Test",
                    Instagram = "inst",
                    Facebook = "fb",
                    Twitter = "tw"
                }
            }
            .AsQueryable());

            VenuesController controller = new VenuesController(mock.Object);
            var venues = controller.GetVenues();
            Assert.True(venues.Count() == 3);
        }

        [Fact]
        public void Should_GetVenue()
        {
            Venue testVenue = new Venue
            {
                Id = 1,
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                Description = "Test",
                ContactPhone = "01234567898",
                Image = "",
                LatLong = "0,0",
                Name = "Test",
                Instagram = "inst",
                Facebook = "fb",
                Twitter = "tw"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.GetById(1)).Returns(testVenue);

            VenuesController controller = new VenuesController(mock.Object);
            var venue = controller.GetVenue(1);
            Assert.IsType<OkObjectResult>(venue);
        }

        [Fact]
        public void Should_PutVenue()
        {
            Venue testVenue = new Venue
            {
                Id = 1,
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                Description = "Test",
                ContactPhone = "01234567898",
                Image = "",
                LatLong = "0,0",
                Name = "Test",
                Instagram = "inst",
                Facebook = "fb",
                Twitter = "tw"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.Update(testVenue)).Returns(true);

            VenuesController controller = new VenuesController(mock.Object);
            var venues = controller.PutVenue(1, testVenue);
            Assert.IsType<NoContentResult>(venues);
        }

        [Fact]
        public void ShouldNot_PutVenue_ModelStateError()
        {
            Venue testVenue = new Venue
            {
                Id = 1,
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                Description = "Test",
                ContactPhone = "01234567898",
                Image = "",
                LatLong = "0,0",
                Name = "Test",
                Instagram = "inst",
                Facebook = "fb",
                Twitter = "tw"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.Update(testVenue)).Returns(true);

            VenuesController controller = new VenuesController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var venues = controller.PutVenue(1, testVenue);
            Assert.IsType<BadRequestObjectResult>(venues);
        }

        [Fact]
        public void ShouldNot_PutVenue_IdMismatch()
        {
            Venue testVenue = new Venue
            {
                Id = 1,
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                Description = "Test",
                ContactPhone = "01234567898",
                Image = "",
                LatLong = "0,0",
                Name = "Test",
                Instagram = "inst",
                Facebook = "fb",
                Twitter = "tw"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.Update(testVenue)).Returns(true);

            VenuesController controller = new VenuesController(mock.Object);
            var venues = controller.PutVenue(2, testVenue);
            Assert.IsType<BadRequestResult>(venues);
        }

        [Fact]
        public void Should_PostVenue()
        {
            Venue testVenue = new Venue
            {
                Id = 1,
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                Description = "Test",
                ContactPhone = "01234567898",
                Image = "",
                LatLong = "0,0",
                Name = "Test",
                Instagram = "inst",
                Facebook = "fb",
                Twitter = "tw"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.Create(testVenue)).Returns(true);

            VenuesController controller = new VenuesController(mock.Object);
            var venues = controller.PostVenue(testVenue);
            Assert.IsType<CreatedAtActionResult>(venues);

        }

        [Fact]
        public void ShouldNot_PostVenue_ModelStateError()
        {
            Venue testVenue = new Venue
            {
                Id = 1,
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                Description = "Test",
                ContactPhone = "01234567898",
                Image = "",
                LatLong = "0,0",
                Name = "Test",
                Instagram = "inst",
                Facebook = "fb",
                Twitter = "tw"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.Create(testVenue)).Returns(true);
            mock.Setup(f => f.Venues.GetById(1)).Returns(testVenue);

            VenuesController controller = new VenuesController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var venues = controller.PostVenue(testVenue);
            Assert.IsType<BadRequestObjectResult>(venues);
        }

        [Fact]
        public void Should_DeleteVenue()
        {
            Venue testVenue = new Venue { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.GetById(1)).Returns(testVenue);
            mock.Setup(f => f.Venues.Delete(testVenue)).Returns(true);

            VenuesController controller = new VenuesController(mock.Object);
            var result = controller.DeleteVenue(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteVenue_ModelStateError()
        {
            Venue testVenue = new Venue { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.Delete(testVenue)).Returns(true);

            VenuesController controller = new VenuesController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeleteVenue(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteVenue_NotFound()
        {
            Venue testVenue = new Venue { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Venues.GetById(10)).Returns((Venue)null);

            VenuesController controller = new VenuesController(mock.Object);
            var result = controller.DeleteVenue(10);
            Assert.IsType<NotFoundResult>(result);
        }

        /* integration testing (expand) */
        /* fetch venues, including showings (event and rooms) for specified event */
        [Theory]
        [InlineData("test 123")]
        [InlineData("event A")]
        [InlineData("_._")]
        public async void Should_GetVenues_IncludeShowingsEventRoom(string name)
        {
            // add seed data for showing & foreign dependencies
            var client = _factory.CreateClient();

            // spoof admin access
            IJwtManager jwtManager = new InMemoryJwtManager(_configuration);
            string testToken = await jwtManager.GenerateJwtStringAsync("test@test.com", new List<Claim> { new Claim(ClaimTypes.Role, "admin") });
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + testToken);

            // get antiforgery token, and add to header
            var aftResponse = await client.GetAsync("/api/account/getCSRFToken");
            var tokenData = JsonConvert.DeserializeAnonymousType(aftResponse.Content.ReadAsStringAsync().Result, new { Token = "", TokenName = "" });
            client.DefaultRequestHeaders.Add(tokenData.TokenName, tokenData.Token);

            Event @event = new Event
            {
                Name = name,
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
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(120),
                PricingStrategyId = 1,
                EventId = 1,
                RoomId = 1
            };

            json = JsonConvert.SerializeObject(showing);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            postResponse = await client.PostAsync("api/showings", content);

            // get response, included expanded foreign records
            var filterResponse = await client.GetAsync($"/api/venues?$expand=Rooms($expand=Showings($expand=Event))&$filter=Rooms/any(r: r/Showings/any(s : s/Event/Name eq '{name}'))");
            string data = await filterResponse.Content.ReadAsStringAsync();
            List<Venue> getVenues = await filterResponse.Content.ReadAsAsync<List<Venue>>();

            if (getVenues != null && getVenues.Count() > 0)
            {
                Assert.True(getVenues[0].Rooms != null && getVenues[0].Rooms[0].Showings != null && getVenues[0].Rooms[0].Showings[0].Event != null);
            }
            else
            {
                if (getVenues.Count == 0)
                    Assert.NotEmpty(getVenues);

                Assert.NotNull(getVenues);
            }

        }
        

    }
}
