using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TcpResponder
{
    public class Program
    {
        static void Main(string[] args)
        {
            Receiver receiver = new Receiver();
            receiver.Run();
        }

    }
}
