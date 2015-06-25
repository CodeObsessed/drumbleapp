using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IPlaceOfInterestCategoryRepository
    {
        IEnumerable<Entities.PlaceOfInterestCategory> GetAll();

        void RemoveRange(IEnumerable<Entities.PlaceOfInterestCategory> placeOfInterestCategories);

        void InsertRange(IEnumerable<Entities.PlaceOfInterestCategory> placeOfInterestCategories);

        Entities.PlaceOfInterestCategory GetByCategory(string category);

        void UpdateRange(IEnumerable<Entities.PlaceOfInterestCategory> placeOfInterestCategories);
    }
}
