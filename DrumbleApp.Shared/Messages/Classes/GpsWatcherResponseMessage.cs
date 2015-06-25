using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.ValueObjects;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class GpsWatcherResponseMessage : BaseMessage<GpsWatcherResponseMessage>
    {
        public GpsWatcherResponseMessageReason Reason { get; private set; }

        public Coordinate Coordinate { get; private set; }

        public bool IsUsingLastKnown { get; private set; }

        public GpsWatcherResponseMessage()
        {
            this.Reason = GpsWatcherResponseMessageReason.Error;
            this.Coordinate = null;
            this.IsUsingLastKnown = false;
        }

        public GpsWatcherResponseMessage(bool isUsingLastKnown, Coordinate coordinate, GpsWatcherResponseMessageReason reason)
            : this ()
        {
            this.Reason = reason;
            this.Coordinate = coordinate;
            this.IsUsingLastKnown = isUsingLastKnown;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void SendError()
        {
            new GpsWatcherResponseMessage().Send();
        }

        public static void Send(bool isUsingLastKnown, Coordinate coordinate, GpsWatcherResponseMessageReason reason)
        {
            new GpsWatcherResponseMessage(isUsingLastKnown, coordinate, reason).Send();
        }
    }
}
