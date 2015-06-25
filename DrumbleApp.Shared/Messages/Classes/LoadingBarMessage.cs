using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class LoadingBarMessage : BaseMessage<LoadingBarMessage>
    {
        public LoadingBarMessageReason Reason { get; private set; }

        public LoadingBarMessage(LoadingBarMessageReason reason)
        {
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(LoadingBarMessageReason reason)
        {
            new LoadingBarMessage(reason).Send();
        }
    }
}
