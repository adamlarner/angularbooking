using AngularBooking.Data;
using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{
    public class DbRepositoryBookingItemTest : DbRepositoryTestBase
    {
        [Fact]
        public void Should_CreateRepository_ForBookingItem()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<BookingItem>(context);
                Assert.NotNull(entity);
            }
        }

        [Fact]
        public void Should_AddBookingItem()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                BookingItem BookingItem = new BookingItem { BookingId = 1, AgreedPriceName = "Adult Test", AgreedPrice = 1f, Location = 24 };

                var entity = new DbRepository<BookingItem>(context);
                bool created = entity.Create(BookingItem);
                Assert.True(created);
            }
        }

        [Fact]
        public void Should_GetBookingItems()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<BookingItem>(context);

                IQueryable<BookingItem> bookingItemsInRepository = entity.Get();
                Assert.Equal(9, bookingItemsInRepository.Count());
            }
        }

        [Theory]
        [InlineData(1)]
        public void Should_GetBookingItemById(int id)
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<BookingItem>(context);
                BookingItem getBookingItem = entity.GetById(id);
                Assert.NotNull(getBookingItem);
            }
        }

        [Fact]
        public void Should_UpdateBookingItem()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<BookingItem>(context);

                // update 'agreedprice' of record
                var updateBookingItem = entity.GetById(1);
                updateBookingItem.AgreedPrice = 2;
                entity.Update(updateBookingItem);

                // check if updated
                var updated = context.BookingItems.SingleOrDefault(f => f.Id == 1 && f.AgreedPrice == 2);

                Assert.NotNull(updated);
            }
        }

        [Fact]
        public void Should_DeleteBookingItem()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<BookingItem>(context);

                var bookingItem = new BookingItem { Id = 1 };

                if (bookingItem == null)
                    Assert.True(false);
                
                bool deleted = entity.Delete(bookingItem);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void Should_BookingItemExist()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<BookingItem>(context);

                var bookingItem = new BookingItem { Id = 1 };

                if (bookingItem == null)
                    Assert.True(false);

                bool exists = entity.Exists(bookingItem);
                Assert.True(exists);
            }
        }

    }
}
