using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class DepartureTimeSelectionMessage : BaseMessage<DepartureTimeSelectionMessage>
    {
        public DepartureTimeSelectionMessageReason Reason { get; private set; }

        public DepartureTimeSelectionMessage(DepartureTimeSelectionMessageReason reason)
        {
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(DepartureTimeSelectionMessageReason reason)
        {
            new DepartureTimeSelectionMessage(reason).Send();
        }
    }
}
