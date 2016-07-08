using Messages.RequestMessages;
using Messages.ReplyMessages;
using Messages;
using System.Threading;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class LeaveGameInitiator: RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Initiating leave game request.");

            LeaveGameRequest leave = new LeaveGameRequest() { };
            int[] to = new int[1] { CurrentGameId };
            RouteMessage = new Routing() { InnerMessage = leave, ToProcessIds = to };
            Request = new Envelope() { Message = RouteMessage, Endpoint = CommProcess.ProxyEndPoint };
            Response = null;
            Reply reply = null;

            leave.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(leave.ConvId);
            logger.Debug("Send leave game reqest.");
            Communicator.Send(Request);
            Thread.Sleep(100);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                logger.Debug("Received a reply for the leave game request.");
                reply = Response.Message as Reply;

                if (reply.Success)
                {
                    logger.Debug("Successfully left game.");

                    Game = null;
                    CurrentGameId = 0;
                    CurrentProcesses = null;
                    GameStatus = SharedObjects.GameInfo.StatusCode.Available;
                    Balloons = null;
                    BalloonStoreId = 0;
                    FilledBalloons = null;
                    WaterSourceId = 0;
                    MyProcess.Status = SharedObjects.ProcessInfo.StatusCode.LeavingGame;
                }

                Dictionary.CloseQueue(reply.ConvId);
            }
            Stop();
        }
    }
}
