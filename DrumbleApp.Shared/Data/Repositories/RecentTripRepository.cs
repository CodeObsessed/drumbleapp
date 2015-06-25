using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;
using DrumbleApp.Shared.Data.Factories;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class RecentTripRepository: GenericRepository<Recent>, IRecentTripRepository
    {
        public RecentTripRepository(CacheContext context)
            : base(context)
        {
            
        }

        public IEnumerable<Entities.Recent> GetAll()
        {
            return base.DbSet.OrderByDescending(x => x.LastUsedDate).Select(x => EntityModelFactory.Create(x));
        }

        public Entities.Recent GetMostRecent()
        {
            return EntityModelFactory.Create(base.DbSet.OrderByDescending(x => x.LastUsedDate).FirstOrDefault());
        }

        public Entities.Recent GetMostFrequent()
        {
            return EntityModelFactory.Create(base.DbSet.OrderByDescending(x => x.NumberOfUses).FirstOrDefault());
        }

        public void Insert(Entities.Recent recentTrip)
        {
            if (base.DbSet.Any(x => x.Latitude == recentTrip.Point.Latitude && x.Longitude == recentTrip.Point.Longitude))
            {
                Recent existingRecentTrip = base.DbSet.Where(x => x.Latitude == recentTrip.Point.Latitude && x.Longitude == recentTrip.Point.Longitude).FirstOrDefault();

                existingRecentTrip.NumberOfUses += 1;
                existingRecentTrip.LastUsedDate = recentTrip.LastUsedDate;
            }
            else
            {
                base.DbSet.InsertOnSubmit(DataModelFactory.Create(recentTrip));
            }
        }

        public IEnumerable<Entities.Recent> GetByName(string searchText)
        {
            return base.DbSet.Where(x => x.Text.ToLower().Contains(searchText.ToLower())).OrderByDescending(x => x.LastUsedDate).Select(x => EntityModelFactory.Create(x));
        }

        public void Update(Entities.Recent recentTrip)
        {
            Recent recentTripForUpdate = base.DbSet.Where(x => x.Id == recentTrip.Id).FirstOrDefault();

            recentTripForUpdate.IsFavourite = recentTrip.IsFavourite;
        }

        public Entities.Recent GetById(System.Guid id)
        {
            return EntityModelFactory.Create(base.DbSet.Where(x => x.Id == id).FirstOrDefault());
        }

        public override void RemoveAll()
        {
            base.DbSet.DeleteAllOnSubmit<Recent>(base.DbSet.Where(x => !x.IsFavourite));
        }
    }
}
