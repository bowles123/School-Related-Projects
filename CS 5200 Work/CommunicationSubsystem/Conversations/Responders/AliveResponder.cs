using System;
using SharedObjects;
using Messages.ReplyMessages;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class AliveResponder: RequestReplyConversation
    {
        protected override void Process(Object objState)
        {
            logger.Debug("Executing AliveResponder.");

            Reply reply = new Reply() { Success = true, Note = "I'm Alive!!" };
            Request = null;
            Response = new Envelope() { Message = reply, Endpoint = CommProcess.RegistryEndPoint };

            Request = Queue.Dequeue(Timeout);

            if (Request != null)
            {
                logger.Debug("Recieved an alive request.");

                reply.SetMessageAndConversationNumbers(MessageNumber.Create(), Request.Message.ConvId);
                logger.Debug("Sent reply to alive request.");

                Communicator.Send(Response);
                Dictionary.CloseQueue(reply.ConvId);
                logger.Debug("Successfully sent reply to alive request.");
            }
            Stop();
        }
    }
}
