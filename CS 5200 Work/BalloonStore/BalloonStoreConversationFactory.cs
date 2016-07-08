using Messages.RequestMessages;
using CommunicationSubsystem;
using CommunicationSubsystem.Conversations.Responders;

namespace BalloonStoreProcess
{
    public class BalloonStoreConversationFactory : ConversationFactory
    {
        /// <summary>
        /// Initializes the types of conversation the balloon store should respond to.
        /// </summary>
        public override void InitializeTypes()
        {
            Add(typeof(AliveRequest), typeof(AliveResponder));
            Add(typeof(GameStatusNotification), typeof(GameStatusResponder));
            Add(typeof(BuyBalloonRequest), typeof(BuyBalloonResponder));
            Add(typeof(ShutdownRequest), typeof(ShutdownResponder));
            Add(typeof(ReadyToStart), typeof(ReadyResponder));
        }
    }
}