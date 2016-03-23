using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;
using Messages;
using SharedObjects;

namespace CommSubTesting
{
    [TestClass]
    public class EnvelopeQueueDictionaryTester
    {
        [TestMethod]
        public void EnvelopeQueueDictionary_TestConstructor()
        {
            EnvelopeQueueDictionary eqd = new EnvelopeQueueDictionary();
            Assert.IsNotNull(eqd);
            Assert.AreEqual(0, eqd.ConversationQueueCount);

            MessageNumber n1_2 = new MessageNumber() { Pid = 1, Seq = 2 };
            EnvelopeQueue q1_2 = eqd.GetByConvId(n1_2);
            Assert.IsNull(q1_2);
        }

        [TestMethod]
        public void EnvelopeQueueDictionary_TestQueueManagement()
        {
            EnvelopeQueueDictionary eqd = new EnvelopeQueueDictionary();

            MessageNumber n00 = new MessageNumber();
            EnvelopeQueue queue01 = eqd.GetByConvId(n00);
            Assert.IsNull(queue01);

            MessageNumber n02 = new MessageNumber() { Pid = 1, Seq = 3};
            EnvelopeQueue queue02 = eqd.CreateQueue(n02);
            Assert.IsNotNull(queue02);
            Assert.AreNotSame(queue01, queue02);
            Assert.AreEqual(0, queue02.Count);
            Assert.AreEqual(1, eqd.ConversationQueueCount);

            MessageNumber n03 = new MessageNumber() { Pid = 5, Seq = 30 };
            EnvelopeQueue queue03 = eqd.CreateQueue(n03);
            Assert.IsNotNull(queue03);
            Assert.AreNotSame(queue01, queue03);
            Assert.AreNotSame(queue02, queue03);
            Assert.AreEqual(0, queue03.Count);
            Assert.AreEqual(2, eqd.ConversationQueueCount);

            MessageNumber n04 = new MessageNumber() { Pid = 1, Seq = 3 };
            EnvelopeQueue queue04 = eqd.CreateQueue(n04);
            Assert.IsNotNull(queue04);
            Assert.AreSame(queue02, queue04);
            Assert.AreEqual(0, queue04.Count);
            Assert.AreEqual(2, eqd.ConversationQueueCount);

            MessageNumber n05 = new MessageNumber() { Pid = 1, Seq = 3 };
            EnvelopeQueue queue05 = eqd.GetByConvId(n05);
            Assert.IsNotNull(queue05);
            Assert.AreSame(queue02, queue05);

            MessageNumber n06 = new MessageNumber() { Pid = 10, Seq = 20 };
            EnvelopeQueue queue06 = eqd.GetByConvId(n06);
            Assert.IsNull(queue06);

            eqd.CloseQueue(n02);
            EnvelopeQueue queue07 = eqd.GetByConvId(n02);
            Assert.IsNull(queue07);

            // Close the requeue queue
            eqd.CloseQueue(n00);
            Assert.AreEqual(1, eqd.ConversationQueueCount);

            queue01 = eqd.GetByConvId(n00);
            Assert.IsNull(queue01);
        }
    }
}
