using Logging;
using MessagingCore.Enums;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Utils;
using ZeroMQ;
using ZeroMQ.Monitoring;

namespace MessagingCore.Base
{
    public abstract class MonitoredDoubleSocketUser
    {
        private const String TCPHostAdress = "*";
        private const Int32 RCVHWM = 1000,
                            SNDHWM = 1000;

        private static readonly TimeSpan MinPollWaitTime,
                                         ZMQLinger;

        static MonitoredDoubleSocketUser()
        {
            MinPollWaitTime = TimeSpan.FromMilliseconds(64);
            ZMQLinger = TimeSpan.FromMilliseconds(500);
        }

        protected MonitoredDoubleSocketUser(String tcpMonitorAddress, 
                                            String udpMonitorAddress, 
                                            ZSocketType socketType, 
                                            TimeSpan pollWaitTime)
        {
            m_context = new ZContext();

            m_tcpSocket = new ZSocket(m_context, socketType)
            {
                Linger = ZMQLinger,
                ReceiveHighWatermark = RCVHWM,
                SendHighWatermark = SNDHWM
            };
            m_tcpSocket.Monitor(tcpMonitorAddress);
            m_tcpSocketMonitor = ZMonitor.Create(m_context, tcpMonitorAddress);
            m_poller = ZPollItem.CreateReceiver();
            m_tcpSocketMonitor.AllEvents += (sender, e) =>
                ScreenLogger.Log(
                        ((Int32)e.Event.Event is 4096) ?
                        $"Event fired on main TCP socket: Handshake Success" :
                        $"Event fired on main TCP socket: {e.Event.Event}",
                    LogObject.EventManager);
            /*
            m_udpSocket = new ZSocket(m_context, socketType)
            {
                Linger = ZMQLinger,
                ReceiveHighWatermark = RCVHWM,
                SendHighWatermark = SNDHWM
            };
            m_udpSocket.Monitor(udpMonitorAddress);
            m_udpSocketMonitor = ZMonitor.Create(m_context, udpMonitorAddress);
            m_poller = ZPollItem.CreateReceiver();
            m_udpSocketMonitor.AllEvents += (sender, e) =>
                ScreenLogger.Log(
                        ((Int32)e.Event.Event is 4096) ?
                        $"Event fired on main UDP socket: Handshake Success" :
                        $"Event fired on main UDP socket: {e.Event.Event}",
                    LogObject.EventManager);
            */

            m_cancelConddition = true;

            m_receiveBuffer = new BufferBlock<(TransportProtocol, ZMessage)>();
            m_tcpSocketSendAction = new ActionBlock<ZMessage>((msg) => { m_tcpSocket.Send(msg); msg.Dispose(); });
            //m_udpSocketSendAction = new ActionBlock<ZMessage>((msg) => { m_udpSocket.Send(msg); msg.Dispose(); });

            m_pollWaitTime = MinPollWaitTime;
            if (pollWaitTime > m_pollWaitTime)
                m_pollWaitTime = pollWaitTime;

            ListenLoop();
        }

        ~MonitoredDoubleSocketUser() => StopWork();

        protected void StartWork(String port)
        {
            m_tcpSocketMonitor.Start();
            //m_udpSocketMonitor.Start();

            var tcp_endpoint = $"tcp://{TCPHostAdress}:{port}";
            //var udp_endpoint = $"udp://{TCPHostAdress}:{port}";

            m_tcpSocket.Bind(tcp_endpoint);
            //m_udpSocket.Bind(udp_endpoint);

            ScreenLogger.Log($"Main TCP socket binded to endpoint: {tcp_endpoint}", LogObject.System);
            //ScreenLogger.Log($"Main UDP socket binded to endpoint: {udp_endpoint}", LogObject.System);
        }

        protected void StopWork()
        {
            var tcp_endpoint = m_tcpSocket.LastEndpoint;
            //var udp_endpoint = m_udpSocket.LastEndpoint;

            m_cancelConddition = false;
            m_loopTask.Wait();

            m_tcpSocketMonitor.Close();
            //m_udpSocketMonitor.Close();

            m_tcpSocket.Close();
            //m_udpSocket.Close();

            m_context.Terminate();

            ScreenLogger.Log($"Main TCP socket stopped work: {tcp_endpoint}", LogObject.System);
            //ScreenLogger.Log($"Main UDP socket stopped work: {udp_endpoint}", LogObject.System);
        }

        private void ListenLoop()
        {
            m_loopTask =
            Task.Run(() =>
            {
                ZMessage msg;
                ZError error;

                while (m_cancelConddition)
                {
                    if (m_tcpSocket.PollIn(m_poller, out msg, out error, m_pollWaitTime))
                        m_receiveBuffer.Post((TransportProtocol.TCP, msg));
                    else
                    {
                        if (error == ZError.ETERM)
                            return;
                        if (error != ZError.EAGAIN)
                            throw new ZException(error);
                    }
                    /*
                    if (m_udpSocket.PollIn(m_poller, out msg, out error, m_pollWaitTime))
                        m_receiveBuffer.Post((TransportProtocol.UDP, msg));
                    else
                    {
                        if (error == ZError.ETERM)
                            return;
                        if (error != ZError.EAGAIN)
                            throw new ZException(error);
                    }
                    */
                }
            });
        }

        protected readonly BufferBlock<(TransportProtocol, ZMessage)> m_receiveBuffer;

        protected readonly ZSocket m_tcpSocket;
        //                           m_udpSocket;
        protected readonly ZMonitor m_tcpSocketMonitor;
        //                            m_udpSocketMonitor;
        protected ActionBlock<ZMessage> m_tcpSocketSendAction;
        //                                m_udpSocketSendAction;

        private ZContext m_context;
        private ZPollItem m_poller;

        private TimeSpan m_pollWaitTime;

        private Boolean m_cancelConddition;
        private Task m_loopTask;
    }
}
