using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using TypeCreator;
using TypeCreator.Base;
using TypeCreator.Enums;

namespace TestApp
{
    public static class Program
    {
        static void Main(string[] args)
        {
            //true - for random strings as field names
            var (msg, description) = CreateTestMessage(64, false);
            //name must be unique
            var newType = MessageTypeCreator.Instance.CreateMessageType("test1_64", description);
            //MethodInfo factoryCreator = typeof(InstanceFactoryCreator).GetMethod("CreateFactory");
            //factoryCreator.MakeGenericMethod(newType);
            //factoryCreator.Invoke(InstanceFactoryCreator.Instance, )
            var factory = InstanceFactoryCreator.Instance.CreateFactory<Message>(newType);

            var newMessage = factory();
            newMessage.SetProperties(msg);
        }

        static (Byte[][] msg, MessagePartDescription[] description) CreateTestMessage(Int32 length, Boolean name)
        {
            if (length % 4 != 0) throw new ArgumentException();
            Contract.EndContractBlock();

            var msg = new Byte[length][];
            var desc = new MessagePartDescription[length];

            for (Int32 i = 0; i < length; i += 4)
            {
                msg[i] = BitConverter.GetBytes(333);
                desc[i] = new MessagePartDescription { Name = (name) ? RandomString(15) : i.ToString(), Type = MessageValueType.Int32 };

                msg[i + 1] = Encoding.ASCII.GetBytes("Make love, not war");
                desc[i + 1] = new MessagePartDescription { Name = (name) ? RandomString(15) : (i + 1).ToString(), Type = MessageValueType.String };

                msg[i + 2] = BitConverter.GetBytes(7.77);
                desc[i + 2] = new MessagePartDescription { Name = (name) ? RandomString(15) : (i + 2).ToString(), Type = MessageValueType.Double };

                msg[i + 3] = Encoding.ASCII.GetBytes("e");
                desc[i + 3] = new MessagePartDescription { Name = (name) ? RandomString(15) : (i + 3).ToString(), Type = MessageValueType.String };
            }

            return (msg, desc);

            String RandomString(Int32 len)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[rand.Next(s.Length)]).ToArray());
            }
        }

        static Random rand = new Random();
    }
}
