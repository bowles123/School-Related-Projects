using System.Collections.Generic;
using System.Threading;
using SharedObjects;

namespace CommunicationSubsystem
{
    public class EnvelopeQueue
    {
        private readonly Queue<Envelope> envelopes = new Queue<Envelope>();
        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);
        private readonly object myLock = new object();

        public int Count { get { return envelopes.Count; } }
        public MessageNumber QueueId { get; }

        /// <summary>
        /// Adds an envelope to the queue.
        /// </summary>
        public void Enqueue(Envelope envelope)
        {
            lock (myLock)
            {
                envelopes.Enqueue(envelope);
                waitEvent.Set();
            }
        }

        /// <summary>
        /// Removes an envelope from the queue, blocking and waiting if nothing is in the queue.
        /// </summary>
        public Envelope Dequeue(int timeout)
        {
            Envelope envelope = null;

            if (envelopes.Count == 0)
                waitEvent.WaitOne(timeout);

            lock (myLock)
            {
                if (envelopes.Count > 0)
                    envelope = envelopes.Dequeue();
            }

            return envelope;
        }
    }
}
