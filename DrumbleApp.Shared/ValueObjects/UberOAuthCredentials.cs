using DrumbleApp.Shared.Enums;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class UberOAuthCredentials
    {
        public string ClientId { get; private set; }
        public string Scope { get; private set; }
        public string ClientSecret { get; private set; }
        public string Code { get; private set; }
        public UberOAuthGrantType GrantType { get; private set; }

        public string AuthenticationUrl
        {
            get
            {
                return "https://login.uber.com/oauth/token";
            }
        }

        public UberOAuthCredentials(string clientId, string clientSecret, UberOAuthGrantType grantType)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.Scope = "request";
            this.Code = String.Empty;
            this.GrantType = grantType;
        }

        public void SetCode(string code)
        {
            this.Code = code;
        }

        public HttpContent GetPostContent()
        {
            if (String.IsNullOrEmpty(this.Code))
                throw new ArgumentNullException("Code cannot be empty");

            StringBuilder postData = new StringBuilder();
            postData.Append(HttpUtility.UrlEncode("client_id") + "=" + HttpUtility.UrlEncode(this.ClientId));
            postData.Append("&" + HttpUtility.UrlEncode("client_secret") + "=" + HttpUtility.UrlEncode(this.ClientSecret));
            postData.Append("&" + HttpUtility.UrlEncode("redirect_uri") + "=" + HttpUtility.UrlEncode("http://localhost"));
            postData.Append("&" + HttpUtility.UrlEncode("grant_type") + "=" + HttpUtility.UrlEncode(GrantType.ToString()));
            postData.Append("&" + HttpUtility.UrlEncode("code") + "=" + HttpUtility.UrlEncode(this.Code));

            return new StringContent(postData.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        public Uri GetLoginUri()
        {
            return new Uri(string.Format("https://login.uber.com/oauth/authorize?response_type=code&client_id={0}&scope=request", this.ClientId));
        }
    }
}
