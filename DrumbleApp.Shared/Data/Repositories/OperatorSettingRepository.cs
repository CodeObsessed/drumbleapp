using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class OperatorSettingRepository : GenericRepository<OperatorSetting>, IOperatorSettingRepository
    {
        public OperatorSettingRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(Entities.OperatorSetting operatorSetting)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(operatorSetting));
        }

        public IEnumerable<Entities.OperatorSetting> GetAll()
        {
            return base.DbSet.Select(x => EntityModelFactory.Create(x));
        }

        public void Update(Entities.OperatorSetting operatorSetting)
        {
            OperatorSetting databaseOperatorSetting = base.DbSet.Single(x => x.Id == operatorSetting.Id);
            databaseOperatorSetting.IsEnabled = operatorSetting.IsEnabled;
            databaseOperatorSetting.HasBeenModified = operatorSetting.HasBeenModified;
        }

        public Entities.OperatorSetting GetByOperator(string name)
        {
            return EntityModelFactory.Create(base.DbSet.SingleOrDefault(x => x.OperatorName == name));
        }
    }
}
