using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class PublicStopRepository : GenericRepository<PublicStop>, IPublicStopRepository
    {
        public PublicStopRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(Entities.PublicStop publicStop)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(publicStop));
        }

        public void InsertRange(IEnumerable<Entities.PublicStop> publicStops)
        {
            base.DbSet.InsertAllOnSubmit(DataModelFactory.Create(publicStops));
        }

        public IEnumerable<Entities.PublicStop> GetAll()
        {
            // Filter out operators not selected.
            IEnumerable<OperatorSetting> operatorSettings = Context.OperatorSettings.Where(x => x.IsEnabled);

            return base.DbSet.Where(x => operatorSettings.Select(y => y.OperatorName).Contains(x.OperatorName)).OrderBy(x => x.Name).Select(x => EntityModelFactory.Create(x));
        }

        public IEnumerable<Entities.PublicStop> GetAll(int limit)
        {
            // Filter out operators not selected.
            IEnumerable<OperatorSetting> operatorSettings = Context.OperatorSettings.Where(x => x.IsEnabled);

            return base.DbSet.Where(x => operatorSettings.Select(y => y.OperatorName).Contains(x.OperatorName)).OrderBy(x => x.Name).Take(limit).Select(x => EntityModelFactory.Create(x));
        }

        public IEnumerable<Entities.PublicStopPoint> GetPointsForMap()
        {
            // Filter out operators not selected.
            IEnumerable<OperatorSetting> operatorSettings = Context.OperatorSettings.Where(x => x.IsEnabled);

            return base.DbSet.Where(x => operatorSettings.Select(y => y.OperatorName).Contains(x.OperatorName)).Select(x => x.StopPoints.First()).Select(x => EntityModelFactory.CreatePointForMap(x));
        }

        public IEnumerable<Entities.PublicStopPoint> GetNearby(int limit, Coordinate location)
        {
            // Filter out operators not selected.
            IEnumerable<OperatorSetting> operatorSettings = Context.OperatorSettings.Where(x => x.IsEnabled);

            return base.DbSet.Where(x => operatorSettings.Select(y => y.OperatorName).Contains(x.OperatorName)).SelectMany(x => x.StopPoints).ToList().OrderBy(x => x.Location.DistanceToCoordinateInMetres(location)).Take(limit).Select(x => EntityModelFactory.CreatePointForMap(x));
        }

        public IEnumerable<Entities.PublicStop> GetNearby(int limit)
        {
            // Filter out operators not selected.
            IEnumerable<OperatorSetting> operatorSettings = Context.OperatorSettings.Where(x => x.IsEnabled);

            return base.DbSet.Where(x => operatorSettings.Select(y => y.OperatorName).Contains(x.OperatorName)).OrderBy(x => x.DistanceFromUserLocationInMeters).Take(limit).Select(x => EntityModelFactory.Create(x));
        }

        public void UpdateDistanceToAll(Coordinate userLocation)
        {
            if (userLocation == null)
                throw new ArgumentNullException("userLocation");

            foreach (PublicStop publicStop in base.DbSet)
            {
                PublicStopPoint firstPoint = publicStop.StopPoints.FirstOrDefault();
                if (firstPoint != null)
                    publicStop.DistanceFromUserLocationInMeters = (int)userLocation.DistanceToCoordinateInMetres(new Coordinate(firstPoint.Latitude, firstPoint.Longitude));
            }
        }

        public IEnumerable<Entities.PublicStop> GetByName(string searchText)
        {
            // Filter out operators not selected.
            IEnumerable<OperatorSetting> operatorSettings = Context.OperatorSettings.Where(x => x.IsEnabled);

            return base.DbSet.Where(x => x.Name.ToLower().Contains(searchText.ToLower()) && operatorSettings.Select(y => y.OperatorName).Contains(x.OperatorName)).OrderBy(x => x.DistanceFromUserLocationInMeters).Take(30).Select(x => EntityModelFactory.Create(x));
        }

        public Entities.PublicStop FindById(Guid publicStopId)
        {
            return base.DbSet.Where(x => x.Id == publicStopId).Select(x => EntityModelFactory.Create(x)).FirstOrDefault();
        }

        public Entities.PublicStop FindByName(string name)
        {
            return EntityModelFactory.Create(base.DbSet.Where(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault());
        }
    }
}
