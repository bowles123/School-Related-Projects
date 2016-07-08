using CommunicationSubsystem;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using CommunicationSubsystem.Conversations.Responders;

namespace UmbrellaSupplierProcess
{
    public class UmbrellaSupplierConversationFactory: ConversationFactory
    {
        /// <summary>
        /// Initializes the types of conversation the umbrella supplier should respond to.
        /// </summary>
        public override void InitializeTypes()
        {
            Add(typeof(AliveRequest), typeof(AliveResponder));
            Add(typeof(GameStatusNotification), typeof(GameStatusResponder));
            Add(typeof(Bid), typeof(BidResponder));
            Add(typeof(ShutdownRequest), typeof(ShutdownResponder));
            Add(typeof(ReadyToStart), typeof(ReadyResponder));
        }
    }
}
