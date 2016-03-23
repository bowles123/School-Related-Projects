using System.Net;
using System.Net.Sockets;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;
using SharedObjects;

namespace CommSubTesting
{
    [TestClass]
    public class NetworkStreamExtensionTester
    {
        [TestMethod]
        public void NetworkStreamExtension_TestEverything()
        {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 14000));
            listener.Start();

            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Loopback, 14000));
            Assert.IsTrue(client.Connected);
            
            TcpClient server = listener.AcceptTcpClient();
            Assert.IsNotNull(server);

            Penny p1 = new Penny() {Id = 10, DigitalSignature = new byte[] {10, 20, 30}};
            NetworkStream serverStream = server.GetStream();
            NetworkStream clientStream = client.GetStream();
            clientStream.ReadTimeout = 1000;

            serverStream.WriteStreamMessage(p1);
            Penny p2 = clientStream.ReadStreamMessage();
            Assert.IsNotNull(p2);
            Assert.AreEqual(p1.Id, p2.Id);
            Assert.AreEqual(p1.DigitalSignature.Length, p2.DigitalSignature.Length);

            for (int i = 0; i < p2.DigitalSignature.Length; i++)
                Assert.AreEqual(p1.DigitalSignature[i], p2.DigitalSignature[i]);
        }
    }
}
