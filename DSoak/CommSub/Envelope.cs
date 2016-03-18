using System.Net;

using Messages;
using SharedObjects;

namespace CommSub
{
    public class Envelope
    {
        public Message Message { get; set; }
        
        public PublicEndPoint EndPoint { get; set; }

        public Envelope() {}

        public Envelope(Message message, PublicEndPoint endPoint)
        {
            Message = message;
            EndPoint = endPoint;
        }

        public Envelope(Message message, IPEndPoint ep) :
            this(message, (ep != null) ? new PublicEndPoint() { IPEndPoint = ep } : null) {}

        public IPEndPoint IPEndPoint
        {
            get
            {
                return (EndPoint == null) ?
                    new IPEndPoint(IPAddress.Any, 0) :
                    EndPoint.IPEndPoint;
            }
            set { EndPoint = (value == null) ? null : new PublicEndPoint() { IPEndPoint = value }; }
        }

        public bool IsValidToSend
        {
            get
            {
                return (Message != null &&
                        EndPoint != null &&
                        EndPoint.Host!="0.0.0.0" &&
                        EndPoint.Port!=0);
            }
        }
    }
}
