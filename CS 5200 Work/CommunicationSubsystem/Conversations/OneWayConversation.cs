using System.Collections.Generic;
using SharedObjects;

namespace CommunicationSubsystem
{
    public abstract class OneWayConversation: Conversation
    {
        public List<PublicEndPoint> ToProcesses { get; }
    }
}
