using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DrumbleApp.Shared.Infrastructure.Extensions;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Common
{
    internal static class RestService
    {
        public static async Task<RestResponse> Get(CancellationToken ct, string url)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.Timeout = new TimeSpan(0, 1, 0); // 1 minute timeout
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    
                    HttpResponseMessage response = new HttpResponseMessage();

                    try
                    {
                        response = await httpClient.GetAsync(url, ct);

                        if (response.IsSuccessStatusCode)
                        {
                            try
                            {
                                var data = response.Content.ReadAsStringAsync().Result;

                                return new RestResponse(data);
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
                                ErrorResponseModel errorResponse = null;
                                try
                                {
                                    errorResponse = JsonConvert.DeserializeObject<ErrorResponseModel>(errorResponseString);

                                    return new RestResponse(errorResponse);
                                }
                                catch (Exception) { }
                            }

                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageNotFound));
                            }
                            else if (response.StatusCode == HttpStatusCode.BadRequest)
                            {
                                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageBadRequest));
                            }
                            else if (response.StatusCode == HttpStatusCode.InternalServerError)
                            {
                                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageInternalServerError));
                            }
                            else
                                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageUnknownError));
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        if (ct.IsCancellationRequested)
                            return new RestResponse(new Exception(AppResources.ApiErrorTaskCancelled));

                        return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageTimeout));
                    }
                    catch (Exception)
                    {
                        return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageUnknownError));
                    }
                }
            }
            else
            {
                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageNoNetwork));
            }
        }

        public static async Task<RestResponse> Get(CancellationToken ct, string url, Guid appKey, Guid token)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.Timeout = new TimeSpan(0, 1, 0); // 1 minute timeout
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Add("user-agent", UserAgent());

                    if (appKey != Guid.Empty)
                        httpClient.DefaultRequestHeaders.Add("appKey", appKey.ToString());
                    if (token != Guid.Empty)
                        httpClient.DefaultRequestHeaders.Add("token", token.ToString());

                    Coordinate location = UserLocation();
                    if (location != null)
                    {
                        httpClient.DefaultRequestHeaders.Add("latitude", location.Latitude.ToString(CultureInfo.InvariantCulture));
                        httpClient.DefaultRequestHeaders.Add("longitude", location.Longitude.ToString(CultureInfo.InvariantCulture));
                    }

                    HttpResponseMessage response = new HttpResponseMessage();

                    try
                    {
                        response = await httpClient.GetAsync(url, ct);

                        if (response.IsSuccessStatusCode)
                        {
                            try
                            {
                                var data = response.Content.ReadAsStringAsync().Result;

                                return new RestResponse(data);
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
                                ErrorResponseModel errorResponse = null;
                                try
                                {
                                    errorResponse = JsonConvert.DeserializeObject<ErrorResponseModel>(errorResponseString);

                                    return new RestResponse(errorResponse);
                                }
                                catch (Exception) { }
                            }

                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageNotFound));
                            }
                            else if (response.StatusCode == HttpStatusCode.BadRequest)
                            {
                                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageBadRequest));
                            }
                            else if (response.StatusCode == HttpStatusCode.InternalServerError)
                            {
                                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageInternalServerError));
                            }
                            else
                                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageUnknownError));
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        if (ct.IsCancellationRequested)
                            return new RestResponse(new Exception(AppResources.ApiErrorTaskCancelled));

                        return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageTimeout));
                    }
                    catch (Exception)
                    {
                        return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageUnknownError));
                    }
                }
            }
            else
            {
                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageNoNetwork));
            }
        }

        public static async Task<RestResponse> Put(CancellationToken ct, string url, string json, Guid appKey, Guid token, int timeOutInMinutes = 1)
        {
            return await Send(ct, "PUT", url, json, appKey, token, timeOutInMinutes);
        }

        public static async Task<RestResponse> Post(CancellationToken ct, string url, string json, Guid appKey, Guid token, int timeOutInMinutes = 1)
        {
            return await Send(ct, "POST", url, json, appKey, token, timeOutInMinutes);
        }

        public static async Task<RestResponse> Patch(CancellationToken ct, string url, string json, Guid appKey, Guid token, int timeOutInMinutes = 1)
        {
            return await Send(ct, "PATCH", url, json, appKey, token, timeOutInMinutes);
        }

        private static async Task<RestResponse> Send(CancellationToken ct, string method, string url, string json, Guid appKey, Guid token, int timeoutInMinutes)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, timeoutInMinutes, 0); // 1 minute timeout
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("user-agent", UserAgent());

                if (appKey != Guid.Empty)
                    httpClient.DefaultRequestHeaders.Add("appkey", appKey.ToString());
                if (token != Guid.Empty)
                    httpClient.DefaultRequestHeaders.Add("token", token.ToString());

                Coordinate location = UserLocation();
                if (location != null)
                {
                    httpClient.DefaultRequestHeaders.Add("latitude", location.Latitude.ToString(CultureInfo.InvariantCulture));
                    httpClient.DefaultRequestHeaders.Add("longitude", location.Longitude.ToString(CultureInfo.InvariantCulture));
                }

                HttpResponseMessage response = new HttpResponseMessage();

                try
                {
                    if (method == "POST")
                        response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), ct);
                    else if (method == "PUT")
                        response = await httpClient.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), ct);
                    else if (method == "PATCH")
                        response = await httpClient.PatchAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), ct);

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var data = response.Content.ReadAsStringAsync().Result;

                            return new RestResponse(data);
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
                            ErrorResponseModel errorResponse = null;
                            try
                            {
                                errorResponse = JsonConvert.DeserializeObject<ErrorResponseModel>(errorResponseString);

                                return new RestResponse(errorResponse);
                            }
                            catch (Exception) { }
                        }

                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageNotFound));
                        }
                        else if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageBadRequest));
                        }
                        else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageInternalServerError));
                        }
                        else
                            return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageUnknownError));
                    }
                }
                catch (TaskCanceledException)
                {
                    if (ct.IsCancellationRequested)
                        return new RestResponse(new Exception(AppResources.ApiErrorTaskCancelled));

                    return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageTimeout));
                }
                catch (Exception)
                {
                    return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageUnknownError));
                }


            }
        }

        private static string UserAgent()
        {
            var maker = Microsoft.Phone.Info.DeviceStatus.DeviceManufacturer;
            var model = Microsoft.Phone.Info.DeviceStatus.DeviceName;
            return string.Format("{0} {1} {2}", maker, model, "Windows Phone " + Environment.OSVersion.Version.ToString());
        }

        private static Coordinate UserLocation()
        {
            // If the Api is being called from a background task, the SimpleIoc will not be populated.
            if (!SimpleIoc.Default.IsRegistered<IInMemoryApplicationSettingModel>() || !SimpleIoc.Default.IsRegistered<IUnitOfWork>())
                return null;
            
            if (SimpleIoc.Default.GetInstance<IInMemoryApplicationSettingModel>().GetSetting(Enums.ApplicationSetting.StoreLocation) == null || !SimpleIoc.Default.GetInstance<IInMemoryApplicationSettingModel>().GetSetting(Enums.ApplicationSetting.StoreLocation).Value)
                return null;

            IUnitOfWork unitOfWork = SimpleIoc.Default.GetInstance<IUnitOfWork>();

            User user = unitOfWork.UserRepository.GetUser();

            if (user != null && user.LastKnownGeneralLocation != null && user.LastKnownGeneralLocation.IsValid())
                return user.LastKnownGeneralLocation;
            else
                return null;
        }
    }
}
