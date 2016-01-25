using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyTesters
{
    [TestClass]
    public class LoginReplyTester
    {
        [TestMethod]
        public void LoginReply_TestEverything()
        {
            LoginReply r1 = new LoginReply();
            Assert.IsFalse(r1.Success);
            Assert.IsNull(r1.ProcessInfo);
            Assert.IsNull(r1.Note);

            ProcessInfo p1 = new ProcessInfo()
            {
                ProcessId = 10,
                Label = "Joey's player",
                EndPoint = new PublicEndPoint { Host = "127.0.0.1", Port = 10030 }
            };

            LoginReply r2 = new LoginReply()
            {
                Success = true,
                ProcessInfo = p1,
                Note = "Testing"
            };

            Assert.IsTrue(r2.Success);
            Assert.AreSame(p1, r2.ProcessInfo);
            Assert.AreEqual("Testing", r2.Note);
            
            byte[] bytes = r2.Encode();
            // string tmp = Encoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            LoginReply r3 = m2 as LoginReply;
            Assert.IsNotNull(r3);
            Assert.AreNotSame(r2, r3);
            Assert.IsTrue(r3.Success);
            Assert.AreNotSame(r2.ProcessInfo, r3.ProcessInfo);
            Assert.AreEqual(r2.ProcessInfo.ProcessId, r3.ProcessInfo.ProcessId);
            Assert.AreEqual(r2.ProcessInfo.Label, r3.ProcessInfo.Label);
            Assert.AreEqual(r2.Note, r3.Note);
        }
    }
}
