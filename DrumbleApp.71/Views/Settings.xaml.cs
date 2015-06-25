using Microsoft.Phone.Controls;

namespace Drumble.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
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