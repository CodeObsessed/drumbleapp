using Facebook;
using Facebook.Client;
using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrumbleApp.Shared.Infrastructure.Helpers
{
    public static class Facebook
    {
        public static FacebookSessionClient FacebookSessionClient = new FacebookSessionClient(DrumbleApp.Shared.Infrastructure.Constants.Constants.FacebookAppId);

        public static async Task<FacebookSession> Authenticate()
        {
            return await FacebookSessionClient.LoginAsync("user_about_me");
        }
    }
}
