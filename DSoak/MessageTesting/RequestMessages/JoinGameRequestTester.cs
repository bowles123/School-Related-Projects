using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class JoinGameRequestTester
    {
        [TestMethod]
        public void JoinGameRequest_TestEverything()
        {
            JoinGameRequest r1 = new JoinGameRequest() { GameId = 10, Process = new ProcessInfo() { ProcessId = 11 }};
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.GameId);
            Assert.AreEqual(11, r1.Process.ProcessId);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            JoinGameRequest r2 = m2 as JoinGameRequest;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.GameId, r2.GameId);
            Assert.AreEqual(r1.Process.ProcessId, r2.Process.ProcessId);
        }
    }
}