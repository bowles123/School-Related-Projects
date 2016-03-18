using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyMessages
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
            Assert.IsNull(r1.ProxyEndPoint);
            Assert.IsNull(r1.PennyBankEndPoint);
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
                ProxyEndPoint = new PublicEndPoint { Host = "127.0.0.1", Port = 13000 },
                PennyBankEndPoint = new PublicEndPoint { Host = "127.0.0.1", Port = 13001 },
                PennyBankPublicKey = new PublicKey() { Exponent = new byte[] { 10, 20} , Modulus = new byte[] { 30, 40 } },
                Note = "Testing"
            };

            Assert.IsTrue(r2.Success);
            Assert.AreSame(p1, r2.ProcessInfo);
            Assert.AreEqual("127.0.0.1:13000", r2.ProxyEndPoint.HostAndPort);
            Assert.AreEqual("127.0.0.1:13001", r2.PennyBankEndPoint.HostAndPort);
            Assert.IsNotNull(r2.PennyBankPublicKey);
            Assert.IsNotNull(r2.PennyBankPublicKey.Exponent);
            Assert.AreEqual(2, r2.PennyBankPublicKey.Exponent.Length);
            Assert.AreEqual(10, r2.PennyBankPublicKey.Exponent[0]);
            Assert.AreEqual(20, r2.PennyBankPublicKey.Exponent[1]);
            Assert.IsNotNull(r2.PennyBankPublicKey.Modulus);
            Assert.AreEqual(2, r2.PennyBankPublicKey.Modulus.Length);
            Assert.AreEqual(30, r2.PennyBankPublicKey.Modulus[0]);
            Assert.AreEqual(40, r2.PennyBankPublicKey.Modulus[1]);
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
            Assert.AreEqual(r2.ProxyEndPoint.HostAndPort, r3.ProxyEndPoint.HostAndPort);
            Assert.AreEqual(r2.PennyBankEndPoint.HostAndPort, r3.PennyBankEndPoint.HostAndPort);
            Assert.IsNotNull(r2.PennyBankPublicKey);
            Assert.IsNotNull(r2.PennyBankPublicKey.Exponent);
            Assert.AreEqual(2, r3.PennyBankPublicKey.Exponent.Length);
            Assert.AreEqual(10, r3.PennyBankPublicKey.Exponent[0]);
            Assert.AreEqual(20, r3.PennyBankPublicKey.Exponent[1]);
            Assert.IsNotNull(r3.PennyBankPublicKey.Modulus);
            Assert.AreEqual(2, r3.PennyBankPublicKey.Modulus.Length);
            Assert.AreEqual(30, r3.PennyBankPublicKey.Modulus[0]);
            Assert.AreEqual(40, r3.PennyBankPublicKey.Modulus[1]);
            Assert.AreEqual(r2.Note, r3.Note);
        }
    }
}
