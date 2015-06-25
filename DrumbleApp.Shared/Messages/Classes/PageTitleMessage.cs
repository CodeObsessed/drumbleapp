using DrumbleApp.Shared.Messages.Base;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class PageTitleMessage : BaseMessage<PageTitleMessage>
    {
        public string PageTitle { get; private set; }

        public PageTitleMessage(string pageTitle)
        {
            this.PageTitle = pageTitle;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(string pageTitle)
        {
            new PageTitleMessage(pageTitle).Send();
        }
    }
}
