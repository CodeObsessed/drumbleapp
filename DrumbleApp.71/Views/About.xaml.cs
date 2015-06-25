using DrumbleApp.Shared.Converters;
using Microsoft.Phone.Controls;
using System;
using System.Reflection;

namespace Drumble.Views
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();

            var nameHelper = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company;

            VersionTextBlock.Text = nameHelper.Version.ToString();

            ReleaseDateTextBlock.Text = TimeConverter.ToRelativeDateString(new DateTime(2015, 4, 4, 15, 48, 0), true);
            CompanyHyperlinkButton.Content = company;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

#if !DEBUG
            FlurryWP8SDK.Api.LogPageView();
#endif
        }
    }
}