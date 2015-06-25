using System.Windows.Controls;
using Microsoft.Phone.Controls;
using DrumbleApp.Shared.Messages.Classes;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Devices;
using System;
using DrumbleApp.Shared.ValueObjects;

namespace Drumble.Views
{
    public partial class StationsAndPlacesOfInterest : PhoneApplicationPage
    {
        public StationsAndPlacesOfInterest()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string pageState = String.Empty;
            string point = String.Empty;
            string fromWhereTo = String.Empty;

            if (NavigationContext.QueryString.TryGetValue("state", out pageState))
            {
                NavigationContext.QueryString.TryGetValue("point", out point);
                NavigationContext.QueryString.TryGetValue("fromWhereTo", out fromWhereTo);

                bool fromWhereToPage = false;
                if (fromWhereTo!= null && fromWhereTo.ToLower() == "true")
                    fromWhereToPage = true;

                if (pageState.ToLower() == "search" && (String.IsNullOrEmpty(point) || point.ToLower() == "a"))
                {
                    StationsAndPlacesOfInterestModeMessage.Send(DrumbleApp.Shared.Messages.Enums.StationsAndPlacesOfInterestModeReason.SearchPointA, fromWhereToPage);
                }
                else if (pageState.ToLower() == "search" && point.ToLower() == "b")
                {
                    StationsAndPlacesOfInterestModeMessage.Send(DrumbleApp.Shared.Messages.Enums.StationsAndPlacesOfInterestModeReason.SearchPointB, fromWhereToPage);
                }
                else if (pageState.ToLower() == "stations" && (String.IsNullOrEmpty(point) || point.ToLower() == "a"))
                {
                    StationsAndPlacesOfInterestModeMessage.Send(DrumbleApp.Shared.Messages.Enums.StationsAndPlacesOfInterestModeReason.StationsPointA, fromWhereToPage);
                }
                else if (pageState.ToLower() == "stations" && point.ToLower() == "b")
                {
                    StationsAndPlacesOfInterestModeMessage.Send(DrumbleApp.Shared.Messages.Enums.StationsAndPlacesOfInterestModeReason.StationsPointB, fromWhereToPage);
                }
                else if (pageState.ToLower() == "map" && (String.IsNullOrEmpty(point) || point.ToLower() == "a"))
                {
                    StationsAndPlacesOfInterestModeMessage.Send(DrumbleApp.Shared.Messages.Enums.StationsAndPlacesOfInterestModeReason.MapPointA, fromWhereToPage);
                }
                else if (pageState.ToLower() == "map" && point.ToLower() == "b")
                {
                    StationsAndPlacesOfInterestModeMessage.Send(DrumbleApp.Shared.Messages.Enums.StationsAndPlacesOfInterestModeReason.MapPointB, fromWhereToPage);
                }
                else
                {
                    StationsAndPlacesOfInterestModeMessage.Send(DrumbleApp.Shared.Messages.Enums.StationsAndPlacesOfInterestModeReason.StationsPointA, fromWhereToPage);
                }
            }

            NavigationContext.QueryString.Clear();
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void Map_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GeoCoordinate location;
            bool canConvert = (sender as Map).TryViewportPointToLocation(e.GetPosition(sender as Map), out location);

            VibrateController.Default.Start(TimeSpan.FromMilliseconds(250));

            if (canConvert)
                PointOnMapMessage.Send(Coordinate.FromGeoCoordinate(location));
        }
    }
}