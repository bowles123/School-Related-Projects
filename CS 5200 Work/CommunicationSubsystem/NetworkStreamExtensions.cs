using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using SharedObjects;
using log4net;

namespace CommunicationSubsystem
{
    public static class NetworkStreamExtensions
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(NetworkStreamExtensions));

        public static bool WriteStreamMessage(this NetworkStream stream, Penny penny)
        {
            bool result = false;
            Logger.DebugFormat("In WriteStreamMessage, penny={0}", (penny == null) ? "null" : penny.Id.ToString());
            if (stream != null && penny != null)
            {
                byte[] bytes = penny.Encode();
                byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bytes.Length));
                if (stream.CanWrite)
                {
                    try
                    {
                        stream.Write(lengthBytes, 0, lengthBytes.Length);
                        stream.Write(bytes, 0, bytes.Length);
                        result = true;
                        Logger.Debug("Write complete");
                    }
                    catch (Exception err)
                    {
                        Logger.Error(err.Message);
                    }
                }
                else
                    Logger.Warn("Stream is not writable");
            }
            return result;
        }

        public static Penny ReadStreamMessage(this NetworkStream stream)
        {
            Logger.DebugFormat("In ReadStreamMessage, with stream.ReadTimeout={0}", (stream==null) ? "null" : stream.ReadTimeout.ToString());

            Penny result = null;

            int bytesRead = 4;
            byte[] bytes = ReadBytes(stream, bytesRead);

            Logger.DebugFormat("Length bytes read = {0}", bytes.Length);

            if (bytesRead == bytes.Length)
            {
                int messageLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
                Logger.DebugFormat("Incoming message will be {0} bytes", messageLength);

                bytes = ReadBytes(stream, messageLength);
                Logger.DebugFormat("Message bytes read = {0}", bytes.Length);

                if (messageLength == bytes.Length)
                    result = Penny.Decode(bytes);
            }
            return result;
        }

        private static byte[] ReadBytes(NetworkStream stream, int bytesToRead)
        {
            byte[] bytes = new byte[bytesToRead];
            int bytesRead = 0;

            Logger.DebugFormat("Try to read {0} length bytes, with stream.CanRead={1} and stream.ReadTimeout={2}", bytesToRead, stream.CanRead, stream.ReadTimeout);

            int remainingTime = stream.ReadTimeout;
            while (stream.CanRead && bytesRead < bytesToRead && remainingTime > 0)
            {
                DateTime ts = DateTime.Now;
                try
                {
                    bytesRead += stream.Read(bytes, bytesRead, bytes.Length - bytesRead);
                }
                catch (IOException) { }
                catch (Exception err)
                {
                    Logger.Warn(err.GetType());
                    Logger.Warn(err.Message);
                }
                remainingTime -= Convert.ToInt32(DateTime.Now.Subtract(ts).TotalMilliseconds);
            }

            if (bytesToRead != bytesRead)
            {
                if (bytesRead > 0)
                    throw new ApplicationException(string.Format("Expected {0} bytes of messsage data, but only {1} of them arrived within {2} ms", bytesToRead, bytesRead, stream.ReadTimeout));
                bytes = new byte[0];
            }
            return bytes;
        }

    }
}
