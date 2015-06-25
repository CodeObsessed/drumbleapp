using GalaSoft.MvvmLight.Messaging;
using System;

namespace DrumbleApp.Shared.Messages.Base
{
    public abstract class BaseMessage<T> where T : class
    {
        protected void Send(T entity)
        {
            Messenger.Default.Send <T> (entity);
        }
    }
}
