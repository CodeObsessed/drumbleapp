using DrumbleApp.Shared.Resources;
using System;
using System.Diagnostics;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Common
{
    public class ErrorResponseModel
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string DeveloperMessage { get; set; }

        public ErrorResponseModel() { }

        public void HandleError(string api, string function)
        {
            switch (ErrorCode)
            {
                case 1050: // Terminated without finding a valid route.
                    throw new Exception(AppResources.ApiErrorPopupMessageNoRouteFound);
                case 1022: // User no longer active.
                    throw new Exception(AppResources.ApiErrorPopupMessageUserAccountDeactivated);
                case 1011: // App update required.
                    throw new Exception(AppResources.ApiErrorPopupMessageUpdateApp);
                case 1040: // Invalid pin.
                    throw new Exception(AppResources.ApiErrorPopupMessageInvalidPin);
                case 1032: // Existing user already authorised.
                    throw new Exception(AppResources.ApiErrorPopupMessageUserAlreadyAuthorised);
                case 1030: // Email not found.
                    throw new Exception(AppResources.ApiErrorPopupMessageEmailNotFound);
                default:
                    Debug.WriteLine("Error response model code not handled - " + api + " / " + function + " / Code: " + this.ErrorCode.ToString());

                    throw new Exception(AppResources.ApiErrorPopupMessageUnknownError);
            }
        }
    }
}
