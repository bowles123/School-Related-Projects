using System;
using System.Net.Sockets;

namespace TcpInitiator
{
    public class Sender
    {

        public string MessageToSend { get; set; }

        public int RepeatCount { get; set; }

        public void Run()
        {
            // Create a TcpClient
            TcpClient client = new TcpClient();

            // Connect the client to the server -- remember that TCP is connection orient
            client.Connect("127.0.0.1", 15000);

            // Note that previous two statement can be combined using one of the
            // TcpClient constructs:
            //
            //  TcpClient client = new TcpClient("127.0.0.1", 15000);
            //
            // IMPORTANT: This constructor is NOT the same as one the takes an IPEndPoint
            // as a parameter.  That constructor binds the TcpClient to a local end point;
            // it does not connect the client to a remote end point.

            NetworkStream stream = client.GetStream();
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(MessageToSend);
            for (int i = 0; i < RepeatCount; i++)
                stream.Write(bytes, 0, bytes.Length); 
   
            // Note that if we don't pause and thereby keep the channel open, it will
            // close the connection before the receiver has a chance to receive the data
            Console.WriteLine("Hit ENTER to exit");
            Console.ReadLine();
        }
    }
}
