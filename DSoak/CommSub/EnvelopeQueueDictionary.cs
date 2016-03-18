using System.Collections.Concurrent;

using SharedObjects;

using log4net;

namespace CommSub
{
    public class EnvelopeQueueDictionary
    {
        #region Private Class Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EnvelopeQueue));

        // Create a dictionary of queues for conversations in progress, plus a lock object for the dictionary
        private readonly ConcurrentDictionary<MessageNumber, EnvelopeQueue> _activeQueues =
            new ConcurrentDictionary<MessageNumber, EnvelopeQueue>(new MessageNumber.MessageNumberComparer());

        #endregion

        #region Public Methods

        public EnvelopeQueue CreateQueue(MessageNumber convId)
        {
            EnvelopeQueue result = null;
            if (convId != null)
            {
                Logger.DebugFormat("CreateQueue for key={0}", convId);
                result = GetByConvId(convId);
                if (result == null)
                {
                    result = new EnvelopeQueue() { QueueId = convId };
                    _activeQueues.TryAdd(convId, result);
                }
            }
            return result;
        }

        public EnvelopeQueue GetByConvId(MessageNumber convId)
        {
            Logger.DebugFormat("GetByConvId for name={0}", convId);

            EnvelopeQueue result;
            _activeQueues.TryGetValue(convId, out result);

            return result;
        }

        public void CloseQueue(MessageNumber queueId)
        {
            Logger.DebugFormat("Remove Queue {0}", queueId);
            EnvelopeQueue queue;
            _activeQueues.TryRemove(queueId, out queue);
        }

        public void ClearAllQueues()
        {
            _activeQueues.Clear();
        }

        public int ConversationQueueCount
        {
            get { return _activeQueues.Count; }
        }
        #endregion
    }
}
