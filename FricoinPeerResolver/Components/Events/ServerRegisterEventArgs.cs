using System;
using FricoinPeerResolver.Components.Interfaces;

namespace FricoinPeerResolver.Components.Events
{
    public class ServerRegisterEventArgs : EventArgs
    {
        private ICollaborativeClientDetails mNewClient;

        public ICollaborativeClientDetails NewClient
        {
            get { return mNewClient; }
        }

        public ServerRegisterEventArgs(ICollaborativeClientDetails newClient)
        {
            mNewClient = newClient;
            
        }
    };
}