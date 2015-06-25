using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using Microsoft.Devices;
using DrumbleApp.Shared.ValueObjects;
using DrumbleApp.Shared.Messages.Classes;

namespace Drumble.Views
{
    public partial class MapPointSelection : PhoneApplicationPage
    {
        public MapPointSelection()
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

        private void Map_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GeoCoordinate location;
            bool canConvert = (sender as Map).TryViewportPointToLocation(e.GetPosition(sender as Map), out location);

            VibrateController.Default.Start(TimeSpan.FromMilliseconds(500));

            if (canConvert)
                MapPointMessage.Send(Coordinate.FromGeoCoordinate(location));
        }
    }
}