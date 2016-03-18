using System;
using System.Collections.Concurrent;
using System.Threading;

using SharedObjects;

using log4net;

namespace CommSub
{
    /// <summary>
    /// This a class defines and tracks queues of envelops.  The queues are thread safe.
    /// 
    /// There is one special queue, called RequestQueue, that is supposed to hold incoming requests.
    /// All other queues are for specific conversations.
    /// 
    /// Use the RequestQueue property or GetByConvId methods to construct and access queue.
    /// 
    /// Note that a queue needs to disposed when it is no longer needed.  To do this call Stop();
    /// </summary>
    public class EnvelopeQueue
    {
        #region Private Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EnvelopeQueue));

        private readonly ConcurrentQueue<Envelope> _myQueue = new ConcurrentQueue<Envelope>();
        private readonly ManualResetEvent _somethingEnqueued = new ManualResetEvent(false);
        #endregion

        #region Public Properties

        public MessageNumber QueueId { get; set; }

        public int Count
        {
            get { return _myQueue.Count; }
        }

        public void Enqueue(Envelope envelope)
        {
            if (envelope != null)
            {
                _myQueue.Enqueue(envelope);
                Logger.DebugFormat("Enqueued an envelope into queue {0}", QueueId);
                _somethingEnqueued.Set();
            }
        }

        public Envelope Dequeue(int timeout)
        {
            Envelope result=null;
            int remainingTime = timeout;
            while (result == null && remainingTime > 0)
            {
                DateTime ts = DateTime.Now;
                if (_myQueue.Count == 0)
                    _somethingEnqueued.WaitOne(timeout);

                if (_myQueue.TryDequeue(out result))
                {
                    _somethingEnqueued.Reset();
                    Logger.DebugFormat("Dequeued an envelope from queue {0}", QueueId);
                }
                else
                    remainingTime -= Convert.ToInt32(DateTime.Now.Subtract(ts).TotalMilliseconds);
            }

            return result;
        }

        #endregion
    }
}
