using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface ISearchItemCategoryRepository
    {
        IEnumerable<Entities.SearchItemCategory> GetAll();

        void RemoveRange(IEnumerable<Entities.SearchItemCategory> searchItemCategories);

        void InsertRange(IEnumerable<Entities.SearchItemCategory> searchItemCategories);
    }
}
