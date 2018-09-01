using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Data
{
    public class SeedDataProvider
    {
        public static void Generate(ApplicationDbContext context, int days = 7, int rooms = 5, int minCols = 6, int maxCols = 12, int minRows = 6, int maxRows = 12)
        {
            // create demo data for booking system
            Random random = new Random();

            // cache initial start time, so that all showings/bookings are accurately ahead of this time
            DateTime now = DateTime.Now;

            // then create entities which don't have strict relational dependencies

            // features
            List<Feature> features = new List<Feature>()
            {
                new Feature
                {
                    Name = "Saver Wednesday Feature",
                    Title = " ",
                    Detail = " ",
                    Link = "#",
                    Image = ImageToBase64("Resources/images/feature/feature1.png"),
                    Order = 2
                },
                new Feature
                {
                    Name = "Booking Showcase",
                    Title = " ",
                    Detail = " ",
                    Link = "https://github.com/adamlarner/angularbooking",
                    Image = ImageToBase64("Resources/images/feature/feature2.png"),
                    Order = 1
                },
            };

            context.AddRange(features);
            context.SaveChanges();

            // events
            List<Event> events = new List<Event>()
            {
                new Event
                {
                    Name = "Office Party - The Movie",
                    Description = "Jeff and his chums have decided to host the most epic office party ever! Featuring photocopiers, staplers, and the occasional post-it note, this movie offers all the amazing shenanigans that you'd expect around the water cooler, and more!",
                    AgeRating = AgeRatingType.PEGI_12A,
                    Duration = 210,
                    Image = ImageToBase64("Resources/images/event/Comedy-1_Advert.jpg")
                },
                new Event
                {
                    Name = "Feeding Time",
                    Description = "When Mittens decides that tinned food isn't to his liking, he instead looks at his owner and friends for an alternative diet...",
                    AgeRating = AgeRatingType.PEGI_18,
                    Duration = 124,
                    Image = ImageToBase64("Resources/images/event/Horror-1_Advert.jpg")
                },
                new Event
                {
                    Name = "The journey to the Moon",
                    Description = "A never before seen take on the first moon landing, featuring plastic figurines and terrible voice acting!",
                    AgeRating = AgeRatingType.PEGI_Universal,
                    Duration = 162,
                    Image = ImageToBase64("Resources/images/event/Documentary-1_Advert.jpg")
                }
            };

            context.Events.AddRange(events);
            context.SaveChanges();

            //venues
            List<Venue> venues = new List<Venue>()
            {
                new Venue
                {
                    Name = "Arty Theater",
                    Description = "This artistic-looking building provides all the necessary features required for watching the latest and greatest film releases. At least in theory; the only showings nowadays feature evil cats and plastic astronauts.",
                    Address1 = "123 Sample Street",
                    Address2 = "Sample Town",
                    Address3 = "Sample City",
                    Address4 = "Sample Region",
                    Address5 = "Sample Postcode",
                    ContactPhone = "0123 456 7890",
                    Website = "http://www.website.com",
                    Facilities = FacilityFlags.AudioDescribed | FacilityFlags.Bar | FacilityFlags.Parking | FacilityFlags.Toilets,
                    Facebook = "http://www.facebook.com",
                    Twitter = "http://www.twitter.com",
                    Instagram = "http://www.instagram.com",
                    Image = ImageToBase64("Resources/images/venue/Arty Theater.jpg"),
                    LatLong = "#"
                },
                new Venue
                {
                    Name = "Blue Theater",
                    Description = "Don't let the garish colour's fool you, for this theater boasts not one, but two subtitle language settings! The screen is also a little makeshift, on account of the theater not really being designed for showing films...",
                    Address1 = "Blue Street",
                    Address2 = "Blueberryville",
                    Address3 = "Blue City",
                    Address4 = "Blueshire",
                    Address5 = "B1 BBB",
                    ContactPhone = "0123 131 1313",
                    Website = "http://www.blueblueblue.com",
                    Facilities = FacilityFlags.Bar | FacilityFlags.Toilets | FacilityFlags.Parking | FacilityFlags.Subtitled | FacilityFlags.DisabledAccess,
                    Facebook = "http://www.facebook.com",
                    Twitter = "http://www.twitter.com",
                    Instagram = "http://www.instagram.com",
                    Image = ImageToBase64("Resources/images/venue/Blue Theater.jpg"),
                    LatLong = "#"
                },
                new Venue
                {
                    Name = "Concrete Theater",
                    Description = "Made from pure concrete, this theater has fantastic acoustic properties! Due to limited access, this theater is not wheelchair friendly. It also smells quite bad...",
                    Address1 = "1 Concrete Theater",
                    Address2 = "Industrial Zone",
                    Address3 = "Gritty",
                    Address4 = "Gray City",
                    Address5 = "GR1 IND",
                    ContactPhone = "0123 111 2233",
                    Website = "http://www.graygraygray.com",
                    Facilities = FacilityFlags.AudioDescribed | FacilityFlags.Bar | FacilityFlags.Toilets,
                    Facebook = "http://www.facebook.com",
                    Twitter = "http://www.twitter.com",
                    Instagram = "http://www.instagram.com",
                    Image = ImageToBase64("Resources/images/venue/Concrete Theater.jpg"),
                    LatLong = "#"
                },
                new Venue
                {
                    Name = "Run-down Theater",
                    Description = "This super-budget theater complex has focused all of it's budget on the interior, and thus looks fairly run-down on the outside. It has a functioning toilet, and there's an off-license down the road. Very classy!",
                    Address1 = "999 Dump Street",
                    Address2 = "Grimeville",
                    Address3 = "Dirt County",
                    Address4 = "Crumbling City",
                    Address5 = "CR1 APP",
                    ContactPhone = "999",
                    Website = "http://www.crackymccrackface.com",
                    Facilities = FacilityFlags.Parking | FacilityFlags.Toilets | FacilityFlags.DisabledAccess | FacilityFlags.GuideDogsPermitted,
                    Facebook = "http://www.facebook.com",
                    Twitter = "http://www.twitter.com",
                    Instagram = "http://www.instagram.com",
                    Image = ImageToBase64("Resources/images/venue/Rundown Theater.jpg"),
                    LatLong = "#"
                }
            };

            context.Venues.AddRange(venues);
            context.SaveChanges();

            // pricing
            PricingStrategy defaultPricing = new PricingStrategy
            {
                Name = "Default Pricing",
                Description = "Default Pricing",
                PricingStrategyItems = new List<PricingStrategyItem>
                {
                    new PricingStrategyItem { Name = "Adult", Description = "Adult admission fee", Price = 8.99f },
                    new PricingStrategyItem { Name = "Child", Description = "Child admission fee", Price = 6.99f },
                    new PricingStrategyItem { Name = "OAP", Description = "Old-aged Pensioner (Over 65) admission fee", Price = 5.99f },
                    new PricingStrategyItem { Name = "Student", Description = "Student admission fee (ID required)", Price = 5.99f }
                }
            };

            PricingStrategy saverPricing = new PricingStrategy
            {
                Name = "Saver Pricing",
                Description = "Discount Pricing for Wednesday",
                PricingStrategyItems = new List<PricingStrategyItem>
                {
                    new PricingStrategyItem { Name = "Adult", Description = "Adult admission fee", Price = 6.99f },
                    new PricingStrategyItem { Name = "Child", Description = "Child admission fee", Price = 4.99f },
                    new PricingStrategyItem { Name = "OAP", Description = "Old-aged Pensioner (Over 65) admission fee", Price = 3.99f },
                    new PricingStrategyItem { Name = "Student", Description = "Student admission fee (ID required)", Price = 3.99f }
                }
            };

            context.PricingStrategies.AddRange(defaultPricing, saverPricing);
            context.SaveChanges();

            // create 5 screens for each venue, and fill each screen with different film showings
            for (int i = 0; i < venues.Count; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int rows = random.Next(6, 12);
                    int columns = random.Next(6, 12);

                    Room newRoom = new Room
                    {
                        VenueId = venues[i].Id,
                        Name = $"Screen {j + 1}",
                        Description = $"Basic Screen, with a total capacity of {rows * columns} seats.",
                        Rows = rows,
                        Columns = columns,
                        Isles = $"{{ \"rows\": [2, {rows - 2}], \"columns\": [2, {columns - 2}] }}"
                    };

                    context.Rooms.Add(newRoom);
                    context.SaveChanges();

                    // create showings for next 7 days
                    List<Showing> showings = new List<Showing>();
                    for(int k = 0; k < 7; k++)
                    {
                        DateTime currentDate = now.AddDays(k);

                        // generate random event id
                        int currentEventId = random.Next(1, 4);

                        // create daily showings (3 hour slots, starting at 9:00)
                        for (int l = 9; l < 23; l += 3)
                        {
                            Showing showing = new Showing
                            {
                                EventId = currentEventId,
                                PricingStrategyId = currentDate.DayOfWeek == DayOfWeek.Wednesday ? 2 : 1, // discount pricing for wednesday
                                RoomId = newRoom.Id,
                                StartTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, l, 0, 0),
                                EndTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, l+2, 40, 0) // 20 minute interlude for cleaning
                            };

                            showings.Add(showing);
                        }
                    }
                    // bulk add
                    context.AddRange(showings);
                    context.SaveChanges();
                }

            }
            
        }

        private static string ImageToBase64(string path)
        {
            //System.IO.Path.DirectorySeparatorChar
            if (!File.Exists(path))
                return string.Empty;

            byte[] bytes = File.ReadAllBytes(path);

            // create DataUrl using file extension
            string converted = $"data:image/{ Path.GetExtension(path).TrimStart('.') };base64,{ Convert.ToBase64String(bytes) }";

            return converted;
        }
    }
}
