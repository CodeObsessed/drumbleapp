using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class PathRepository : GenericRepository<Path>, IPathRepository
    {
        public PathRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(DrumbleApp.Shared.Entities.Path path)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(path));
        }

        public IEnumerable<DrumbleApp.Shared.Entities.Path> GetAllCached(IEnumerable<DrumbleApp.Shared.Entities.PublicTransportOperator> publicTransportOperators)
        {
            return base.DbSet.Where(x => x.IsPinned == false).Select(x => EntityModelFactory.Create(x, publicTransportOperators));
        }

        public DrumbleApp.Shared.Entities.Path GetPinned(Guid tripId, IEnumerable<DrumbleApp.Shared.Entities.PublicTransportOperator> publicTransportOperators)
        {
            return base.DbSet.Where(x => x.IsPinned == true && x.TripId == tripId).Select(x => EntityModelFactory.Create(x, publicTransportOperators)).FirstOrDefault();
        }

        public bool ExistsCached()
        {
            return base.DbSet.Any(x => x.StartDate < DateTime.Now && x.EndDate > DateTime.Now && x.IsPinned == false);
        }
    }
}
