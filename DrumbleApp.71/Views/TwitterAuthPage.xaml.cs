using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using DrumbleApp.Shared.Infrastructure.Constants;
using Hammock.Authentication.OAuth;
using Hammock;
using Hammock.Silverlight.Compat;
using DrumbleApp.Shared.ValueObjects;
using DrumbleApp.Shared.Messages.Classes;

namespace Drumble.Views
{
    public partial class TwitterAuthPage : PhoneApplicationPage
    {
        string _oAuthToken;
        string _oAuthTokenSecret;
        bool isCanceled = true;

        public TwitterAuthPage()
        {
            InitializeComponent();
        }

        private void BrowserControl_Navigating(object sender, NavigatingEventArgs e)
        {
            if (!e.Uri.AbsoluteUri.Contains(Constants.CallbackUri))
                return;
            e.Cancel = true;
            var arguments = e.Uri.AbsoluteUri.Split('?');
            if (arguments.Length < 1)
                return;
            GetAccessToken(arguments[1]);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GetTwitterToken();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (isCanceled)
                TwitterAccessMessage.Send(null, DrumbleApp.Shared.Messages.Enums.TwitterAccessMessageReason.CanceledAuthorisation);
        }

        private void GetTwitterToken()
        {
            var credentials = new OAuthCredentials
            {
                Type = OAuthType.RequestToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = Constants.ConsumerKey,
                ConsumerSecret = Constants.ConsumerKeySecret,
                Version = Constants.OAuthVersion,
                CallbackUrl = Constants.CallbackUri
            };
            var client = new RestClient
            {
                Authority = "https://api.twitter.com/oauth",
                Credentials = credentials,
                HasElevatedPermissions = true,
                SilverlightAcceptEncodingHeader = "gizp",
                DecompressionMethods = DecompressionMethods.GZip,
            };

            var request = new RestRequest
            {
                Path = "/request_token"
            };
            client.BeginRequest(request, new RestCallback(TwitterRequestTokenCompleted));
        }

        private void TwitterRequestTokenCompleted(RestRequest request, RestResponse response, object userstate)
        {
            _oAuthToken = GetQueryParameter(response.Content, "oauth_token");
            _oAuthTokenSecret = GetQueryParameter(response.Content, "oauth_token_secret");

            var authorizeUrl = Constants.AuthorizeUri + "?oauth_token=" + _oAuthToken;
            if (String.IsNullOrEmpty(_oAuthToken) || String.IsNullOrEmpty(_oAuthTokenSecret))
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show("error calling twitter"));
                return;
            }
            Dispatcher.BeginInvoke(() => 
            {
                BrowserControl.Navigate(new Uri(authorizeUrl));
            });
        }

        private static string GetQueryParameter(string input, string parameterName)
        {
            foreach (string item in input.Split('&'))
            {
                var parts = item.Split('=');
                if (parts[0] == parameterName)
                {
                    return parts[1];
                }
            }
            return String.Empty;
        }

        private void GetAccessToken(string uri)
        {
            var requestToken = GetQueryParameter(uri, "oauth_token");
            if (requestToken != _oAuthToken)
            {
                MessageBox.Show("Twitter auth tokens don't match");
            }
            var requestVerifier = GetQueryParameter(uri, "oauth_verifier");
            var credentials = new OAuthCredentials
            {
                Type = OAuthType.AccessToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = Constants.ConsumerKey,
                ConsumerSecret = Constants.ConsumerKeySecret,
                Token = _oAuthToken,
                TokenSecret = _oAuthTokenSecret,
                Verifier = requestVerifier
            };
            var client = new RestClient
            {
                Authority = "https://api.twitter.com/oauth",
                Credentials = credentials,
                HasElevatedPermissions = true
            };
            var request = new RestRequest
            {
                Path = "/access_token"
            };
            client.BeginRequest(request, new RestCallback(RequestAccessTokenCompleted));
        }

        private void RequestAccessTokenCompleted(RestRequest request, RestResponse response, object userstate)
        {
            isCanceled = false;

            string accessToken = GetQueryParameter(response.Content, "oauth_token");
            string accessTokenSecret = GetQueryParameter(response.Content, "oauth_token_secret");
            string userId = GetQueryParameter(response.Content, "user_id");
            string screenName = GetQueryParameter(response.Content, "screen_name");

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(accessTokenSecret))
            {
                TwitterAccess twitterAccess = new TwitterAccess(accessToken, accessTokenSecret, userId, screenName);

                TwitterAccessMessage.Send(twitterAccess, DrumbleApp.Shared.Messages.Enums.TwitterAccessMessageReason.Authorised);
            }
            else
            {
                TwitterAccessMessage.Send(null, DrumbleApp.Shared.Messages.Enums.TwitterAccessMessageReason.FailedAuthorisation);
            }

            Dispatcher.BeginInvoke(() =>
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            });
        }

    }
}