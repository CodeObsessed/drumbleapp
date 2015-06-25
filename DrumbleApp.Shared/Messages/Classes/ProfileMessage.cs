using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class ProfileMessage : BaseMessage<ProfileMessage>
    {
        public ProfileMessageReason Reason { get; private set; }

        public ProfileMessage(ProfileMessageReason reason)
        {
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(ProfileMessageReason reason)
        {
            new ProfileMessage(reason).Send();
        }
    }
}
