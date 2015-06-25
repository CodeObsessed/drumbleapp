using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using Microsoft.Devices;
using DrumbleApp.Shared.ValueObjects;
using DrumbleApp.Shared.Messages.Classes;
using System.Windows;

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
            Map map = (sender as Map);
            Point p = e.GetPosition(map);
            GeoCoordinate location = (map).ConvertViewportPointToGeoCoordinate(p);

            VibrateController.Default.Start(TimeSpan.FromMilliseconds(500));

            MapPointMessage.Send(Coordinate.FromGeoCoordinate(location));
        }
    }
}