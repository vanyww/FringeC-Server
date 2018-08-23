using MiddlewareLogic.Commands;
using MiddlewareLogic.Enums;
using MiddlewareLogic.Events;
using System;
using System.Collections.Generic;
using TypeCreator.Base;
using Utils;

namespace MiddlewareLogic.MessageTypeWrappers.Base
{
    public abstract class Messenger
    {
        public Messenger(MessengerInfo info)
        {
            Commands = new Dictionary<String, OuterCommand>();
            m_info = info;
        }

        public event EventHandler<CommandAddedEventArgs> CommandAdded;

        public void AddCommand(OuterCommand commandInfo)
        {
            Commands.Add(commandInfo.Name, commandInfo);
            CommandAdded?.Invoke(this, new CommandAddedEventArgs(commandInfo));
        }

        public MessengerType Type => m_info.Type;
        public DeviceType Device => m_info.Device;
        public TransportProtocol Protocol => m_info.Protocol;
        public String Name => m_info.Name;
        public String DeviceName => m_info.DeviceName;
        public Byte Id => m_info.Id;
        public IList<MessagePartDescription>[] MessageDescriptions => m_info.MessageDescriptions;
        public Dictionary<String, OuterCommand> Commands { get; }

        private MessengerInfo m_info;
    }
}
