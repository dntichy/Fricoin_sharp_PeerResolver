using System;
using FricoinPeerResolver.Components.Interfaces;

namespace FricoinPeerResolver.Components.Messages
{
    public class CollaborativeNotesReceiveMessageEventArgs : EventArgs
    {
        private IMessage mMessage;

        public IMessage Message
        {
            get { return mMessage; }
        }

        public CollaborativeNotesReceiveMessageEventArgs(IMessage newMessage)
        {
            mMessage = newMessage;
        }
    }
}