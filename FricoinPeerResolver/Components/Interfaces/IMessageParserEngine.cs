﻿using System;

namespace FricoinPeerResolver.Components.Interfaces
{
    public interface IMessageParserEngine
    {
        bool ContainsMessageType(int messageType);
        bool AddMessageType(int messageType, IMessage messageInstance);
        bool ReplaceMessageType(int messageType, IMessage messageInstance);
        void RemoveMessageType(int messageType);
        IMessage ParseMessage(Byte[] data);
    }
}