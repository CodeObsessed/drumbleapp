using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows;
using Drumble.Resources;

namespace Drumble.Views
{
    public partial class UberSurgePricingConfirmation : PhoneApplicationPage
    {
        public UberSurgePricingConfirmation()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string href;

            if (NavigationContext.QueryString.TryGetValue("href", out href))
            {
                if (!String.IsNullOrEmpty(href))
                {
                    LoginControl.Navigate(new Uri(href));

                    MessageBox.Show(AppResources.UberSurgePricingConfirmationPopupText, AppResources.CustomPopupMessageTypeInformation, MessageBoxButton.OK);
                }
            }
        }

        private void LoginControl_Navigating(object sender, NavigatingEventArgs e)
        {
            // TODO Cannot get surge pricing to activate, assuming this for now. User can always just go back on the phone.
            if (e.Uri.AbsoluteUri.Contains("localhost"))
            {
                NavigationService.GoBack();
            }
        }
    }
}