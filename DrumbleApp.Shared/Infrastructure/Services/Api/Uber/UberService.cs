using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber
{
    public sealed class UberService : IUberService
    {
        private UberApi uberApi = new UberApi();

         
        public UberService()
        {

        }

        public async Task<UberOption> GetUberOption(CancellationToken ct, Coordinate startLocation, Coordinate endLocation)
        {
            IEnumerable<UberPrice> uberPricesNearby = await uberApi.GetPriceEstimates(ct, startLocation, endLocation);

            if (!uberPricesNearby.Any())
            {
                return null;
            }

            UberPrice uberPrice = uberPricesNearby.FirstOrDefault();

            UberTime uberTime = await uberApi.GetTimeEstimate(ct, uberPrice.ProductId, startLocation);

            if (uberTime == null)
            {
                return null;
            }

            return new UberOption(uberPrice.ProductId, uberPrice.CurrencyCode, uberPrice.PriceEstimate, uberPrice.LowEstimate, uberPrice.HighEstimate, uberPrice.DurationInSeconds, uberPrice.DistanceInMeters, uberTime.TimeEstimateInSeconds, ClientId);
        }

        public async Task<UberAuthenticationDetails> Authenticate(CancellationToken ct, UberOAuthCredentials credentials)
        {
            UberOAuthResponse response = await uberApi.Authenticate(ct, credentials);

            if (response == null)
                return null;

            return new UberAuthenticationDetails(response.access_token, response.token_type, response.expires_in, response.refresh_token, response.scope);
        }

        public async Task<ValueObjects.UberRequest> PostUberRequest(CancellationToken ct, string accessToken, Guid productId, Coordinate startLocation, Coordinate endLocation, string surgeConfirmationId)
        {
            DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberRequest uberRequest = await uberApi.PostRequest(ct, accessToken, productId, startLocation, endLocation, surgeConfirmationId);

            if (uberRequest == null)
            {
                return null;
            }

            Uri surgeMultiplierHref = null;
            if (!String.IsNullOrEmpty(uberRequest.surge_multiplier_href))
            {
                surgeMultiplierHref = new Uri(uberRequest.surge_multiplier_href);
            }

            return new ValueObjects.UberRequest(Guid.Parse(uberRequest.request_id), uberRequest.status, uberRequest.surge_multiplier, uberRequest.eta, null, null, null, surgeMultiplierHref, uberRequest.surge_confirmation_id);
        }

        public async Task<string> PutUberRequest(CancellationToken ct, string accessToken, Guid requestId, string status)
        {
            string uberRequest = await uberApi.PutRequest(ct, accessToken, requestId, status);

            return null;
        }

        public async Task<string> PutUberProduct(CancellationToken ct, string accessToken, Guid productId, double surgeMultilpier)
        {
            string uberRequest = await uberApi.PutProduct(ct, accessToken, productId, surgeMultilpier);

            return null;
        }

        public async Task<ValueObjects.UberRequest> GetUberRequest(CancellationToken ct, string accessToken, Guid requestId)
        {
            DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberRequest uberRequest = await uberApi.GetRequest(ct, accessToken, requestId);

            if (uberRequest == null)
            {
                return null;
            }

            ValueObjects.UberDriver uberDriver = null;

            if (uberRequest.driver != null)
            {
                uberDriver = new ValueObjects.UberDriver(uberRequest.driver.phone_number, uberRequest.driver.rating, new Uri(uberRequest.driver.picture_url), uberRequest.driver.name);
            }

            ValueObjects.UberVehicle uberVehicle = null;

            if (uberRequest.vehicle != null)
            {
                uberVehicle = new ValueObjects.UberVehicle(uberRequest.vehicle.make, uberRequest.vehicle.model, uberRequest.vehicle.license_plate, new Uri(uberRequest.vehicle.picture_url));
            }

            ValueObjects.Coordinate uberLocation = null;

            if (uberRequest.location != null)
            {
                uberLocation = new ValueObjects.Coordinate(uberRequest.location.latitude, uberRequest.location.longitude);
            }

            Uri surgeMultiplierHref = null;
            if (!String.IsNullOrEmpty(uberRequest.surge_multiplier_href))
            {
                surgeMultiplierHref = new Uri(uberRequest.surge_multiplier_href);
            }

            return new ValueObjects.UberRequest(Guid.Parse(uberRequest.request_id), uberRequest.status, uberRequest.surge_multiplier, uberRequest.eta, uberVehicle, uberDriver, uberLocation, surgeMultiplierHref, uberRequest.surge_confirmation_id);
        }

        public async Task<ValueObjects.UberMap> GetUberMap(CancellationToken ct, string accessToken, Guid requestId)
        {
            DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberMap uberMap = await uberApi.GetMap(ct, accessToken, requestId);

            if (uberMap == null)
            {
                return null;
            }

            return new ValueObjects.UberMap(new Uri(uberMap.href));
        }

        public async Task<bool> DeleteUberRequest(CancellationToken ct, string accessToken, Guid requestId)
        {
            return await uberApi.DeleteRequest(ct, accessToken, requestId);
        }

    }
}
