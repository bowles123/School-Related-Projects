using System;
using Messages.RequestMessages;
using Messages.ReplyMessages;


namespace CommunicationSubsystem.Conversations.Initiators
{
    public class LogoutInitiator: RequestReplyConversation
    {
        protected override void Process(Object objState)
        {
            logger.Debug("Initiating logout request.");

            LogoutRequest logout = new LogoutRequest() { };
            Request = new Envelope() { Message = logout, Endpoint = CommProcess.RegistryEndPoint };
            Response = null;
            Reply message = null;
            int triesLeft = RetryAmount;

            logout.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(logout.ConvId);
            logger.Debug("Send logout request.");
            Communicator.Send(Request);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                message = Response.Message as Reply;
                logger.Debug("Received logout request.");

                if (message.Success)
                {
                    logger.Debug("Successfully logged out.");
                    Game = null;
                    Games = null;
                    CommProcess.PennyBankEndPoint = null;
                    CommProcess.ProxyEndPoint = null;
                    PennyKey = null;
                    MyProcess.Status = SharedObjects.ProcessInfo.StatusCode.Unknown;
                }

                Dictionary.CloseQueue(logout.ConvId);
            }
            Stop();
        }
    }
}
