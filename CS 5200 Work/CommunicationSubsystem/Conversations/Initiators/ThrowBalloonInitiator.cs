using Messages;
using System;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;
using System.Threading;

namespace CommunicationSubsystem.Conversations.Initiators
{
    public class ThrowBalloonInitiator: RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Attempting to initiate throw balloon request.");

            Random rand = new Random();
            int[] to;
            int player = 0;
            to = new int[1] { Game.GameManagerId };
            Balloon balloon = FilledBalloons.Dequeue();

            foreach (GameProcessData process in CurrentProcesses)
            {
                if (process.Type == ProcessInfo.ProcessType.Player &&
                    process.ProcessId != MessageNumber.LocalProcessId)
                {
                    player = process.ProcessId;
                    break;
                }
            }

            if (!balloon.IsFilled)
            {
                logger.Debug("Balloon is not filled.");
                Conversation conversation = Factory.CreateFromConversationType(typeof(FillBalloonInitiator));
                conversation.Launch();
            }
            else
            {
                logger.Debug("Inititating throw balloon request.");

                ThrowBalloonRequest throwBalloon = new ThrowBalloonRequest()
                { Balloon = balloon, TargetPlayerId = player };
                RouteMessage = new Routing() { InnerMessage = throwBalloon, ToProcessIds = to };
                Request = new Envelope() { Message = RouteMessage, Endpoint = CommProcess.ProxyEndPoint };
                Response = null;
                Reply reply = null;

                throwBalloon.InitMessageAndConversationNumbers();
                Queue = Dictionary.CreateQueue(throwBalloon.ConvId);
                logger.Debug("Send throw balloon request.");
                Communicator.Send(Request);
                Thread.Sleep(100);

                while (Response == null && RetryAmount > tries)
                {
                    tries++;
                    Response = Queue.Dequeue(Timeout);
                }

                if (Response != null)
                {
                    logger.Debug("Received a reply to the throw balloon request.");
                    reply = Response.ActualMessage as Reply;
                    Dictionary.CloseQueue(reply.ConvId);
                }
                else
                    FilledBalloons.Enqueue(throwBalloon.Balloon);
            }
            Stop();
        }
    }
}
