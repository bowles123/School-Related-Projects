using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;
using System.Threading;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class PennyValidationInitiator : RequestReplyConversation
    {
        public static bool ValidPenny { get { return validPenny; } }
        private static bool validPenny;

        protected override void Process(object state)
        {
            logger.Debug("Initiating PennyValidationInitiator.");

            Penny[] pennies = new Penny[1] { PennyToValidate };
            PennyValidation validation = new PennyValidation()
            { MarkAsUsedIfValid = true, Pennies = pennies };
            Request = new Envelope() { Message = validation, Endpoint = CommProcess.PennyBankEndPoint };
            Response = null;
            Reply reply = null;

            validation.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(validation.ConvId);
            logger.DebugFormat("Send penny validation to {0}", Request.Endpoint);
            Communicator.Send(Request);
            Thread.Sleep(100);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                logger.Debug("Received a reply to the PennyValidation message.");
                reply = Response.ActualMessage as Reply;
                validPenny = reply.Success;
                Dictionary.CloseQueue(reply.ConvId);
            }
            Stop();
        }
    }
}
