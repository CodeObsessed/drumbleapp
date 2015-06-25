using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class NewAnnouncementsMessage: BaseMessage<NewAnnouncementsMessage>
    {
        public NewAnnouncementsMessage()
        {
            
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(bool any)
        {
            new NewAnnouncementsMessage().Send();
        }
    }
}
