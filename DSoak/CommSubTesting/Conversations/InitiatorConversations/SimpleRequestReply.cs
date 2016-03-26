using System;

using CommSubTesting;
using CommSub.Conversations.InitiatorConversations;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace CommSubTesting.Conversations.InitiatorConversations
{
    public class SimpleRequestReply : RequestReply
    {
        #region Private Data Members
        private static readonly Type[] MyAllowedReplyTypes = new Type[] { typeof(Reply) };
        #endregion

        protected override Messages.Message CreateRequest()
        {
            return new DummyRequest();
        }

        protected override Type[] AllowedReplyTypes
        {
            get { return MyAllowedReplyTypes; }
        }
    }
}
