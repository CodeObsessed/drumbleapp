using DrumbleApp.Shared.Enums;
using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public sealed class CacheSetting : ViewModelBase
    {
        private Guid id;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false)]
        public Guid Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    RaisePropertyChanging("Id");
                    id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        private DateTime? lastRefreshedDateUtc;
        [Column]
        public DateTime? LastRefreshedDateUtc
        {
            get { return lastRefreshedDateUtc; }
            set
            {
                if (lastRefreshedDateUtc != value)
                {
                    RaisePropertyChanging("LastRefreshedDateUtc");
                    lastRefreshedDateUtc = value;
                    RaisePropertyChanged("LastRefreshedDateUtc");
                }
            }
        }

        private ResourceType resourceType;
        [Column]
        public ResourceType ResourceType
        {
            get { return resourceType; }
            set
            {
                if (resourceType != value)
                {
                    RaisePropertyChanging("ResourceType");
                    resourceType = value;
                    RaisePropertyChanged("ResourceType");
                }
            }
        }


        private int cacheValidDurationInSeconds;
        [Column]
        public int CacheValidDurationInSeconds
        {
            get { return cacheValidDurationInSeconds; }
            set
            {
                if (cacheValidDurationInSeconds != value)
                {
                    RaisePropertyChanging("CacheValidDurationInSeconds");
                    cacheValidDurationInSeconds = value;
                    RaisePropertyChanged("CacheValidDurationInSeconds");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public CacheSetting(Guid id, ResourceType resourceType, DateTime? lastRefreshedDateUtc, int cacheValidDurationInSeconds)
        {
            this.Id = id;
            this.ResourceType = resourceType;
            this.LastRefreshedDateUtc = lastRefreshedDateUtc;
            this.CacheValidDurationInSeconds = cacheValidDurationInSeconds;
        }

        public CacheSetting()
        {
        }
    }
}
