using AngularBooking.Data;
using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{
    public class DbRepositoryBookingTest : DbRepositoryTestBase
    {
        [Fact]
        public void Should_CreateRepository_ForBooking()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Booking>(context);
                Assert.NotNull(entity);
            }
        }

        [Fact]
        public void Should_AddBooking()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                Booking booking = new Booking { CustomerId = 1, ShowingId = 1, Status = 0, BookedDate = DateTime.Now };

                var entity = new DbRepository<Booking>(context);
                bool created = entity.Create(booking);
                Assert.True(created);
            }
        }

        [Fact]
        public void Should_GetBookings()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Booking>(context);

                IQueryable<Booking> bookingsInRepository = entity.Get();
                Assert.Equal(2, bookingsInRepository.Count());
            }
        }

        [Theory]
        [InlineData(1)]
        public void Should_GetBookingById(int id)
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Booking>(context);
                Booking getBooking = entity.GetById(id);
                Assert.NotNull(getBooking);
            }
        }

        [Fact]
        public void Should_UpdateBooking()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Booking>(context);
                // update 'paid' status of record
                var updateBooking = entity.GetById(1);
                updateBooking.Status = BookingStatus.PaymentComplete;
                entity.Update(updateBooking);

                // check if updated
                var updated = context.Bookings.SingleOrDefault(f => f.Id == 1 && f.Status == BookingStatus.PaymentComplete);

                Assert.NotNull(updated);
            }
        }

        [Fact]
        public void Should_DeleteBooking()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Booking>(context);

                var booking = new Booking { Id = 1, ShowingId = 1, CustomerId = 1 };

                if (booking == null)
                    Assert.True(false);
                
                bool deleted = entity.Delete(booking);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void Should_BookingExist()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Booking>(context);

                var booking = new Booking { Id = 1 };

                if (booking == null)
                    Assert.True(false);

                bool exists = entity.Exists(booking);
                Assert.True(exists);
            }
        }

    }
}
