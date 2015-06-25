
namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class TwitterInfo
    {
        public string AccessToken { get; private set; }
        public string AccessTokenSecret { get; private set; }
        public string TwitterId { get; private set; }

        public TwitterInfo(string accessToken, string accessTokenSecret, string twitterId)
        {
            this.AccessToken = accessToken;
            this.AccessTokenSecret = accessTokenSecret;
            this.TwitterId = twitterId;
        }
    }
}
