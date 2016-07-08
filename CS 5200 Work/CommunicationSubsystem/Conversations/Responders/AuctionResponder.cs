using System.Threading;
using Messages.RequestMessages;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class AuctionResponder: RequestReplyProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Executing AuctionResponder.");
            ProxyEP = CommProcess.ProxyEndPoint;

            Bid bid = null;
            Penny[] pennies;
            Envelope bidResponse = null;
            BidAck acknowledge = null;
            int[] to;
            Request = null;

            Request = Queue.Dequeue(Timeout);

            if (Request != null)
            {
                logger.Debug("Recieved an auction announcement.");
                AuctionAnnouncement request = Request.ActualMessage as AuctionAnnouncement;
                
                if (request.MinimumBid <= 15 && Pennies.Count != 0)
                {
                    pennies = new Penny[request.MinimumBid];
                    for (int i = 0; i < pennies.Length; i++)
                        pennies[i] = Pennies.Dequeue();
                    bid = new Bid() { Success = true, Pennies = pennies };
                    to = new int[1] { Request.Message.ConvId.Pid };
                    RouteMessage = new Routing() { InnerMessage = bid, ToProcessIds = to };
                    Response = new Envelope() { Message = RouteMessage, Endpoint = ProxyEP };
                    logger.DebugFormat("Sending reply to auction announcement to {0}", Response.Endpoint);

                    bid.SetMessageAndConversationNumbers(MessageNumber.Create(), request.ConvId);
                    Communicator.Send(Response);
                    logger.Debug("Successfully sent response to auction announcement.");
                    Thread.Sleep(100);

                    while (bidResponse == null && RetryAmount > tries)
                    {
                        tries++;
                        bidResponse = Queue.Dequeue(Timeout);
                    }

                    if (bidResponse != null)
                    {
                        if (bidResponse.ActualMessageType.Equals(typeof(BidAck)))
                        {
                            logger.Debug("Received an auction acknowledgement.");
                            acknowledge = bidResponse.ActualMessage as BidAck;

                            if (acknowledge.Won)
                            {
                                // Add the umbrella to the inventory.
                                logger.Debug("Won the umbrella auction.");
                                Umbrellas.Enqueue(acknowledge.Umbrella);
                            }
                            else
                                for (int i = 0; i < pennies.Length; i++)
                                    Pennies.Enqueue(pennies[i]);
                            Dictionary.CloseQueue(acknowledge.ConvId);
                        }
                        else
                            for (int i = 0; i < pennies.Length; i++)
                                Pennies.Enqueue(pennies[i]);
                    }
                    else
                        for (int i = 0; i < pennies.Length; i++)
                            Pennies.Enqueue(pennies[i]);
                }
                else
                    logger.Debug("No pennies to bid on an umbrella with.");

            }
            Stop();
        }
    }
}
