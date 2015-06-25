using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.ValueObjects;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class SearchItemMessage : BaseMessage<SearchItemMessage>
    {
        public SearchItemMessageReason Reason { get; private set; }
        public SearchItem SearchItem { get; private set; }

        public SearchItemMessage(SearchItem searchItem, SearchItemMessageReason reason)
        {
            this.SearchItem = searchItem;
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(SearchItem searchItem, SearchItemMessageReason reason)
        {
            new SearchItemMessage(searchItem, reason).Send();
        }
    }
}
