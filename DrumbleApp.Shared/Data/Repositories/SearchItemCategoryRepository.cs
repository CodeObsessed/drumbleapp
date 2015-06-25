using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class SearchItemCategoryRepository : GenericRepository<SearchItemCategory>, ISearchItemCategoryRepository
    {
        public SearchItemCategoryRepository(CacheContext context)
            : base(context)
        {

        }

        public IEnumerable<Entities.SearchItemCategory> GetAll()
        {
            return base.DbSet.Select(x => EntityModelFactory.Create(x));
        }

        public void RemoveRange(IEnumerable<Entities.SearchItemCategory> searchItemCategories)
        {
            if (searchItemCategories == null)
                throw new ArgumentNullException("searchItemCategories");

            base.DbSet.DeleteAllOnSubmit(searchItemCategories.Select(x => DataModelFactory.Create(x)));
        }

        public void InsertRange(IEnumerable<Entities.SearchItemCategory> searchItemCategories)
        {
            if (searchItemCategories == null)
                throw new ArgumentNullException("searchItemCategories");

            base.DbSet.InsertAllOnSubmit(DataModelFactory.Create(searchItemCategories));
        }
    }
}
