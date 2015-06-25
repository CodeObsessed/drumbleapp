using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class FavouriteMessage : BaseMessage<FavouriteMessage>
    {
        public FavouriteMessageReason Reason { get; private set; }
        public Favourite Favourite { get; private set; }

        public FavouriteMessage(Favourite favourite, FavouriteMessageReason reason)
        {
            this.Favourite = favourite;
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(Favourite favourite, FavouriteMessageReason reason)
        {
            new FavouriteMessage(favourite, reason).Send();
        }
    }
}
