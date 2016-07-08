using Messages;
using SharedObjects;

namespace CommunicationSubsystem
{
    public abstract class RequestReplyProxy: TimeoutRetryConversation
    {
        /// <summary>
        /// Proxy EndPoint of the conversation.
        /// </summary>
        public PublicEndPoint ProxyEP
        {
            get { return proxyEndPoint; }
            protected set { proxyEndPoint = value; }
        }
        public Routing RouteMessage { get; set; }

        private PublicEndPoint proxyEndPoint;
    }
}
