using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Messages.Base;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class PageLoadMessage : BaseMessage<PageLoadMessage>
    {
        public ApplicationPage Page { get; private set; }

        public PageLoadMessage(ApplicationPage page)
        {
            this.Page = page;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(ApplicationPage page)
        {
            new PageLoadMessage(page).Send();
        }
    }
}
