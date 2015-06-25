using DrumbleApp.Infrastructure.Models;
using System.Data.Linq;

namespace DrumbleApp.Infrastructure.DataEntities
{
    public class CacheContext : DataContext
    {
        public CacheContext()
            : base("Data Source=isostore:/Drumblecache.sdf")
        {
        }

        public Table<AppSettingTable> AppSettings { get; set; }
        //public Table<User> Users;
        //public Table<PublicTransportOperator> PublicTransportOperators;
        //public Table<CacheSetting> CacheSettings;
        //public Table<About> Abouts;
        //public Table<TransportMode> TransportModes;
        //public Table<OperatorSetting> OperatorSettings;
        //public Table<SearchItemCategory> SearchItemCategories;
        //public Table<Recent> RecentTrips;
        //public Table<PlaceOfInterestCategory> PlaceOfInterestCategories;
        //public Table<Weather> Weathers;
        //public Table<Path> Paths;
    }
}
