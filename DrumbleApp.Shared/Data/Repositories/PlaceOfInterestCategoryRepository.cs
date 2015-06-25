using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class PlaceOfInterestCategoryRepository : GenericRepository<PlaceOfInterestCategory>, IPlaceOfInterestCategoryRepository
    {
        public PlaceOfInterestCategoryRepository(CacheContext context)
            : base(context)
        {

        }

        public IEnumerable<Entities.PlaceOfInterestCategory> GetAll()
        {
            return base.DbSet.Select(x => EntityModelFactory.Create(x));
        }

        public void RemoveRange(IEnumerable<Entities.PlaceOfInterestCategory> placeOfInterestCategories)
        {
            if (placeOfInterestCategories == null)
                throw new ArgumentNullException("placeOfInterestCategories");

            base.DbSet.DeleteAllOnSubmit(placeOfInterestCategories.Select(x => DataModelFactory.Create(x)));
        }

        public void InsertRange(IEnumerable<Entities.PlaceOfInterestCategory> placeOfInterestCategories)
        {
            if (placeOfInterestCategories == null)
                throw new ArgumentNullException("placeOfInterestCategories");

            base.DbSet.InsertAllOnSubmit(DataModelFactory.Create(placeOfInterestCategories));
        }

        public Entities.PlaceOfInterestCategory GetByCategory(string category)
        {
            return EntityModelFactory.Create(base.DbSet.Where(x => x.Category == category).SingleOrDefault());
        }

        public void UpdateRange(IEnumerable<Entities.PlaceOfInterestCategory> placeOfInterestCategories)
        {
            foreach (Entities.PlaceOfInterestCategory poi in placeOfInterestCategories)
            {
                Models.PlaceOfInterestCategory poiForUpdate = base.DbSet.Where(x => x.Category == poi.Category).SingleOrDefault();

                if (poiForUpdate != null)
                    poiForUpdate.IsChecked = poi.IsChecked;
            }
        }
    }
}
