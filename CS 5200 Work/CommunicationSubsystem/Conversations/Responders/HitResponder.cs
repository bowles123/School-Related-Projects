using Messages.ReplyMessages;
using SharedObjects;
using Messages;
using CommunicationSubsystem.Conversations.Initiators;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class HitResponder: RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Executing HitResponder.");
            ProxyEP = CommProcess.ProxyEndPoint;

            Reply reply = new Reply() { Success = true, Note = "Owww!" };
            Request = null;
            int[] to = new int[1] { CurrentGameId };
            RouteMessage = new Routing() { InnerMessage = reply, ToProcessIds = to };
            Response = new Envelope() { Message = RouteMessage, Endpoint = ProxyEP };

            Request = Queue.Dequeue(Timeout);

            if (Request != null)
            {
                logger.Debug("Recieved a hit notification.");
                LifePoints--;

                if(LifePoints == 0)
                {
                    Conversation conversation = Factory.CreateFromConversationType(typeof(
                        LeaveGameInitiator));
                    conversation.Launch();
                }

                reply.SetMessageAndConversationNumbers(MessageNumber.Create(), Request.Message.ConvId);
                logger.Debug("Send reply to hit notification.");

                Communicator.Send(Response);
                Dictionary.CloseQueue(reply.ConvId);
                logger.Debug("Successfully sent reply to hit notification.");
            }
            Stop();
        }
    }
}
