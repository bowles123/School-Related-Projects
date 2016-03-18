using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;

namespace MessageTesting.ReplyMessages
{
    [TestClass]
    public class JoinGameReplyTester
    {
        [TestMethod]
        public void JoinGameReply_TestEverything()
        {
            JoinGameReply r1 = new JoinGameReply();
            Assert.IsFalse(r1.Success);
            Assert.IsNull(r1.Note);

            JoinGameReply r2 = new JoinGameReply()
            {
                Success = true,
                Note = "Testing",
                GameId = 10,
                InitialLifePoints = 100
            };

            Assert.IsTrue(r2.Success);
            Assert.AreEqual("Testing", r2.Note);
            Assert.AreEqual(10, r2.GameId);
            Assert.AreEqual(100, r2.InitialLifePoints);

            byte[] bytes = r2.Encode();
            string tmp = Encoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            JoinGameReply r3 = m2 as JoinGameReply;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreEqual(r2.Note, r3.Note);
            Assert.AreEqual(r2.GameId, r3.GameId);
            Assert.AreEqual(r2.InitialLifePoints, r3.InitialLifePoints);
        }
    }
}
