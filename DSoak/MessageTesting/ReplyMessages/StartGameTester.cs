using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;

namespace MessageTesting.ReplyMessages
{
    [TestClass]
    public class StartGameTester
    {
        [TestMethod]
        public void StartGameReply_TestEverything()
        {
            StartGame r1 = new StartGame();
            Assert.IsFalse(r1.Success);
            Assert.IsNull(r1.Note);

            StartGame r2 = new StartGame()
            {
                Success = true,
                Note = "Testing"
            };

            Assert.IsTrue(r2.Success);
            Assert.AreEqual("Testing", r2.Note);

            byte[] bytes = r2.Encode();
            string tmp = Encoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            StartGame r3 = m2 as StartGame;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreEqual(r2.Note, r3.Note);
        }
    }
}
