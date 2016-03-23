using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;
using Messages;
using Messages.RequestMessages;
using SharedObjects;

using log4net;
using log4net.Config;

namespace CommSubTesting
{
    [TestClass]
    public class EnvelopeQueueTester
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EnvelopeQueueTester));

        private List<Envelope> writeList;
        private List<Envelope> readList;
        private int readAttempts;
        private object writeLock = new object();
        private object readLock = new object();
        
        private EnvelopeQueue sharedQueue = null;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            XmlConfigurator.Configure();
            if (log.IsDebugEnabled)
                log.Debug("Starting EnvelopeQueueTester");
        }

        [TestMethod]
        public void EnvelopeQueue_TestConstructor()
        {
            EnvelopeQueue queue01 = new EnvelopeQueue();
            Assert.IsNotNull(queue01);
            Assert.AreEqual(0, queue01.Count);
        }

            [TestMethod]
            public void EnvelopeQueue_TestEnqueueAndDequeue()
            {
                EnvelopeQueue queue01 = new EnvelopeQueue();
                Assert.AreEqual(0, queue01.Count);

                Envelope e1 = new Envelope();
                queue01.Enqueue(e1);
                Assert.AreEqual(1, queue01.Count);

                Envelope e2 = new Envelope();
                queue01.Enqueue(e2);
                Assert.AreEqual(2, queue01.Count);

                Envelope e3 = new Envelope();
                queue01.Enqueue(e3);
                Assert.AreEqual(3, queue01.Count);

                Envelope e4 = queue01.Dequeue(2000);
                Assert.AreEqual(2, queue01.Count);
                Assert.IsNotNull(e4);
                Assert.AreSame(e1, e4);

                Envelope e5 = queue01.Dequeue(2000);
                Assert.AreEqual(1, queue01.Count);
                Assert.IsNotNull(e5);
                Assert.AreSame(e2, e5);

                Envelope e6 = queue01.Dequeue(2000);
                Assert.AreEqual(0, queue01.Count);
                Assert.IsNotNull(e6);
                Assert.AreSame(e3, e6);

                DateTime ts1 = DateTime.Now;
                Envelope e7 = queue01.Dequeue(2000);
                Assert.AreEqual(0, queue01.Count);
                Assert.IsNull(e7);
                Assert.IsTrue(DateTime.Now.Subtract(ts1).TotalMilliseconds>=2000);
            }
            [TestMethod]
            public void EnvelopeQueue_TestLotsOfSequentialOperations()
            {
                EnvelopeQueue queue = new EnvelopeQueue();
                Assert.AreEqual(0, queue.Count);
                for (int i = 0; i < 1000; i++)
                {
                    Envelope e1 = CreateTestEnvelope();
                    queue.Enqueue(e1);
                    Assert.AreEqual(1, queue.Count);

                    DateTime ts = DateTime.Now;
                    Envelope e2 = queue.Dequeue(1000);
                    Assert.IsTrue(DateTime.Now.Subtract(ts).TotalMilliseconds < 100);
                    Assert.IsNotNull(e2);
                    Assert.AreSame(e1, e2);
                    Assert.AreEqual(0, queue.Count);
                }
            }

            [TestMethod]
            public void EnvelopeQueue_TestConcurrentAccess()
            {
                log.Debug("In EnvelopeQueue_TestConcurrentAccess");

                int numberOfEnvelopes = 1000;
                readAttempts = 0;
                writeList = new List<Envelope>();
                readList = new List<Envelope>();
                sharedQueue = new EnvelopeQueue();

                log.DebugFormat("Lauch {0} Enqueue and Dequeue Operations", numberOfEnvelopes);
                Parallel.For(0, 1000, i =>
                    {
                        Task.Run(() => { EnqueueHelper(); });
                        Task.Run(() => { DequeueHelper(); });                        
                    });
                log.DebugFormat("Launched");

                // Check for completion of all the writes -- must be within 10 seconds
                int remainingWriteWaitTime = 10000;
                int remainingReadWaitTime = 11000;
                bool writesDone = false;
                while (!writesDone && remainingWriteWaitTime > 0)
                {
                    Thread.Sleep(1000);
                    remainingWriteWaitTime -= 1000;
                    remainingReadWaitTime -= 1000;
                    lock (writeLock)
                    {
                        writesDone = (writeList.Count == numberOfEnvelopes);
                    }
                }
                Assert.IsTrue(writesDone);
                log.DebugFormat("Writes done");

                // Check for completion of all the reads -- must be with 11 seconds
                bool readsDone = false;
                while (!readsDone && remainingReadWaitTime > 0)
                {
                    Thread.Sleep(1000);
                    remainingReadWaitTime -= 1000;
                    lock (readLock)
                    {
                        readsDone = (readList.Count == numberOfEnvelopes);
                    }
                }
                log.Debug((readsDone) ? "Reads done" : "Some reads did not work");
                Assert.AreEqual(numberOfEnvelopes, readAttempts);

                // Check for each envelope
                lock (writeLock)
                {
                    for (int writeIndex = 0; writeIndex<writeList.Count; writeIndex++)
                    {
                        lock (readList)
                        {
                            if (!readList.Contains(writeList[writeIndex]))
                                Assert.Fail("Read list does not contain message {0}", writeList[writeIndex].Message.MsgId.ToString());
                        }
                    }
                }

                // Throw a hard failure is reads weren't done
                Assert.IsTrue(readsDone);
            }

            private void EnqueueHelper()
            {
                Envelope e = CreateTestEnvelope();
                sharedQueue.Enqueue(e);

                lock (writeLock)
                {
                    writeList.Add(e);
                }
            }

            private void DequeueHelper()
            {
                lock (readLock)
                {
                    readAttempts++;
                }

                Envelope e = sharedQueue.Dequeue(2000);

                if (e != null)
                {
                    lock (readLock)
                    {
                        readList.Add(e);
                    }
                }
                else
                    log.DebugFormat("In ReaderHelp, Dequeue failed");
            }

            private Envelope CreateTestEnvelope()
            {
                MessageNumber messageNumber = MessageNumber.Create();
                AliveRequest message = new AliveRequest() { MsgId = messageNumber, ConvId = messageNumber.Clone() };
                Envelope e = new Envelope() { Message = message };
                return e;
            }

    }
}
