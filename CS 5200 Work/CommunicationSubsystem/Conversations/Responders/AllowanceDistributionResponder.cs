using System.Net.Sockets;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class AllowanceDistributionResponder: Conversation
    {
        protected override void Process(object state)
        {
            logger.Debug("Executing allowance distribution responder.");
            Request = null;
            AllowanceDeliveryRequest request = null;
            Reply reply = new Reply() { Success = true };
            Envelope envelope = new Envelope() { Message = reply, Endpoint = CommProcess.PennyBankEndPoint };
            
            Request = Queue.Dequeue(Timeout);

            if (Request != null)
            {
                logger.Debug("Received an allowance distribution request.");
                reply.SetMessageAndConversationNumbers(MessageNumber.Create(), Request.Message.ConvId);
                Communicator.Send(envelope);

                request = Request.Message as AllowanceDeliveryRequest;
                TcpClient socket = new TcpClient(CommProcess.PennyBankEndPoint.Host, request.PortNumber);
                NetworkStream stream = socket.GetStream();
                stream.ReadTimeout = 100;

                while (Pennies.Count < request.NumberOfPennies)
                {
                    try
                    {
                        Pennies.Enqueue(NetworkStreamExtensions.ReadStreamMessage(stream));
                        if (Pennies.Peek() == null)
                        {
                            logger.Debug("Didn't recieve penny.");
                            Pennies.Dequeue();
                        }
                        else
                            logger.Debug("Received a penny from the penny bank.");
                    }
                    catch (SocketException exception)
                    {
                        logger.Debug("Socket exception was thrown.");
                        if (exception.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            logger.Debug("Connection was reset.");
                            break;
                        }
                    }
                }

                if (Pennies.Count == 0)
                {
                    logger.Debug("Shutting down conversation because pennies weren't received"
                        + "after 25 tries.");
                }
                else
                    logger.Debug("Received all the pennies expected from the penny bank.");
                socket.Close();
                Dictionary.CloseQueue(Request.Message.ConvId);
            }
            Stop();
        }
    }
}
