using Messages.ReplyMessages;
using Messages;
using SharedObjects;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class ExitGameResponder: RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Executing ExitGameResponder.");
            ProxyEP = CommProcess.ProxyEndPoint;

            Reply reply = new Reply() { Success = true };
            int[] to = new int[1] { Game.GameManagerId };
            RouteMessage = new Routing() { InnerMessage = reply, ToProcessIds = to };
            Response = new Envelope() { Message = RouteMessage, Endpoint = ProxyEP };

            Request = Queue.Dequeue(Timeout);

            if (Request != null)
            {
                logger.Debug("Received an exit game request.");
                reply.SetMessageAndConversationNumbers(MessageNumber.Create(), Request.ActualMessage.ConvId);

                Game = null;
                LifePoints = 0;
                CurrentGameId = 0;
                GamesPlayed++;

                Communicator.Send(Response);
                logger.DebugFormat("Successfully sent reply to exit game request to {0}.", Response.Endpoint);
                Dictionary.CloseQueue(reply.ConvId);
            }
            Stop();
        }
    }
}
