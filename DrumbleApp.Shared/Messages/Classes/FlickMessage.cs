using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class FlickMessage: BaseMessage<FlickMessage>
    {
        public FlickMessageReason Reason { get; private set; }
        public string ControlName { get; private set; }

        public FlickMessage(FlickMessageReason reason)
        {
            this.Reason = reason;
        }

        public FlickMessage(FlickMessageReason reason, string controlName)
            : this(reason)
        {
            this.ControlName = controlName;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(FlickMessageReason reason)
        {
            new FlickMessage(reason).Send();
        }

        public static void Send(FlickMessageReason reason, string controlName)
        {
            new FlickMessage(reason, controlName).Send();
        }
    }
}
