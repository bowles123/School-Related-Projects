using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyMessages
{
    [TestClass]
    public class BidTester
    {
        [TestMethod]
        public void Bid_TestEverything()
        {
            Bid r1 = new Bid();
            Assert.IsFalse(r1.Success);
            Assert.IsNull(r1.Note);

            Bid r2 = new Bid()
            {
                Success = true,
                Note = "Testing",
                Pennies = new Penny[] { new Penny() { Id = 10 } }
            };

            Assert.IsTrue(r2.Success);
            Assert.AreEqual("Testing", r2.Note);
            Assert.IsNotNull(r2.Pennies);
            Assert.AreEqual(1, r2.Pennies.Length);
            Assert.AreEqual(10, r2.Pennies[0].Id);

            byte[] bytes = r2.Encode();
            string tmp = Encoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            Bid r3 = m2 as Bid;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreEqual(r2.Note, r3.Note);
            Assert.IsNotNull((r3.Pennies));
            Assert.AreEqual(1, r3.Pennies.Length);
            Assert.AreEqual(10, r3.Pennies[0].Id);
        }
    }
}
