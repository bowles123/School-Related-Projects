using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace SimpleUDPSocket
{
    public class SimpleReceiver
    {
        private readonly UdpClient _myUdpClient;

        public SimpleReceiver()
        {
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 0);
            _myUdpClient = new UdpClient(localEp);
            _myUdpClient.Client.ReceiveTimeout = 1000;
        }

        public IPEndPoint LocalEp
        {
            get
            {
                IPEndPoint result = null;
                if (_myUdpClient != null)
                    result = _myUdpClient.Client.LocalEndPoint as IPEndPoint;
                return result;
            }
        }

        public void ReceiveStuff()
        {
            DisplayEndPoints();

            Console.WriteLine();
            Console.WriteLine("Receiving...");
            bool quit = false;
            while (!quit)
            {
                IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
                byte[] bytes = null;

                try
                {
                    bytes = _myUdpClient.Receive(ref remoteEp);
                }
                catch (SocketException err)
                {
                    if (err.SocketErrorCode != SocketError.TimedOut)
                        throw;
                }

                if (bytes != null)
                    quit = DecodeAndDisplayMessage(remoteEp, bytes);
            }
        }

        private bool DecodeAndDisplayMessage(IPEndPoint remoteEp, byte[] bytes)
        {
            bool quit = false;

            if (bytes != null)
            {
                Message message = Message.Decode(bytes);
                if (message != null)
                {
                    Console.WriteLine(string.Format("Message {0} send at {1} from {2} --> {3}",
                        message.Id,
                        message.Timestamp,
                        remoteEp.ToString(),
                        message.Text));
                    quit = (message.Text.Trim().ToUpper() == "EXIT");
                }
            }
            return quit;
        }


        private void DisplayEndPoints()
        {
            Console.WriteLine();
            if (LocalEp.Address.Equals(IPAddress.Any))
            {
                Console.WriteLine("Receiver accepting messages at the following end points");
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                        DisplayAddressesForAdapter(adapter, LocalEp.Port);
                }
            }
            else
            {
                Console.WriteLine("Receiver accepting messages at the following end point");
                Console.WriteLine("\t{0}", LocalEp);
            }
        }

        private void DisplayAddressesForAdapter(NetworkInterface adapter, int port)
        {
            IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
            UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
            if (uniCast != null)
            {
                foreach (UnicastIPAddressInformation uni in uniCast)
                    if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                        Console.WriteLine("\t{0}:{1}", uni.Address, port);
            }
        }
    }
}
