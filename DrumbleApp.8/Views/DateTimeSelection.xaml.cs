using Microsoft.Phone.Controls;
using DrumbleApp.Shared.Messages.Classes;

namespace Drumble.Views
{
    public partial class DateTimeSelection : PhoneApplicationPage
    {
        public DateTimeSelection()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

#if !DEBUG
            FlurryWP8SDK.Api.LogPageView();
#endif

            string pageState;
            if (NavigationContext.QueryString.TryGetValue("pagestate", out pageState))
            {
                if (pageState == "custom")
                {
                    DepartureTimeSelectionMessage.Send(DrumbleApp.Shared.Messages.Enums.DepartureTimeSelectionMessageReason.Custom);
                }
                else
                {
                    DepartureTimeSelectionMessage.Send(DrumbleApp.Shared.Messages.Enums.DepartureTimeSelectionMessageReason.Interval);
                }
            }

            NavigationContext.QueryString.Clear();
        }
    }
}