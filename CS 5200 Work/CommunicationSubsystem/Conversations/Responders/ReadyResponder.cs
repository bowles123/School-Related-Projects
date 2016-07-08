using Messages.ReplyMessages;
using Messages;
using SharedObjects;
using System.Threading;
using System;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class ReadyResponder: RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Executing ReadyResponder.");
            ProxyEP = CommProcess.ProxyEndPoint;

            Reply reply = new Reply() { Success = true, Note = "Ready!" };
            int[] to = new int[1] { Game.GameManagerId };
            RouteMessage = new Routing() { ToProcessIds = to, InnerMessage = reply };
            Response = new Envelope() { Message = RouteMessage, Endpoint = ProxyEP };
            Request = null;
            Envelope readyResponse = null;
            StartGame start = null;
           
            Request = Queue.Dequeue(Timeout);
            RouteMessage.SetMessageAndConversationNumbers(MessageNumber.Create(), Request.Message.ConvId);

            if (Request != null)
            {
                logger.Debug("Received a ready to start request.");

                reply.SetMessageAndConversationNumbers(RouteMessage.MsgId, Request.Message.ConvId);
                logger.DebugFormat("Send reply to {0}'s ready to start request.", Game.GameManagerId);

                Communicator.Send(Response);
                logger.Debug("Successfully sent reply to ready to start request.");
            }

            Thread.Sleep(100);
            while (Response == null && RetryAmount > tries)
            {
                tries++;
                readyResponse = Queue.Dequeue(Timeout);
            }

            if (readyResponse != null)
            {
                logger.Debug("Received a start game request.");         
                start = readyResponse.Message as StartGame;

                if (start.Success)
                {
                    logger.Debug("Game will be beginning.");
                    Console.WriteLine("Starting Game.");
                }
                Dictionary.CloseQueue(start.ConvId);
            }
            Stop();
        }
    }
}
