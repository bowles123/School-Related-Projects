using System.Collections.Generic;
using System.Linq;
using CommSub;

namespace CommSubTesting.Conversations.ResponderConversations
{
    public class DummyAliveConversation : Conversation
    {
        private static readonly List<DummyAliveConversation> MyCreatedInstances = new List<DummyAliveConversation>();

        public static List<DummyAliveConversation> CreatedInstances
        {
            get { return MyCreatedInstances; }
        }

        public static DummyAliveConversation LastCreatedInstance
        {
            get
            {
                return (CreatedInstances.Count != 0) ? CreatedInstances.Last() : null;
            }
        }

        public DummyAliveConversation()
        {
            CreatedInstances.Add(this);
        }

        public bool ExecuteWasCalled { get; set; }

        public override void Execute(object context = null)
        {
            if (PreExecuteAction != null)
                PreExecuteAction(context);

            ExecuteWasCalled = true;

            if (PostExecuteAction != null)
                PostExecuteAction(context);
        }
    }
}
