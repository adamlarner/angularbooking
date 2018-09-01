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
    public class ShowingsControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IConfiguration _configuration;

        public ShowingsControllerTest(WebApplicationFactory<Startup> factory)
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
        public void Should_GetShowings()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.Get()).Returns(new List<Showing>
            {
                new Showing { Id = 1, RoomId = 1, EventId = 1, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 12:00"), EndTime = DateTime.Parse("01-01-2020 13:00"), Bookings = new List<Booking>() },
                new Showing { Id = 2, RoomId = 2, EventId = 2, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 14:00"), EndTime = DateTime.Parse("01-01-2020 15:00"), Bookings = new List<Booking>() },
                new Showing { Id = 3, RoomId = 2, EventId = 2, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 15:30"), EndTime = DateTime.Parse("01-01-2020 16:00"), Bookings = new List<Booking>() },
                new Showing { Id = 4, RoomId = 3, EventId = 3, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 17:00"), EndTime = DateTime.Parse("01-01-2020 18:00"), Bookings = new List<Booking>() },
                new Showing { Id = 5, RoomId = 3, EventId = 4, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 19:00"), EndTime = DateTime.Parse("01-01-2020 20:10"), Bookings = new List<Booking>() },
            }
            .AsQueryable());

            ShowingsController controller = new ShowingsController(mock.Object);
            var showings = controller.GetShowings();
            Assert.True(showings.Count() == 5);
        }

        [Fact]
        public void Should_GetShowing()
        {
            Showing testShowing = new Showing { Id = 1, RoomId = 1, EventId = 1, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 12:00"), EndTime = DateTime.Parse("01-01-2020 13:00"), Bookings = new List<Booking>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.GetById(1)).Returns(testShowing);

            ShowingsController controller = new ShowingsController(mock.Object);
            var showing = controller.GetShowing(1);
            Assert.IsType<OkObjectResult>(showing);
        }

        [Fact]
        public void Should_PutShowing()
        {
            Showing testShowing = new Showing { Id = 1, RoomId = 1, EventId = 1, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 12:00"), EndTime = DateTime.Parse("01-01-2020 13:00"), Bookings = new List<Booking>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.Update(testShowing)).Returns(true);

            ShowingsController controller = new ShowingsController(mock.Object);
            var showings = controller.PutShowing(1, testShowing);
            Assert.IsType<NoContentResult>(showings);
        }

        [Fact]
        public void ShouldNot_PutShowing_ModelStateError()
        {
            Showing testShowing = new Showing { Id = 1, RoomId = 1, EventId = 1, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 12:00"), EndTime = DateTime.Parse("01-01-2020 13:00"), Bookings = new List<Booking>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.Update(testShowing)).Returns(true);

            ShowingsController controller = new ShowingsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var showings = controller.PutShowing(1, testShowing);
            Assert.IsType<BadRequestObjectResult>(showings);
        }

        [Fact]
        public void ShouldNot_PutShowing_IdMismatch()
        {
            Showing testShowing = new Showing { Id = 1, RoomId = 1, EventId = 1, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 12:00"), EndTime = DateTime.Parse("01-01-2020 13:00"), Bookings = new List<Booking>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.Update(testShowing)).Returns(true);

            ShowingsController controller = new ShowingsController(mock.Object);
            var showings = controller.PutShowing(2, testShowing);
            Assert.IsType<BadRequestResult>(showings);
        }

        [Fact]
        public void Should_PostShowing()
        {
            Showing testShowing = new Showing { Id = 1, RoomId = 1, EventId = 1, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 12:00"), EndTime = DateTime.Parse("01-01-2020 13:00"), Bookings = new List<Booking>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.Create(testShowing)).Returns(true);

            ShowingsController controller = new ShowingsController(mock.Object);
            var showings = controller.PostShowing(testShowing);
            Assert.IsType<CreatedAtActionResult>(showings);

        }

        [Fact]
        public void ShouldNot_PostShowing_ModelStateError()
        {
            Showing testShowing = new Showing { Id = 1, RoomId = 1, EventId = 1, PricingStrategyId = 1, StartTime = DateTime.Parse("01-01-2020 12:00"), EndTime = DateTime.Parse("01-01-2020 13:00"), Bookings = new List<Booking>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.Create(testShowing)).Returns(true);
            mock.Setup(f => f.Showings.GetById(1)).Returns(testShowing);

            ShowingsController controller = new ShowingsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var showings = controller.PostShowing(testShowing);
            Assert.IsType<BadRequestObjectResult>(showings);
        }

        [Fact]
        public void Should_DeleteShowing()
        {
            Showing testShowing = new Showing { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.GetById(1)).Returns(testShowing);
            mock.Setup(f => f.Showings.Delete(testShowing)).Returns(true);

            ShowingsController controller = new ShowingsController(mock.Object);
            var result = controller.DeleteShowing(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteShowing_ModelStateError()
        {
            Showing testShowing = new Showing { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.Delete(testShowing)).Returns(true);

            ShowingsController controller = new ShowingsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeleteShowing(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteShowing_NotFound()
        {
            Showing testShowing = new Showing { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Showings.GetById(10)).Returns((Showing)null);

            ShowingsController controller = new ShowingsController(mock.Object);
            var result = controller.DeleteShowing(10);
            Assert.IsType<NotFoundResult>(result);
        }

        /* integration testing (expand) */
        /* fetch showings for specific venue by name */
        [Theory]
        [InlineData("test 123")]
        [InlineData("venue A")]
        [InlineData("_._")]
        public async void Should_GetShowings_IncludeEventRoomVenue(string name)
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
                Name = name,
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
            var filterResponse = await client.GetAsync($"/api/showings?$expand=Event,Room($expand=Venue)&$filter=Room/Venue/Name eq '{name}'");
            string data = await filterResponse.Content.ReadAsStringAsync();
            List<Showing> getShowings = await filterResponse.Content.ReadAsAsync<List<Showing>>();

            if (getShowings != null && getShowings.Count() > 0)
            {
                Assert.True(getShowings[0].Event != null && getShowings[0].Room != null && getShowings[0].Room.Venue != null);
            }
            else
            {
                if (getShowings.Count == 0)
                    Assert.NotEmpty(getShowings);

                Assert.NotNull(getShowings);
            }

        }

        [Fact]
        public async void Should_GetShowingAllocations()
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
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(120),
                PricingStrategyId = 1,
                EventId = 1,
                RoomId = 1
            };

            json = JsonConvert.SerializeObject(showing);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            postResponse = await client.PostAsync("api/showings", content);

            Customer customer = new Customer
            {
                Address1 = "",
                Address2 = "",
                Address3 = "",
                Address4 = "",
                Address5 = "",
                ContactEmail = "",
                ContactPhone = "",
                FirstName = "",
                LastName = ""
            };

            json = JsonConvert.SerializeObject(customer);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            postResponse = await client.PostAsync("api/customers", content);

            Booking booking = new Booking
            {
                Id = 0,
                BookedDate = DateTime.Now,
                ShowingId = 1,
                CustomerId = 1,
                Status = BookingStatus.PaymentComplete
            };

            json = JsonConvert.SerializeObject(booking);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            postResponse = await client.PostAsync("api/bookings", content);

            for (int i = 0; i < 5; i++)
            {
                BookingItem bookingItem = new BookingItem
                {
                    Id = 0,
                    AgreedPrice = 4.2f,
                    AgreedPriceName = "",
                    BookingId = 1,
                    Location = i
                };

                json = JsonConvert.SerializeObject(bookingItem);
                content = new StringContent(json, Encoding.UTF8, "application/json");
                postResponse = await client.PostAsync("api/bookingitems", content);
            }

            var availabilityResponse = await client.GetAsync("api/showings/allocations/1");

            if (availabilityResponse.StatusCode != System.Net.HttpStatusCode.OK)
                Assert.True(false);

            string data = await availabilityResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(data))
                Assert.True(false);
            
        }

    }
}
