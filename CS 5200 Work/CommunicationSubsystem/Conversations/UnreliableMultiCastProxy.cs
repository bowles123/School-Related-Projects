using System.Collections.Generic;
using SharedObjects;

namespace CommunicationSubsystem
{
    public abstract class UnreliableMultiCastProxy: OneWayConversation
    {
        /// <summary>
        /// Proxy EndPoint of the conversation.
        /// </summary>
        public PublicEndPoint ProxyEP
        {
            get { return proxyEndPoint; }
            protected set { proxyEndPoint = value; }
        }

        private PublicEndPoint proxyEndPoint;
    }
}
