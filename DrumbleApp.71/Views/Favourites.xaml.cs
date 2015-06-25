using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Drumble.Resources;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace Drumble.Views
{
    public partial class Favourites : PhoneApplicationPage
    {
        public Favourites()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

#if !DEBUG
            FlurryWP8SDK.Api.LogPageView();
#endif

            string fromWhereto;
            if (NavigationContext.QueryString.TryGetValue("fromwhereto", out fromWhereto))
            {
                if (fromWhereto == "true")
                {
                    Messenger.Default.Send<DrumbleApp.Shared.Enums.FavouritesPageState>(DrumbleApp.Shared.Enums.FavouritesPageState.FromWhereTo);
                }
            }
            string state;
            if (NavigationContext.QueryString.TryGetValue("state", out state))
            {
                if (state == "favourites")
                {
                    Messenger.Default.Send<DrumbleApp.Shared.Enums.FavouritesPageState>(DrumbleApp.Shared.Enums.FavouritesPageState.Favourites);
                }
                else if (state == "recent")
                {
                    Messenger.Default.Send<DrumbleApp.Shared.Enums.FavouritesPageState>(DrumbleApp.Shared.Enums.FavouritesPageState.Recent);
                }
            }
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}