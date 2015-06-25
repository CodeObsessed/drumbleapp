using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Wrappers;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel
{
    public static class ResultBuilder
    {
        public static IEnumerable<PublicTransportOperator> BuildOperatorsResult(IEnumerable<OperatorResultModel> operators)
        {
            if (operators == null)
                throw new ArgumentNullException("operators");

            return operators.Select(x => ModelFactory.Create(x));
        }

        public static IEnumerable<PublicStop> BuildStopsResult(IEnumerable<StopResultModel> stops)
        {
            if (stops == null)
                throw new ArgumentNullException("stops");

            return stops.Select(x => ModelFactory.Create(x));
        }

        public static Trip BuildTripResult(Guid tripId, TripJsonWrapper tripJsonWrapper)
        {
            if (tripJsonWrapper == null)
                throw new ArgumentNullException("tripJsonWrapper");

            return ModelFactory.Create(tripId, tripJsonWrapper);
        }

        public static IEnumerable<SearchItem> BuildSearchItemsResult(IEnumerable<SearchItemResultModel> searchItems)
        {
            if (searchItems == null)
                return null;

            return searchItems.Select(x => ModelFactory.Create(x));
        }

        public static IEnumerable<Entities.PlaceOfInterest> BuildPlacesOfInterestResult(List<PlaceOfInterestCategory> placeOfInterestCategories, IEnumerable<LocationResultModel> pointsOfInterest)
        {
            if (pointsOfInterest == null)
                throw new ArgumentNullException("pointsOfInterest");

            return pointsOfInterest.Select(x => ModelFactory.Create(placeOfInterestCategories, x));
        }

        public static IEnumerable<PathOption> BuildPathResult(IEnumerable<PathOptionModel> paths, IEnumerable<PublicTransportOperator> publicTransportOperators)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            List<PathOption> results = new List<PathOption>();
            int order = 0;

            foreach (PathOptionModel path in paths)
            {
                results.Add(ModelFactory.Create(path, publicTransportOperators, order));

                order += 1;
            }

            return results;
        }

        public static IEnumerable<Announcement> BuildAnnouncementsResult(IEnumerable<AnnouncementResultModel> announcements)
        {
            if (announcements == null)
                throw new ArgumentNullException("announcements");

            return announcements.Select(x => ModelFactory.Create(x));
        }
    }
}
