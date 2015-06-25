using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.Messages.Base;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class RecentTripMessage : BaseMessage<RecentTripMessage>
    {
        public RecentTripMessageReason Reason { get; private set; }
        public Recent RecentTrip { get; private set; }

        public RecentTripMessage(Recent recentTrip, RecentTripMessageReason reason)
        {
            this.RecentTrip = recentTrip;
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(Recent recentTrip, RecentTripMessageReason reason)
        {
            new RecentTripMessage(recentTrip, reason).Send();
        }
    }
}
