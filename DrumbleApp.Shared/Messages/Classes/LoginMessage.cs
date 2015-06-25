using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class LoginMessage : BaseMessage<LoginMessage>
    {
        public LoginMessageReason Reason { get; private set; }

        public LoginMessage(LoginMessageReason reason)
        {
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(LoginMessageReason reason)
        {
            new LoginMessage(reason).Send();
        }
    }
}
