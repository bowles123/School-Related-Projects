using System;
using System.Collections.Generic;

namespace CommSub
{
    public abstract class ConversationFactory
    {
        private readonly Dictionary<Type, Type> _typeMappings = new Dictionary<Type, Type>();

        public CommProcess Process { get; set; }
        public int DefaultMaxRetries { get; set; }
        public int DefaultTimeout { get; set; }

        public bool IncomingMessageCanStartConversation(Type messageType)
        {
            return _typeMappings.ContainsKey(messageType);
        }

        public virtual Conversation CreateFromMessageType(Type messageType, Envelope envelope)
        {
            Conversation conversation = null;
            if (messageType != null && _typeMappings.ContainsKey(messageType))
                conversation = CreateFromConversationType(_typeMappings[messageType], envelope);
            return conversation;
        }

        public virtual T CreateFromConversationType<T>(Envelope envelope = null) where T : Conversation, new()
        {
            T conversation = new T()
            {
                Process = Process,
                MaxRetries = DefaultMaxRetries,
                Timeout = DefaultTimeout,
                IncomingEnvelope = envelope
            };
            return conversation;
        }

        public abstract void Initialize();

        protected void Add(Type messageType, Type conversationType)
        {
            if (messageType != null && conversationType != null && !_typeMappings.ContainsKey(messageType))
                _typeMappings.Add(messageType, conversationType);
        }

        protected virtual Conversation CreateFromConversationType(Type conversationType, Envelope envelope = null)
        {
            Conversation conversation = null;
            if (conversationType != null)
            {
                conversation = Activator.CreateInstance(conversationType) as Conversation;
                if (conversation != null)
                {
                    conversation.Process = Process;
                    conversation.MaxRetries = DefaultMaxRetries;
                    conversation.Timeout = DefaultTimeout;
                    conversation.IncomingEnvelope = envelope;
                }
            }
            return conversation;
        }
    }
}
