using Microsoft.Phone.Controls;

namespace Drumble.Views
{
    public partial class MainMenu : PhoneApplicationPage
    {
        public MainMenu()
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