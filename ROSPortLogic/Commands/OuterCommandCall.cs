using System;
using System.Threading;
using TypeCreator.Base;

namespace MiddlewareLogic.Commands
{
    public sealed class OuterCommandCall
    {
        public OuterCommandCall(OuterCommand command, Message callMessage,
                                Action<OuterCommandCall> callAction, Action<OuterCommandCall> callBufferAction)
        {
            m_callAction = callAction;
            m_callBufferAction = callBufferAction;
            Command = command;
            CallMessage = callMessage;

            m_wasPerformed = false;
            m_spinWait = new SpinWait();
            Reply = null;
            lock (m_lock)
                CallId = m_nextId++;
        }

        static OuterCommandCall()
        {
            m_nextId = 0;
            m_lock = new Object();
        }
        
        public void WaitForReply(CancellationToken token)
        {
            if (m_wasPerformed) return;

            while (!m_wasPerformed)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                if (!m_spinWait.NextSpinWillYield)
                    m_spinWait.SpinOnce();
            }
        }

        public void WaitForReply()
        {
            if (m_wasPerformed) return;

            while (!m_wasPerformed)
                if (!m_spinWait.NextSpinWillYield)           
                    m_spinWait.SpinOnce();
        }

        public void Call() => m_callAction(this);

        public void CallBuffered() => m_callBufferAction(this);

        internal void NotifyCommandPerformed(CommandReply repl)
        {
            CommandPerformed?.Invoke(this, repl);
            m_wasPerformed = true;
        }
        
        public Int32 CallId { get; }
        public OuterCommand Command { get; }
        public Message CallMessage { get; }
        public CommandReply Reply { get; internal set; }
        public Boolean WasPerformed => m_wasPerformed;

        public event EventHandler<CommandReply> CommandPerformed;

        private SpinWait m_spinWait;
        private Boolean m_wasPerformed;
        private static Int32 m_nextId;
        private static Object m_lock;
        private Action<OuterCommandCall> m_callAction, 
                                         m_callBufferAction;
    }
}
