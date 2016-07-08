using System.Threading;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using Messages;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class RaiseUmbrellaInitiator: RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Executing RaiseUmbrellaInitiator.");
            ProxyEP = CommProcess.ProxyEndPoint;

            RaiseUmbrellaRequest raise = new RaiseUmbrellaRequest() { Umbrella = Umbrellas.Dequeue() };
            int[] to = new int[1] { Game.GameManagerId };
            Reply reply = null;
            Response = null;
            Request = new Envelope()
            {
                Message = new Routing()
                { InnerMessage = raise, ToProcessIds = to },
                Endpoint = ProxyEP
            };

            raise.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(raise.ConvId);
            logger.DebugFormat("Send raise umbrella request to {0}.", Request.Endpoint);
            Communicator.Send(Request);
            Thread.Sleep(100);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                logger.Debug("Received a reply to the raise umbrella request.");
                reply = Response.ActualMessage as Reply;

                if (reply.Success)
                    UmbrellaRaised = true;
                else
                    Umbrellas.Enqueue(raise.Umbrella);
                Dictionary.CloseQueue(reply.ConvId);
            }
            Stop();
        }
    }
}
