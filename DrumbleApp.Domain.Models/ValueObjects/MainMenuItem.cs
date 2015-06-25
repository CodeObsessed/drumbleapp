using System;

namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class MainMenuItem
    {
        public Uri Page { get; private set; }
        public int Order { get; private set; }
        public string Icon { get; private set; }
        public string Text { get; private set; }

        public MainMenuItem(int order, Uri page, string icon, string text)
        {
            this.Order = order;
            this.Icon = icon;
            this.Text = text;
            this.Page = page;
        }

        public void SetIconPressed()
        {
            this.Icon = this.Icon.Replace("/W/", "/LB/");
        }

        public void ResetIcon()
        {
            this.Icon = this.Icon.Replace("/LB/", "/W/");
        }
    }
}
