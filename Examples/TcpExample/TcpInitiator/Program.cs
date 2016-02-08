using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpInitiator
{
    public class Program
    {
        static void Main(string[] args)
        {
            string messageTosend = (args.Length > 0) ? args[0] : "Nothing to says";
            TcpClient client = new TcpClient("127.0.01", 15000);
            while (!client.Connected)
                Thread.Sleep((100));
            NetworkStream stream = client.GetStream();

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(messageTosend);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
