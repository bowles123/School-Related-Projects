using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace CommSubTesting
{
    [TestClass]
    public class CommunicatorTester
    {
        [TestMethod]
        public void Communicator_TestGoodConstructionAndStart()
        {
            Communicator comm1 = new Communicator() { MinPort = 12000, MaxPort = 12099 };
            Assert.IsNotNull(comm1);
            Assert.AreEqual(12000, comm1.MinPort);
            Assert.AreEqual(12099, comm1.MaxPort);
            Assert.AreEqual(0, comm1.Port);

            comm1.Start();
            Assert.IsTrue(comm1.Port >= comm1.MinPort && comm1.Port <= comm1.MaxPort);
            Assert.AreEqual(0, comm1.IncomingAvailable());

            Communicator comm2 = new Communicator() { MinPort = 12000, MaxPort = 12099 };
            Assert.IsNotNull(comm2);
            Assert.AreEqual(0, comm2.Port);

            comm2.Start();
            Assert.IsTrue(comm2.Port >= comm2.MinPort && comm2.Port <= comm2.MaxPort);
            Assert.AreNotEqual(comm1.Port, comm2.Port);
        }

        [TestMethod]
        public void Communicator_TestUnavilablePort()
        {
            Communicator comm1 = new Communicator() { MinPort = 12000, MaxPort = 12099 };
            comm1.Start();
            Assert.IsTrue(comm1.Port >= comm1.MinPort && comm1.Port <= comm1.MaxPort);

            Assert.AreEqual(0, comm1.IncomingAvailable());

            Communicator comm2 = new Communicator() { MinPort = comm1.Port, MaxPort = comm1.Port };
            Assert.IsNotNull(comm2);
            Assert.AreEqual(comm1.Port, comm2.MinPort);
            Assert.AreEqual(comm1.Port, comm2.MaxPort);
            Assert.AreEqual(0, comm2.Port);

            try
            {
                comm2.Start();
                Assert.Fail("Expected excepted not throw");
            }
            catch (ApplicationException) { }
            catch (Exception)
            {
                Assert.Fail("Unexpected exception");
            }

        }

        [TestMethod]
        public void Communicator_TestBadConstruction()
        {
            Communicator comm1 = new Communicator() { MinPort = -12000, MaxPort = 12099 };
            Assert.IsNotNull(comm1);
            Assert.AreEqual(-12000, comm1.MinPort);
            Assert.AreEqual(12099, comm1.MaxPort);

            try
            {
                comm1.Start();
                Assert.Fail("Exception exception");
            }
            catch { }
        }

        [TestMethod]
        public void Communicator_TestSendReceive()
        {
            Communicator comm1 = new Communicator() { MinPort = 12000, MaxPort = 12009 };
            Communicator comm2 = new Communicator() { MinPort = 12010, MaxPort = 12019 };

            comm1.Start();
            comm2.Start();

            PublicEndPoint targetEp = new PublicEndPoint() { Host = "127.0.0.1", Port = comm2.Port };
            LoginRequest msg = new LoginRequest
                                    {
                                        ProcessLabel = "Test proess",
                                        ProcessType = ProcessInfo.ProcessType.Player,
                                        Identity = new IdentityInfo() { Alias = "Joe", ANumber="A00024", FirstName="Joseph", LastName="Jones" }
                                    };
            Envelope env01 = new Envelope(msg, targetEp);

            comm1.Send(env01);

            Thread.Sleep(1000);
            Assert.IsTrue(comm2.IncomingAvailable()>0);

            Envelope env02 = comm2.Receive();
            Assert.AreNotSame(env01, env02);
            Assert.IsTrue(env02.Message is LoginRequest);
            Assert.AreEqual(env01.Message.MsgId, env02.Message.MsgId);
            LoginRequest r02 = env02.Message as LoginRequest;
            Assert.AreEqual(msg.ProcessLabel, r02.ProcessLabel);
        }

        [TestMethod]
        public void Communicator_TestTimeout()
        {
            Communicator comm1 = new Communicator() { MinPort = 12000, MaxPort = 12009 };
            comm1.Start();

            DateTime ts01 = DateTime.Now;
            Envelope env01 = comm1.Receive(2000);
            DateTime ts02 = DateTime.Now;
            Assert.IsNull(env01);
            TimeSpan delta = ts02.Subtract(ts01);
            Assert.IsTrue(delta.TotalMilliseconds > 2000);
        }

    }
}
