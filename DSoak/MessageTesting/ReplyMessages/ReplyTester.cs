using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyMessages
{
    [TestClass]
    public class ReplyTester
    {
        [TestMethod]
        public void Reply_TestEverything()
        {
            Reply r1 = new Reply();
            Assert.IsFalse(r1.Success);
            Assert.IsNull(r1.Note);

            Reply r2 = new Reply()
            {
                Success = true,
                Note = "Testing"
            };

            Assert.IsTrue(r2.Success);
            Assert.AreEqual("Testing", r2.Note);
            
            byte[] bytes = r2.Encode();
            string tmp = Encoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            Reply r3 = m2 as Reply;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreEqual(r2.Note, r3.Note);

        }
    }
}
