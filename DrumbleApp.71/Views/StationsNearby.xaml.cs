using Microsoft.Phone.Controls;
using System.Windows.Controls;

namespace Drumble.Views
{
    public partial class StationsNearby : PhoneApplicationPage
    {
        public StationsNearby()
        {
            InitializeComponent();
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}