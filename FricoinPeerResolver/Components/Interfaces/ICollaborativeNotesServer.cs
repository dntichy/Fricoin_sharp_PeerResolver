using Engine.Network;
using FricoinPeerResolver.Components.Enums;

namespace FricoinPeerResolver.Components.Interfaces
{
    public interface ICollaborativeNotesServer
    {
        int ListenPort
        {
            get;
            set;
        }

        IMessageParserEngine MessageParser
        {
            get;
        }

        event ServerRegisterEvent OnRegisterClient;

        InitState Initialize();
    }
}