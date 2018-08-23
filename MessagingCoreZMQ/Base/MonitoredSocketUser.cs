using Logging;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ZeroMQ;
using ZeroMQ.Monitoring;

namespace MessagingCore.Base
{
    public abstract class MonitoredSocketUser
    {
        private const String TCPHostAdress = "*";
        private static readonly TimeSpan MinPollWaitTime,
                                         ZMQLinger;
        private const Int32 RCVHWM = 1000,
                            SNDHWM = 1000;

        static MonitoredSocketUser()
        {
            MinPollWaitTime = TimeSpan.FromMilliseconds(64);
            ZMQLinger = TimeSpan.Zero;
        }

        protected MonitoredSocketUser(String monitorAddress, ZSocketType socketType, TimeSpan pollWaitTime)
        {
            m_context = new ZContext();
            m_socket = new ZSocket(m_context, socketType)
            {
                Linger = ZMQLinger,
                ReceiveHighWatermark = RCVHWM,
                SendHighWatermark = SNDHWM
            };
            m_socket.Monitor(monitorAddress);
            m_monitor = ZMonitor.Create(m_context, monitorAddress);
            m_poller = ZPollItem.CreateReceiver();
            m_monitor.AllEvents += (sender, e) =>
                ScreenLogger.Log(
                        ((Int32)e.Event.Event is 4096) ?
                        $"Event fired on socket {e.Event.Address}: Handshake Success" :
                        $"Event fired on socket {e.Event.Address}: {e.Event.Event}",
                    LogObject.EventManager);
            m_cancelCondition = true;

            m_receiveBuffer = new BufferBlock<ZMessage>();
            m_sendAction = new ActionBlock<ZMessage>((msg) => { m_socket.Send(msg); msg.Dispose(); });

            m_pollWaitTime = MinPollWaitTime;
            if (pollWaitTime > m_pollWaitTime)
                m_pollWaitTime = pollWaitTime;

            ListenLoop();
        }

        ~MonitoredSocketUser() => StopWork();

        protected void StartWork(String port)
        {
            m_monitor.Start();
            var endpoint = $"tcp://{TCPHostAdress}:{port}";
            m_socket.Bind(endpoint);

            ScreenLogger.Log($"Socket binded to endpoint: {endpoint}", LogObject.System);
        }

        //if already called
        protected void StopWork()
        {
            var endpoint = m_socket.LastEndpoint;

            m_cancelCondition = false;
            m_loopTask.Wait();
            m_monitor.Close();
            m_socket.Close();
            m_context.Terminate();

            ScreenLogger.Log($"Socket stopped work: {endpoint}", LogObject.System);
        }

        private void ListenLoop()
        {
            m_loopTask =
            Task.Run(() =>
            {
                while (m_cancelCondition)
                {
                    if (m_socket.PollIn(m_poller, out ZMessage msg, out ZError error, m_pollWaitTime))
                    {
                        m_receiveBuffer.Post(msg);
                    }
                    else
                    {
                        if (error == ZError.ETERM)
                            return;
                        if (error != ZError.EAGAIN)
                            throw new ZException(error);
                    }
                }
            });
        }

        protected readonly BufferBlock<ZMessage> m_receiveBuffer;

        protected readonly ZSocket m_socket;
        protected readonly ZMonitor m_monitor;

        protected ActionBlock<ZMessage> m_sendAction;

        private ZContext m_context;
        private ZPollItem m_poller;

        private TimeSpan m_pollWaitTime;

        private Boolean m_cancelCondition;
        private Task m_loopTask;
    }
}
