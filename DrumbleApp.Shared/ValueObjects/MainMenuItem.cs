
using DrumbleApp.Shared.Interfaces;
namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class MainMenuItem
    {
        private INavigationService navigationService;
        private string page;

        public int Order { get; private set; }
        public string Icon { get; private set; }
        public string Text { get; private set; }

        public MainMenuItem(int order, string icon, string text, string page, INavigationService navigationService)
        {
            this.Order = order;
            this.Icon = icon;
            this.Text = text;
            this.page = page;
            this.navigationService = navigationService;
        }

        public void SetIconPressed()
        {
            this.Icon = this.Icon.Replace("/W/", "/LB/");
        }

        public void ResetIcon()
        {
            this.Icon = this.Icon.Replace("/LB/", "/W/");
        }

        public void Navigate()
        {
            if (!string.IsNullOrEmpty(page)) 
                navigationService.NavigateTo(page);
        }
    }
}
