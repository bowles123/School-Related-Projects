using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace CommSubTesting
{
    [TestClass]
    public class EnvelopeTester
    {
        [TestMethod]
        public void Envelope_TestEverything()
        {
            // Test Case 1: Check default constructor
            Envelope env01 = new Envelope();
            Assert.IsNull(env01.Message);
            Assert.IsNull(env01.EndPoint);
            Assert.IsNotNull(env01.IPEndPoint);
            Assert.AreEqual((new IPEndPoint(IPAddress.Any, 0)).ToString(), env01.IPEndPoint.ToString());
            Assert.AreEqual(0, env01.IPEndPoint.Port); 
            Assert.IsFalse(env01.IsValidToSend);

            // Check the IPEndPoint setter
            env01.IPEndPoint = new IPEndPoint(IPAddress.Parse("129.123.7.88"), 14000);
            Assert.IsNotNull(env01.EndPoint);
            Assert.AreEqual("129.123.7.88:14000", env01.EndPoint.HostAndPort);
            // Check the Message setter
            env01.Message = new LoginReply();
            Assert.IsNotNull(env01.Message);
            Assert.IsTrue(env01.Message is LoginReply);

            // Check the constructor with Message and IPEndPoint parameters
            PublicEndPoint targetEp = new PublicEndPoint() { HostAndPort = "129.123.7.32:5500" };
            Message msg = new LoginRequest { ProcessLabel = "Test proess",
                                             ProcessType = ProcessInfo.ProcessType.Player,
                                             Identity = new IdentityInfo() { Alias = "Joe", ANumber="A00024", FirstName="Joseph", LastName="Jones" } };
            Envelope env02 = new Envelope(msg, targetEp);
            Assert.AreSame(msg, env02.Message);
            Assert.AreSame(targetEp, env02.EndPoint);
            Assert.IsTrue(env02.IsValidToSend);

            // Check constructor with Message and IPEndPoint
            Envelope env03 = new Envelope(msg, targetEp.IPEndPoint);
            Assert.AreSame(msg, env03.Message);
            Assert.AreNotSame(targetEp, env03.EndPoint);
            Assert.AreEqual(targetEp, env03.EndPoint);
            Assert.IsTrue(env03.IsValidToSend);

            // Check constructor with null parameters
            Envelope env04 = new Envelope(null, (IPEndPoint) null);
            Assert.IsNull(env04.Message);
            Assert.IsNull(env04.EndPoint);
            Assert.IsNotNull(env04.IPEndPoint);
            Assert.AreEqual((new IPEndPoint(IPAddress.Any, 0)).ToString(), env04.IPEndPoint.ToString());
            Assert.IsFalse(env04.IsValidToSend);

            // Check IPEndPoint setter property to a null
            env03.IPEndPoint = null;
            Assert.AreEqual((new IPEndPoint(IPAddress.Any, 0)).ToString(), env03.IPEndPoint.ToString());
            Assert.AreEqual(0, env03.IPEndPoint.Port);

            // Check IsValid to Send
            PublicEndPoint ep05 = new PublicEndPoint() { HostAndPort = "www.google.com:80" };
            env03.EndPoint = ep05;
            LoginReply r5 = new LoginReply();
            env03.Message = r5;
            Assert.AreSame(ep05, env03.EndPoint);
            Assert.AreSame(r5, env03.Message);
            Assert.IsTrue(env03.IsValidToSend);
            env03.EndPoint.Port = 0;
            Assert.IsFalse(env03.IsValidToSend);
            env03.EndPoint.Port = 80;
            env03.EndPoint.Host = string.Empty;
            Assert.IsFalse(env03.IsValidToSend);
            env03.EndPoint = null;
            Assert.IsFalse(env03.IsValidToSend);
            env03.EndPoint = ep05;
            env03.Message = null;
            Assert.IsFalse(env03.IsValidToSend);




        }
    }
}
