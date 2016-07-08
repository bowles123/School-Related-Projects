using Messages.RequestMessages;
using Messages.ReplyMessages;
using Messages;
using System.Threading;

using SharedObjects;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class BuyBalloonInitiator : RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Initiating balloon request.");
            Penny penny = null;

            if (Pennies.Count < 0)
                logger.Debug("Process has insufficient pennies to purchase a balloon.");

            if (Factory.BalloonStoreId <= 0)
                logger.Debug("Process doesn't have the Balloon Store's Id.");
            else
            {
                BuyBalloonRequest request = new BuyBalloonRequest() { Penny = Pennies.Dequeue() }; 
                int[] to = new int[1] { Factory.BalloonStoreId };
                RouteMessage = new Routing() { InnerMessage = request, ToProcessIds = to };
                Request = new Envelope() { Message = RouteMessage, Endpoint = CommProcess.ProxyEndPoint };
                Response = null;
                BalloonReply reply = null;

                request.InitMessageAndConversationNumbers();
                Queue = Dictionary.CreateQueue(request.ConvId);
                logger.DebugFormat("Attempting to send buy ballon request to {0}.", CommProcess.ProxyEndPoint);
                Communicator.Send(Request);
                Thread.Sleep(100);

                while (Response == null && RetryAmount > tries)
                {
                    tries++;
                    Response = Queue.Dequeue(Timeout);
                }

                if (Response != null)
                {
                    logger.Debug("Received balloon reply.");
                    reply = Response.ActualMessage as BalloonReply;
                    if (reply.Success)
                        Balloons.Enqueue(reply.Balloon);
                    else
                        Pennies.Enqueue(penny);
                    Dictionary.CloseQueue(reply.ConvId);
                }
                else
                    Pennies.Enqueue(penny);
            }
            Stop();
        }
    }
}
