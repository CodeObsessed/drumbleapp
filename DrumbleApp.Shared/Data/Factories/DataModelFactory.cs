using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace DrumbleApp.Shared.Data.Factories
{
    public static class DataModelFactory
    {
        public static TransportMode Create(Entities.TransportMode transportMode)
        {
            if (transportMode == null)
            {
                throw new ArgumentNullException("transportMode");
            }

            return new TransportMode(transportMode.ApplicationTransportMode, transportMode.IsEnabled);
        }

        public static OperatorSetting Create(Entities.OperatorSetting operatorSetting)
        {
            if (operatorSetting == null)
            {
                throw new ArgumentNullException("operatorSetting");
            }

            return new OperatorSetting(operatorSetting.Id, operatorSetting.OperatorName, operatorSetting.IsEnabled, operatorSetting.HasBeenModified);
        }

        public static PlaceOfInterestCategory Create(DrumbleApp.Shared.Entities.PlaceOfInterestCategory placeOfInterestCategory)
        {
            if (placeOfInterestCategory == null)
                throw new ArgumentNullException("placeOfInterestCategory");

            return new PlaceOfInterestCategory(placeOfInterestCategory.Id, placeOfInterestCategory.Category, placeOfInterestCategory.ImageBinary);
        }

        public static SearchItemCategory Create(DrumbleApp.Shared.Entities.SearchItemCategory searchItemCategory)
        {
            if (searchItemCategory == null)
            {
                throw new ArgumentNullException("searchItemCategory");
            }

            return new SearchItemCategory(searchItemCategory.Id, searchItemCategory.ImageBinary, searchItemCategory.Text);
        }

        public static AppSetting Create(DrumbleApp.Shared.Entities.AppSetting appSetting)
        {
            if (appSetting == null)
            {
                throw new ArgumentNullException("appSetting");
            }

            return new AppSetting(appSetting.ApplicationSetting, appSetting.Value);
        }

        public static User Create(DrumbleApp.Shared.Entities.User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return new User(user.Id, user.Token.Value, (user.Country == null) ? null : user.Country.Code, (user.Country == null) ? null : user.Country.Name, user.DismissedLocationPopup, (user.LastKnownGeneralLocation == null) ? Double.NaN : user.LastKnownGeneralLocation.Latitude, (user.LastKnownGeneralLocation == null) ? Double.NaN : user.LastKnownGeneralLocation.Longitude, user.LastLocationUpdate, user.DismissedRateAppPopup, user.DismissedSignUpPopup, user.IsBumbleRegistered, (user.Email == null) ? string.Empty : user.Email.EmailAddress, user.IsFacebookRegistered, user.IsTwitterRegistered, user.FirstName, user.LastName, (user.FacebookInfo == null) ? null : user.FacebookInfo.FacebookId, (user.FacebookInfo == null) ? null : user.FacebookInfo.AccessToken, (user.TwitterInfo == null) ? null : user.TwitterInfo.AccessToken, (user.TwitterInfo == null) ? null : user.TwitterInfo.AccessTokenSecret, (user.TwitterInfo == null) ? null : user.TwitterInfo.TwitterId, user.TwitterHandle, user.AppUsageCount, user.DismissedLoginUberPopup, (user.UberInfo == null) ? null : user.UberInfo.AccessToken, (user.UberInfo == null) ? null : user.UberInfo.RefreshToken, user.IsUberAuthenticated);
        }

        public static IEnumerable<PlaceOfInterestCategory> Create(IEnumerable<DrumbleApp.Shared.Entities.PlaceOfInterestCategory> placeOfInterestCategories)
        {
            if (placeOfInterestCategories == null)
                throw new ArgumentNullException("placeOfInterestCategories");

            return placeOfInterestCategories.Select(x => Create(x));
        }

        public static IEnumerable<SearchItemCategory> Create(IEnumerable<DrumbleApp.Shared.Entities.SearchItemCategory> searchItemCategories)
        {
            if (searchItemCategories == null)
            {
                throw new ArgumentNullException("searchItemCategories");
            }

            return searchItemCategories.Select(x => Create(x));
        }

        public static PublicTransportOperator Create(DrumbleApp.Shared.Entities.PublicTransportOperator publicTransportOperator)
        {
            if (publicTransportOperator == null)
            {
                throw new ArgumentNullException("publicTransportOperator");
            }

            return new PublicTransportOperator(publicTransportOperator.Id, publicTransportOperator.Name, publicTransportOperator.DisplayName, publicTransportOperator.Category, publicTransportOperator.RouteMapUrl, publicTransportOperator.TwitterHandle, publicTransportOperator.FacebookPage, publicTransportOperator.WebsiteAddress, publicTransportOperator.ContactEmail, publicTransportOperator.ContactNumber, publicTransportOperator.IsPublic);
        }

        public static IEnumerable<PublicTransportOperator> Create(IEnumerable<DrumbleApp.Shared.Entities.PublicTransportOperator> publicTransportOperators)
        {
            if (publicTransportOperators == null)
            {
                throw new ArgumentNullException("publicTransportOperators");
            }

            return publicTransportOperators.Select(x => Create(x));
        }

        public static CacheSetting Create(DrumbleApp.Shared.Entities.CacheSetting cacheSetting)
        {
            if (cacheSetting == null)
            {
                throw new ArgumentNullException("cacheSetting");
            }

            return new CacheSetting(cacheSetting.Id, cacheSetting.ResourceType, cacheSetting.LastRefreshedDateUtc, cacheSetting.CacheValidDurationInSeconds);
        }

        public static Weather Create(DrumbleApp.Shared.Entities.Weather weather)
        {
            if (weather == null)
                return null;

            return new Weather(weather.Id, weather.IconCode, weather.LastRefreshedDate);
        }

        public static Recent Create(Entities.Recent recentTrip)
        {
            if (recentTrip == null)
                throw new ArgumentNullException("recentTrip");

            return new Recent(recentTrip.Id, recentTrip.Point, recentTrip.Text, recentTrip.CreatedDate, recentTrip.LastUsedDate, recentTrip.NumberOfUses, recentTrip.IsFavourite);
        }

        public static Recent Create(Entities.Favourite favourite)
        {
            if (favourite == null)
                throw new ArgumentNullException("favourite");

            return new Recent(favourite.Id, favourite.Point, favourite.Text, favourite.CreatedDate, favourite.LastUsedDate, favourite.NumberOfUses, true);
        }

        public static Path Create(Entities.Path path)
        {
            if (path == null)
                return null;

            return new Path(path.TripId, path.LocationText, path.DestinationText, path.Location, path.Destination, path.StartDate, path.EndDate, path.IsPinned, path.JsonObject, path.Order);
        }

        public static UberTrip Create(Entities.UberTrip uberTrip)
        {
            if (uberTrip == null)
                return null;

            return new UberTrip(uberTrip.Id, uberTrip.RequestId, uberTrip.Location, uberTrip.Destination, uberTrip.CreatedDate);
        }
    }
}
