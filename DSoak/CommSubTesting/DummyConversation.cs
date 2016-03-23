using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;

namespace CommSubTesting
{
    public class DummyConversation : Conversation
    {
        private static List<DummyConversation> createdInstances = new List<DummyConversation>();

        public static List<DummyConversation> CreatedInstances
        {
            get { return createdInstances; }
        }

        public static DummyConversation LastCreatedInstance
        {
            get
            {
                return (createdInstances.Count != 0) ? createdInstances.Last() : null;
            }
        }

        public DummyConversation()
        {
            createdInstances.Add(this);
        }

        public bool ExecuteWasCalled { get; set; }

        public override void Execute(object context)
        {
            ExecuteWasCalled = true;
        }
    }
}
