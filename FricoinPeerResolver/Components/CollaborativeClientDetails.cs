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

        public int CompareTo(ICollaborativeClientDetails other)
        {
            if (Equals(other)) return 0;
            else return 1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return (mClientListenPort == ((ICollaborativeClientDetails)obj).ClientListenPort &&
                    mClientIPAddress.Equals(((ICollaborativeClientDetails)obj).ClientIPAddress)); 
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}