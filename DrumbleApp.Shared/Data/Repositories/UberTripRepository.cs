using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public class UberTripRepository : GenericRepository<UberTrip>, IUberTripRepository
    {
        public UberTripRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(DrumbleApp.Shared.Entities.UberTrip uberTrip)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(uberTrip));
        }

        public IEnumerable<DrumbleApp.Shared.Entities.UberTrip> GetAll()
        {
            return base.DbSet.Select(x => EntityModelFactory.Create(x));
        }

        public bool ExistsCached()
        {
            return base.DbSet.Any();
        }
    }
}
