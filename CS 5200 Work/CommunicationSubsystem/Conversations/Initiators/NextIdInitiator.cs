using Messages.RequestMessages;
using Messages.ReplyMessages;
using System.Threading; 

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class NextIdInitiator : RequestReplyConversation
    {
        protected override void Process(object state)
        {
            logger.Debug("Initiating NextIdInitiator.");

            NextIdRequest req = new NextIdRequest() { NumberOfIds = NumIds };
            NextIdReply reply = null;
            Response = null;
            Request = new Envelope() { Message = req, Endpoint = CommProcess.RegistryEndPoint };

            req.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(req.ConvId);
            logger.DebugFormat("Send NextIdRequest to {0}", Request.Endpoint);
            Communicator.Send(Request);
            Thread.Sleep(100);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                logger.Debug("Recieved response to NextIdRequest.");
                reply = Response.ActualMessage as NextIdReply;

                NextId = reply.NextId;
                NumIds = reply.NumberOfIds;
                Dictionary.CloseQueue(req.ConvId);
            }
            Stop();
        }
    }
}
