using System.Net;
using System.Net.Sockets;
using SharedObjects;
using Messages;
using log4net;

namespace CommunicationSubsystem
{
    public class Communicator
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Communicator));

        public UdpClient MySocket { get; set; }
        public ConversationDictionary Dictionary { get; set; }
        public int Port { get { return ((IPEndPoint)MySocket?.Client.LocalEndPoint).Port; } }

        public Communicator()
        {
            MySocket = new UdpClient(0);
        }

        /// <summary>
        /// Sends an envelope to the specified endpoint.
        /// </summary>
        public void Send(Envelope envelope)
        {
            logger.DebugFormat("Attempting to send envelope with a {0} message.", envelope.Message);

            if (envelope.IsValidToSend)
            {
                byte[] request = envelope.Message.Encode();
                MySocket.Send(request, request.Length, envelope.Endpoint.IPEndPoint);
                logger.DebugFormat("Successfully sent envelope with a {0} message.", envelope.Message);
            }
            else
                logger.Debug("The envelope is not valid to send.");
        }

        /// <summary>
        /// Retrieves envelope from any endpoint.
        /// </summary>
        public Envelope Retrieve(int timeout)
        {
            logger.Debug("Attempting to retrieve an envelope.");

            Envelope envelope = null;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = null;

            MySocket.Client.ReceiveTimeout = timeout;

            try
            {
                bytes = MySocket.Receive(ref endpoint);
                logger.Debug("Recieved envelope.");
            }
            catch
            {
                logger.Debug("Didn't recieve envelope before timeout.");
            }
            
            if (bytes != null)
            {
                envelope = new Envelope()
                {
                    Message = Message.Decode(bytes),
                    Endpoint = new PublicEndPoint() { IPEndPoint = endpoint }
                };
                logger.DebugFormat("Successfully retrieved a {0} message", envelope.Message);
            }
            return envelope;
        }
    }
}
