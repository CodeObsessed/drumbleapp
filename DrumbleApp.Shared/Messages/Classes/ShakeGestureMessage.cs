using DrumbleApp.Shared.Messages.Base;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class ShakeGestureMessage: BaseMessage<ShakeGestureMessage>
    {
        public ShakeGestureMessage()
        {
            
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void SendMessage()
        {
            new ShakeGestureMessage().Send();
        }
    }
}
