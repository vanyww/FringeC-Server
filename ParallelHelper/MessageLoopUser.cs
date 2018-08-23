using System;
using System.Threading.Tasks.Dataflow;

namespace ParallelHelper
{
    public abstract class MessageLoopUser<T> : IDisposable
    {
        public MessageLoopUser() =>
            m_worker = new ActionBlock<T>((obj) => ProcessMessage(obj));

        public void HandleMessage(T message) =>
            m_worker.Post(message);

        protected abstract void ProcessMessage(T message);

        public void Dispose()
        {
            m_worker.Complete();
            m_worker.Completion.Wait();
        }

        private ActionBlock<T> m_worker;
    }
}