using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class TransportModeRepository : GenericRepository<TransportMode>, ITransportModeRepository
    {
        public TransportModeRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(DrumbleApp.Shared.Entities.TransportMode transportMode)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(transportMode));
        }

        public IEnumerable<DrumbleApp.Shared.Entities.TransportMode> GetAll()
        {
            return base.DbSet.Select(x => EntityModelFactory.Create(x));
        }

        public DrumbleApp.Shared.Entities.TransportMode FindByType(ApplicationTransportMode applicationTransportMode)
        {
            return base.DbSet.Where(x => x.ApplicationTransportMode == applicationTransportMode).Select(x => EntityModelFactory.Create(x)).Single();
        }

        public void Update(DrumbleApp.Shared.Entities.TransportMode transportMode)
        {
            TransportMode databaseTransportMode = base.DbSet.Single(x => x.ApplicationTransportMode == transportMode.ApplicationTransportMode);
            databaseTransportMode.IsEnabled = transportMode.IsEnabled;
        }
    }
}
