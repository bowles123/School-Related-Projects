using System;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class GetGameListInitiator: RequestReplyConversation
    {
        protected override void Process(Object objState)
        {
            GameListRequest gameList = new GameListRequest()
            { StatusFilter = (int)GameInfo.StatusCode.Available };
            Request = new Envelope() { Message = gameList, Endpoint = CommProcess.RegistryEndPoint };
            Response = null;
            GameListReply reply = null;

            gameList.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(gameList.ConvId);
            logger.Debug("Send game list request to the registry.");
            Communicator.Send(Request);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                reply = Response.Message as GameListReply;
                logger.Debug("Received a game list reply.");
                Games = reply.GameInfo;
                Dictionary.CloseQueue(gameList.ConvId);
            }

            Stop();
        }
    }
}
