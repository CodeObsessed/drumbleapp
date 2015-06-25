using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class WeatherRepository : GenericRepository<Weather>, IWeatherRepository
    {
        public WeatherRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(DrumbleApp.Shared.Entities.Weather weather)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(weather));
        }

        public DrumbleApp.Shared.Entities.Weather Get()
        {
            return EntityModelFactory.Create(base.DbSet.FirstOrDefault());
        }

        public void DeleteAll()
        {
            base.DbSet.DeleteAllOnSubmit(base.DbSet);
        }
    }
}
