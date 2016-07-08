using System;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class LoginInitiator : RequestReplyConversation
    {
        protected override void Process(Object objState)
        {
            logger.Debug("Initiating login request");

            LoginRequest login = new LoginRequest()
            {
                Identity = Identity,
                ProcessLabel = Identity.Alias,
                ProcessType = ProcessType,
                PublicKey = CommProcess.PublicKey
            };
            Request = new Envelope() { Message = login, Endpoint = CommProcess.RegistryEndPoint };
            Response = null;
            LoginReply reply = null;

            login.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(login.ConvId);

            logger.Debug("Send login request.");
            Communicator.Send(Request);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                reply = Response.Message as LoginReply;
                logger.Debug("Received login reply.");
                MyProcess = reply.ProcessInfo;
                CommProcess.ProxyEndPoint = reply.ProxyEndPoint;
                CommProcess.PennyBankEndPoint = reply.PennyBankEndPoint;
                PennyKey = reply.PennyBankPublicKey;
                MessageNumber.LocalProcessId = MyProcess.ProcessId;

                if (MyProcess.Status == ProcessInfo.StatusCode.Registered)
                {
                    Console.WriteLine("Process is now registered.");
                }

                Dictionary.CloseQueue(login.ConvId);
            }
            Stop();
        }
    }
}
