using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class GpsWatcherMessage : BaseMessage<GpsWatcherMessage>
    {
        public GpsWatcherMessageReason Reason { get; private set; }

        public GpsWatcherMessage(GpsWatcherMessageReason reason)
        {
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(GpsWatcherMessageReason reason)
        {
            new GpsWatcherMessage(reason).Send();
        }
    }
}
