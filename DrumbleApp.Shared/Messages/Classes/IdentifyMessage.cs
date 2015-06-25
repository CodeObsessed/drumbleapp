using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Entities;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class IdentifyMessage : BaseMessage<IdentifyMessage>
    {
        public IdentifyMessageReason Reason { get; private set; }
        public User User { get; private set; }

        public IdentifyMessage(User user, IdentifyMessageReason reason)
        {
            this.User = user;
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(User user, IdentifyMessageReason reason)
        {
            new IdentifyMessage(user, reason).Send();
        }
    }
}
