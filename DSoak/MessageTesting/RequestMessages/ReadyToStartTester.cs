using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class ReadyToStartTester
    {
        [TestMethod]
        public void ReadyToStart_TestEverything()
        {
            ReadyToStart r1 = new ReadyToStart() { GameId = 10};
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.GameId);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            ReadyToStart r2 = m2 as ReadyToStart;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.GameId, r2.GameId);
        }
    }
}