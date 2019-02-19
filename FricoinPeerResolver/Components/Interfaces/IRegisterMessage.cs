using System;

namespace FricoinPeerResolver.Components.Interfaces
{
    public interface IRegisterMessage : IMessage
    {
        CollaborativeClientDetails Client { get; set; }

        String Group { get; set; }
    }
}