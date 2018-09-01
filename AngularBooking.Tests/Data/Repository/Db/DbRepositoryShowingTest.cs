using AngularBooking.Data;
using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{
    public class DbRepositoryShowingTest : DbRepositoryTestBase
    {
        [Fact]
        public void Should_CreateRepository_ForShowing()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Showing>(context);
                Assert.NotNull(entity);
            }
        }

        [Fact]
        public void Should_AddShowing()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                Showing showing = new Showing { EventId = 1, RoomId = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), PricingStrategyId = 1 };

                var entity = new DbRepository<Showing>(context);
                bool created = entity.Create(showing);
                Assert.True(created);
            }
        }

        [Fact]
        public void Should_GetShowings()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Showing>(context);

                IQueryable<Showing> showingsInRepository = entity.Get();
                Assert.Equal(5, showingsInRepository.Count());
            }
        }

        [Theory]
        [InlineData(1)]
        public void Should_GetShowingById(int id)
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Showing>(context);
                Showing getShowing = entity.GetById(id);
                Assert.NotNull(getShowing);
            }
        }

        [Fact]
        public void Should_UpdateShowing()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Showing>(context);
                DateTime modifiedStartTime = DateTime.Parse("02-01-2020 12:00");
                DateTime modifiedEndTime = DateTime.Parse("02-01-2020 12:59");

                // update 'starttime' and 'endtime' status of record
                var updateShowing = entity.GetById(1);

                updateShowing.StartTime = modifiedStartTime;
                updateShowing.EndTime = modifiedEndTime;
                entity.Update(updateShowing);

                // check if updated
                var updated = context.Showings.SingleOrDefault(f => f.Id == 1 && f.StartTime == modifiedStartTime && f.EndTime == modifiedEndTime);

                Assert.NotNull(updated);
            }
        }

        [Fact]
        public void ShouldNot_UpdateShowing_OverlapsNextShowing()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Showing>(context);
                DateTime modifiedStartTime = DateTime.Parse("02-01-2020 12:00");
                DateTime modifiedEndTime = DateTime.Parse("02-01-2020 13:10");

                // update 'starttime' and 'endtime' status of record
                var updateShowing = entity.GetById(1);

                updateShowing.StartTime = modifiedStartTime;
                updateShowing.EndTime = modifiedEndTime;
                entity.Update(updateShowing);

                // check if updated
                var updated = context.Showings.SingleOrDefault(f => f.Id == 1 && f.StartTime == modifiedStartTime && f.EndTime == modifiedEndTime);

                Assert.Null(updated);
            }
        }

        [Fact]
        public void Should_DeleteShowing()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Showing>(context);

                var showing = new Showing { Id = 1 };

                if (showing == null)
                    Assert.True(false);
                
                bool deleted = entity.Delete(showing);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void Should_ShowingExist()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Showing>(context);

                var showing = new Showing { Id = 1 };

                if (showing == null)
                    Assert.True(false);

                bool exists = entity.Exists(showing);
                Assert.True(exists);
            }
        }

    }
}
