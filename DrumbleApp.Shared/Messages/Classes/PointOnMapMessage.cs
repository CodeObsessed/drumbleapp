using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.ValueObjects;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class PointOnMapMessage : BaseMessage<PointOnMapMessage>
    {
        public PointOnMapMessageReason Reason { get; private set; }
        public PublicStopPoint PublicStopPoint { get; private set; }
        public Entities.PlaceOfInterest PlaceOfInterest { get; private set; }
        public Coordinate Coordinate { get; private set; }

        public PointOnMapMessage(PublicStopPoint publicStopPoint)
        {
            this.PublicStopPoint = publicStopPoint;
            this.Reason = PointOnMapMessageReason.PublicStopPoint;
        }

        public PointOnMapMessage(Entities.PlaceOfInterest placeOfInterest)
        {
            this.PlaceOfInterest = placeOfInterest;
            this.Reason = PointOnMapMessageReason.PlaceOfInterest;
        }

        public PointOnMapMessage(Coordinate coordinate)
        {
            this.Coordinate = coordinate;
            this.Reason = PointOnMapMessageReason.CustomPoint;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(PublicStopPoint publicStopPoint)
        {
            new PointOnMapMessage(publicStopPoint).Send();
        }

        public static void Send(Entities.PlaceOfInterest placeOfInterest)
        {
            new PointOnMapMessage(placeOfInterest).Send();
        }

        public static void Send(Coordinate coordinate)
        {
            new PointOnMapMessage(coordinate).Send();
        }
    }
}
