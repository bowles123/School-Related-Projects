using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyMessages
{
    [TestClass]
    public class BalloonReplyTester
    {
        [TestMethod]
        public void BalloonReply_TestEverything()
        {
            BalloonReply r1 = new BalloonReply();
            Assert.IsFalse(r1.Success);
            Assert.IsNull(r1.Note);

            BalloonReply r2 = new BalloonReply()
            {
                Success = true,
                Note = "Testing",
                Balloon = new Balloon() { Id = 10 }
            };

            Assert.IsTrue(r2.Success);
            Assert.AreEqual("Testing", r2.Note);
            Assert.IsNotNull(r2.Balloon);
            Assert.AreEqual(10, r2.Balloon.Id);

            byte[] bytes = r2.Encode();
            string tmp = Encoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            BalloonReply r3 = m2 as BalloonReply;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreEqual(r2.Note, r3.Note);
            Assert.IsNotNull((r3.Balloon));
            Assert.AreEqual(10, r3.Balloon.Id);
        }
    }
}
