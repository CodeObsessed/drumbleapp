using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Entities;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class AuthoriseMessage : BaseMessage<AuthoriseMessage>
    {
        public AuthoriseMessageReason Reason { get; private set; }
        public User User { get; private set; }

        public AuthoriseMessage(User user, AuthoriseMessageReason reason)
        {
            this.User = user;
            this.Reason = reason;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(User user, AuthoriseMessageReason reason)
        {
            new AuthoriseMessage(user, reason).Send();
        }
    }
}
