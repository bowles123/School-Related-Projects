using Utils;
using Messages;
using log4net;
using System.Threading;
using System;

namespace CommunicationSubsystem
{
    public class Dispatcher: BackgroundThread
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Dispatcher));
        private const int TIMEOUT = 3000;

        public ConversationDictionary Dictionary { get; set; }
        public Communicator Communicator { get; set; }
        public ConversationFactory Factory { get; set; }

        /// <summary>
        /// Starts the dispatcher's processing.
        /// </summary>
        protected override void Process(object state)
        {
            while(KeepGoing)
            {
                Envelope envelope = Communicator.Retrieve(TIMEOUT);
                EnvelopeQueue queue = null;

                // if there is an envelope check for the conversation in the dictionary
                if (envelope != null)
                {
                    logger.DebugFormat("Received an envelope with message of type {0}", envelope.ActualMessageType);
                    Message message = envelope.ActualMessage;
                    Type type = envelope.ActualMessageType;

                    queue = Dictionary.GetByConversation(envelope.Message.ConvId);

                    if (queue == null)
                    {
                        logger.DebugFormat("Conversation {0} does not already exist.", message.ConvId);

                        if (Factory.IncomingMessageCanStartConversation(type))
                        {
                            Conversation conversation = Factory.CreateFromMessageType(envelope);

                            if (conversation != null)
                            {
                                conversation.Launch();
                                Thread.Sleep(100);

                                // Set up dispatcher's factory information so it can get back to the player.
                                Factory.CurrentGameId = conversation.CurrentGameId;
                                Factory.Game = conversation.Game;
                                Factory.GameStatus = conversation.GameStatus;
                                Factory.Pennies = conversation.Pennies;

                                if (conversation.LifePoints > 0)
                                    Factory.LifePoints = conversation.LifePoints;
                                if (conversation.GamesPlayed > 0)
                                    Factory.GamesPlayed = conversation.GamesPlayed;

                                Factory.CurrentProcesses = conversation.CurrentProcesses;
                                Factory.Alive = conversation.Alive;
                                Factory.Process.Status = conversation.MyProcess.Status;
                                Factory.Umbrellas = conversation.Umbrellas;
                                Factory.BalloonStoreId = conversation.BalloonStoreId;
                                Factory.WaterSourceId = conversation.WaterSourceId;
                                Factory.PennyBankId = conversation.PennyBankId;
                                Factory.UmbrellaRaised = conversation.UmbrellaRaised;
                            }
                        }
                        else
                        {
                            logger.DebugFormat("Process should not respond to a {0} message.", message.GetType());
                        }
                    }
                    else
                    {
                        logger.DebugFormat("Conversation {0} already exists.", message.ConvId);
                        queue.Enqueue(envelope);
                    }
                }
                Thread.Sleep(10);
            }
        }
    }
}
