using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.ValueObjects;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class MapPointMessage : BaseMessage<MapPointMessage>
    {
        public Coordinate Point { get; private set; }

        public MapPointMessage(Coordinate point)
        {
            this.Point = point;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(Coordinate point)
        {
            new MapPointMessage(point).Send();
        }
    }
}
