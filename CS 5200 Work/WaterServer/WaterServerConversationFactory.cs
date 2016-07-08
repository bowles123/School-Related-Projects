using Messages.RequestMessages;
using CommunicationSubsystem;
using CommunicationSubsystem.Conversations.Responders;

namespace WaterServerProcess
{
    public class WaterServerConversationFactory: ConversationFactory
    {
        /// <summary>
        /// Initializes the types of conversation the water server should respond to.
        /// </summary>
        public override void InitializeTypes()
        {
            Add(typeof(AliveRequest), typeof(AliveResponder));
            Add(typeof(GameStatusNotification), typeof(GameStatusResponder));
            Add(typeof(FillBalloonRequest), typeof(FillBalloonResponder));
            Add(typeof(ShutdownRequest), typeof(ShutdownResponder));
            Add(typeof(ReadyToStart), typeof(ReadyResponder));
        }
    }
}
