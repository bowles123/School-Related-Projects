using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace SimpleUDPSocket
{
    public class SimpleSender
    {
        private static int nextId = 0;
        private UdpClient myUdpClient;

        public List<IPEndPoint> Peers { get; set; }

        public SimpleSender()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
            myUdpClient = new UdpClient(localEP);
            Peers = new List<IPEndPoint>();
        }

        public void SendStuff()
        {
            string cmd = string.Empty;
            while (string.IsNullOrEmpty(cmd) || cmd.Trim().ToUpper() != "EXIT")
            {
                Console.Write("A=Add Peer, S=Send Message, or EXIT: " );
                cmd = Console.ReadLine();
                switch (cmd.Trim().ToUpper())
                {
                    case "A":
                        AddPeer();
                        break;
                    case "S":
                        AskForMessageAndSend();
                        break;
                    case "EXIT":
                        SendToPeers(cmd);
                        break;
                }
            }
        }

        private void AddPeer()
        {
            Console.Write("Enter Peer EP (host:port): ");
            string peer = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(peer))
            {
                IPEndPoint peerAddress = EndPointParser.Parse(peer);
                if (peerAddress!=null)
                    Peers.Add(peerAddress);
            }
        }

        private void AskForMessageAndSend()
        {
            string message = string.Empty;
            Console.Write("Enter a message to send: ");
            message = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(message))
                SendToPeers(message);
        }

        private void SendToPeers(string message)
        {
            if (Peers != null && Peers.Count > 0)
            {
                Message msg = new Message()
                {
                    Id = GetNextId(),
                    Timestamp = DateTime.Now,
                    Text = message
                };

                byte[] bytes = msg.Encode();

                foreach (IPEndPoint ep in Peers)
                {
                    int bytesSent = myUdpClient.Send(bytes, bytes.Length, ep);
                    Console.WriteLine("Send to {0} was {1}", ep, (bytesSent == bytes.Length) ? "Successful" : "Not Successful");
                }
            }
        }

        private int GetNextId()
        {
            if (nextId == Int32.MaxValue)
                nextId = 0;
            return nextId++;
        }
    }
}
