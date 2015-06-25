using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using DrumbleApp.Shared.Messages.Classes;
using Drumble.Resources;
using System.Globalization;

namespace Drumble.Views
{
    public partial class Share : PhoneApplicationPage
    {
        private string shareText = string.Empty;

        public Share()
        {
            InitializeComponent();

            PageTitleMessage.Send(AppResources.HeaderShare);

            //TODO Get the correct share text.
            shareText = string.Format(CultureInfo.CurrentCulture ,"{0} {1}.", AppResources.ShareText, AppResources.ShareLink);
        }

        private void MessageTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SmsComposeTask smsComposeTask = new SmsComposeTask();

            smsComposeTask.Body = shareText;

            smsComposeTask.Show();
        }

        private void EmailTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Body = shareText;

            emailComposeTask.Show();
        }

        private void SocialTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShareStatusTask shareStatusTask = new ShareStatusTask();

            shareStatusTask.Status = shareText;

            shareStatusTask.Show();
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