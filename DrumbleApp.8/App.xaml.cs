using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Drumble.Resources;
using GalaSoft.MvvmLight.Threading;
using System.Device.Location;
using DrumbleApp.Shared.Enums;
using GalaSoft.MvvmLight.Messaging;
using DrumbleApp.Shared.ValueObjects;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using System.Linq;
using Drumble;
using DrumbleApp.Shared.Data.Configuration;

namespace DrumbleApp._8
{
    public partial class App : Application
    {
        // Setup the watcher
        private GeoCoordinateWatcher watcher;
        private GpsWatcherState gpsWatcherStatus = GpsWatcherState.Stopped;
        private bool runGpsWatcherContinuously = false;

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Get the custom theme
            //var rd = App.Current.Resources.MergedDictionaries[0];

            // Set custom Theme
            //ThemeManager.SetCustomTheme(rd, Theme.Light);

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = false;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            Messenger.Default.Register<CustomPopupMessage>(this, (action) => DisplayCustomPopupMessage(action));

            Messenger.Default.Register<CustomPopupMessageWithAction>(this, (action) => DisplayCustomPopupMessageWithAction(action));

            Messenger.Default.Register<AppCommandMessage>(this, (action) => RemoveBackEntries(action));

            Messenger.Default.Register<GpsWatcherMessage>(this, (action) => SetGpsWatcher(action));

            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "e168f5c3-930c-46b5-b381-9c83ca0bd66c";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "VbJBf9k-qL9VIwnCjFkr6A";
        }

        private static void RemoveBackEntries(AppCommandMessage appCommandMessage)
        {
            if (appCommandMessage.Reason == AppCommandMessageReason.RemoveBackEntries)
            {
                while ((Application.Current.RootVisual as PhoneApplicationFrame).BackStack.Any())
                    (Application.Current.RootVisual as PhoneApplicationFrame).RemoveBackEntry();
            }
        }

        private static void DisplayCustomPopupMessageWithAction(CustomPopupMessageWithAction customPopupMessageWithAction)
        {
            CustomMessageBox messageBoxResult = new CustomMessageBox()
            {
                Caption = customPopupMessageWithAction.Caption,
                Message = customPopupMessageWithAction.Message,
                LeftButtonContent = customPopupMessageWithAction.LeftButtonText,
                RightButtonContent = customPopupMessageWithAction.RightButtonText
            };

            if (customPopupMessageWithAction.HyperLink != null)
            {
                TiltEffect.SetIsTiltEnabled(customPopupMessageWithAction.HyperLink, true);

                messageBoxResult.Content = customPopupMessageWithAction.HyperLink;
            }

            messageBoxResult.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        if (customPopupMessageWithAction.LeftButtonAction != null)
                            customPopupMessageWithAction.LeftButtonAction();
                        break;
                    case CustomMessageBoxResult.RightButton:
                        if (customPopupMessageWithAction.RightButtonAction != null)
                            customPopupMessageWithAction.RightButtonAction();
                        break;
                    case CustomMessageBoxResult.None:
                        if (customPopupMessageWithAction.NoButtonAction != null)
                            customPopupMessageWithAction.NoButtonAction();
                        break;
                    default:
                        break;
                }
            };

            messageBoxResult.Show();
        }

        private static void DisplayCustomPopupMessage(CustomPopupMessage customPopupMessage)
        {
            CustomMessageBox messageBoxResult = new CustomMessageBox()
            {
                Caption = customPopupMessage.Caption,
                Message = customPopupMessage.Message,
                LeftButtonContent = customPopupMessage.LeftButtonText,
                RightButtonContent = customPopupMessage.RightButtonText
            };

            if (customPopupMessage.HyperLink != null)
            {
                TiltEffect.SetIsTiltEnabled(customPopupMessage.HyperLink, true);

                messageBoxResult.Content = customPopupMessage.HyperLink;
            }

            messageBoxResult.Show();
        }

        private void SetGpsWatcher(GpsWatcherMessage gpsWatcherMessage)
        {
            switch (gpsWatcherMessage.Reason)
            {
                case GpsWatcherMessageReason.Start:
                    runGpsWatcherContinuously = false;
                    StartGpsWatcher();
                    break;
                case GpsWatcherMessageReason.Stop:
                    StopGpsWatcher();
                    break;
                case GpsWatcherMessageReason.StartContinuous:
                    runGpsWatcherContinuously = true;
                    StartGpsWatcher();
                    break;
            }
        }

        private void StopGpsWatcher()
        {
            if (watcher != null)
            {
                watcher.Stop();

                gpsWatcherStatus = GpsWatcherState.Stopped;

                LoadingBarMessage.Send(LoadingBarMessageReason.Hide);
            }
        }

        private void StartGpsWatcher()
        {
            if (gpsWatcherStatus != GpsWatcherState.Running)
            {
                LoadingBarMessage.Send(LoadingBarMessageReason.Show);
                // Prevent multiple view models from starting the watcher.
                gpsWatcherStatus = GpsWatcherState.Running;

                if (watcher == null)
                {
                    watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                    watcher.MovementThreshold = 20;
                    watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(WatcherStatusChanged);
                    watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(WatcherPositionChanged);
                    watcher.Start();
                }
                else
                {
                    watcher.Start();
                }
            }
        }

        private void WatcherPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Send out coordinate to whoever registers to receive it:
            if (gpsWatcherStatus != GpsWatcherState.Stopped && gpsWatcherStatus != GpsWatcherState.Disabled)
                GpsWatcherResponseMessage.Send(false, new Coordinate(e.Position.Location.Latitude, e.Position.Location.Longitude), GpsWatcherResponseMessageReason.Coordinate);

            if (!runGpsWatcherContinuously)
                StopGpsWatcher();
        }

        private void WatcherStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    StopGpsWatcher();

                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        MessageBoxResult locationServiceDisabled = MessageBox.Show(AppResources.LocationPhoneDisabledHelpAlert, AppResources.LocationPhoneDisabledHelpCaption, MessageBoxButton.OKCancel);
                        if (locationServiceDisabled == MessageBoxResult.OK)
                        {
                            // TODO windows phone 7 version of this
                            //Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
                        }
                    }
                    else
                    {
                        DisplayCustomPopupMessage(new CustomPopupMessage(CustomPopupMessageType.Error, AppResources.LocationPhoneErrorHelpAlert, AppResources.CustomPopupGenericOkMessage, null));
                    }

                    GpsWatcherResponseMessage.SendError();

                    break;

                case GeoPositionStatus.Initializing:
                    // The Location Service is initializing.
                    // Disable the Start Location button.
                    //startLocationButton.IsEnabled = false;
                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    // Alert the user and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is not available.";
                    //stopLocationButton.IsEnabled = true;
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    // Show the current position and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is available.";
                    //stopLocationButton.IsEnabled = true;
                    break;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
#if !DEBUG
          FlurryWP8SDK.Api.StartSession(DatabaseSetup.FlurryApiKey);  
#endif
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
#if !DEBUG
          FlurryWP8SDK.Api.StartSession(DatabaseSetup.FlurryApiKey);  
#endif
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            ViewModelLocator.Cleanup();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
#if !DEBUG
            FlurryWP8SDK.Api.LogError("Application_UnhandledException", e.ExceptionObject);
#endif
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            DispatcherHelper.Initialize();

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                AppResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
                DrumbleApp.Shared.Resources.AppResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                //RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                //FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                //RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}