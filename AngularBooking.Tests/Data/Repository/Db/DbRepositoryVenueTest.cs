using AngularBooking.Data;
using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{
    public class DbRepositoryVenueTest : DbRepositoryTestBase
    {
        [Fact]
        public void Should_CreateRepository_ForVenue()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Venue>(context);
                Assert.NotNull(entity);
            }
        }

        [Fact]
        public void Should_AddVenue()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                Venue venue = new Venue
                {
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    ContactPhone = "01234567898",
                    Description = "Test",
                    Name = "Test",
                    Facilities = FacilityFlags.Bar,
                    Image = "",
                    Facebook = "fb",
                    Twitter = "tw",
                    Instagram = "inst",
                    LatLong = "0,0",
                    Website = "www"
                };

                var entity = new DbRepository<Venue>(context);
                bool created = entity.Create(venue);
                Assert.True(created);
            }
        }

        [Fact]
        public void Should_GetVenues()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Venue>(context);

                IQueryable<Venue> venuesInRepository = entity.Get();
                Assert.Equal(2, venuesInRepository.Count());
            }
        }

        [Theory]
        [InlineData(1)]
        public void Should_GetVenueById(int id)
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Venue>(context);
                Venue getVenue = entity.GetById(id);
                Assert.NotNull(getVenue);
            }
        }

        [Fact]
        public void Should_UpdateVenue()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Venue>(context);
                // update 'facilities' flag of record
                var updateVenue = entity.GetById(1);
                updateVenue.Facilities = FacilityFlags.Subtitled;
                entity.Update(updateVenue);

                // check if updated
                var updated = context.Venues.SingleOrDefault(f => f.Id == 1 && f.Facilities == FacilityFlags.Subtitled);

                Assert.NotNull(updated);
            }
        }

        [Fact]
        public void Should_DeleteVenue()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Venue>(context);

                var venue = new Venue { Id = 1 };

                if (venue == null)
                    Assert.True(false);
                
                bool deleted = entity.Delete(venue);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void Should_VenueExist()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Venue>(context);

                var venue = new Venue { Id = 1 };

                if (venue == null)
                    Assert.True(false);

                bool exists = entity.Exists(venue);
                Assert.True(exists);
            }
        }

    }
}
