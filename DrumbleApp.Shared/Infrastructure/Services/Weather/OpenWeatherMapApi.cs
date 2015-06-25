using DrumbleApp.Shared.Infrastructure.Services.Api.Common;
using DrumbleApp.Shared.Infrastructure.Services.Weather.ResultModel;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrumbleApp.Shared.Infrastructure.Services.Weather
{
    public sealed class OpenWeatherMapApi : IWeatherApi
    {
        public async Task<DrumbleApp.Shared.Entities.Weather> GetDataWeather (Coordinate location)
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
                        response = await httpClient.GetAsync("http://api.openweathermap.org/data/2.5/weather?apiid=9e25b137bd122c680565cd282de608e1&lat=" + location.Latitude.ToString(CultureInfo.InvariantCulture) + "&lon=" + location.Longitude.ToString(CultureInfo.InvariantCulture));

                        if (response.IsSuccessStatusCode)
                        {
                            try
                            {
                                var data = response.Content.ReadAsStringAsync().Result;

                                var weatherJson = JsonConvert.DeserializeObject<WeatherResultModel>(data);

                                return new DrumbleApp.Shared.Entities.Weather(weatherJson.Weather.First().Icon, DateTime.Now);
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

                            /*if (!String.IsNullOrEmpty(errorResponseString))
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
                                return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageUnknownError));*/
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        //if (ct.IsCancellationRequested)
                       //     return new RestResponse(new Exception(AppResources.ApiErrorTaskCancelled));

                        //return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageTimeout));
                    }
                    catch (Exception)
                    {
                        //return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageUnknownError));
                    }
                }
            }
            else
            {
                //return new RestResponse(new Exception(AppResources.ApiErrorPopupMessageNoNetwork));
            }
            return null;
        }

    }
}
