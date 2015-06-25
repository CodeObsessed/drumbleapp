
namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class TwitterAccess
    {
        public string AccessToken { get; private set; }
        public string AccessTokenSecret { get; private set; }
        public string UserId { get; private set; }
        public string ScreenName { get; private set; }

        public TwitterAccess(string accessToken, string accessTokenSecret, string userId, string screenName)
        {
            this.AccessToken = accessToken;
            this.AccessTokenSecret = accessTokenSecret;
            this.UserId = userId;
            this.ScreenName = screenName;
        }
    }
}
