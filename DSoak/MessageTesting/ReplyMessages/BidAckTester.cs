using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyMessages
{
    [TestClass]
    public class BidAckTester
    {
        [TestMethod]
        public void BidAck_TestEverything()
        {
            BidAck r1 = new BidAck();
            Assert.IsFalse(r1.Success);
            Assert.IsNull(r1.Note);

            BidAck r2 = new BidAck()
            {
                Success = true,
                Note = "Testing",
                Umbrella = new Umbrella() { Id = 10 }
            };

            Assert.IsTrue(r2.Success);
            Assert.AreEqual("Testing", r2.Note);
            Assert.IsNotNull(r2.Umbrella);
            Assert.AreEqual(10, r2.Umbrella.Id);

            byte[] bytes = r2.Encode();
            string tmp = Encoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            BidAck r3 = m2 as BidAck;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreEqual(r2.Note, r3.Note);
            Assert.IsNotNull((r3.Umbrella));
            Assert.AreEqual(10, r3.Umbrella.Id);
        }
    }
}
