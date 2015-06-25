/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:TumbleApp"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using DrumbleApp.Shared.Data.Configuration;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Infrastructure.Services.Aggregate;
using DrumbleApp.Shared.Infrastructure.Services.Api.Drumble;
using DrumbleApp.Shared.Infrastructure.Services.Api.Uber;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble;
using DrumbleApp.Shared.Infrastructure.Services.Navigation;
using DrumbleApp.Shared.Infrastructure.Services.Weather;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Models;
using DrumbleApp.Shared.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;

namespace Drumble
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<CacheContext>();

            SimpleIoc.Default.Register<INavigationService, NavigationService>();

            SimpleIoc.Default.Register<IUnitOfWork, UnitOfWork>();

            SimpleIoc.Default.Register<IUberService, UberService>();

            SimpleIoc.Default.Register<IWeatherApi, OpenWeatherMapApi>();

            SimpleIoc.Default.Register<IInMemoryApplicationSettingModel, InMemoryApplicationSettingModel>();
            
            SimpleIoc.Default.Register<IAggregateService, AggregateService>();

            SimpleIoc.Default.Register<IDrumbleApiService>(() =>
            {
                return new DrumbleApiService(new Uri(DatabaseSetup.DrumbleGatewayUrl, UriKind.Absolute), DatabaseSetup.AppKey, SimpleIoc.Default.GetInstance<IUnitOfWork>());
            });

            SimpleIoc.Default.Register<IBumbleApiService>(() =>
            {
                return new BumbleApiService(new Uri(DatabaseSetup.BumbleGatewayUrl, UriKind.Absolute), DatabaseSetup.AppKey, SimpleIoc.Default.GetInstance<IUnitOfWork>());
            });

            SimpleIoc.Default.Register<HeaderViewModel>();

            SimpleIoc.Default.Register<SplashScreenViewModel>();

            SimpleIoc.Default.Register<MapsViewModel>();

            SimpleIoc.Default.Register<MainMenuViewModel>();

            SimpleIoc.Default.Register<WhereToViewModel>();

            SimpleIoc.Default.Register<SettingsViewModel>();
            
            SimpleIoc.Default.Register<ModesViewModel>();

            SimpleIoc.Default.Register<DateTimeSelectionViewModel>();

            SimpleIoc.Default.Register<TripSelectionViewModel>();

            SimpleIoc.Default.Register<FavouritesViewModel>();

            SimpleIoc.Default.Register<TripDetailsViewModel>();

            SimpleIoc.Default.Register<MessageUsViewModel>();

            SimpleIoc.Default.Register<AboutViewModel>();

            SimpleIoc.Default.Register<AnnouncementsViewModel>();

            SimpleIoc.Default.Register<SearchViewModel>();

            SimpleIoc.Default.Register<MapPointSelectionViewModel>();

            SimpleIoc.Default.Register<UberAuthenticationViewModel>();

            SimpleIoc.Default.Register<UberTripDetailsViewModel>();

            SimpleIoc.Default.Register<UberMapViewModel>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public MapPointSelectionViewModel MapPointSelection
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MapPointSelectionViewModel>();
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public MapsViewModel Maps
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MapsViewModel>();
            }

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public AnnouncementsViewModel Announcements
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AnnouncementsViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public SearchViewModel Search
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SearchViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public MainMenuViewModel MainMenu
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainMenuViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public WhereToViewModel WhereTo
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WhereToViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public HeaderViewModel Header
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HeaderViewModel>();
            }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public SplashScreenViewModel SplashScreen
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SplashScreenViewModel>();
            }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public ModesViewModel Modes
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ModesViewModel>();
            }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public DateTimeSelectionViewModel DateTimeSelection
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DateTimeSelectionViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public TripSelectionViewModel TripSelection
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TripSelectionViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public FavouritesViewModel Favourites
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FavouritesViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public TripDetailsViewModel TripDetails
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TripDetailsViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public MessageUsViewModel MessageUs
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MessageUsViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public AboutViewModel About
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AboutViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public UberAuthenticationViewModel UberAuthentication
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UberAuthenticationViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public UberTripDetailsViewModel UberTripDetails
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UberTripDetailsViewModel>();
            }
        }
     
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public UberMapViewModel UberMap
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UberMapViewModel>();
            }
        }

        public static void Cleanup()
        {

        }
    }
}
