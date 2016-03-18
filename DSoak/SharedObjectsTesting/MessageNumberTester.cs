using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectsTesting
{
    [TestClass]
    public class MessageNumberTester
    {
        [TestInitialize]
        public void Setup()
        {
            MessageNumber.LocalProcessId = 100;
        }

        [TestMethod]
        public void MessageNumber_TestConstruction()
        {
            MessageNumber.ResetSeqNumber();

            MessageNumber mn0 = new MessageNumber();
            Assert.AreEqual(0, mn0.Pid);
            Assert.AreEqual(0, mn0.Seq);
            Assert.AreEqual(0, mn0.GetHashCode());

            MessageNumber mn1 = MessageNumber.Create();
            Assert.AreEqual(100, mn1.Pid);
            Assert.AreEqual(1, mn1.Seq);
            Assert.AreEqual(0x00640001, mn1.GetHashCode());

            MessageNumber mn2 = MessageNumber.Create();
            Assert.AreEqual(100, mn1.Pid);
            Assert.AreEqual(mn1.Seq + 1, mn2.Seq);

            // Test sequential allocation
            for (int i = 0; i < 100; i++)
            {
                MessageNumber mn4 = MessageNumber.Create();
                Assert.AreEqual(100, mn1.Pid);
                Assert.AreEqual(mn2.Seq + i + 1, mn4.Seq);
            }

            MessageNumber.SetSeqNumber(Int32.MaxValue - 2);
            MessageNumber mn5 = MessageNumber.Create();
            Assert.AreEqual(Int32.MaxValue - 1, mn5.Seq);
            mn5 = MessageNumber.Create();
            Assert.AreEqual(Int32.MaxValue, mn5.Seq);
            mn5 = MessageNumber.Create();
            Assert.AreEqual(1, mn5.Seq);

            MessageNumber mn3 = new MessageNumber();
            Assert.AreEqual(0, mn3.Pid);
            Assert.AreEqual(0, mn3.Seq);
        }

        [TestMethod]
        public void MessageNumber_TestComparison()
        {
            MessageNumber.ResetSeqNumber();

            MessageNumber mn0 = new MessageNumber();
            MessageNumber mn1 = MessageNumber.Create();
            MessageNumber mn2 = MessageNumber.Create();
            MessageNumber mn3 = new MessageNumber() { Pid = mn2.Pid, Seq = mn2.Seq };

            Assert.AreEqual(mn2.Pid, mn3.Pid);
            Assert.AreEqual(mn2.Seq, mn3.Seq);

            Assert.IsTrue(mn2.Equals(mn3));
            Assert.IsFalse(mn2.Equals(mn1));
            Assert.IsFalse(mn2.Equals(null));
            Assert.IsFalse(mn2.Equals(MessageNumber.Create()));

            Assert.IsTrue(mn2 == mn3);
            Assert.IsTrue(mn1 <= mn3);
            Assert.IsTrue(mn1 < mn3);
            Assert.IsTrue(mn3 != mn1);
            Assert.IsTrue(mn3 >= mn1);
            Assert.IsTrue(mn3 > mn1);

            MessageNumber.MessageNumberComparer comparer = new MessageNumber.MessageNumberComparer();
            Assert.IsTrue(comparer.Equals(mn2, mn3));
            Assert.AreEqual(0, comparer.GetHashCode(mn0));
            Assert.AreEqual(0x00640001, comparer.GetHashCode(mn1));
        }

    }
}
