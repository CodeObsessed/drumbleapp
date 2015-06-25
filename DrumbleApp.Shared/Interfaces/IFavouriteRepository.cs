using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IFavouriteRepository
    {
        IEnumerable<Entities.Favourite> GetAll();

        IEnumerable<Entities.Favourite> GetRecent();

        bool Insert(Entities.Favourite favourite);

        bool Exists(Entities.Favourite favourite);

        void Delete(Entities.Favourite favourite);

        IEnumerable<Entities.Favourite> GetByName(string searchText);

        Entities.Favourite GetById(System.Guid id);
    }
}
