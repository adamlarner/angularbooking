using AngularBooking.Data;
using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{
    public class DbRepositoryPricingStrategyTest : DbRepositoryTestBase
    {
        [Fact]
        public void Should_CreateRepository_ForPricingStrategy()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategy>(context);
                Assert.NotNull(entity);
            }
        }

        [Fact]
        public void Should_AddPricingStrategy()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                PricingStrategy pricingStrategy = new PricingStrategy { Name = "Test", Description = "Test" };

                var entity = new DbRepository<PricingStrategy>(context);
                bool created = entity.Create(pricingStrategy);
                Assert.True(created);
            }
        }

        [Fact]
        public void Should_GetPricingStrategys()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategy>(context);

                IQueryable<PricingStrategy> pricingStrategiesInRepository = entity.Get();
                Assert.Equal(2, pricingStrategiesInRepository.Count());
            }
        }

        [Theory]
        [InlineData(1)]
        public void Should_GetPricingStrategyById(int id)
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategy>(context);
                PricingStrategy getPricingStrategy = entity.GetById(id);
                Assert.NotNull(getPricingStrategy);
            }
        }

        [Fact]
        public void Should_UpdatePricingStrategy()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategy>(context);
                // update 'description' status of record
                var updatePricingStrategy = entity.GetById(1);
                updatePricingStrategy.Description = "test changed";
                entity.Update(updatePricingStrategy);

                // check if updated
                var updated = context.PricingStrategies.SingleOrDefault(f => f.Id == 1 && f.Description == "test changed");

                Assert.NotNull(updated);
            }
        }

        [Fact]
        public void Should_DeletePricingStrategy()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategy>(context);

                var pricingStrategy = new PricingStrategy { Id = 1 };

                if (pricingStrategy == null)
                    Assert.True(false);
                
                bool deleted = entity.Delete(pricingStrategy);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void Should_PricingStrategyExist()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<PricingStrategy>(context);

                var pricingStrategy = new PricingStrategy { Id = 1 };

                if (pricingStrategy == null)
                    Assert.True(false);

                bool exists = entity.Exists(pricingStrategy);
                Assert.True(exists);
            }
        }

    }
}
