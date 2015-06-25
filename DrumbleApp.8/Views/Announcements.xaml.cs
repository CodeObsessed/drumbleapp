using Microsoft.Phone.Controls;
using DrumbleApp.ShakeGestures;
using System;
using DrumbleApp.Shared.Messages.Classes;

namespace Drumble.Views
{
    public partial class Announcements : PhoneApplicationPage
    {
        public Announcements()
        {
            InitializeComponent();

            // register shake event
            ShakeGesturesHelper.Instance.ShakeGesture += new EventHandler<ShakeGestureEventArgs>(Instance_ShakeGesture);

            ShakeGesturesHelper.Instance.Active = true;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

#if !DEBUG
            FlurryWP8SDK.Api.LogPageView();
#endif
        }

        private void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        {
            ShakeGestureMessage.SendMessage();
        }
    }
}