using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting
{
    [TestClass]
    public class LoginRequestTester
    {
        [TestMethod]
        public void LoginRequest_TestEverything()
        {
            LoginRequest r1 = new LoginRequest();
            Assert.IsNull(r1.Identity);

            IdentityInfo i1 = new IdentityInfo()
                                {
                                    ANumber = "2375423",
                                    FirstName = "Tom",
                                    LastName = "Thompson",
                                    Alias = "Tommy"
                                };

            LoginRequest r2 = new LoginRequest()
                                {
                                    ProcessType = ProcessInfo.ProcessType.Player,
                                    ProcessLabel = "Test Player",
                                    Identity = i1
                                };
            Assert.AreEqual(ProcessInfo.ProcessType.Player, r2.ProcessType);
            Assert.AreEqual("Test Player", r2.ProcessLabel);
            Assert.IsNotNull(r2.Identity);
            Assert.AreEqual("2375423", r2.Identity.ANumber);
            Assert.AreEqual("Tom", r2.Identity.FirstName);
            Assert.AreEqual("Thompson", r2.Identity.LastName);
            Assert.AreEqual("Tommy", r2.Identity.Alias);

            byte[] bytes = r2.Encode();

            Message m2 = Message.Decode(bytes);
            LoginRequest r3 = m2 as LoginRequest;
            Assert.AreEqual(r3.ProcessType, r2.ProcessType);
            Assert.AreEqual(r2.ProcessLabel, r3.ProcessLabel);
            Assert.IsNotNull(r3.Identity);
            Assert.AreEqual(r2.Identity.ANumber, r3.Identity.ANumber);
            Assert.AreEqual(r2.Identity.FirstName, r3.Identity.FirstName);
            Assert.AreEqual(r2.Identity.LastName, r3.Identity.LastName);
            Assert.AreEqual(r2.Identity.Alias, r3.Identity.Alias);
        }
    }
}
