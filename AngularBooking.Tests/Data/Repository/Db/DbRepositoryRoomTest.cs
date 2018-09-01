using AngularBooking.Data;
using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{
    public class DbRepositoryRoomTest : DbRepositoryTestBase
    {
        [Fact]
        public void Should_CreateRepository_ForRoom()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Room>(context);
                Assert.NotNull(entity);
            }
        }

        [Fact]
        public void Should_AddRoom()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                Room room = new Room { Name = "Test", Description = "Test", VenueId = 1, Rows = 10, Columns = 10, Isles = "{}" };

                var entity = new DbRepository<Room>(context);
                bool created = entity.Create(room);
                Assert.True(created);
            }
        }

        [Fact]
        public void Should_GetRooms()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Room>(context);

                IQueryable<Room> roomsInRepository = entity.Get();
                Assert.Equal(4, roomsInRepository.Count());
            }
        }

        [Theory]
        [InlineData(1)]
        public void Should_GetRoomById(int id)
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Room>(context);
                Room getRoom = entity.GetById(id);
                Assert.NotNull(getRoom);
            }
        }

        [Fact]
        public void Should_UpdateRoom()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Room>(context);
                // update 'description' of record
                var updateRoom = entity.GetById(1);
                updateRoom.Description = "Test Changed";
                entity.Update(updateRoom);

                // check if updated
                var updated = context.Rooms.SingleOrDefault(f => f.Id == 1 && f.Description == "Test Changed");

                Assert.NotNull(updated);
            }
        }

        [Fact]
        public void Should_DeleteRoom()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Room>(context);

                var room = new Room { Id = 1 };

                if (room == null)
                    Assert.True(false);
                
                bool deleted = entity.Delete(room);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void Should_RoomExist()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Room>(context);

                var room = new Room { Id = 1 };

                if (room == null)
                    Assert.True(false);

                bool exists = entity.Exists(room);
                Assert.True(exists);
            }
        }

    }
}
