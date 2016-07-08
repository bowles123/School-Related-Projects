using System;
using System.Net;
using Messages;
using SharedObjects;

namespace CommunicationSubsystem
{
    public class Envelope
    {
        public Message Message { get; set; }
        public PublicEndPoint Endpoint { get; set; }

        public IPEndPoint IPEndPoint
        {
            get
            {
                return (Endpoint == null) ?
                    new IPEndPoint(IPAddress.Any, 0) :
                    Endpoint.IPEndPoint;
            }
            set { Endpoint = (value == null) ? null : new PublicEndPoint() { IPEndPoint = value }; }
        }

        /// <summary>
        /// States whether an envelope is valid to send.
        /// </summary>
        public bool IsValidToSend
        {
            get
            {
                return (Message != null &&
                        Endpoint != null &&
                        Endpoint.Host != "0.0.0.0" &&
                        Endpoint.Port != 0);
            }
        }

        /// <summary>
        /// Returns the actual message within the envelope
        /// </summary>
        public Message ActualMessage
        {
            get
            {
                Routing routing = Message as Routing;
                Message result = (routing != null) ? routing.InnerMessage : Message;
                return result;
            }
        }

        /// <summary>
        /// Returns the actual message type within the envelope
        /// </summary>
        public Type ActualMessageType
        {
            get
            {
                Type type = null;
                Message m = ActualMessage;
                if (m != null)
                    type = m.GetType();
                return type;
            }
        }
    }
}
