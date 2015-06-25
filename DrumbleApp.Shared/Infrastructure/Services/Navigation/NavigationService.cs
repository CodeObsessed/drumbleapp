using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;
using System.Linq;
using DrumbleApp.Shared.Interfaces;

namespace DrumbleApp.Shared.Infrastructure.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private PhoneApplicationFrame _mainFrame;

        public event NavigatingCancelEventHandler Navigating;

        public NavigationService() { }
        public void NavigateTo(string url)
        {
            NavigateTo(new Uri(url, UriKind.Relative));
        }

        public void NavigateTo(Uri uri)
        {
            if (EnsureMainFrame())
            {
                _mainFrame.Navigate(uri);
            }
        }

        public void GoBack()
        {
            if (EnsureMainFrame()
                && _mainFrame.CanGoBack)
            {
                _mainFrame.GoBack();
            }
        }

        public void GoForward()
        {
            if (EnsureMainFrame()
                && _mainFrame.CanGoForward)
            {
                _mainFrame.GoForward();
            }
        }

        public void RemoveBackEntries()
        {
            if (EnsureMainFrame()
                && _mainFrame.CanGoBack)
            {
                while (_mainFrame.BackStack.Count() > 0)
                    _mainFrame.RemoveBackEntry();
            }
        }

        public void RemoveBackEntry()
        {
            if (EnsureMainFrame()
                && _mainFrame.CanGoBack)
            {
                if (_mainFrame.BackStack.Count() > 0)
                    _mainFrame.RemoveBackEntry();
            }
        }

        public string CurrentPage()
        {
            if (EnsureMainFrame())
            {
                return _mainFrame.CurrentSource.ToString();
            }

            return String.Empty;
        }

        private bool EnsureMainFrame()
        {
            if (_mainFrame != null)
            {
                return true;
            }

            _mainFrame = Application.Current.RootVisual as PhoneApplicationFrame;

            if (_mainFrame != null)
            {
                // Could be null if the app runs inside a design tool
                _mainFrame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };

                return true;
            }

            return false;
        }
    }
}
