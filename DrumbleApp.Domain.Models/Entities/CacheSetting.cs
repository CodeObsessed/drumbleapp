using DrumbleApp.Domain.Models.Enums;
using System;

namespace DrumbleApp.Domain.Models.Entities
{
    public sealed class CacheSetting
    {
        public Guid Id { get; private set; }
        public DateTime? LastRefreshedDateUtc { get; set; }
        public ResourceType ResourceType { get; private set; }
        public int CacheValidDurationInSeconds { get; private set; }

        public CacheSetting(DateTime? lastRefreshedDateUtc, ResourceType resourceType)
        {
            this.Id = Guid.NewGuid();
            this.ResourceType = resourceType;
            this.LastRefreshedDateUtc = lastRefreshedDateUtc;
            switch (resourceType)
            {
                case ResourceType.Operators:
                    this.CacheValidDurationInSeconds = 604800; // Valid 2 weeks
                    break;
                default:
                    this.CacheValidDurationInSeconds = 604800; // Valid 1 week by default
                    break;
            }
        }

        public CacheSetting(Guid id, DateTime? lastRefreshedDate, ResourceType resourceType)
            : this (lastRefreshedDate, resourceType)
        {
            this.Id = id;
        }
    }
}
