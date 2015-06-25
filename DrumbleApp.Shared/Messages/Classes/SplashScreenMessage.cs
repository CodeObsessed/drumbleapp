using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class SplashScreenMessage : BaseMessage<SplashScreenMessage>
    {
        public SplashScreenMessageReason Reason { get; private set; }

        public SplashScreenMessage(SplashScreenMessageReason reason)
        {
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(SplashScreenMessageReason reason)
        {
            new SplashScreenMessage(reason).Send();
        }
    }
}
