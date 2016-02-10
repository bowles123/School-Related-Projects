using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpResponder
{
    public class Receiver
    {
        private bool _keepGoing;
        private readonly byte[] _buffer = new byte[256];

        public void Run()
        {
            IPEndPoint myEp = new IPEndPoint(IPAddress.Any, 15000);
            TcpListener server = new TcpListener(myEp);
            server.Start();

            _keepGoing = true;
            while (_keepGoing)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                ThreadPool.QueueUserWorkItem(ReceiveDataFromClient, stream);
            }
        }

        private void ReceiveDataFromClient(object myStream)
        {
            NetworkStream stream = myStream as NetworkStream;

            if (stream != null)
            {
                bool stayConnected = true;
                while (stayConnected)
                    stayConnected = ReadSomeData(stream);
            }
        }

        private bool ReadSomeData(NetworkStream stream)
        {
            bool stayConnected = true;
            try
            {
                int bytesRead = stream.Read(_buffer, 0, _buffer.Length);
                if (bytesRead > 0)
                {
                    string data = System.Text.Encoding.ASCII.GetString(_buffer, 0, bytesRead);
                    Console.WriteLine("Received {0} bytes: {1}", bytesRead, data);

                    if (data == "$" || data == "*")
                        stayConnected = false;

                    if (data == "$")
                        _keepGoing = false;
                }
            }
            catch (Exception err)
            {
                if (err is IOException && err.InnerException is SocketException &&
                    (err.InnerException as SocketException).SocketErrorCode == SocketError.ConnectionReset)
                    Console.WriteLine("Sender closed the connection");
                else
                    Console.WriteLine(err);
                stayConnected = false;
            }

            return stayConnected;
        }
    }
}
