using MiddlewareLogic.Enums;
using MiddlewareLogic.MessageTypeWrappers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeCreator.Base;
using TypeCreator.Enums;
using Utils;

namespace MiddlewareLogic.Router.Base
{
    public static class MessageCodec
    {
        private const Int32 NodeInfoIndex = 0,
                            NodeTypeIndex = 0,
                            DeviceTypeIndex = 1,
                            TransportProtocolIndex = 2,
                            //---
                            NodeNameIndex = 1,
                            //---
                            DeviceNameIndex = 2,
                            //---
                            MessengerIdIndex = 3,
                            //---
                            TypesDescriptionIndex = 4;

        public static MessengerInfo DecodeMessengerInitMsg(IList<Byte[]> message)
        {
            var msgrInfo = new MessengerInfo
            {
                Type = (MessengerType)message[NodeInfoIndex][NodeTypeIndex],
                Device = (DeviceType)message[NodeInfoIndex][DeviceTypeIndex],
                Protocol = (TransportProtocol)message[NodeInfoIndex][TransportProtocolIndex],
                Name = Encoding.ASCII.GetString(message[NodeNameIndex]),
                DeviceName = Encoding.ASCII.GetString(message[DeviceNameIndex]),
                Id = message[MessengerIdIndex][0]
            };

            if (msgrInfo.Type == MessengerType.Raw || msgrInfo.Type == MessengerType.None)
                return msgrInfo;

            Int32 sectionsNumber = (Int32)msgrInfo.Type;
            var parts = new List<MessagePartDescription>[sectionsNumber];

            Byte[] name;
            MessagePartDescription msgPart;
            Int32 j = TypesDescriptionIndex;

            for (Int32 i = 0; i < sectionsNumber; i++)
            {
                parts[i] = new List<MessagePartDescription>();

                while (j < message.Count && (name = message[j]).Length != 0)
                {
                    msgPart = new MessagePartDescription
                    {
                        Name = Encoding.ASCII.GetString(name),
                        Type = (MessageValueType)message[j + 1][0]
                    };

                    j += 2;
                    parts[i].Add(msgPart);
                }
                j++;
            }
            msgrInfo.MessageDescriptions = parts;
            return msgrInfo;
        }

        public static (MessageMeta, IList<Byte[]>) DecodeMessage(IList<Byte[]> message)
        {
            return (new MessageMeta
            {
                Type = (MessageSubtype)message[0][0],
                MessengerId = message[1][0]
            },
                    message.Skip(2).ToArray()
                    );
        }

        public static IList<Byte[]> EncodeMessage(Byte messengerId, IList<Byte[]> message)
        {
            var encodedMessage = new Byte[message.Count + 2][];
            encodedMessage[0] = new Byte[] { (Byte)MessageSubtype.Goal };
            encodedMessage[1] = new Byte[] { messengerId };

            for (Int32 i = 2, k = 0; i < message.Count + 2; i++, k++) encodedMessage[i] = message[k];

            return encodedMessage;
        }
    }
}
