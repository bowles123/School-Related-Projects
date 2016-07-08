using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class GetPublicKeyInitiator: RequestReplyConversation
    {
        public static int Id { get; set; }
        public static PublicKey Key { get { return key; } }

        private static PublicKey key;

        protected override void Process(object state)
        {
            logger.Debug("Initiating GetPublicKeyInitiator.");

            GetKeyRequest request = new GetKeyRequest() { ProcessId = Id };
            Envelope envelope = new Envelope() { Message = request, Endpoint = CommProcess.RegistryEndPoint };
            PublicKeyReply reply = null;
            Response = null;

            request.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(request.ConvId);

            logger.DebugFormat("Sending request to {0}", envelope.Endpoint);
            Communicator.Send(envelope);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                reply = Response.ActualMessage as PublicKeyReply;
                key = reply.Key;
                Dictionary.CloseQueue(request.ConvId);
            }
            Stop();
        }
    }
}
