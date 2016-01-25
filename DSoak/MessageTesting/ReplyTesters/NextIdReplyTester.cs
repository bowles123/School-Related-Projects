using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyTesters
{
    [TestClass]
    public class NextIdTester
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
                Note = "Testing"
            };

            Assert.IsTrue(r2.Success);
            Assert.AreEqual("Testing", r2.Note);
            
            byte[] bytes = r2.Encode();

            Message m2 = Message.Decode(bytes);
            NextIdReply r3 = m2 as NextIdReply;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreEqual(r2.Note, r3.Note);
        }
    }
}
