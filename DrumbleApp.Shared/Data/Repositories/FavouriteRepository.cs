using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class FavouriteRepository : GenericRepository<Recent>, IFavouriteRepository
    {
        public FavouriteRepository(CacheContext context)
            : base(context)
        {
            
        }

        public IEnumerable<Entities.Favourite> GetAll()
        {
            return base.DbSet.Where(x => x.IsFavourite).OrderByDescending(x => x.LastUsedDate).Select(x => EntityModelFactory.CreateFavourite(x));
        }

        public IEnumerable<Entities.Favourite> GetRecent()
        {
            return base.DbSet.Where(x => x.IsFavourite).OrderByDescending(x => x.LastUsedDate).Take(3).Select(x => EntityModelFactory.CreateFavourite(x));
        }

        public bool Exists(Entities.Favourite favourite)
        {
            if (base.DbSet.Any(x => x.IsFavourite && x.Latitude == favourite.Point.Latitude && x.Longitude == favourite.Point.Longitude))
            {
                return true;
            }

            return false;
        }

        public bool Insert(Entities.Favourite favourite)
        {
            if (Exists(favourite))
                return false;

            Recent existingRecentTrip = base.DbSet.Where(x => x.Latitude == favourite.Point.Latitude && x.Longitude == favourite.Point.Longitude).FirstOrDefault();

            if (existingRecentTrip != null)
            {
                existingRecentTrip.IsFavourite = true;
            }
            else
            {
                base.DbSet.InsertOnSubmit(DataModelFactory.Create(favourite));
            }

            return true;
        }

        public void Delete(Entities.Favourite favourite)
        {
            Recent recentTripForDelete = base.DbSet.Where(x => x.Id == favourite.Id).FirstOrDefault();

            recentTripForDelete.IsFavourite = false;
        }

        public IEnumerable<Entities.Favourite> GetByName(string searchText)
        {
            return base.DbSet.Where(x => x.IsFavourite && (x.Text.ToLower().Contains(searchText.ToLower()))).OrderByDescending(x => x.LastUsedDate).Select(x => EntityModelFactory.CreateFavourite(x));
        }

        public Entities.Favourite GetById(System.Guid id)
        {
            return EntityModelFactory.CreateFavourite(base.DbSet.Where(x => x.Id == id).FirstOrDefault());
        }
    }
}
