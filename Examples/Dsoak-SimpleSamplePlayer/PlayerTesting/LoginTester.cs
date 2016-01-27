using System;
using System.Threading;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Net.Sockets;
using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace PlayerTesting
{
    [TestClass]
    public class LoginTester
    {
        [TestMethod]
        public void Login_BasicLogin()
        {
            // Create udp client that represent the registry
            UdpClient mockClient = new UdpClient();
            int mockClientPort = ((IPEndPoint) mockClient.Client.LocalEndPoint).Port;
            PublicEndPoint mockClientEP = new PublicEndPoint()
            {
                Host = "127.0.0.1",
                Port = mockClientPort
            };

            // Create player with all the necessary registry end point and identity information
            TestablePlayer player = new TestablePlayer()
            {
                RegistryEndPoint = mockClientEP,
                FirstName = "Joe",
                LastName = "Jones",
                ANumber = "A01234",
                Alias = "Joey",
                ProcessLabel = "Joey's Player"
            };

            // Initialize the player
            player.Initialize();

            // Call login method
            Thread loginThread = new Thread(new ThreadStart(player.TestLogin));
            loginThread.Start();

            // Get message from udp client
            IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = mockClient.Receive(ref senderEP);

            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);

            Message msg = Message.Decode(bytes);

            // Assert not null and is a LoginRequest
            Assert.IsNotNull(msg);
            Assert.IsTrue(msg is LoginRequest);
            LoginRequest request = msg as LoginRequest;
            Assert.AreEqual("Joe", request.Identity.FirstName);

            // Send back a LoginReply  

            // Assert ProcessInfo of player is same as mock LoginReply
            Thread.Sleep(2000);

        }
    }

    public class TestablePlayer : Player.Player
    {
        public void TestLogin()
        {
            base.TryToLogin();
        }
    }
}
