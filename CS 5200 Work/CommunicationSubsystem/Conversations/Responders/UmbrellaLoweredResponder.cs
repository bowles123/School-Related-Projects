using Messages.ReplyMessages;
using Messages;
using SharedObjects;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class UmbrellaLoweredResponder: RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Executing UmbrellaLoweredResponder.");
            ProxyEP = CommProcess.ProxyEndPoint;

            int[] to = new int[1] { Game.GameManagerId };
            Reply reply = new Reply() { Success = true };
            RouteMessage = new Routing() { InnerMessage = reply, ToProcessIds = to };
            Response = new Envelope() { Message = RouteMessage, Endpoint = ProxyEP };
            Request = null;

            Request = Queue.Dequeue(Timeout);

            if (Request != null)
            {
                logger.Debug("Received an umbrella lowered request.");
                UmbrellaRaised = false;

                reply.SetMessageAndConversationNumbers(MessageNumber.Create(), Request.ActualMessage.ConvId);
                logger.DebugFormat("Sending reply to umbrella lowered request to {0}.", Response.Endpoint);

                Communicator.Send(Response);
                Dictionary.CloseQueue(reply.ConvId);
                logger.Debug("Successfully sent reply to umbrella lowered request.");
            }
            Stop();
        }
    }
}
