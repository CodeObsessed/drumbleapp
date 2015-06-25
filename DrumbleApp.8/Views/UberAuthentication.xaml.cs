using Microsoft.Phone.Controls;
using DrumbleApp.Shared.ValueObjects;
using DrumbleApp.Shared.Enums;

namespace Drumble.Views
{
    public partial class UberAuthentication : PhoneApplicationPage
    {
        private UberOAuthCredentials credentials;
            
        public UberAuthentication()
        {
            InitializeComponent();

            this.credentials = new UberOAuthCredentials("-PNH94q8gmuRmb5ZvnEVYO0P-ysj_8-_", "1h-6l1ZVRciE0suS9E5LRjbXvoXg-Q6d3lZfaFxX", UberOAuthGrantType.authorization_code);

            this.LoginControl.Navigate(this.credentials.GetLoginUri());
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

#if !DEBUG
            FlurryWP8SDK.Api.LogPageView();
#endif
        }
    }
}