using Microsoft.Phone.Controls;
using DrumbleApp.Shared.Messages.Classes;

namespace Drumble.Views
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string pageState;
            if (NavigationContext.QueryString.TryGetValue("pagestate", out pageState))
            {
                if (pageState == "splash")
                {
                    LoginMessage.Send(DrumbleApp.Shared.Messages.Enums.LoginMessageReason.Splash);
                }
                else
                {
                    LoginMessage.Send(DrumbleApp.Shared.Messages.Enums.LoginMessageReason.InApp);
                }
            }
        }
    }
}