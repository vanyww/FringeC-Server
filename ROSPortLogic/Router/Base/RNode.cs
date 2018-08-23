using MessagingCore.Main;
using MiddlewareLogic.Commands;
using MiddlewareLogic.Commands.Wrappers;
using MiddlewareLogic.Events;
using MiddlewareLogic.MessageTypeWrappers;
using MiddlewareLogic.MessageTypeWrappers.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks.Dataflow;
using SystemSignalsLogic.Events;
using TypeCreator;
using TypeCreator.Base;
using TypeCreator.Enums;
using Utils;

namespace MiddlewareLogic.Router.Base
{
    public class RNode
    {
        private const Int32 NodeMessengerIndex = 0;

        static RNode() => CompletedCommandsBuffer = new BufferBlock<OuterCommandCall>();

        public RNode(Node msgNode)
        {
            CalledCommands = new Dictionary<Int32, OuterCommandCall>();
            m_messageNode = msgNode;
            Messengers = new Dictionary<Byte, Messenger>();

            m_processMessageAction = new ActionBlock<IList<Byte[]>>(msg =>
            {
                (MessageMeta Meta, IList<Byte[]> Data) = MessageCodec.DecodeMessage(msg);
                MessageProcessor.ProcessMessage(Meta.Type, Data, Messengers[Meta.MessengerId]);
            });

            m_processCommandAction = new ActionBlock<IList<Byte[]>>(msg =>
            {
                var reply = CommandCodec.DecodeReplyIds(msg);
                var calledCommand = CalledCommands[reply.CallId];
                CalledCommands.Remove(reply.CallId);
                calledCommand.Reply = CommandCodec.DecodeReplyFully(reply, calledCommand, msg);
                calledCommand.NotifyCommandPerformed(reply);
            });

            m_encodeCommandCallTransform = new TransformBlock<OuterCommandCall, (Int32, IList<Byte[]>)>(call =>
                CommandCodec.EncodeCommandCall(call));

            m_messageNode.InMessageBuffer.LinkTo(m_processMessageAction);

            m_messageNode.CommandInMessageBuffer.LinkTo(m_processCommandAction);
            m_encodeCommandCallTransform.LinkTo(m_messageNode.CommandOutMessageBuffer);

            m_messageNode.TCPSocketSignalsWorker.MessengerInitialization += OnMessengerInitialization;
            m_messageNode.CMDSocketSignalsWorker.CommandInitialization += OnCommandInitialization;
        }

        private void OnCommandInitialization(Object sender, NodeSignalEventArgs e)
        {
            var commandInfo = CommandCodec.DecodeCommandInit(m_messageNode.Identity, e.Data);

            commandInfo.MessageWrapper = CreateWrapper($"c_{commandInfo.Name}{commandInfo.Node}_msg",
                                           commandInfo.RequestDescription,
                                           $"c_{commandInfo.Name}{commandInfo.Node}_repl",
                                           commandInfo.ReplyDescription);

            commandInfo.CallCommandAction = CallCommand;
            commandInfo.CallCommandBufferAction = CallCommandBufferReply;

            if (commandInfo.Messenger == NodeMessengerIndex) NodeMessenger.AddCommand(commandInfo);
            else Messengers[commandInfo.Messenger].AddCommand(commandInfo);

            IMessageCreator CreateWrapper(String msgName, IList<MessagePartDescription> msgDesc,
                              String repName, IList<MessagePartDescription> replDesc)
            {
                var msg = (msgDesc is null) ?
                    typeof(MockMessage) :
                    (msgDesc[0].Type == MessageValueType.Raw) ?
                    typeof(RawMessage) :
                    MessageTypeCreator.Instance.CreateMessageType(msgName, msgDesc);
                var repl = (replDesc is null) ?
                    typeof(MockMessage) :
                    (replDesc[0].Type == MessageValueType.Raw) ?
                    typeof(RawMessage) :
                    MessageTypeCreator.Instance.CreateMessageType(repName, replDesc);

                return (IMessageCreator)Activator.CreateInstance(
                        typeof(MessageCreator<,>).MakeGenericType(msg, repl));
            }
        }

        private void OnMessengerInitialization(Object sender, NodeSignalEventArgs e)
        { 
            var msgrInfo = MessageCodec.DecodeMessengerInitMsg(e.Data);

            Type[] types = null;
            Messenger instance;

            if (!(msgrInfo.MessageDescriptions is null))
            {
                types = new Type[msgrInfo.MessageDescriptions.Length];

                for (Int32 i = 0; i < types?.Length; i++)
                    types[i] = MessageTypeCreator.Instance.
                        CreateMessageType(
                        $"NodeType_{m_messageNode.Identity}_{msgrInfo.Id}_{i}",
                        msgrInfo.MessageDescriptions[i]);

                instance = (Messenger)Activator.CreateInstance(
                    MessengerTypeCreator.CreateMessengerType(msgrInfo.Type, types), msgrInfo);
            }
            else instance = new MockMessenger(msgrInfo);

            if (instance is IServiceMessenger<Message, Message> service)
            {
                var innerNewMessageBuffer = new TransformBlock<Message, (Node Node, IList<Byte[]> Msg)>((msg) =>
                    (m_messageNode, MessageCodec.EncodeMessage(instance.Id, msg.GetPropertiesBytes())));
                service.GoalBuffer.LinkTo(innerNewMessageBuffer);
                innerNewMessageBuffer.LinkTo(m_messageNode.OutMessageBuffer);
            }

            if (msgrInfo.Id == NodeMessengerIndex)
            {
                NodeMessenger = instance;
                NodeMessengerAdded?.Invoke(this, new MessengerAddedEventArgs(instance));
            }
            else
            {
                Messengers.Add(msgrInfo.Id, instance);
                MessengerAdded?.Invoke(this, new MessengerAddedEventArgs(instance));
            }

            m_messageNode.TCPSocketSignalsWorker.SendNullSignal();
            /*
            ScreenLogger.Log(new LogEntry($"Node [{Node.NodeName}] initialized as {Node.NodeType}",
                LogObject.TypeCreator,
                (MsgDescription is null) ? null : new[] { new LogEntry(MsgDescription.TypesToString()) })); */
        }

        private void CallCommandBufferReply(OuterCommandCall call)
        {
            if (call.WasPerformed)
                throw new InvalidOperationException("Call was already performed");
            Contract.EndContractBlock();

            call.CommandPerformed += (_sender, _e) => CompletedCommandsBuffer.Post(call);
            CalledCommands.Add(call.CallId, call);
            m_encodeCommandCallTransform.Post(call);
        }

        private void CallCommand(OuterCommandCall call)
        {
            if (call.WasPerformed)
                throw new InvalidOperationException("Call was already performed");
            Contract.EndContractBlock();

            CalledCommands.Add(call.CallId, call);
            m_encodeCommandCallTransform.Post(call);
        }

        public event EventHandler<MessengerAddedEventArgs> MessengerAdded;
        public event EventHandler<MessengerAddedEventArgs> NodeMessengerAdded;

        public String Name => m_messageNode.Name;
        public Int32 Identity => m_messageNode.Identity;
        public NodeStatus Status => m_messageNode.Status;
        public Messenger NodeMessenger { get; private set; }

        public Dictionary<Byte, Messenger> Messengers { get; }
        public Dictionary<Int32, OuterCommandCall> CalledCommands { get; }

        internal Node MessageNode => m_messageNode;
        private Node m_messageNode;

        private ActionBlock<IList<Byte[]>> m_processMessageAction;
        private ActionBlock<IList<Byte[]>> m_processCommandAction;

        private TransformBlock<OuterCommandCall, (Int32, IList<Byte[]>)> m_encodeCommandCallTransform;

        public static BufferBlock<OuterCommandCall> CompletedCommandsBuffer { get; }
    }
}
