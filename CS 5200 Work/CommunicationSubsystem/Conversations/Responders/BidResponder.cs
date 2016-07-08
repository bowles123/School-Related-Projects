using Messages.ReplyMessages;
using Messages;
using SharedObjects;
using System.Threading;
using System.Security.Cryptography;
using System.Collections.Generic;
using CommunicationSubsystem.Conversations.Initiators;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class BidResponder : RequestReplyProxy
    {
        private bool partofGame;
        private bool valid;

        public static List<int> usedPennies;
        public static int MinimumBid;

        public BidResponder()
        {
            usedPennies = BuyBalloonResponder.usedPennies;
            if (usedPennies == null)
                usedPennies = new List<int>();
        }

        protected override void Process(object state)
        {
            Umbrella umbrella = null;
            logger.Debug("Executing BuyBalloonResponder.");
            ProxyEP = CommProcess.ProxyEndPoint;

            if (Umbrellas.Count != 0)
                umbrella = Umbrellas.Dequeue();

            BidAck reply = new BidAck() { Success = true, Umbrella = umbrella, Won = true };
            Routing response;
            int[] to;
            Request = null;

            Request = Queue.Dequeue(Timeout);
            reply.SetMessageAndConversationNumbers(MessageNumber.Create(), Request.ActualMessage.ConvId);

            if (Request != null)
            {
                Bid request = Request.ActualMessage as Bid;
                logger.Debug("Recieved a bid.");
                to = new int[1] { Request.ActualMessage.ConvId.Pid };

                if (request.Pennies.Length >= MinimumBid)
                {
                    logger.Debug("Bid was less than minimum bid.");
                    reply.Success = false;
                    reply.Note = "Amount of pennies does not match minimum bid.";
                    response = new Routing() { InnerMessage = reply, ToProcessIds = to };
                    Response = new Envelope() { Message = response, Endpoint = ProxyEP };
                    valid = false;
                }

                // Ensure that the number of umbrellas is not zero.
                if (valid && Umbrellas.Count == 0)
                {
                    logger.Debug("Out of umbrellas.");
                    reply.Success = false;
                    reply.Note = "No more balloons remaining the the inventory.";
                    response = new Routing() { InnerMessage = reply, ToProcessIds = to };
                    Response = new Envelope() { Message = response, Endpoint = ProxyEP };
                    valid = false;
                }
                else
                    valid = true;

                if (Game.Status == GameInfo.StatusCode.InProgress)
                {
                    if (valid)
                    {
                        // Ensure that process is part of the game.
                        if (CurrentProcesses != null && CurrentProcesses.Length > 0)
                            foreach (GameProcessData process in CurrentProcesses)
                            {
                                if (process.ProcessId == Request.Message.ConvId.Pid &&
                                        process.Type == ProcessInfo.ProcessType.Player)
                                {
                                    partofGame = true;
                                    break;
                                }
                            }

                        if (!partofGame)
                        {
                            logger.DebugFormat("Process {0} is not part of the game.",
                                Request.ActualMessage.ConvId);
                            Umbrellas.Enqueue(reply.Umbrella);
                            reply.Umbrella = null;
                            reply.Success = false;
                            reply.Note = "You are not part of this game.";
                            response = new Routing() { InnerMessage = reply, ToProcessIds = to };
                            Response = new Envelope() { Message = response, Endpoint = ProxyEP };
                            valid = false;
                        }
                        else
                            valid = true;
                    }
                    foreach (Penny penny in request.Pennies)
                    { 
                        if (valid)
                        {
                            // Ensure that penny was not used in previous request.
                            if (usedPennies.Count > 0 && usedPennies.Contains(penny.Id))
                            {
                                logger.Debug("Penny has already been used.");
                                Umbrellas.Enqueue(reply.Umbrella);
                                reply.Umbrella = null;
                                reply.Success = false;
                                reply.Note = "The penny has already been used.";
                                response = new Routing() { InnerMessage = reply, ToProcessIds = to };
                                Response = new Envelope() { Message = response, Endpoint = ProxyEP };
                                valid = false;
                            }
                            else
                                valid = true;
                        }

                        if (valid)
                        {
                            // Check digital signature with the penny bank's public key
                            byte[] pennyBytes = penny.DataBytes();
                            SHA1Managed hasher = new SHA1Managed();
                            byte[] pennyHash = hasher.ComputeHash(pennyBytes);
                            RSAPKCS1SignatureDeformatter rsaSignComparer =
                                new RSAPKCS1SignatureDeformatter(PennyRSA);
                            rsaSignComparer.SetHashAlgorithm("SHA1");
                            bool verified = rsaSignComparer.VerifySignature(pennyHash,
                                penny.DigitalSignature);

                            if (!verified)
                            {
                                logger.Debug("Penny's signature is not valid.");
                                Umbrellas.Enqueue(reply.Umbrella);
                                reply.Umbrella = null;
                                reply.Success = false;
                                reply.Note = "The penny's signature is not valid.";
                                response = new Routing() { InnerMessage = reply, ToProcessIds = to };
                                Response = new Envelope() { Message = response, Endpoint = ProxyEP };
                                valid = false;
                            }
                            else
                                valid = true;
                        }

                        if (valid)
                        {
                            // Validate Penny with the Pennybank.
                            Conversation convo = Factory.CreateFromConversationType(
                                typeof(PennyValidationInitiator));
                            convo.PennyToValidate = penny;
                            convo.Launch();
                            while (convo.Status == "Running") Thread.Sleep(0);

                            if (PennyValidationInitiator.ValidPenny && valid)
                            {
                                reply.Success = true;
                                logger.Debug("Penny is valid to use.");
                                response = new Routing() { InnerMessage = reply, ToProcessIds = to };
                                Response = new Envelope() { Message = response, Endpoint = ProxyEP };
                                usedPennies.Add(penny.Id);
                            }
                            else
                            {
                                reply.Success = false;
                                logger.Debug("Penny did not come from the penny bank.");
                                Umbrellas.Enqueue(reply.Umbrella);
                                reply.Umbrella = null;
                                reply.Note = "Penny is not a valid penny from the penny bank.";
                                response = new Routing() { InnerMessage = reply, ToProcessIds = to };
                                Response = new Envelope() { Message = response, Endpoint = ProxyEP };
                                valid = false;
                            }
                        }
                    }
                }
                else
                {
                    reply.Success = false;
                    logger.Debug("Game is not in progress.");
                    Umbrellas.Enqueue(reply.Umbrella);
                    reply.Umbrella = null;
                    reply.Note = "Cannot bid on an umbrella unless the game is in progress.";
                    response = new Routing() { InnerMessage = reply, ToProcessIds = to };
                    Response = new Envelope() { Message = response, Endpoint = ProxyEP };
                    valid = false;
                }

                logger.Debug("Attempting to send bid acknowledgement.");
                Communicator.Send(Response);
                Dictionary.CloseQueue(reply.ConvId);
                logger.Debug("Successfully sent reply to bid.");
            }
            Stop();
        }
    }
}
