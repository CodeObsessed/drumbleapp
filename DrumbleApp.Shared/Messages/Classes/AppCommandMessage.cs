using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class AppCommandMessage : BaseMessage<AppCommandMessage>
    {
        public AppCommandMessageReason Reason { get; private set; }

        public AppCommandMessage(AppCommandMessageReason reason)
        {
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(AppCommandMessageReason reason)
        {
            new AppCommandMessage(reason).Send();
        }
    }
}
