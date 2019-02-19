using System;

namespace FricoinPeerResolver.Components.Interfaces

{
    public interface IMessage
    {
        int Type { get; }
        Byte[] GetMessagePacket();
        bool Parse(Byte[] data);
        IMessage Clone();
    }
}