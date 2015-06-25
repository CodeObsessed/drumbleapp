using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Infrastructure.Services.Api.Common;
using DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel.Models;
using DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel.Results;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Drumble
{
    public sealed class DrumbleApiService : IDrumbleApiService
    {
        private Uri baseUri;
        private Guid appKey;
        private IUnitOfWork unitOfWork;

        public DrumbleApiService(Uri baseUri, Guid appKey, IUnitOfWork unitOfWork)
        {
            this.baseUri = baseUri;
            this.appKey = appKey;
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> RegisterEmail(CancellationToken ct, User user, Email email)
        {
            // TODO
            //string url = baseUri.ToString() + "users/emailaddress";
            await TaskEx.Delay(1000);

            return true;

            /*string url = " https://Tumblegatewaytest.Bumble.co.za/v1/" + "users/emailaddress";

            string json = "{\"token\":\"" + user.Token.ToString() + "\"";
            json += ",\"emailaddress\":\"" + email.EmailAddress + "\"";
            json += "}";

            RestResponse response = await RestService.Post(ct, url, json);

            if (response.Success)
            {
                var result = JsonConvert.DeserializeObject<RegisterResult>(response.Data);

                return true;
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return false;
            }
            else
            {
                throw new Exception(response.Data);
            }*/
        }

        public async Task<User> RegisterFacebook(CancellationToken ct, User user)
        {
            // TODO
            string url = baseUri.ToString() + "users";
            /*await TaskEx.Delay(1000);

             user.IsBumbleRegistered = true;
            user.IsFacebookRegistered = true;

            return user;*/

            //string url = " https://Tumblegatewaytest.Bumble.co.za/v1/" + "users/emailaddress";

            string json = "{\"token\":\"" + user.Token.ToString() + "\"";
            json += ",\"facebookId\":\"" + user.FacebookInfo.FacebookId + "\"";
            json += "}";

            RestResponse response = await RestService.Patch(ct, url, json, Guid.Empty, user.Token.Value);

            if (response.Success)
            {
                var result = JsonConvert.DeserializeObject<RegisterResult>(response.Data);

                user.IsBumbleRegistered = true;
                user.IsFacebookRegistered = true;

                return user;
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return user;
            }
            else
            {
                throw new Exception(response.Data);
            }
        }

        public async Task<User> RegisterTwitter(CancellationToken ct, User user)
        {
            // TODO
            //string url = baseUri.ToString() + "users/emailaddress";
            await TaskEx.Delay(1000);

            user.IsBumbleRegistered = true;
            user.IsTwitterRegistered = true;

            return user;

            /*string url = " https://Tumblegatewaytest.Bumble.co.za/v1/" + "users/emailaddress";

            string json = "{\"token\":\"" + user.Token.ToString() + "\"";
            json += ",\"emailaddress\":\"" + email.EmailAddress + "\"";
            json += "}";

            RestResponse response = await RestService.Post(ct, url, json);

            if (response.Success)
            {
                var result = JsonConvert.DeserializeObject<RegisterResult>(response.Data);

                return true;
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return false;
            }
            else
            {
                throw new Exception(response.Data);
            }*/
        }

        public async Task<Token> RegisterAnonymous(CancellationToken ct)
        {
            string url = baseUri.ToString() + "users/";

            RestResponse response = await RestService.Post(ct, url, String.Empty, appKey, Guid.Empty);

            if (response.Success)
            {
                RegisterResult result = JsonConvert.DeserializeObject<RegisterResult>(response.Data);

                return new Token(Guid.Parse(result.Token));
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

        public async Task<ContactResult> Contact(CancellationToken ct, User user, Email from, string subject, string message)
        {
            string url = baseUri.ToString() + "users/" + user.Token.Value.ToString() + "/messages";

            string json = "{\"message\":" + JsonConvert.ToString(subject + "\n\n" + message) + "}";

            RestResponse response = await RestService.Post(ct, url, json, Guid.Empty, user.Token.Value);

            if (response.Success)
            {
                return JsonConvert.DeserializeObject<ContactResult>(response.Data);
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

        public async Task<User> LoginFacebook(CancellationToken ct, User user)
        {
            await TaskEx.Delay(1000);

             user.IsBumbleRegistered = true;
            user.IsFacebookRegistered = true;

            return user;
            /*string url = baseUri.ToString() + "authentication/login";

            string json = "{\"appkey\":\"" + appKey.ToString() + "\",\"pin\":\"" + onetimePin.PinNumber + "\",\"emailaddress\":\"" + email.EmailAddress + "\"}";

            RestResponse response = await RestService.Post(url, json);

            if (response.Success)
            {
                return JsonConvert.DeserializeObject<IdentifyResult>(response.Data);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }*/
        }

        public async Task<User> LoginTwitter(CancellationToken ct, User user)
        {
            await TaskEx.Delay(1000);

            user.IsBumbleRegistered = true;
            user.IsTwitterRegistered = true;

            return user;
            /*string url = baseUri.ToString() + "authentication/login";

            string json = "{\"appkey\":\"" + appKey.ToString() + "\",\"pin\":\"" + onetimePin.PinNumber + "\",\"emailaddress\":\"" + email.EmailAddress + "\"}";

            RestResponse response = await RestService.Post(url, json);

            if (response.Success)
            {
                return JsonConvert.DeserializeObject<IdentifyResult>(response.Data);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }*/
        }

        public async Task<bool> LoginEmail(CancellationToken ct, Email email)
        {
            // TODO
            //string url = baseUri.ToString() + "users/emailaddress";
            await TaskEx.Delay(1000);

            return true;

            /*string url = " https://Tumblegatewaytest.Bumble.co.za/v1/" + "users/emailaddress";

            string json = "{\"token\":\"" + user.Token.ToString() + "\"";
            json += ",\"emailaddress\":\"" + email.EmailAddress + "\"";
            json += "}";

            RestResponse response = await RestService.Post(ct, url, json);

            if (response.Success)
            {
                var result = JsonConvert.DeserializeObject<RegisterResult>(response.Data);

                return true;
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return false;
            }
            else
            {
                throw new Exception(response.Data);
            }*/
        }

        public async Task<User> Identify(CancellationToken ct, Email email, Pin onetimePin)
        {
            await TaskEx.Delay(1000);
            User user = new User();

            user.IsBumbleRegistered = true;
            user.IsFacebookRegistered = true;

            return user;

            /*string url = baseUri.ToString() + "authentication/login";

            string json = "{\"appkey\":\"" + appKey.ToString() + "\",\"pin\":\"" + onetimePin.PinNumber + "\",\"emailaddress\":\"" + email.EmailAddress + "\"}";

            RestResponse response = await RestService.Post(url, json);

            if (response.Success)
            {
                return JsonConvert.DeserializeObject<IdentifyResult>(response.Data);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }*/
        }

        public async Task<User> Authorise(CancellationToken ct, User user, Email email, Pin onetimePin)
        {
            await TaskEx.Delay(1000);

            user.IsBumbleRegistered = true;

            return user;

            /*string url = baseUri.ToString() + "authentication/login";

            string json = "{\"appkey\":\"" + appKey.ToString() + "\",\"pin\":\"" + onetimePin.PinNumber + "\",\"emailaddress\":\"" + email.EmailAddress + "\"}";

            RestResponse response = await RestService.Post(url, json);

            if (response.Success)
            {
                return JsonConvert.DeserializeObject<IdentifyResult>(response.Data);
            }
            else if (!response.IsException)
            {
                response.Error.HandleError(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                return null;
            }
            else
            {
                throw new Exception(response.Data);
            }*/
        }
    }
}
