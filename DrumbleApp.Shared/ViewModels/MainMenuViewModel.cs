using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DrumbleApp.Shared.ViewModels
{
    public class MainMenuViewModel : AnalyticsBase, IDisposable
    {
        public MainMenuViewModel(IAggregateService aggregateService)
            : base(ApplicationPage.MainMenu, aggregateService)
        {

        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            PageTitleMessage.Send(AppResources.HeaderMainMenu);

            SelectedMenuItem = null;
        }

        #endregion

        #region Local Functions

        private void BuildMainMenu(MainMenuItem selectedMenuItem)
        {
            MainMenu.Clear();

            //MainMenu.Add(new MainMenuItem(0, "/Images/64/W/MenuWhereTo.png", AppResources.MainMenuDrumble, "/Views/WhereTo.xaml", NavigationService));
            MainMenu.Add(new MainMenuItem(3, "/Images/64/W/MenuFavourites.png", AppResources.MainMenuFavouritesRecent, "/Views/Favourites.xaml?state=favourites", NavigationService));
            MainMenu.Add(new MainMenuItem(5, "/Images/64/W/MenuAnnouncement.png", AppResources.MainMenuAnnouncementsDelays, "/Views/Announcements.xaml", NavigationService));
            MainMenu.Add(new MainMenuItem(6, "/Images/64/W/MenuTransportationMaps.png", AppResources.MainMenuMaps, "/Views/Maps.xaml", NavigationService));
            MainMenu.Add(new MainMenuItem(9, "/Images/64/W/MenuMessageUs.png", AppResources.MainMenuMessageUs, "/Views/Message.xaml", NavigationService));
            MainMenu.Add(new MainMenuItem(10, "/Images/64/W/MenuShare.png", AppResources.MainMenuShareApp, "/Views/Share.xaml", NavigationService));
            MainMenu.Add(new MainMenuItem(11, "/Images/64/W/MenuChangeCountry.png", AppResources.MainMenuChangeCountry, "/Views/SplashScreen.xaml?pagestate=changecountry", NavigationService));
            MainMenu.Add(new MainMenuItem(12, "/Images/64/W/MenuSettings.png", AppResources.MainMenuSettings, "/Views/Settings.xaml", NavigationService));
            MainMenu.Add(new MainMenuItem(13, "/Images/64/W/MenuAbout.png", AppResources.MainMenuAbout, "/Views/About.xaml", NavigationService));

            if (selectedMenuItem != null)
            {
                MainMenu.Where(x => x.Order == selectedMenuItem.Order).First().SetIconPressed();
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<MainMenuItem> mainMenu = new ObservableCollection<MainMenuItem>();
        public ObservableCollection<MainMenuItem> MainMenu
        {
            get { return mainMenu; }
        }

        private MainMenuItem selectedMenuItem;
        public MainMenuItem SelectedMenuItem 
        {
            get
            {
                return selectedMenuItem;
            }
            set
            {
                selectedMenuItem = value;

                BuildMainMenu(selectedMenuItem);

                if (selectedMenuItem != null)
                {
                    selectedMenuItem.Navigate();
                }
            }
        }

        #endregion

        #region Cleanup

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    UnitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}