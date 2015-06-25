using DrumbleApp.Shared.Messages.Base;
using DrumbleApp.Shared.Models;

namespace DrumbleApp.Shared.Messages.Classes
{
    public sealed class WhereToMessage : BaseMessage<WhereToMessage>
    {
        public WhereToModel WhereToModel { get; private set; }

        public WhereToMessage(WhereToModel whereToModel)
        {
            this.WhereToModel = whereToModel;
        }

        private void Send()
        {
            base.Send(this);
        }

        public static void Send(WhereToModel whereToModel)
        {
            new WhereToMessage(whereToModel).Send();
        }
    }
}
