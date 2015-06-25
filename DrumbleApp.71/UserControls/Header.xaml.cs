using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using System;
using System.Windows.Media.Imaging;

namespace Drumble.UserControls
{
    public partial class Header : UserControl
    {
#if DEBUG
        DispatcherTimer timer;
#endif

        public Header()
        {
            InitializeComponent();

 #if DEBUG
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,3);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
#endif

            Loaded += (s, e) =>
            {
                if ((Application.Current.RootVisual as PhoneApplicationFrame).CanGoBack)
                    BackButtonImage.Source = new BitmapImage(new Uri("/Images/64/LB/IconBack.png", UriKind.Relative));
                else
                    BackButtonImage.Source = new BitmapImage(new Uri("/Images/64/LB/IconDrumble.png", UriKind.Relative));
            };
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if ((Application.Current.RootVisual as PhoneApplicationFrame).CanGoBack)
                (Application.Current.RootVisual as PhoneApplicationFrame).GoBack();
        }

#if DEBUG
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                // These are TextBlock controls that are created in the page’s XAML file.      
                Memory.Text = (Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage/ 1024 / 1024).ToString() + "Mb";
            }
            catch (Exception)
            {
            }
        }
#endif

    }
}
