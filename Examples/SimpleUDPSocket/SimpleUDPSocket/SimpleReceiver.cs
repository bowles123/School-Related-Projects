using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

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

        public void DoStuff()
        {
            DisplayEndPoints();

            Console.WriteLine();
            Console.WriteLine("Receiving...");
            string message = string.Empty;
            while (message.Trim().ToUpper() != "EXIT")
            {
                IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
                byte[] receiveBuffer = null;
                try
                {
                    receiveBuffer = _myUdpClient.Receive(ref remoteEp);
                }
                catch (SocketException err)
                {
                    Console.WriteLine("Timeout");
                }
                if (receiveBuffer != null)
                {
                    message = Encoding.Unicode.GetString(receiveBuffer);
                    Console.WriteLine("Message from " + remoteEp.ToString() + " --> " + message);
                }
            }
        }

        private void DisplayEndPoints()
        {
            Console.WriteLine();
            IPEndPoint localEp = _myUdpClient.Client.LocalEndPoint as IPEndPoint;
            if (localEp!=null && localEp.Address.Equals(IPAddress.Any))
            {
                Console.WriteLine("Receiver accepting messages at the following end points");
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                        DisplayAddressesForAdapter(adapter, localEp.Port);
                }
            }
            else
            {
                Console.WriteLine("Receiver accepting messages at the following end point");
                Console.WriteLine("\t{0}", localEp);
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
