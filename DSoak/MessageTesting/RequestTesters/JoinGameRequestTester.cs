using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting
{
    [TestClass]
    public class JoinGameRequestTester
    {
        [TestMethod]
        public void JoinGameRequest_TestDefaultConstructor()
        {
            JoinGameRequest request = new JoinGameRequest();
            Assert.AreEqual(0, request.GameId);
            Assert.IsNull(request.Player);
        }

        [TestMethod]
        public void JoinGameRequest_TestProperties()
        {
            ProcessInfo player = new ProcessInfo()
            {
                ProcessId = 1,
                Type = ProcessInfo.ProcessType.Player,
                EndPoint = new PublicEndPoint() { HostAndPort = "abc.usu.edu:12345" },
                Label = "Test Process",
                Status = ProcessInfo.StatusCode.Initializing,
                AliveTimestamp = DateTime.Now 
            };

            JoinGameRequest request = new JoinGameRequest()
            {
                GameId = 10,
                Player = player
            };
            Assert.AreEqual(10, request.GameId );
            Assert.AreSame(player,request.Player);
        }

        [TestMethod]
        public void JoinGameRequest_EncodeAndDecode()
        {
            ProcessInfo player = new ProcessInfo()
            {
                ProcessId = 1,
                Type = ProcessInfo.ProcessType.Player,
                EndPoint = new PublicEndPoint() { HostAndPort = "abc.usu.edu:12345" },
                Label = "Test Process",
                Status = ProcessInfo.StatusCode.Initializing,
                AliveTimestamp = DateTime.Now
            };

            JoinGameRequest request = new JoinGameRequest()
            {
                GameId = 10,
                Player = player
            };

            byte[] bytes = request.Encode();

            Message decodedMessage = Message.Decode(bytes);

            JoinGameRequest decodedRequest = decodedMessage as JoinGameRequest;
            Assert.IsNotNull(decodedRequest);
            Assert.AreEqual(request.GameId, decodedRequest.GameId);
            Assert.IsNotNull(decodedRequest.Player);
            Assert.AreEqual(request.Player.ProcessId, decodedRequest.Player.ProcessId);
            Assert.AreEqual(request.Player.Type, decodedRequest.Player.Type);
            Assert.AreEqual(request.Player.EndPoint, decodedRequest.Player.EndPoint);
            Assert.AreEqual(request.Player.Status, decodedRequest.Player.Status);
            Assert.IsNull(decodedRequest.Player.AliveTimestamp);
        }
    }
}
