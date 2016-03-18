using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

using Messages;
using SharedObjects;
using log4net;

namespace CommSub
{
    public class Communicator
    {
        #region Private Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Communicator));
        private static readonly ILog LoggerDeep = LogManager.GetLogger(typeof(Communicator) + "_Deep");

        private UdpClient _myUdpClient;
        #endregion

        #region Public Properties
        public int MinPort { get; set; }
        public int MaxPort { get; set; }
        public int Port { get { return (_myUdpClient != null) ? ((IPEndPoint)_myUdpClient.Client.LocalEndPoint).Port : 0; } }

        #endregion

        #region Public Methods
        public void Start()
        {
            Logger.Debug("Start communicator");
            bool bindSuccessfull = false;

            ValidPorts();

            int portToTry = MinPort;
            while (!bindSuccessfull && portToTry <= MaxPort)
            {
                try
                {
                    IPEndPoint localEp = new IPEndPoint(IPAddress.Any, portToTry);
                    _myUdpClient = new UdpClient(localEp);
                    bindSuccessfull = true;
                }
                catch (SocketException)
                {
                    portToTry++;
                }
            }
            if (!bindSuccessfull)
                throw new ApplicationException("Cannot bind the socket to a port");
        }

        public void Stop()
        {
            Logger.Debug("Stop of communicator");
            if (_myUdpClient != null)
            {
                _myUdpClient.Close();
                _myUdpClient = null;
            }
        }

        public int IncomingAvailable()
        {
            return ((_myUdpClient==null) ? 0 : _myUdpClient.Available);
        }

        public Envelope Receive(int timeout = 0)
        {
            Envelope result = null;

            IPEndPoint ep;
            byte[] receivedBytes = ReceiveBytes(timeout, out ep);
            if (receivedBytes != null && receivedBytes.Length>0)
            {
                PublicEndPoint pep = new PublicEndPoint() { IPEndPoint = ep };
                Message message = Message.Decode(receivedBytes);
                if (message != null)
                {
                    result = new Envelope(message, pep);
                    Logger.DebugFormat("Just received message, Nr={0}, Conv={1}, Type={2}, From={3}",
                        (result.Message.MsgId==null) ? "null" : result.Message.MsgId.ToString(),
                        (result.Message.ConvId==null) ? "null" : result.Message.ConvId.ToString(),
                        result.Message.GetType().Name,
                        (result.IPEndPoint==null) ? "null" : result.IPEndPoint.ToString());
                }
                else
                {
                    Logger.ErrorFormat("Cannot decode message received from {0}", pep);
                    string tmp = Encoding.ASCII.GetString(receivedBytes);
                    Logger.ErrorFormat("Message={0}", tmp);
                }
            }
        
            return result;
        }

        public bool Send(Envelope outgoingEnvelope)
        {
            bool result = false;
            if (outgoingEnvelope == null || !outgoingEnvelope.IsValidToSend)
                Logger.Warn("Invalid Envelope or Message");
            else
            {
                byte[] bytesToSend =outgoingEnvelope.Message.Encode();

                Logger.DebugFormat("Send out: {0} to {1}", Encoding.ASCII.GetString(bytesToSend), outgoingEnvelope.EndPoint);

                try
                {
                    _myUdpClient.Send(bytesToSend, bytesToSend.Length, outgoingEnvelope.EndPoint.IPEndPoint);
                    result = true;
                    Logger.Debug("Send complete");
                }
                catch (Exception err)
                {
                    Logger.Warn(err.ToString());
                }
            }
            return result;
        }

        #endregion

        #region Private Methods

        private byte[] ReceiveBytes(int timeout, out IPEndPoint ep)
        {
            byte[] receivedBytes = null;
            ep = null;
            if (_myUdpClient != null)
            {
                _myUdpClient.Client.ReceiveTimeout = timeout;
                ep = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    LoggerDeep.Debug("Try receive bytes from anywhere");
                    receivedBytes = _myUdpClient.Receive(ref ep);
                    Logger.Debug("Back from receive");

                    if (Logger.IsDebugEnabled)
                    {
                        if (receivedBytes != null)
                        {
                            string tmp = Encoding.ASCII.GetString(receivedBytes);
                            Logger.DebugFormat("Incoming message={0}", tmp);
                        }
                    }
                }
                catch (SocketException err)
                {
                    if (err.SocketErrorCode != SocketError.TimedOut && err.SocketErrorCode != SocketError.Interrupted)
                        Logger.Warn(err.Message);
                }
                catch (Exception err)
                {
                    Logger.Warn(err.Message);
                }
            }
            return receivedBytes;
        }

        private void ValidPorts()
        {
            if ((MinPort != 0 && (MinPort < IPEndPoint.MinPort || MinPort > IPEndPoint.MaxPort)) ||
                (MaxPort != 0 && (MaxPort < IPEndPoint.MinPort || MaxPort > IPEndPoint.MaxPort)))
                throw new ApplicationException("Invalid port specifications");
        }
        #endregion

    }
}
