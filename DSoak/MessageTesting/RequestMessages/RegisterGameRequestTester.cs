using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class RegisterGameRequestTester
    {
        [TestMethod]
        public void RegisterGameRequest_TestEverything()
        {
            RegisterGameRequest r1 = new RegisterGameRequest() { Game = new GameInfo() { GameId = 10 } };
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.Game.GameId);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            RegisterGameRequest r2 = m2 as RegisterGameRequest;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.Game.GameId, r2.Game.GameId);
        }
    }
}