using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using DrumbleApp.Shared.Messages.Classes;

namespace Drumble.Views
{
    public partial class SplashScreen : PhoneApplicationPage
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string pageState;
            if (NavigationContext.QueryString.TryGetValue("pagestate", out pageState))
            {
                if (pageState == "facebooklogin")
                {
                    SplashScreenMessage.Send(DrumbleApp.Shared.Messages.Enums.SplashScreenMessageReason.FacebookLogin);
                }
                else if (pageState == "twitterlogin")
                {
                    SplashScreenMessage.Send(DrumbleApp.Shared.Messages.Enums.SplashScreenMessageReason.TwitterLogin);
                }
                else if (pageState == "Bumblelogin")
                {
                    SplashScreenMessage.Send(DrumbleApp.Shared.Messages.Enums.SplashScreenMessageReason.BumbleLogin);
                }
                else if (pageState == "changecountry")
                {
                    SplashScreenMessage.Send(DrumbleApp.Shared.Messages.Enums.SplashScreenMessageReason.ChangeCountry);
                }
                else if (pageState == "resetapp")
                {
                    SplashScreenMessage.Send(DrumbleApp.Shared.Messages.Enums.SplashScreenMessageReason.ResetApp);
                }

                NavigationContext.QueryString.Clear();
            }
        }
    }
}