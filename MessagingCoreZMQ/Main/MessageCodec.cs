using MessagingCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;

namespace MessagingCore.Main
{
    internal static class MessageCodec
    {
        public static ZMessage EncodeMessage(Int32 dealer, IList<Byte[]> message)
        {
            var result = new ZMessage { new ZFrame(dealer), new ZFrame((Byte)MessageType.Common) };

            if (message != null && message.Count != 0)
                result.AddRange(message.Select(msgPart => new ZFrame(msgPart)));

            return result;
        }

        public static ZMessage EncodeCommandMessage(Int32 dealer, IList<Byte[]> message)
        {
            var result = new ZMessage { new ZFrame(dealer), new ZFrame((Byte)MessageType.Command) };

            if (message != null && message.Count != 0)
                result.AddRange(message.Select(msgPart => new ZFrame(msgPart)));

            return result;
        }

        public static ZMessage EncodeNodeSignalMessage(Int32 dealer, IList<Byte[]> message)
        {
            var result = new ZMessage { new ZFrame(dealer), new ZFrame((Byte)MessageType.NodeSignal) };

            if (message != null && message.Count != 0)
                result.AddRange(message.Select(msgPart => new ZFrame(msgPart)));

            return result;
        }

        private const Int32 NodeNameIndex = 0;

        public static MessageData DecodeMessage(ZMessage message) =>
            new MessageData
            {
                Identity = message.Pop().ReadInt32(),
                Type = (MessageType)message.Pop().ReadAsByte(),
                Data = message.Select((frame) => frame.Read()).ToArray()
            };

        public static String DecodeNodeInitialization(IList<Byte[]> message) =>
            Encoding.ASCII.GetString(message[NodeNameIndex]);
    }
}
