using FricoinPeerResolver.Components.Enums;
using FricoinPeerResolver.Components.Events;

namespace FricoinPeerResolver.Components.Interfaces
{
    public interface IServer
    {
        int ListenPort { get; set; }

        IMessageParserEngine MessageParser { get; }

        event ServerRegisterEvent OnRegisterClient;

        InitState Initialize();
    }
}