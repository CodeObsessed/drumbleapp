
namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class FacebookInfo
    {
        public string AccessToken { get; set; }
        public string FacebookId { get; set; }

        public FacebookInfo(string accessToken, string facebookId)
        {
            this.AccessToken = accessToken;
            this.FacebookId = facebookId;
        }
    }
}
