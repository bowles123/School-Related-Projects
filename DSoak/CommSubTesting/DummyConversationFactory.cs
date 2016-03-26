using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;
using CommSubTesting.Conversations.ResponderConversations;
using Messages.RequestMessages;

namespace CommSubTesting
{
    public class DummyConversationFactory : ConversationFactory
    {
        public override void Initialize()
        {
            // Add a mapping of a message type to a conversation type.
            // A real concrete ConversationFactory would be defined
            // in the application layer and would probably contain
            // multiple mappings.
            Add(typeof(AliveRequest), typeof(DummyAliveConversation));
            Add(typeof(DummyRequest), typeof(SimpleRequestReply));
        }
    }
}
