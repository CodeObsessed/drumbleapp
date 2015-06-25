using Microsoft.Phone.Controls;
using System;

namespace Drumble.Views
{
    public partial class Message : PhoneApplicationPage
    {
        public Message()
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

        private void RichTextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RichTextBoxWaterMark.Visibility = System.Windows.Visibility.Collapsed;
            MessageTextBox.Focus();
        }

        private void MessageTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(MessageTextBox.Text))
                RichTextBoxWaterMark.Visibility = System.Windows.Visibility.Visible;
        }
    }
}