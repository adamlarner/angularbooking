using AngularBooking.Data;
using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{
    public class DbRepositoryPricingStrategyItemTest : DbRepositoryTestBase
    {
        [Fact]
        public void Should_CreateRepository_ForPricingStrategyItem()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategyItem>(context);
                Assert.NotNull(entity);
            }
        }

        [Fact]
        public void Should_AddPricingStrategyItem()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                PricingStrategyItem pricingStrategyItem = new PricingStrategyItem { Name = "Test", Description = "Test", Price = 1, PricingStrategyId = 1 };

                var entity = new DbRepository<PricingStrategyItem>(context);
                bool created = entity.Create(pricingStrategyItem);
                Assert.True(created);
            }
        }

        [Fact]
        public void Should_GetPricingStrategyItems()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategyItem>(context);

                IQueryable<PricingStrategyItem> pricingStrategyItemsInRepository = entity.Get();
                Assert.Equal(6, pricingStrategyItemsInRepository.Count());
            }
        }

        [Theory]
        [InlineData(1)]
        public void Should_GetPricingStrategyItemById(int id)
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategyItem>(context);
                PricingStrategyItem getPricingStrategyItem = entity.GetById(id);
                Assert.NotNull(getPricingStrategyItem);
            }
        }

        [Fact]
        public void Should_UpdatePricingStrategyItem()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategyItem>(context);
                // update 'price' value of record
                var updatePricingStrategyItem = entity.GetById(1);
                updatePricingStrategyItem.Price = 10;
                entity.Update(updatePricingStrategyItem);

                // check if updated
                var updated = context.PricingStrategyItems.SingleOrDefault(f => f.Id == 1 && f.Price == 10);

                Assert.NotNull(updated);
            }
        }

        [Fact]
        public void Should_DeletePricingStrategyItem()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategyItem>(context);

                var pricingStrategyItem = new PricingStrategyItem { Id = 1 };

                if (pricingStrategyItem == null)
                    Assert.True(false);
                
                bool deleted = entity.Delete(pricingStrategyItem);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void Should_PricingStrategyItemExist()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategyItem>(context);

                var pricingStrategyItem = new PricingStrategyItem { Id = 1 };

                if (pricingStrategyItem == null)
                    Assert.True(false);

                bool exists = entity.Exists(pricingStrategyItem);
                Assert.True(exists);
            }
        }

    }
}
