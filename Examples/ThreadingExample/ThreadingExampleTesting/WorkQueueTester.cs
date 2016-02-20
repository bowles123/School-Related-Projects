using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadingExample;

namespace ThreadingExampleTesting
{
    [TestClass]
    public class WorkQueueTester
    {
        [TestMethod]
        public void WorkQueue_TestEnqueueAndDequeue()
        {
            WorkQueue queue = new WorkQueue();

            WorkItem item1 = new WorkItem() {Id = 10};
            queue.Enqueue(item1);

            WorkItem item2 = new WorkItem() {Id = 12};
            queue.Enqueue(item2);

            WorkItem item3 = queue.Dequeue(10);
            Assert.AreSame(item1, item3);

            WorkItem item4 = queue.Dequeue(10);
            Assert.AreSame(item2, item4);

            WorkItem item5 = queue.Dequeue(10);
            Assert.IsNull(item5);
        }

        [TestMethod]
        public void WorkQueue_SimpleConcurrentAccessTest()
        {
            WorkQueue queue = new WorkQueue();
            Thread t1 = new Thread(SimpleConcurrentAccessTest_EnqueueWorkItems);
            Thread t2 = new Thread(SimpleConcurrentAccessTest_DequeueItems);
            t1.Start(queue);
            t2.Start(queue);

            t1.Join();
            t2.Join();
        }

        private void SimpleConcurrentAccessTest_EnqueueWorkItems(object queue)
        {
            WorkQueue workQueue = queue as WorkQueue;
            if (workQueue == null)
                return;

            for (int i = 0; i < 1000; i++)
            {
                WorkItem item = new WorkItem() {Id = i};
                workQueue.Enqueue(item);
                Thread.Sleep(0);
            }
        }

        private void SimpleConcurrentAccessTest_DequeueItems(object queue)
        {
            WorkQueue workQueue = queue as WorkQueue;
            if (workQueue == null)
                return;

            int numberOfItemsDequeued = 0;
            int countNulls = 0;

            while (countNulls == 0)
            {
                WorkItem item = workQueue.Dequeue(1000);
                if (item != null)
                    Assert.AreEqual(++numberOfItemsDequeued, item.Id);
                else
                    countNulls++;
            }

            Assert.AreEqual(1000, numberOfItemsDequeued);
            Assert.AreEqual(1, countNulls);
        }

        private readonly int[] _workItemDequeueCounts = new int[1000];

        [TestMethod]
        public void WorkQueue_FullConcurrentAccessTest()
        {
            for (int i = 0; i < 1000; i++)
                _workItemDequeueCounts[i] = 0;

            WorkQueue queue = new WorkQueue();
            Thread t1 = new Thread(FullConcurrentAccessTest_EnqueueWorkItems);
            Thread t2 = new Thread(FullConcurrentAccessTest_DequeueItems);
            Thread t3 = new Thread(FullConcurrentAccessTest_DequeueItems);
            Thread t4 = new Thread(FullConcurrentAccessTest_DequeueItems);
            t1.Start(queue);
            t2.Start(queue);
            t3.Start(queue);
            t4.Start(queue);

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();

            for (int i=0; i<1000; i++)
                Assert.AreEqual(1, _workItemDequeueCounts[i], "dequeue count for {0} is {1} instead of 1", i, _workItemDequeueCounts[1]);
        }


        private void FullConcurrentAccessTest_EnqueueWorkItems(object queue)
        {
            WorkQueue workQueue = queue as WorkQueue;
            if (workQueue == null)
                return;

            for (int i = 0; i < 1000; i++)
            {
                WorkItem item = new WorkItem() { Id = i };
                workQueue.Enqueue(item);
                Thread.Sleep(0);
            }
        }

        private void FullConcurrentAccessTest_DequeueItems(object queue)
        {
            WorkQueue workQueue = queue as WorkQueue;
            if (workQueue == null)
                return;

            WorkItem item;
            while ((item = workQueue.Dequeue(1000)) != null)
            {
                Assert.IsTrue(item.Id>=0 && item.Id<1000);
                _workItemDequeueCounts[item.Id]++;
            }

        }

    }
}
