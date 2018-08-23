using MessagingCore.Events;
using MessagingCore.Main;
using MiddlewareLogic.Events;
using MiddlewareLogic.Router.Base;
using System;
using System.Collections.Generic;

namespace MiddlewareLogic.Router
{
    public sealed class RouteWorker
    {
        public RouteWorker()
        {
            m_worker = new MessageWorker();
            m_rnodes = new Dictionary<Int32, RNode>();
            m_worker.NodeRegistered += OnNodeAdded;
        }

        private void OnNodeAdded(Object sender, NodeRegisteredEventArgs e)
        {
            var rnode = new RNode(e.RegisteredNode);
            m_rnodes.Add(rnode.Identity, rnode);

            RNodeAdded?.Invoke(this, new RNodeAddedEventArgs(rnode));
        }

        public void Start() => m_worker.Start();

        public void Stop() => m_worker.Close();

        public Dictionary<Int32, RNode> RNodes => m_rnodes;

        public event EventHandler<RNodeAddedEventArgs> RNodeAdded;

        private MessageWorker m_worker;
        private Dictionary<Int32, RNode> m_rnodes;
    }
}
