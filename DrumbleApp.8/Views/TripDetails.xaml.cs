using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Models;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Drumble.Views
{
    public partial class TripDetails : PhoneApplicationPage, IDisposable
    {
        public TripDetails()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Messenger.Default.Register<TripMessage>(this, (action) => DisplayRoute(action));

                // Send a request for the map route to be sent.
                PageLoadMessage.Send(DrumbleApp.Shared.Enums.ApplicationPage.TripDetails);
            };
            Unloaded += (s, e) =>
            {
                Dispose();
            };
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

#if !DEBUG
            FlurryWP8SDK.Api.LogPageView();
#endif

            string tripId;

            if (NavigationContext.QueryString.TryGetValue("tripId", out tripId))
            {
                if (!String.IsNullOrEmpty(tripId))
                {
                    IUnitOfWork unitOfWork = SimpleIoc.Default.GetInstance<IUnitOfWork>();

                    Path pinnedPath = unitOfWork.PathRepository.GetPinned(Guid.Parse(tripId), unitOfWork.PublicTransportOperatorRepository.GetAll());
                    User user = unitOfWork.UserRepository.GetUser();

                    List<PathOption> pathOptions = new List<PathOption>();
                    pathOptions.Add(pinnedPath.PathOption);

                    PathResultsModel results = new PathResultsModel(new Address(pinnedPath.LocationText, pinnedPath.LocationText, pinnedPath.Location), new Address(pinnedPath.DestinationText, pinnedPath.DestinationText, pinnedPath.Destination), pathOptions, SimpleIoc.Default.GetInstance<IBumbleApiService>(), user);

                    SimpleIoc.Default.Unregister<PathResultsModel>();

                    SimpleIoc.Default.Register<PathResultsModel>(() =>
                    {
                        return results;
                    });

                    SimpleIoc.Default.Unregister<TripResultsModel>();

                    SimpleIoc.Default.Register<TripResultsModel>(() =>
                    {
                        return new TripResultsModel();
                    });
                }
            }
           
            NavigationContext.QueryString.Clear();
        }

        private void DisplayRoute(TripMessage tripMessage)
        {
            RouteMap.MapElements.Clear();

            if (tripMessage.Trip.BoundingBoxBottomRight != null && tripMessage.Trip.BoundingBoxTopLeft != null)
            {
                RouteMap.SetView(LocationRectangle.CreateBoundingRectangle(tripMessage.Trip.BoundingBoxBottomRight.GeoCoordinate, tripMessage.Trip.BoundingBoxTopLeft.GeoCoordinate));
            }
           
            foreach (TripRoute tripRoute in tripMessage.Trip.TripRoutes)
            {
                if (tripRoute.Coordinates.Count() >= 2)
                {
                    MapPolyline route = new MapPolyline();

                    route.StrokeColor = ThemeColourConverter.GetBrushFromHex(tripRoute.Colour).Color;
                    route.StrokeThickness = 9;

                    MapPolyline routeBlack = new MapPolyline();

                    routeBlack.StrokeColor = ThemeColourConverter.GetBrushFromHex(ThemeColourConverter.Black).Color;
                    routeBlack.StrokeThickness = 11;

                    foreach (Coordinate coordinate in tripRoute.Coordinates)
                    {
                        route.Path.Add(coordinate.GeoCoordinate);
                        routeBlack.Path.Add(coordinate.GeoCoordinate);
                    }

                    RouteMap.MapElements.Add(routeBlack);
                    RouteMap.MapElements.Add(route);
                }
            }
        }

        private void OnHeaderFlick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Vertical)
            {
                // User flicked towards top
                if (e.VerticalVelocity < 0)
                {
                    FlickMessage.Send(DrumbleApp.Shared.Messages.Enums.FlickMessageReason.FlickedUp, "MapToggle");
                }
                // User flicked towards bottom
                else if (e.VerticalVelocity > 0)
                {
                    FlickMessage.Send(DrumbleApp.Shared.Messages.Enums.FlickMessageReason.FlickedDown, "MapToggle");
                }
            }
        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                // User flicked towards left
                if (e.HorizontalVelocity < 0)
                {
                    FlickMessage.Send(DrumbleApp.Shared.Messages.Enums.FlickMessageReason.FlickedLeft, "MainList");
                }
                // User flicked towards right
                else if (e.HorizontalVelocity > 0)
                {
                    FlickMessage.Send(DrumbleApp.Shared.Messages.Enums.FlickMessageReason.FlickedRight, "MainList");
                }
            }
        }

        private void OnFlickBottom(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                // User flicked towards left
                if (e.HorizontalVelocity < 0)
                {
                    FlickMessage.Send(DrumbleApp.Shared.Messages.Enums.FlickMessageReason.FlickedLeft, "BottomList");
                }
                // User flicked towards right
                else if (e.HorizontalVelocity > 0)
                {
                    FlickMessage.Send(DrumbleApp.Shared.Messages.Enums.FlickMessageReason.FlickedRight, "BottomList");
                }
            }
            else if (e.Direction == System.Windows.Controls.Orientation.Vertical)
            {
                // User flicked towards top
                if (e.VerticalVelocity < 0)
                {
                    FlickMessage.Send(DrumbleApp.Shared.Messages.Enums.FlickMessageReason.FlickedUp, "BottomList");
                }
                // User flicked towards bottom
                else if (e.VerticalVelocity > 0)
                {
                    FlickMessage.Send(DrumbleApp.Shared.Messages.Enums.FlickMessageReason.FlickedDown, "BottomList");
                }
            }
        }

        #region Cleanup

        public void Dispose()
        {
            Messenger.Default.Unregister<TripMessage>(this);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}