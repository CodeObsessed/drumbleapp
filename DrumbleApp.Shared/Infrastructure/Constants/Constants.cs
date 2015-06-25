using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrumbleApp.Shared.Infrastructure.Constants
{
    public static class Constants
    {
        public static readonly string RequestTokenUri = "https://api.twitter.com/oauth/request_token";
        public static readonly string OAuthVersion = "1.1";
        public static readonly string CallbackUri = "http://www.bing.com";
        public static readonly string AuthorizeUri = "https://api.twitter.com/oauth/authorize";
        public static readonly string AccessTokenUri = "https://api.twitter.com/oauth/access_token";
    }
}
