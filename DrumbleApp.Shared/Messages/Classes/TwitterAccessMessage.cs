using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.ValueObjects;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class TwitterAccessMessage : BaseMessage<TwitterAccessMessage>
    {
        public TwitterAccessMessageReason Reason { get; private set; }
        public TwitterAccess TwitterAccess { get; private set; }

        public TwitterAccessMessage(TwitterAccess twitterAccess, TwitterAccessMessageReason reason)
        {
            this.TwitterAccess = twitterAccess;
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(TwitterAccess twitterAccess, TwitterAccessMessageReason reason)
        {
            new TwitterAccessMessage(twitterAccess, reason).Send();
        }
    }
}
