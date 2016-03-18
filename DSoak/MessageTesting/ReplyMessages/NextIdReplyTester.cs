using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;

namespace MessageTesting.ReplyMessages
{
    [TestClass]
    public class NextIdReplyTester
    {
        [TestMethod]
        public void NextIdReply_TestEverything()
        {
            NextIdReply r1 = new NextIdReply();
            Assert.IsFalse(r1.Success);
            Assert.IsNull(r1.Note);

            NextIdReply r2 = new NextIdReply()
            {
                Success = true,
                Note = "Testing",
                NextId = 100,
                NumberOfIds = 10
            };

            Assert.IsTrue(r2.Success);
            Assert.AreEqual("Testing", r2.Note);
            Assert.AreEqual(100, r2.NextId);
            Assert.AreEqual(10, r2.NumberOfIds);

            byte[] bytes = r2.Encode();
            string tmp = Encoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            NextIdReply r3 = m2 as NextIdReply;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreEqual(r2.Note, r3.Note);
            Assert.AreEqual(100, r3.NextId);
            Assert.AreEqual(10, r3.NumberOfIds);
        }
    }
}
