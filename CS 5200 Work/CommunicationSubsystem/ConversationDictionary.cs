using System.Collections.Generic;
using SharedObjects;
using log4net;

namespace CommunicationSubsystem
{
    public class ConversationDictionary
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ConversationDictionary));
        private readonly Dictionary<MessageNumber, EnvelopeQueue> envelopeDictionary;
        private readonly object myLock = new object();

        public ConversationDictionary()
        {
            envelopeDictionary = new Dictionary<MessageNumber, EnvelopeQueue>();
        }

        /// <summary>
        /// Gets the dictionary of conversations.
        /// </summary>
        public Dictionary<MessageNumber, EnvelopeQueue> getConversations()
        {
            return envelopeDictionary;
        }

        /// <summary>
        /// Gets an EnvelopeQueue based on the conversation id.
        /// </summary>
        public EnvelopeQueue GetByConversation(MessageNumber label)
        {
            EnvelopeQueue queue = null;
            logger.DebugFormat("Attempt to get an envelope queue with a conversation id of {0}.", label);

            lock(myLock)
            {
                if (envelopeDictionary.Count > 0)
                    envelopeDictionary.TryGetValue(label, out queue);
            }
            if (queue != null)
            {
                logger.DebugFormat("Recieved an envelope queue with a conversation id of {0}.", label);
            }

            return queue;
        }

        /// <summary>
        /// Creates an EnvelopeQueue for a new conversation.
        /// </summary>
        public EnvelopeQueue CreateQueue(MessageNumber conversationId)
        {
            logger.Debug("Attempt to create an envelope queue.");

            EnvelopeQueue queue = new EnvelopeQueue();
            lock(myLock)
            {
                envelopeDictionary.Add(conversationId, queue);
            }
            if (envelopeDictionary[conversationId] == queue)
            {
                logger.DebugFormat("Created envelope queue with conversation id of {0} succesfully.", conversationId);
            }
            return queue;
        }

        /// <summary>
        /// Closes the EnvelopeQueue of a finished conversation.
        /// </summary>
        public void CloseQueue(MessageNumber conversationId)
        {
            lock(myLock)
            {
                logger.DebugFormat("Closing Envelope Queue with conversation id of {0}: ",
                    conversationId);
                envelopeDictionary.Remove(conversationId);
            }
        }
    }
}
