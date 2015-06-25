using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Messages.Base;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class TripMessage : BaseMessage<TripMessage>
    {
        public Trip Trip { get; private set; }

        public TripMessage(Trip trip)
        {
            this.Trip = trip;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(Trip trip)
        {
            new TripMessage(trip).Send();
        }
    }
}
