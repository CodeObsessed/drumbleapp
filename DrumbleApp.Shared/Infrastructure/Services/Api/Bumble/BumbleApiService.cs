using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Infrastructure.Services.Api.Common;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Results;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Wrappers;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using DrumbleApp.Shared.Resources;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble
{
    public sealed class BumbleApiService : IBumbleApiService
    {
        private Uri baseUri;
        private Guid appKey;
        private IUnitOfWork unitOfWork;

        // Debug section. Overide Token by giving them values below.
        private Guid overideTokenV1 = Guid.Empty; // Default: Guid.Empty
        private Guid overideTokenV2 = Guid.Empty; // Default: Guid.Empty
        

        // Change version numbers here to point all calls ata certain version.
        private string version1 = "v1"; // Default: v1
        private string version2 = "v2"; // Default: v2

        public BumbleApiService(Uri baseUri, Guid appKey, IUnitOfWork unitOfWork)
        {
            this.baseUri = baseUri;
            this.appKey = appKey;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PublicTransportOperator>> Operators(CancellationToken ct, User user)
        {
            string url = baseUri.ToString() + version1 + "/operators/";

            RestResponse response = await RestService.Get(ct, url, Guid.Empty, ((overideTokenV1 != Guid.Empty) ? overideTokenV1 : user.Token.Value));

            if (response.Success)
            {
                var getOperatorsJsonWrapper = JsonConvert.DeserializeObject<GetOperatorsJsonWrapper>(response.Data);

                return ResultBuilder.BuildOperatorsResult(getOperatorsJsonWrapper.Operators);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }
        }

        // TODO - countries api call
        public async Task<ObservableCollection<Country>> Countries()
        {
            await TaskEx.Delay(1000);
            ObservableCollection<Country> countries = new ObservableCollection<Country>();

            countries.Add(new Country("za", "South Africa"));

            return countries;
        }

        public async Task<Address> ReverseGeoCode(CancellationToken ct, User user, Coordinate location)
        {
            string parameters = "?latitude=" + location.Latitude.ToString(CultureInfo.InvariantCulture);
            parameters += "&longitude=" + location.Longitude.ToString(CultureInfo.InvariantCulture);

            string url = baseUri.ToString() + version1 + "/locations/" + parameters;

            RestResponse response = await RestService.Get(ct, url, Guid.Empty, ((overideTokenV1 != Guid.Empty) ? overideTokenV1 : user.Token.Value));

            if (response.Success)
            {
                // Api stopped returning an error if you are not in Cape Town. (2014/09/22)
                if (String.IsNullOrEmpty(response.Data))
                    return new Address(AppResources.UnkownAddress, AppResources.UnkownAddress, location);

                var reverseGeoCodeJsonWrapper = JsonConvert.DeserializeObject<GeoCodeJsonWrapper>(response.Data);

                if (!reverseGeoCodeJsonWrapper.Locations.Any())
                    return null;

                return new Address(reverseGeoCodeJsonWrapper.Locations.First().Address, reverseGeoCodeJsonWrapper.Locations.First().Name, location);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }
        }

        public async Task<IEnumerable<SearchItem>> Search(CancellationToken ct, User user, string searchText, Coordinate userLocation)
        {
            string parameters = "?searchText=" + HttpUtility.UrlEncode(searchText);

            if (userLocation != null)
            {
                parameters += "&latitude=" + userLocation.Latitude.ToString(CultureInfo.InvariantCulture);
                parameters += "&longitude=" + userLocation.Longitude.ToString(CultureInfo.InvariantCulture);
            }

            string url = baseUri.ToString() + version1 + "/searchitems/" + parameters;

            RestResponse response = await RestService.Get(ct, url, Guid.Empty, ((overideTokenV1 != Guid.Empty) ? overideTokenV1 : user.Token.Value));

            if (response.Success)
            {
                var searchItemsJsonWrapper = JsonConvert.DeserializeObject<SearchItemsJsonWrapper>(response.Data);

                return ResultBuilder.BuildSearchItemsResult(searchItemsJsonWrapper.SearchItems);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }
        }

        public async Task<IEnumerable<Entities.PlaceOfInterest>> PlacesOfInterest(CancellationToken ct, User user, string searchText, string[] categories)
        {
            string parameters = String.Empty;
            if (!String.IsNullOrEmpty(searchText))
                parameters = "?searchtext=" + HttpUtility.UrlEncode(searchText);
            else if (categories != null && categories.Length > 0)
            {
                parameters = "?categories=" + string.Join(",", categories);
            }

            if (user.LastKnownGeneralLocation.IsValid())
                parameters += "&latitude=" + user.LastKnownGeneralLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "&longitude=" + user.LastKnownGeneralLocation.Longitude.ToString(CultureInfo.InvariantCulture);

            string url = baseUri.ToString() + version1 + "/locations/" + parameters;

            RestResponse response = await RestService.Get(ct, url, Guid.Empty, ((overideTokenV1 != Guid.Empty) ? overideTokenV1 : user.Token.Value));

            if (response.Success)
            {
                var pointOfInterestJsonWrapper = JsonConvert.DeserializeObject<PointOfInterestJsonWrapper>(response.Data);

                return ResultBuilder.BuildPlacesOfInterestResult(unitOfWork.PlaceOfInterestCategoryRepository.GetAll().ToList(), pointOfInterestJsonWrapper.Locations);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }
        }

        public async Task<IEnumerable<PathOption>> Path(CancellationToken ct, User user, Coordinate startLocation, Coordinate endLocation, bool isDeparting, DateTime? date, int? timeOffset, List<string> excludedModes, List<string> excludedOperators)
        {
            string parameters = "?startlatitude=" + startLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "&startlongitude=" + startLocation.Longitude.ToString(CultureInfo.InvariantCulture);
            parameters += "&endlatitude=" + endLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "&endlongitude=" + endLocation.Longitude.ToString(CultureInfo.InvariantCulture);

            if (isDeparting)
            {
                if (date != null)
                    parameters += "&startdate=" + date.Value.ToString("o");
                else if (timeOffset != null)
                    parameters += "&startdate=" + DateTime.UtcNow.AddMinutes(timeOffset.Value).ToString("o");
            }
            else
            {
                if (date != null)
                    parameters += "&enddate=" + date.Value.ToString("o");
                else if (timeOffset != null)
                    parameters += "&enddate=" + DateTime.UtcNow.AddMinutes(timeOffset.Value).ToString("o");
            }

            if (excludedModes != null && excludedModes.Count() > 0)
            {
                foreach (string mode in excludedModes)
                    parameters += "&excludedmodes[]=" + mode;
            }

            if (excludedOperators != null && excludedOperators.Count() > 0)
            {
                foreach (string excludedOperator in excludedOperators)
                    parameters += "&excludedoperators[]=" + excludedOperator;
            }

            string url = baseUri.ToString() + version1 + "/paths/" + parameters;

            RestResponse response = await RestService.Get(ct, url, Guid.Empty, ((overideTokenV2 != Guid.Empty) ? overideTokenV2 : user.Token.Value));

            if (response.Success)
            {
#if DEBUG
                Debug.WriteLine(response.Data);
#endif

                var getPathJsonWrapper = JsonConvert.DeserializeObject<GetPathResult>(response.Data);

                return ResultBuilder.BuildPathResult(getPathJsonWrapper.Results, unitOfWork.PublicTransportOperatorRepository.GetAll());
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }
        }

        public async Task<Trip> Trip(CancellationToken ct, User user, Guid tripId)
        {
            string parameters = "?tripId=" + tripId.ToString();

            string url = baseUri.ToString() + version1 + "/trips/" + parameters;

            RestResponse response = await RestService.Get(ct, url, Guid.Empty, user.Token.Value);

            if (response.Success)
            {
                var tripJsonWrapper = JsonConvert.DeserializeObject<TripJsonWrapper>(response.Data);

                return ResultBuilder.BuildTripResult(tripId, tripJsonWrapper);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }
        }

        public async Task<IEnumerable<Announcement>> Announcements(CancellationToken ct, User user, List<string> excludedModes, List<string> excludedOperators, DateTime? afterDate = null)
        {
            string url = baseUri.ToString() + version1 + "/announcements/";

            // Add guid to prevent caching of results.
            url += "?cache=" + Guid.NewGuid().ToString();

            if (excludedModes != null && excludedModes.Count() > 0)
            {
                foreach (string mode in excludedModes)
                    url += "&excludedmodes[]=" + mode;
            }

            if (excludedOperators != null && excludedOperators.Count() > 0)
            {
                foreach (string excludedOperator in excludedOperators)
                    url += "&excludedoperators[]=" + excludedOperator;
            }

            if (afterDate != null)
            {
                url += "&afterdate=" + afterDate.Value.ToString("o");
            }

            RestResponse response = await RestService.Get(ct, url, Guid.Empty, ((overideTokenV2 != Guid.Empty) ? overideTokenV2 : user.Token.Value));

            if (response.Success)
            {
                var announcementJsonWrapper = JsonConvert.DeserializeObject<AnnouncementJsonWrapper>(response.Data);

                IEnumerable<Announcement> announcements = ResultBuilder.BuildAnnouncementsResult(announcementJsonWrapper.Announcements);

                return announcements.Where(x => !excludedModes.Intersect(x.Modes).Any() && !excludedOperators.Contains(x.OperatorName)).OrderByDescending(x => x.StartDate);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }
        }
    }
}
