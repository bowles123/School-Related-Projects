using SharedObjects;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System.Threading;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class FillBalloonInitiator : RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Initiating fill balloon request.");

            Penny[] Money = new Penny[2] { Factory.Pennies.Dequeue(), Factory.Pennies.Dequeue() };
            FillBalloonRequest fill = new FillBalloonRequest() { Pennies = Money, Balloon = Balloons.Dequeue() };
            int[] to = new int[1] { Factory.WaterSourceId };
            RouteMessage = new Routing() { InnerMessage = fill, ToProcessIds = to };
            Request = new Envelope() { Message = RouteMessage, Endpoint = CommProcess.ProxyEndPoint };
            Response = null;
            BalloonReply reply = null;

            fill.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(fill.ConvId);
            logger.Debug("Send fill balloon request.");
            Communicator.Send(Request);
            Thread.Sleep(100);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                logger.Debug("Received a balloon reply.");
                reply = Response.ActualMessage as BalloonReply;

                if (reply.Success && reply.Balloon.IsFilled)
                    FilledBalloons.Enqueue(reply.Balloon);
                else
                    Communicator.Send(Request);

                Dictionary.CloseQueue(reply.ConvId);
            }
            else
            {
                Pennies.Enqueue(Money[0]);
                Pennies.Enqueue(Money[1]);
            }
            Stop();
        }
    }
}
