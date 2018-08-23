using CommandPortLogic.Wrappers;                                                    
using MessagingCore.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks.Dataflow;
using TypeCreator;
using TypeCreator.Base;
using TypeCreator.Enums;

namespace CommandPortLogic.Main
{
    public sealed class OuterCommandsManager
    {
        public const Int32 SystemId = -1;

        private OuterCommandsManager()
        {
            m_outerCommands = new Dictionary<(Int32, String), OuterCommand>();
            m_calledCommands = new Dictionary<Int32, OuterCommandCall>();
            m_worker = new CommandMessageWorker();
            m_repliesBuffer = new BufferBlock<OuterCommandCall>();
            m_commandInitAction = new ActionBlock<(Int32 Identity, Byte[][] Msg)>(
                msg => AddOuterCommand(msg.Identity, msg.Msg));
            m_commandSendAction = new ActionBlock<(Int32 Identity, Byte[][] Msg)>(
                msg => m_worker.SendMessage(msg.Identity, msg.Msg));
            m_commandReplyAction = new ActionBlock<OuterCommandCall>(
                cmd => cmd.NotifyCommandPerformed(cmd.Reply));
            m_decodeTransform = new TransformBlock<(Int32 Identity, Byte[][] Msg), OuterCommandCall>(
                msg =>
                {
                    var reply = CommandCodec.DecodeReplyIds(msg.Identity, msg.Msg);
                    var calledCommand = m_calledCommands[reply.CallId];
                    m_calledCommands.Remove(reply.CallId);
                    calledCommand.Reply = CommandCodec.DecodeReplyFully(reply, calledCommand, msg.Msg);
                    return calledCommand;
                });
            m_encodeTransform = new TransformBlock<OuterCommandCall, (Int32 Identity, Byte[][] Msg)>(
                call => CommandCodec.EncodeCommandCall(call));

            m_worker.CommandInitMessagesBuffer.LinkTo(m_commandInitAction);
            m_worker.CommandOuterMessageBuffer.LinkTo(m_decodeTransform);
            m_decodeTransform.LinkTo(m_commandReplyAction);
            m_encodeTransform.LinkTo(m_commandSendAction);
        }

        public void AddOuterCommand(Int32 identity, Byte[][] message)
        {
            var commandInfo = CommandCodec.DecodeCommandInit(identity, message);
            commandInfo.MessageWrapper = CreateWrapper($"c_{commandInfo.Name}{commandInfo.Node}_msg",
                                                       commandInfo.RequestDescription,
                                                       $"c_{commandInfo.Name}{commandInfo.Node}_repl",
                                                       commandInfo.ReplyDescription);

            m_outerCommands.Add((commandInfo.Node, commandInfo.Name), commandInfo);
            CommandAdded?.Invoke(this, commandInfo);

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

        public void CallCommandBufferReply(OuterCommandCall call)
        {
            if (call.WasPerformed)
                throw new InvalidOperationException("Call was already performed");
            Contract.EndContractBlock();

            call.CommandPerformed += (sender, reply) =>
                m_repliesBuffer.Post((OuterCommandCall)sender);
            CallCommand(call);
        }

        public void CallCommand(OuterCommandCall call)
        {
            if (call.WasPerformed)
                throw new InvalidOperationException("Call was already performed");
            Contract.EndContractBlock();

            m_calledCommands.Add(call.CallId, call);
            m_encodeTransform.Post(call);
        }

        public Boolean TryGetCommand((Int32, String) id, out OuterCommand command) =>
            m_outerCommands.TryGetValue(id, out command);

        public void Start() => m_worker.Start();
        public void Stop() => m_worker.Close();

        public static void Initialize()
        {
            if (m_instance is null)
                m_instance = new OuterCommandsManager();
        }

        public static OuterCommandsManager Instance => m_instance ?? (m_instance = new OuterCommandsManager());
        private static OuterCommandsManager m_instance;

        public BufferBlock<OuterCommandCall> RepliesBuffer => m_repliesBuffer;
        public event EventHandler<OuterCommand> CommandAdded;

        private BufferBlock<OuterCommandCall> m_repliesBuffer;
        private ActionBlock<(Int32, Byte[][])> m_commandInitAction,
                                               m_commandSendAction;
        private ActionBlock<OuterCommandCall> m_commandReplyAction;
        private TransformBlock<(Int32 Identity, Byte[][] Msg), OuterCommandCall> m_decodeTransform;
        private TransformBlock<OuterCommandCall, (Int32 Identity, Byte[][] Msg)> m_encodeTransform;

        private CommandMessageWorker m_worker;
        private Dictionary<(Int32, String), OuterCommand> m_outerCommands;
        private Dictionary<Int32, OuterCommandCall> m_calledCommands;
    }
}
