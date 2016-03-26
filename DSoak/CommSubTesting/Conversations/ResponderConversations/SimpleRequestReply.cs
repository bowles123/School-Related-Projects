using System;

using Messages.ReplyMessages;

using CommSub.Conversations.ResponderConversations;

namespace CommSubTesting.Conversations.ResponderConversations
{
    public class SimpleRequestReply : RequestReply
    {
        private static readonly Type[] MyAllowedTypes = new Type[] { typeof(DummyRequest) };

        protected override Type[] AllowedTypes { get { return MyAllowedTypes; } }

        protected override Messages.Message CreateReply()
        {
            return new Reply() { Success = true };
        }
    }
}
