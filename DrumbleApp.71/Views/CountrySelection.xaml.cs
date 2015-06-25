using Microsoft.Phone.Controls;
using System.Windows;

namespace Drumble.Views
{
    public partial class CountrySelection : PhoneApplicationPage
    {
        public CountrySelection()
        {
            InitializeComponent();
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if ((Application.Current.RootVisual as PhoneApplicationFrame).CanGoBack)
                (Application.Current.RootVisual as PhoneApplicationFrame).GoBack();
        }
    }
}