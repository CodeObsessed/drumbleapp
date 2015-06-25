using Microsoft.Phone.Controls;
using System;

namespace Drumble.Views
{
    public partial class TripSelection : PhoneApplicationPage
    {
        public TripSelection()
        {
            InitializeComponent();
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