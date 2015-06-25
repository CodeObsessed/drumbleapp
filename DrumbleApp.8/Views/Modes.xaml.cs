using Microsoft.Phone.Controls;

namespace Drumble.Views
{
    public partial class Modes : PhoneApplicationPage
    {
        public Modes()
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