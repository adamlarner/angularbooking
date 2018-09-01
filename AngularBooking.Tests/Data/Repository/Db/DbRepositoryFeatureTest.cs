using AngularBooking.Data;
using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Data.Repository.Db
{
    public class DbRepositoryFeatureTest : DbRepositoryTestBase
    {
        [Fact]
        public void Should_CreateRepository_ForFeature()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Feature>(context);
                Assert.NotNull(entity);
            }
        }

        [Fact]
        public void Should_AddFeature()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                Feature feature = new Feature { Name = "Test Feature", Title = "Test Title", Detail = "Test Detail", Link = "Test Link", Image = "Image Data", Order = 1 };

                var entity = new DbRepository<Feature>(context);
                bool created = entity.Create(feature);
                Assert.True(created);
            }
        }

        [Fact]
        public void Should_GetFeatures()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Feature>(context);

                IQueryable<Feature> featuresInRepository = entity.Get();
                Assert.Equal(2, featuresInRepository.Count());
            }
        }

        [Theory]
        [InlineData(1)]
        public void Should_GetFeatureById(int id)
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Feature>(context);
                Feature getFeature = entity.GetById(id);
                Assert.NotNull(getFeature);
            }
        }

        [Fact]
        public void Should_UpdateFeature()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Feature>(context);
                // update 'duration' of record
                var updateFeature = entity.GetById(1);
                updateFeature.Title = "changed title";
                entity.Update(updateFeature);

                // check if updated
                var updated = context.Features.SingleOrDefault(f => f.Id == 1 && f.Title == "changed title");

                Assert.NotNull(updated);
            }
        }

        [Fact]
        public void Should_DeleteFeature()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Feature>(context);

                var feature = new Feature { Id = 1 };

                if (feature == null)
                    Assert.True(false);
                
                bool deleted = entity.Delete(feature);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void Should_FeatureExist()
        {
            using (ApplicationDbContext context = SeedContext())
            {
                var entity = new DbRepository<Feature>(context);

                var feature = new Feature { Id = 1 };

                if (feature == null)
                    Assert.True(false);

                bool exists = entity.Exists(feature);
                Assert.True(exists);
            }
        }

    }
}
