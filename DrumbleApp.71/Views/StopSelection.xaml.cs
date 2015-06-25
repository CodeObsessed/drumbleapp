using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace Drumble.Views
{
    public partial class StopSelection : PhoneApplicationPage
    {
        public StopSelection()
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