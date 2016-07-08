using System;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using Messages;
using System.Threading;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class JoinGameInitiator: RequestReplyProxy
    {
        protected override void Process(Object objState)
        {
            logger.Debug("Initiating join game request.");

            JoinGameRequest game = new JoinGameRequest() { Process = MyProcess, GameId = Game.GameId };
            int[] to = new int[1] { Game.GameManagerId };
            RouteMessage = new Routing() { InnerMessage = game, ToProcessIds = to };
            Request = new Envelope() { Message = RouteMessage, Endpoint = CommProcess.ProxyEndPoint };
            Response = null;
            JoinGameReply reply = null;

            game.InitMessageAndConversationNumbers();
            Queue = Dictionary.CreateQueue(game.ConvId);
            logger.DebugFormat("Attmepting to send join game request to {0}.", CommProcess.ProxyEndPoint);
            Communicator.Send(Request);
            Thread.Sleep(100);

            while (Response == null && RetryAmount > tries)
            {
                tries++;
                Response = Queue.Dequeue(Timeout);
            }

            if (Response != null)
            {
                logger.Debug("Received join game reply.");

                Routing message = Response.Message as Routing;
                reply = message.InnerMessage as JoinGameReply;
                CurrentGameId = reply.GameId;
                GameStatus = Game.Status;
                LifePoints = reply.InitialLifePoints;
                Console.WriteLine("Process is now part of a game.");

                Dictionary.CloseQueue(game.ConvId);
            }
            Stop();
        }
    }
}
