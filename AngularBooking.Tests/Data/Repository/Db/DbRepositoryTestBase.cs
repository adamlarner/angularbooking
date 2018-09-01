using AngularBooking.Data;
using AngularBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{     
    public abstract class DbRepositoryTestBase
    {
        public static ApplicationDbContext SeedContext()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlite("DataSource=:memory:");
            var context = new ApplicationDbContext(builder.Options);
            context.Database.OpenConnection();
            context.Database.Migrate();

            // initial seed database (assists in testing referential integrity during testing)
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // add venues
            context.Venues.AddRange(new Venue[] 
            {
                new Venue
                {
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    ContactPhone = "01235678987",
                    Description = "Desc",
                    Image = string.Empty,
                    Facebook = "fb",
                    Instagram = "inst",
                    Twitter = "Tw",
                    LatLong = "0,0",
                    Name = "Cinema 1",
                    Website = "www",
                    Facilities = FacilityFlags.DisabledAccess | FacilityFlags.Parking
                },
                new Venue
                {
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    ContactPhone = "01235678987",
                    Description = "Desc",
                    Image = string.Empty,
                    Facebook = "fb",
                    Instagram = "inst",
                    Twitter = "Tw",
                    LatLong = "0,0",
                    Name = "Cinema 2",
                    Website = "www3",
                    Facilities = FacilityFlags.DisabledAccess | FacilityFlags.Subtitled
                }
            });

            context.SaveChanges();

            // add rooms
            context.Rooms.AddRange(new Room[]
            {
                new Room
                {
                    VenueId = 1,
                    Name = "Screen 1",
                    Description = "A screen...",
                    Columns = 10,
                    Rows = 10,
                    Isles = "{ json: \"sample\" }"                    
                },
                new Room
                {
                    VenueId = 1,
                    Name = "Screen 2",
                    Description = "Another screen...",
                    Columns = 12,
                    Rows = 12,
                    Isles = "{ json: \"sample\" }"
                },
                new Room
                {
                    VenueId = 2,
                    Name = "Screen 1",
                    Description = "A screen...",
                    Columns = 14,
                    Rows = 14,
                    Isles = "{ json: \"sample\" }"
                },
                new Room
                {
                    VenueId = 2,
                    Name = "Screen 2",
                    Description = "Another screen...",
                    Columns = 16,
                    Rows = 16,
                    Isles = "{ json: \"sample\" }"
                }
            });

            context.SaveChanges();

            // add events
            context.Events.AddRange(new Event[]
            {
                new Event
                {
                    Name = "Movie",
                    Description = "A movie, perhaps with a plot. Perhaps not.",
                    Duration = TimeSpan.FromSeconds(60).Ticks,
                    Image = "",
                    AgeRating = AgeRatingType.PEGI_PG
                },
                new Event
                {
                    Name = "Scary Moovy",
                    Description = "Something to do with a cow",
                    Duration = TimeSpan.FromSeconds(120).Ticks,
                    Image = "",
                    AgeRating = AgeRatingType.PEGI_18
                },
                new Event
                {
                    Name = "Yet Another Movie",
                    Description = "Holy Sweet Potato!",
                    Duration = TimeSpan.FromSeconds(240).Ticks,
                    Image = "",
                    AgeRating = AgeRatingType.PEGI_15
                }
            });

            context.SaveChanges();

            // add customers
            context.Customers.AddRange(new Customer[] 
            {
                new Customer
                {
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    ContactEmail = "Test1@test.com",
                    ContactPhone = "01231234567",
                    FirstName = "First",
                    LastName = "Last"
                },
                new Customer
                {
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    ContactEmail = "Test2@test.com",
                    ContactPhone = "01231234567",
                    FirstName = "First",
                    LastName = "Last"
                }
            });

            context.SaveChanges();

            // add pricing strategies
            context.PricingStrategies.AddRange(new PricingStrategy[]
            {
                new PricingStrategy
                {
                    Name = "Default Pricing",
                    Description = "Default pricing for all showings",
                    PricingStrategyItems = new List<PricingStrategyItem>
                    {
                        new PricingStrategyItem { Name = "Adult", Description = "Aged 16 or over", Price = 5.2f },
                        new PricingStrategyItem { Name = "Child", Description = "Aged 15 or under", Price = 4.1f },
                        new PricingStrategyItem { Name = "OAP", Description = "Aged 65 or over", Price = 3.5f },
                    }
                },
                new PricingStrategy
                {
                    Name = "Wednesday Pricing",
                    Description = "Discount pricing for all showings on a Wednesday",
                    PricingStrategyItems = new List<PricingStrategyItem>
                    {
                        new PricingStrategyItem { Name = "Adult (Saver)", Description = "(Special Discount!) Aged 16 or over", Price = 4.2f },
                        new PricingStrategyItem { Name = "Child (Saver)", Description = "(Special Discount!) Aged 15 or under", Price = 3.1f },
                        new PricingStrategyItem { Name = "OAP (Saver)", Description = "(Special Discount!) Aged 65 or over", Price = 2.5f },
                    }
                },
            });

            context.SaveChanges();

            // add showings
            context.Showings.AddRange(new Showing[]
            {
                new Showing { RoomId = 1, EventId = 1, PricingStrategyId = 1, StartTime = DateTime.Parse("02-01-2020 12:00"), EndTime = DateTime.Parse("02-01-2020 12:59") },
                new Showing { RoomId = 1, EventId = 2, PricingStrategyId = 1, StartTime = DateTime.Parse("02-01-2020 13:00"), EndTime = DateTime.Parse("02-01-2020 14:00") },
                new Showing { RoomId = 2, EventId = 3, PricingStrategyId = 2, StartTime = DateTime.Parse("01-01-2020 12:00"), EndTime = DateTime.Parse("01-01-2020 13:00") },
                new Showing { RoomId = 3, EventId = 1, PricingStrategyId = 2, StartTime = DateTime.Parse("01-01-2020 12:00"), EndTime = DateTime.Parse("01-01-2020 12:59") },
                new Showing { RoomId = 3, EventId = 2, PricingStrategyId = 2, StartTime = DateTime.Parse("01-01-2020 13:00"), EndTime = DateTime.Parse("01-01-2020 14:00") },
            });

            context.SaveChanges();

            // add bookings
            context.Bookings.AddRange(new Booking[]
            {
                new Booking
                {
                    CustomerId = 1,
                    BookedDate = DateTime.Parse("01-01-2020 08:00"),
                    Status = BookingStatus.PaymentPending,
                    ShowingId = 1,
                    BookingItems = new List<BookingItem>
                    {
                        new BookingItem { AgreedPriceName = "Adult", AgreedPrice = 5.2f, Location = 0 },
                        new BookingItem { AgreedPriceName = "Adult", AgreedPrice = 5.2f, Location = 1 },
                        new BookingItem { AgreedPriceName = "Adult", AgreedPrice = 5.2f, Location = 2 },
                        new BookingItem { AgreedPriceName = "Adult", AgreedPrice = 5.2f, Location = 3 },
                        new BookingItem { AgreedPriceName = "Adult", AgreedPrice = 5.2f, Location = 4 }
                    }
                },
                new Booking
                {
                    CustomerId = 2,
                    BookedDate = DateTime.Parse("01-01-2020 09:00"),
                    Status = BookingStatus.PaymentPending,
                    ShowingId = 3,
                    BookingItems = new List<BookingItem>
                    {
                        new BookingItem { AgreedPriceName = "Adult (Saver)", AgreedPrice = 4.2f, Location = 12 },
                        new BookingItem { AgreedPriceName = "Adult (Saver)", AgreedPrice = 4.2f, Location = 13 },
                        new BookingItem { AgreedPriceName = "Child (Saver)", AgreedPrice = 3.1f, Location = 14 },
                        new BookingItem { AgreedPriceName = "Child (Saver)", AgreedPrice = 3.1f, Location = 15 },
                    }
                }
            });

            context.SaveChanges();

            // add features
            context.Features.AddRange(new Feature[]
                {
                    new Feature
                    {
                        Title = "New Title 1",
                        Detail = "New Detail 1",
                        Name = "Feature 1",
                        Image = "Image Data Here",
                        Order = 1,
                        Link = "www.something.com"
                    },
                    new Feature
                    {
                        Title = "New Title 2",
                        Detail = "New Detail 2",
                        Name = "Feature 2",
                        Image = "Image Data Here",
                        Order = 2,
                        Link = "www.something-again.com"
                    },
                });

            context.SaveChanges();

            return context;
        }
        
    }
}
