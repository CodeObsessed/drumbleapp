using DrumbleApp.Domain.Models.Entities;
using System;
using System.Collections.Generic;

namespace DrumbleApp.Domain.Interfaces
{
    public interface IFavouriteRepository
    {
        IEnumerable<Favourite> Favourites { get; }

        bool Insert(Favourite favourite);

        bool Exists(Favourite favourite);

        void Delete(Favourite favourite);

        IEnumerable<Favourite> GetByName(string searchText);

        Favourite GetById(Guid id);
    }
}
