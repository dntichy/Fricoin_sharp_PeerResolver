using System;
using FricoinPeerResolver.Components.Interfaces;

namespace FricoinPeerResolver.Components
{
    public class CollaborativeClientDetails : ICollaborativeClientDetails
    {
        private String mClientName;
        private String mClientIPAddress;
        private int mClientListenPort;

        public String ClientName
        {
            get { return mClientName; }
            set { mClientName = value; }
        }

        public String ClientIPAddress
        {
            get { return mClientIPAddress; }
            set { mClientIPAddress = value; }
        }

        public int ClientListenPort
        {
            get { return mClientListenPort; }
            set { mClientListenPort = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return (mClientListenPort == ((CollaborativeClientDetails) obj).ClientListenPort &&
                    mClientIPAddress == ((CollaborativeClientDetails) obj).ClientIPAddress &&
                    mClientName == ((CollaborativeClientDetails) obj).ClientName); //base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}