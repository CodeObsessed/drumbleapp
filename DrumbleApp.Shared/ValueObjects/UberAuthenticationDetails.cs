
namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class UberAuthenticationDetails
    {
        public string AccessToken { get; private set; }
        public string TokenType { get; private set; }
        public int ExpiresIn { get; private set; }
        public string RefreshToken { get; private set; }
        public string Scope { get; private set; }

        public UberAuthenticationDetails(string accessToken, string tokenType, int expiresIn, string refreshToken, string scope)
        {
            this.AccessToken = accessToken;
            this.TokenType = tokenType;
            this.ExpiresIn = expiresIn;
            this.RefreshToken = refreshToken;
            this.Scope = scope;
        }
    }
}
