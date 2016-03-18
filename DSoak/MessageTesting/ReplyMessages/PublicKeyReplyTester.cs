using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyMessages
{
    [TestClass]
    public class PublicKeyReplyTester
    {
        [TestMethod]
        public void PublicKeyReply_TestEverything()
        {
            PublicKeyReply r1 = new PublicKeyReply();
            Assert.IsFalse(r1.Success);
            Assert.IsNull(r1.Note);

            PublicKeyReply r2 = new PublicKeyReply()
            {
                Success = true,
                Note = "Testing",
                Key = new PublicKey() { Exponent = new byte[] {10} }
            };

            Assert.IsTrue(r2.Success);
            Assert.AreEqual("Testing", r2.Note);
            Assert.AreEqual(10, r2.Key.Exponent[0]);

            byte[] bytes = r2.Encode();
            string tmp = Encoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            PublicKeyReply r3 = m2 as PublicKeyReply;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreEqual(r2.Note, r3.Note);
            Assert.AreEqual(10, r3.Key.Exponent[0]);
        }
    }
}
