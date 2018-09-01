using AngularBooking.Data;
using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{
    public class DbRepositoryEventTest : DbRepositoryTestBase
    {
        [Fact]
        public void Should_CreateRepository_ForEvent()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Event>(context);
                Assert.NotNull(entity);
            }
        }

        [Fact]
        public void Should_AddEvent()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                Event @event = new Event { Name = "Test Event", Description = "Test Description", AgeRating = AgeRatingType.PEGI_12, Image = "", Duration = 120 };

                var entity = new DbRepository<Event>(context);
                bool created = entity.Create(@event);
                Assert.True(created);
            }
        }

        [Fact]
        public void Should_GetEvents()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Event>(context);

                IQueryable<Event> eventsInRepository = entity.Get();
                Assert.Equal(3, eventsInRepository.Count());
            }
        }

        [Theory]
        [InlineData(1)]
        public void Should_GetEventById(int id)
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Event>(context);
                Event getEvent = entity.GetById(id);
                Assert.NotNull(getEvent);
            }
        }

        [Fact]
        public void Should_UpdateEvent()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Event>(context);
                // update 'duration' of record
                var updateEvent = entity.GetById(1);
                updateEvent.Duration = 360;
                entity.Update(updateEvent);

                // check if updated
                var updated = context.Events.SingleOrDefault(f => f.Id == 1 && f.Duration == 360);

                Assert.NotNull(updated);
            }
        }

        [Fact]
        public void Should_DeleteEvent()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Event>(context);

                var @event = new Event { Id = 1 };

                if (@event == null)
                    Assert.True(false);
                
                bool deleted = entity.Delete(@event);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void Should_EventExist()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Event>(context);

                var @event = new Event { Id = 1 };

                if (@event == null)
                    Assert.True(false);

                bool exists = entity.Exists(@event);
                Assert.True(exists);
            }
        }

    }
}
