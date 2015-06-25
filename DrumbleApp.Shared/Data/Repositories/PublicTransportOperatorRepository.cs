using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class PublicTransportOperatorRepository : GenericRepository<PublicTransportOperator>, IPublicTransportOperatorRepository
    {
        public PublicTransportOperatorRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(DrumbleApp.Shared.Entities.PublicTransportOperator publicTransportOperator)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(publicTransportOperator));
        }

        public void InsertRange(IEnumerable<DrumbleApp.Shared.Entities.PublicTransportOperator> publicTransportOperators)
        {
            base.DbSet.InsertAllOnSubmit(DataModelFactory.Create(publicTransportOperators));
        }

        public IEnumerable<DrumbleApp.Shared.Entities.PublicTransportOperator> GetAll()
        {
            return base.DbSet.OrderBy(x => x.Name).Select(x => EntityModelFactory.Create(x));
        }

        public Entities.PublicTransportOperator GetById(Guid operatorId)
        {
            return EntityModelFactory.Create(base.DbSet.SingleOrDefault(x => x.Id == operatorId));
        }
    }
}
