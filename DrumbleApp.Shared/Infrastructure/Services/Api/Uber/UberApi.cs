using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using DrumbleApp.Shared.ValueObjects;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using DrumbleApp.Shared.Infrastructure.Services.Api.Common;
using System.Threading;
using System.Reflection;
using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Wrappers;
using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model;
using System.Linq;
using System.Net.Http;
using System.Diagnostics;
using DrumbleApp.Shared.Resources;
using System.Net.Http.Headers;
using System.Text;
using System.Net;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber
{
    internal sealed class UberApi
    {
        private Uri baseUri = new Uri("https://api.uber.com/v1");
        private Uri sandboxUri = new Uri("https://sandbox-api.uber.com/v1");

        internal async Task<IEnumerable<UberPrice>> GetPriceEstimates(CancellationToken ct, Coordinate startLocation, Coordinate endLocation)
        {
            string url = baseUri.ToString() + "/estimates/price?server_token=" + serverToken + "&start_latitude=" + startLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "&start_longitude=" + startLocation.Longitude.ToString(CultureInfo.InvariantCulture) + "&end_latitude=" + endLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "&end_longitude=" + endLocation.Longitude.ToString(CultureInfo.InvariantCulture);

            RestResponse response = await RestService.Get(ct, url);

            if (response.Success)
            {
                var getPricesJsonWrapper = JsonConvert.DeserializeObject<UberPriceResultWrapper>(response.Data);

                return ResultBuilder.BuildUberPriceResult(getPricesJsonWrapper.Prices);
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

        internal async Task<UberTime> GetTimeEstimate(CancellationToken ct, Guid productId, Coordinate startLocation)
        {
            string url = baseUri.ToString() + "/estimates/time?server_token=" + serverToken + "&start_latitude=" + startLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "&start_longitude=" + startLocation.Longitude.ToString(CultureInfo.InvariantCulture) + "&product_id=" + productId.ToString();

            RestResponse response = await RestService.Get(ct, url);

            if (response.Success)
            {
                var getTimesJsonWrapper = JsonConvert.DeserializeObject<UberTimeResultWrapper>(response.Data);

                return ResultBuilder.BuildUberTimeResult(getTimesJsonWrapper.Times.FirstOrDefault(x => x.product_id == productId));
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

        internal async Task<UberOAuthResponse> Authenticate(CancellationToken ct, UberOAuthCredentials credentials)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 1, 0); // 1 minute timeout

                HttpResponseMessage response = new HttpResponseMessage();

                try
                {
                    response = await httpClient.PostAsync(credentials.AuthenticationUrl, credentials.GetPostContent(), ct);

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var data = response.Content.ReadAsStringAsync().Result;
                            
                            return JsonConvert.DeserializeObject<UberOAuthResponse>(data);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);

                            return null;
                        }
                    }
                    else
                    {
                        string errorResponseString = response.Content.ReadAsStringAsync().Result;

                        Debug.WriteLine(errorResponseString);

                        return null;
                    }
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("TaskCanceledException");

                    return null;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);

                    return null;
                }
            }
        }

        internal async Task<DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberRequest> PostRequest(CancellationToken ct, string accessToken, Guid productId, Coordinate startLocation, Coordinate endLocation, string surgeConfirmationId)
        {
            string url = sandboxUri.ToString() + "/requests";

            string json = "{\"start_latitude\":\"" + startLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "\"";
            json += ",\"start_longitude\":\"" + startLocation.Longitude.ToString(CultureInfo.InvariantCulture) + "\"";
            json += ",\"product_id\":\"" + productId.ToString() + "\"";
            json += ",\"end_latitude\":\"" + endLocation.Latitude.ToString(CultureInfo.InvariantCulture) + "\"";
            json += ",\"end_longitude\":\"" + endLocation.Longitude.ToString(CultureInfo.InvariantCulture) + "\"";

            if (!String.IsNullOrEmpty(surgeConfirmationId))
                json += ",\"surge_confirmation_id\":\"" + surgeConfirmationId + "\"";

            json += "}";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 1, 0); // 1 minute timeout
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage response = new HttpResponseMessage();

                try
                {
                    
                    response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), ct);
                 
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var data = response.Content.ReadAsStringAsync().Result;

                            // Testing surge pricing.
                            /*string testError = "{ \"meta\": { \"surge_confirmation\": { \"href\": \"https://api.uber.com/v1/surge-confirmations/e100a670\", \"surge_confirmation_id\": \"e100a670\" } }, \"errors\":[ { \"status\": 409, \"code\": \"surge\", \"title\": \"Surge pricing is currently in effect for this product.\" } ] }";
                            var errorResponse = JsonConvert.DeserializeObject<DrumbleApp.Shared.Infrastructure.Services.Api.Uber.ResultModel.UberErrorResponseModel>(testError);

                            var completeResponse = JsonConvert.DeserializeObject<DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberRequest>(data);
                            completeResponse.surge_multiplier = 2.2;
                            completeResponse.surge_multiplier_href = errorResponse.meta.surge_confirmation.href;
                            completeResponse.surge_confirmation_id = errorResponse.meta.surge_confirmation.surge_confirmation_id;*/

                            return JsonConvert.DeserializeObject<DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberRequest>(data);
                        }
                        catch (Exception)
                        {
                            throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                        }
                    }
                    else
                    {
                        string errorResponseString = response.Content.ReadAsStringAsync().Result;

                        Debug.WriteLine(errorResponseString);

                        if (!String.IsNullOrEmpty(errorResponseString))
                        {
                            if (JsonConvert.DeserializeObject<UberError>(errorResponseString).code == "unauthorized")
                                throw new Exception(AppResources.ApiErrorPopupMessageUnauthorizedError);
                            else if (JsonConvert.DeserializeObject<UberError>(errorResponseString).code == "not_found")
                                throw new Exception(AppResources.ApiErrorPopupMessageNotFoundError);
                            else
                            {
                                try
                                {
                                    // Try cast to an uber error response.
                                    var errorResponse = JsonConvert.DeserializeObject<DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberRequest>(errorResponseString);

                                    if (!errorResponse.IsEmpty())
                                    {
                                        return errorResponse;
                                    }
                                }
                                catch (Exception) { }

                                throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                            }
                        }

                        throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (ct.IsCancellationRequested)
                        throw new Exception(AppResources.ApiErrorTaskCancelled);

                    throw new Exception(AppResources.ApiErrorPopupMessageTimeout); 
                }
                catch (Exception e)
                {
                    if (e.Message == AppResources.ApiErrorPopupMessageUnauthorizedError)
                        throw new Exception(AppResources.ApiErrorPopupMessageUnauthorizedError);
                    if (e.Message == AppResources.ApiErrorPopupMessageNotFoundError)
                        throw new Exception(AppResources.ApiErrorPopupMessageNotFoundError);

                    throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                }
            }
        }

        internal async Task<string> PutRequest(CancellationToken ct, string accessToken, Guid requestId, string status)
        {
            string url = sandboxUri.ToString() + "/sandbox/requests/" + requestId.ToString();

            string json = "{\"status\":\"" + status + "\"";

            json += "}";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 1, 0); // 1 minute timeout
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage response = new HttpResponseMessage();

                try
                {

                    response = await httpClient.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), ct);

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var data = response.Content.ReadAsStringAsync().Result;

                            return null;
                        }
                        catch (Exception)
                        {
                            throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                        }
                    }
                    else
                    {
                        string errorResponseString = response.Content.ReadAsStringAsync().Result;

                        Debug.WriteLine(errorResponseString);

                        throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (ct.IsCancellationRequested)
                        throw new Exception(AppResources.ApiErrorTaskCancelled);

                    throw new Exception(AppResources.ApiErrorPopupMessageTimeout);
                }
                catch (Exception)
                {
                    throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                }
            }
        }
        
        internal async Task<DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberRequest> GetRequest(CancellationToken ct, string accessToken, Guid requestId)
        {
            string url = sandboxUri.ToString() + "/requests/" + requestId.ToString();

            url += "?cache=" + Guid.NewGuid().ToString();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 1, 0); // 1 minute timeout
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage response = new HttpResponseMessage();

                try
                {

                    response = await httpClient.GetAsync(url, ct);

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var data = response.Content.ReadAsStringAsync().Result;

                            return JsonConvert.DeserializeObject<DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberRequest>(data);
                        }
                        catch (Exception)
                        {
                            throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                        }
                    }
                    else
                    {
                        string errorResponseString = response.Content.ReadAsStringAsync().Result;

                        Debug.WriteLine(errorResponseString);

                        if (!String.IsNullOrEmpty(errorResponseString))
                        {
                            UberError errorResponse = null;

                            errorResponse = JsonConvert.DeserializeObject<UberError>(errorResponseString);

                            if (errorResponse.code == "unauthorized")
                                throw new Exception(AppResources.ApiErrorPopupMessageUnauthorizedError);
                            else if (errorResponse.code == "not_found")
                                throw new Exception(AppResources.ApiErrorPopupMessageNotFoundError);
                            else
                                throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);

                        }

                        throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (ct.IsCancellationRequested)
                        throw new Exception(AppResources.ApiErrorTaskCancelled);

                    throw new Exception(AppResources.ApiErrorPopupMessageTimeout);
                }
                catch (Exception e)
                {
                    if (e.Message == AppResources.ApiErrorPopupMessageUnauthorizedError)
                        throw new Exception(AppResources.ApiErrorPopupMessageUnauthorizedError);
                    if (e.Message == AppResources.ApiErrorPopupMessageNotFoundError)
                        throw new Exception(AppResources.ApiErrorPopupMessageNotFoundError);

                    throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                }
            }
        }

        internal async Task<DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberMap> GetMap(CancellationToken ct, string accessToken, Guid requestId)
        {
            string url = sandboxUri.ToString() + "/requests/" + requestId.ToString() + "/map";

            url += "?cache=" + Guid.NewGuid().ToString();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 1, 0); // 1 minute timeout
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage response = new HttpResponseMessage();

                try
                {
                    response = await httpClient.GetAsync(url, ct);

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var data = response.Content.ReadAsStringAsync().Result;

                            return JsonConvert.DeserializeObject<DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model.UberMap>(data);
                        }
                        catch (Exception)
                        {
                            throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                        }
                    }
                    else
                    {
                        string errorResponseString = response.Content.ReadAsStringAsync().Result;

                        Debug.WriteLine(errorResponseString);

                        if (!String.IsNullOrEmpty(errorResponseString))
                        {
                            UberError errorResponse = null;

                            errorResponse = JsonConvert.DeserializeObject<UberError>(errorResponseString);

                            if (errorResponse.code == "unauthorized")
                                throw new Exception(AppResources.ApiErrorPopupMessageUnauthorizedError);
                            else if (errorResponse.code == "not_found")
                                throw new Exception(AppResources.ApiErrorPopupMessageNotFoundError);
                            else
                                throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);

                        }

                        throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (ct.IsCancellationRequested)
                        throw new Exception(AppResources.ApiErrorTaskCancelled);

                    throw new Exception(AppResources.ApiErrorPopupMessageTimeout);
                }
                catch (Exception e)
                {
                    if (e.Message == AppResources.ApiErrorPopupMessageUnauthorizedError)
                        throw new Exception(AppResources.ApiErrorPopupMessageUnauthorizedError);
                    if (e.Message == AppResources.ApiErrorPopupMessageNotFoundError)
                        throw new Exception(AppResources.ApiErrorPopupMessageNotFoundError);

                    throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                }
            }
        }
       
        internal async Task<string> PutProduct(CancellationToken ct, string accessToken, Guid productId, double surgeMultiplier)
        {
            string url = sandboxUri.ToString() + "/sandbox/products/" + productId.ToString();

            string json = "{\"surge_multilpier\":" + surgeMultiplier.ToString(CultureInfo.InvariantCulture) + "";

            json += "}";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 1, 0); // 1 minute timeout
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage response = new HttpResponseMessage();

                try
                {

                    response = await httpClient.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), ct);

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var data = response.Content.ReadAsStringAsync().Result;

                            return null;
                        }
                        catch (Exception)
                        {
                            throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                        }
                    }
                    else
                    {
                        string errorResponseString = response.Content.ReadAsStringAsync().Result;

                        Debug.WriteLine(errorResponseString);

                        throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (ct.IsCancellationRequested)
                        throw new Exception(AppResources.ApiErrorTaskCancelled);

                    throw new Exception(AppResources.ApiErrorPopupMessageTimeout);
                }
                catch (Exception)
                {
                    throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                }
            }
        }

        internal async Task<bool> DeleteRequest(CancellationToken ct, string accessToken, Guid requestId)
        {
            string url = sandboxUri.ToString() + "/requests/" + requestId.ToString();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 1, 0); // 1 minute timeout
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage response = new HttpResponseMessage();

                try
                {

                    response = await httpClient.DeleteAsync(url, ct);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (TaskCanceledException)
                {
                    if (ct.IsCancellationRequested)
                        throw new Exception(AppResources.ApiErrorTaskCancelled);

                    throw new Exception(AppResources.ApiErrorPopupMessageTimeout);
                }
                catch (Exception)
                {
                    throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
                }
            }
        }
    }
}
