using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class AboutViewModel : AnalyticsBase, IDisposable
    {
        public AboutViewModel(IAggregateService aggregateService)
            : base(ApplicationPage.About, aggregateService)
        {
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            PageTitleMessage.Send(AppResources.HeaderAbout);
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();
        }

        #endregion

        #region Local Functions

        private void Company()
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(AppResources.CompanyWebsiteLink, UriKind.Absolute);
            webBrowserTask.Show();
        }

        private void Contact()
        {
            NavigationService.NavigateTo("/Views/Message.xaml");
        }

        private void Settings()
        {
            NavigationService.NavigateTo("/Views/Settings.xaml");
        }

        private void AppInfoOn()
        {
            AppInfoOnVisibility = Visibility.Visible;
            AppInfoOffVisibility = Visibility.Collapsed;
            DeveloperOnVisibility = Visibility.Collapsed;
            DeveloperOffVisibility = Visibility.Visible;
            PoliciesOnVisibility = Visibility.Collapsed;
            PoliciesOffVisibility = Visibility.Visible;
        }

        private void PoliciesOn()
        {
            AppInfoOnVisibility = Visibility.Collapsed;
            AppInfoOffVisibility = Visibility.Visible;
            DeveloperOnVisibility = Visibility.Collapsed;
            DeveloperOffVisibility = Visibility.Visible;
            PoliciesOnVisibility = Visibility.Visible;
            PoliciesOffVisibility = Visibility.Collapsed;
        }

        private void DeveloperOn()
        {
            AppInfoOnVisibility = Visibility.Collapsed;
            AppInfoOffVisibility = Visibility.Visible;
            DeveloperOnVisibility = Visibility.Visible;
            DeveloperOffVisibility = Visibility.Collapsed;
            PoliciesOnVisibility = Visibility.Collapsed;
            PoliciesOffVisibility = Visibility.Visible;
        }

        #endregion

        #region Properties

        private Visibility appInfoOffVisibility = Visibility.Collapsed;
        public Visibility AppInfoOffVisibility
        {
            get { return appInfoOffVisibility; }
            set
            {
                appInfoOffVisibility = value;
                RaisePropertyChanged("AppInfoOffVisibility");
            }
        }

        private Visibility appInfoOnVisibility = Visibility.Visible;
        public Visibility AppInfoOnVisibility
        {
            get { return appInfoOnVisibility; }
            set
            {
                appInfoOnVisibility = value;
                RaisePropertyChanged("AppInfoOnVisibility");
            }
        }

        private Visibility policiesOffVisibility = Visibility.Visible;
        public Visibility PoliciesOffVisibility
        {
            get { return policiesOffVisibility; }
            set
            {
                policiesOffVisibility = value;
                RaisePropertyChanged("PoliciesOffVisibility");
            }
        }

        private Visibility policiesOnVisibility = Visibility.Collapsed;
        public Visibility PoliciesOnVisibility
        {
            get { return policiesOnVisibility; }
            set
            {
                policiesOnVisibility = value;
                RaisePropertyChanged("PoliciesOnVisibility");
            }
        }

        private Visibility developerOffVisibility = Visibility.Visible;
        public Visibility DeveloperOffVisibility
        {
            get { return developerOffVisibility; }
            set
            {
                developerOffVisibility = value;
                RaisePropertyChanged("DeveloperOffVisibility");
            }
        }

        private Visibility developerOnVisibility = Visibility.Collapsed;
        public Visibility DeveloperOnVisibility
        {
            get { return developerOnVisibility; }
            set
            {
                developerOnVisibility = value;
                RaisePropertyChanged("DeveloperOnVisibility");
            }
        }
        
        #endregion

        #region Commands

        public RelayCommand AppInfoOffCommand
        {
            get { return new RelayCommand(AppInfoOn); }
        }
        
        public RelayCommand PoliciesOffCommand
        {
            get { return new RelayCommand(PoliciesOn); }
        }

        public RelayCommand DeveloperOffCommand
        {
            get { return new RelayCommand(DeveloperOn); }
        }

        public RelayCommand CompanyCommand
        {
            get { return new RelayCommand(Company); }
        }

        public RelayCommand ContactCommand
        {
            get { return new RelayCommand(Contact); }
        }

        public RelayCommand SettingsCommand
        {
            get { return new RelayCommand(Settings); }
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
