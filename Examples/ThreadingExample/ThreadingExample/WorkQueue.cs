using System.Collections.Generic;
using System.Threading;


namespace ThreadingExample
{
    public class WorkQueue
    {
        private readonly Queue<WorkItem> _queue = new Queue<WorkItem>();
        private readonly AutoResetEvent _waitEvent = new AutoResetEvent(false);

        private readonly object _myLock = new object();

        public void Enqueue(WorkItem item)
        {
            lock (_myLock)
            {
                _queue.Enqueue(item);
                _waitEvent.Set();
            }
        }

        public WorkItem Dequeue(int timeout)
        {
            WorkItem item = null;

            // If there is nothing in the queue, then wait
            if (_queue.Count == 0)
                _waitEvent.WaitOne(timeout);

            lock (_myLock)
            {
                if (_queue.Count > 0)
                    item = _queue.Dequeue();
            }

            // Note: Even if there was something in the queue at the beginning of the
            // method (causing the WaitOne to be skipping), there may be in anything
            // in the queue once the process enters the critical section.  The consequence
            // is simply, that a null will be returned.  That should be a problem to
            // the calling routine because it has to handle a null value on timeout anyway.

            return item;
        }
    }
}
