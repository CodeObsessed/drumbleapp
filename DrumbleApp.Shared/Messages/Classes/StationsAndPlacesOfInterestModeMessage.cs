using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class StationsAndPlacesOfInterestModeMessage : BaseMessage<StationsAndPlacesOfInterestModeMessage>
    {
        public StationsAndPlacesOfInterestModeReason Reason { get; private set; }
        public bool FromWhereTo { get; private set; }

        public StationsAndPlacesOfInterestModeMessage(StationsAndPlacesOfInterestModeReason reason, bool fromWhereTo)
        {
            this.Reason = reason;
            this.FromWhereTo = fromWhereTo;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(StationsAndPlacesOfInterestModeReason reason, bool fromWhereTo)
        {
            new StationsAndPlacesOfInterestModeMessage(reason, fromWhereTo).Send();
        }
    }
}
