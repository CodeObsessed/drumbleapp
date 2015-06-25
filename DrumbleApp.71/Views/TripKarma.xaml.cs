using Microsoft.Phone.Controls;

namespace Drumble.Views
{
    public partial class TripKarma : PhoneApplicationPage
    {
        public TripKarma()
        {
            InitializeComponent();
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}