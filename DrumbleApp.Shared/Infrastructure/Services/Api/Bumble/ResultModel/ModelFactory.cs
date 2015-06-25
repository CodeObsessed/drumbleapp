using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Wrappers;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel
{
    public static class ModelFactory
    {
        public static PublicTransportOperator Create(OperatorResultModel operatorResultModel)
        {
            if (operatorResultModel == null)
            {
                throw new ArgumentNullException("operatorResultModel");
            }

            return new PublicTransportOperator(operatorResultModel.Name, operatorResultModel.DisplayName, operatorResultModel.Category, operatorResultModel.RouteMapUrl, operatorResultModel.TwitterHandle, operatorResultModel.FacebookPage, operatorResultModel.WebsiteAddress, operatorResultModel.ContactEmail, operatorResultModel.ContactNumber, operatorResultModel.IsPublic);
        }

        public static Trip Create(Guid tripId, TripJsonWrapper trip)
        {
            if (trip == null)
            {
                throw new ArgumentNullException("trip");
            }

            return new Trip(tripId, ModelFactory.Create(trip.BoundingBoxTopLeft), ModelFactory.Create(trip.BoundingBoxBottomRight), ModelFactory.Create(trip.Routes));
        }

        public static IEnumerable<TripRoute> Create(IEnumerable<MapRouteResultModel> routes)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            return routes.Select(x => ModelFactory.Create(x));
        }

        public static TripRoute Create(MapRouteResultModel route)
        {
            if (route == null)
            {
                throw new ArgumentNullException("route");
            }

            return new TripRoute(route.Colour, ModelFactory.Create(route.Points));
        }

        public static Entities.PlaceOfInterest Create(List<PlaceOfInterestCategory> placeOfInterestCategories, LocationResultModel locationResultModel)
        {
            if (locationResultModel == null)
                throw new ArgumentNullException("locationResultModel");

            PlaceOfInterestCategory poiCategory;

            if (locationResultModel.ResourceId == null)
            {
                poiCategory = placeOfInterestCategories.Where(x => x.Category == "Unknown").FirstOrDefault();
            }
            else
            {
                poiCategory = placeOfInterestCategories.Where(x => x.Id == locationResultModel.ResourceId).FirstOrDefault();

                if (poiCategory == null)
                    poiCategory = placeOfInterestCategories.Where(x => x.Category == "Unknown").FirstOrDefault();
            }

            return new Entities.PlaceOfInterest(locationResultModel.Name, poiCategory, locationResultModel.Address, new Coordinate(locationResultModel.Point.Latitude, locationResultModel.Point.Longitude), locationResultModel.Distance);
        }

        public static SearchItem Create(SearchItemResultModel searchItemResultModel)
        {
            if (searchItemResultModel == null)
            {
                throw new ArgumentNullException("searchItemResultModel");
            }

            return new SearchItem(searchItemResultModel.Name, searchItemResultModel.Description, searchItemResultModel.Point.Latitude, searchItemResultModel.Point.Longitude, (searchItemResultModel.Distance == null) ? -1 : searchItemResultModel.Distance.Value);
        }

        public static PublicStop Create(StopResultModel stopResultModel)
        {
            if (stopResultModel == null)
            {
                throw new ArgumentNullException("stopResultModel");
            }
            PublicStop modelPublicStop = new PublicStop(stopResultModel.Name, stopResultModel.Operator, stopResultModel.Mode, -1);
            modelPublicStop.StopPoints = ModelFactory.Create(modelPublicStop, stopResultModel.StopLocations);
            return modelPublicStop;
        }

        public static PublicStopPoint Create(PublicStop publicStop, StopPointResultModel stopPointResultModel)
        {
            if (stopPointResultModel == null)
            {
                throw new ArgumentNullException("stopPointResultModel");
            }

            return new PublicStopPoint(stopPointResultModel.Name, stopPointResultModel.Address, publicStop.Id, ModelFactory.Create(stopPointResultModel.Point));
        }

        public static PublicStopPoint Create(StopPointResultModel stopPointResultModel)
        {
            if (stopPointResultModel == null)
                throw new ArgumentNullException("stopPointResultModel");

            return new PublicStopPoint(stopPointResultModel.Name, stopPointResultModel.Address, ModelFactory.Create(stopPointResultModel.Point));
        }

        public static Coordinate Create(PointResultModel pointResultModel)
        {
            if (pointResultModel == null)
                return null;

            return new Coordinate(pointResultModel.Latitude, pointResultModel.Longitude);
        }

        
        public static IEnumerable<Coordinate> Create(IEnumerable<PointResultModel> pointsResultModel)
        {
            if (pointsResultModel == null)
            {
                throw new ArgumentNullException("pointsResultModel");
            }

            List<Coordinate> results = new List<Coordinate>();

            foreach (PointResultModel pointResultModel in pointsResultModel)
            {
                results.Add(ModelFactory.Create(pointResultModel));
            }

            return results;
        }


        public static IEnumerable<PublicStopPoint> Create(PublicStop publicStop, IEnumerable<StopPointResultModel> stopPointsResultModel)
        {
            if (stopPointsResultModel == null)
            {
                throw new ArgumentNullException("stopPointsResultModel");
            }

            List<PublicStopPoint> results = new List<PublicStopPoint>();

            foreach (StopPointResultModel stopPointResultModel in stopPointsResultModel)
            {
                results.Add(ModelFactory.Create(publicStop, stopPointResultModel));
            }

            return results;
        }

        public static PathOption Create(PathOptionModel pathOptionsModel, IEnumerable<PublicTransportOperator> publicTransportOperators, int order)
        {
            if (pathOptionsModel == null)
                throw new ArgumentNullException("pathOptionsModel");

            if (publicTransportOperators == null)
                throw new ArgumentNullException("publicTransportOperators");

            return new PathOption(order, pathOptionsModel.TripId, pathOptionsModel.StartTime, pathOptionsModel.EndTime, pathOptionsModel.EstimatedTotalCost, pathOptionsModel.TotalWalkingDistance, pathOptionsModel.InitialWalkingDistance, ModelFactory.Create(pathOptionsModel.Stages, publicTransportOperators), pathOptionsModel.FareMessages, JsonConvert.SerializeObject(pathOptionsModel));
        }

        public static IEnumerable<Stage> Create(IEnumerable<StageResultModel> stagesResultModel, IEnumerable<PublicTransportOperator> publicTransportOperators)
        {
            if (stagesResultModel == null)
                throw new ArgumentNullException("stagesResultModel");

            List<Stage> results = new List<Stage>();
            int order = 0;
            foreach (StageResultModel stageResultModel in stagesResultModel)
            {
                results.Add(ModelFactory.Create(stageResultModel, publicTransportOperators.Where(x => x.Name == stageResultModel.Operator).FirstOrDefault(), order));

                order += 1;
            }

            return results;
        }

        public static Stage Create(StageResultModel stageResultModel, PublicTransportOperator publicTransportOperator, int order)
        {
            if (stageResultModel == null)
            {
                throw new ArgumentNullException("stageResultModel");
            }

            return new Stage(order, stageResultModel.Name, stageResultModel.Mode, stageResultModel.Operator, stageResultModel.Duration, stageResultModel.Cost, stageResultModel.Colour, stageResultModel.Description, ModelFactory.Create(stageResultModel.StageLocations, stageResultModel.Name, stageResultModel.Colour), ModelFactory.Create(stageResultModel.Announcements));
        }

        public static IEnumerable<Announcement> Create(IEnumerable<AnnouncementResultModel> announcementsResultModel)
        {
            if (announcementsResultModel == null)
                return new List<Announcement>();

            List<Announcement> results = new List<Announcement>();

            foreach (AnnouncementResultModel announcementResultModel in announcementsResultModel)
            {
                results.Add(ModelFactory.Create(announcementResultModel));
            }

            return results;
        }

        public static Announcement Create(AnnouncementResultModel announcementResultModel)
        {
            if (announcementResultModel == null)
                return null;

            return new Announcement(announcementResultModel.Description, announcementResultModel.AnnouncementType, announcementResultModel.Operator, announcementResultModel.StartDate, announcementResultModel.EndDate, ModelFactory.Create(announcementResultModel.Point), announcementResultModel.Modes);
        }

        private static IncidentLocation Create(IncidentLocationResultModel incidentLocationResultModel)
        {
            if (incidentLocationResultModel == null)
                return null;

            return new IncidentLocation(incidentLocationResultModel.RoadWay, incidentLocationResultModel.Direction, incidentLocationResultModel.Location);
        }

        public static IEnumerable<RouteStop> Create(IEnumerable<StagePointResultModel> stagePointsResultModel, string routeName, string routeColour)
        {
            if (stagePointsResultModel == null)
                throw new ArgumentNullException("stagePointsResultModel");

            List<RouteStop> results = new List<RouteStop>();
            int count = 0;

            foreach (StagePointResultModel stagePointResultModel in stagePointsResultModel)
            {
                if (count == 0)
                    results.Add(ModelFactory.Create(stagePointResultModel, routeName, routeColour, Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed));
                else if (count == stagePointsResultModel.Count() - 1)
                    results.Add(ModelFactory.Create(stagePointResultModel, routeName, routeColour, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed));
                else
                    results.Add(ModelFactory.Create(stagePointResultModel, routeName, routeColour, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed));

                count += 1;
            }

            return results;
        }

        public static RouteStop Create(StagePointResultModel stagePointResultModel, string routeName, string routeColour, Visibility arrivalVisibility, Visibility departureVisibility, Visibility intermediateVisibility)
        {
            if (stagePointResultModel == null)
                throw new ArgumentNullException("stagePointResultModel");

            return new RouteStop(ModelFactory.Create(stagePointResultModel), stagePointResultModel.Time.Value, routeColour, routeName, arrivalVisibility, departureVisibility, intermediateVisibility);
        }

        public static PublicStopPoint Create(StagePointResultModel stagePointResultModel)
        {
            if (stagePointResultModel == null)
                throw new ArgumentNullException("stagePointResultModel");

            return new PublicStopPoint(stagePointResultModel.Name, stagePointResultModel.Description, ModelFactory.Create(stagePointResultModel.Point));
        }
    }
}
