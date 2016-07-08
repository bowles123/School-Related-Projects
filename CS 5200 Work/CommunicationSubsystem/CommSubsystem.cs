
namespace CommunicationSubsystem
{
    public class CommSubsystem
    {
        public EnvelopeQueue Queue { get; set; }
        public ConversationDictionary Dictionary { get; set; }
        public ConversationFactory Factory { get; set; }
        public Dispatcher Dispatcher { get; set; }
        public Communicator Communicator { get; set; }
    }
}
