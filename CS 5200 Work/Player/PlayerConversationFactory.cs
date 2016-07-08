using CommunicationSubsystem;
using Messages.RequestMessages;
using CommunicationSubsystem.Conversations.Responders;

namespace PlayerProcess
{
    public class PlayerConversationFactory : ConversationFactory
    {
        /// <summary>
        /// Initialized types of conversations that the player should respond to.
        /// </summary>
        public override void InitializeTypes()
        {
            Add(typeof(AliveRequest), typeof(AliveResponder));
            Add(typeof(GameStatusNotification), typeof(GameStatusResponder));
            Add(typeof(ReadyToStart), typeof(ReadyResponder));
            Add(typeof(ShutdownRequest), typeof(ShutdownResponder));
            Add(typeof(HitNotification), typeof(HitResponder));
            Add(typeof(AllowanceDeliveryRequest), typeof(AllowanceDistributionResponder));
            Add(typeof(ExitGameRequest), typeof(ExitGameResponder));
            Add(typeof(AuctionAnnouncement), typeof(AuctionResponder));
            Add(typeof(UmbrellaLoweredNotification), typeof(UmbrellaLoweredResponder));
        }
    }
}
