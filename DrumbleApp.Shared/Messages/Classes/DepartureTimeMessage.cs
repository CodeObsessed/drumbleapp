using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.ValueObjects;
using System;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class DepartureTimeMessage : BaseMessage<DepartureTimeMessage>
    {
        public DepartureTimeMessageReason Reason { get; private set; }
        public DateTime DateTime { get; private set; }
        public PredefinedDepartureTime PredefinedDepartureTime { get; private set; }
        public bool IsDeparting { get; private set; }

        public DepartureTimeMessage(DateTime datetime, bool isDeparting)
        {
            this.DateTime = datetime;
            this.Reason = DepartureTimeMessageReason.DateTime;
            this.IsDeparting = isDeparting;
        }

        public DepartureTimeMessage(PredefinedDepartureTime departureTime, bool isDeparting)
        {
            this.PredefinedDepartureTime = departureTime;
            this.Reason = DepartureTimeMessageReason.PreDefined;
            this.IsDeparting = isDeparting;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(DateTime datetime, bool isDeparting)
        {
            new DepartureTimeMessage(datetime, isDeparting).Send();
        }

        public static void Send(PredefinedDepartureTime departureTime, bool isDeparting)
        {
            new DepartureTimeMessage(departureTime, isDeparting).Send();
        }
    }
}
