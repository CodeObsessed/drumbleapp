
namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class UberInfo
    {
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        public UberInfo(string accessToken, string refreshToken)
        {
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
        }
    }
}
