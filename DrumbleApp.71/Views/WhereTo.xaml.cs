using Microsoft.Phone.Controls;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using DrumbleApp.Shared.Entities;
using System;

namespace Drumble.Views
{
    public partial class WhereTo : PhoneApplicationPage
    {
        public WhereTo()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id;

            if (NavigationContext.QueryString.TryGetValue("favouriteId", out id))
            {
                if (!String.IsNullOrEmpty(id))
                {
                    IUnitOfWork unitOfWork = SimpleIoc.Default.GetInstance<IUnitOfWork>();

                    Favourite favourite = unitOfWork.FavouriteRepository.GetById(Guid.Parse(id));

                    FavouriteMessage.Send(favourite, DrumbleApp.Shared.Messages.Enums.FavouriteMessageReason.SetAsWhereTo);
                }
            }
            else if (NavigationContext.QueryString.TryGetValue("recentTripId", out id))
            {
                if (!String.IsNullOrEmpty(id))
                {
                    IUnitOfWork unitOfWork = SimpleIoc.Default.GetInstance<IUnitOfWork>();

                    Recent recentTrip = unitOfWork.RecentTripRepository.GetById(Guid.Parse(id));

                    RecentTripMessage.Send(recentTrip, DrumbleApp.Shared.Messages.Enums.RecentTripMessageReason.SetAsWhereToDestination);
                }
            }

            if (NavigationContext.QueryString.TryGetValue("userId", out id))
            {
                if (!String.IsNullOrEmpty(id))
                {
#if !DEBUG
                    FlurryWP8SDK.Api.SetUserId(id);
#endif
                }
            }
#if !DEBUG
            FlurryWP8SDK.Api.LogPageView();
#endif

            NavigationContext.QueryString.Clear();
        }
    }
}