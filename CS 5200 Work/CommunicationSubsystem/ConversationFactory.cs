using System;
using System.Collections.Generic;
using SharedObjects;
using log4net;
using System.Security.Cryptography;

namespace CommunicationSubsystem
{
    public abstract class ConversationFactory
    {
        public ProcessInfo.ProcessType ProcessType { get; set; }
        public Communicator Communicator { get; set; }
        public int DefaultTimeout { get; set; }
        public int DefaultMaxTries { get; set; }
        public ConversationDictionary Dictionary { get; set; }
        public IdentityInfo Identity { get; set; }
        public ProcessInfo Process { get; set; }
        public GameInfo Game { get; set; }
        public GameInfo[] Games { get; set; }
        public Queue<Penny> Pennies { get; set; }
        public Queue<Balloon> FilledBalloons { get; set; }
        public int BalloonStoreId { get; set; }
        public int WaterSourceId { get; set; }
        public int PennyBankId { get; set; }
        public PublicKey PennyKey { get; set; }
        public Queue<Balloon> Balloons { get; set; }
        public bool Alive { get; set; }
        public int LifePoints { get; set; }
        public int CurrentGameId { get; set; }
        public GameProcessData[] CurrentProcesses { get; set; }
        public CommunicationProcess CommProcess { get; set; }
        public GameInfo.StatusCode GameStatus { get; set; }
        public Queue<Umbrella> Umbrellas { get; set; }
        public int NumIds { get; set; }
        public RSACryptoServiceProvider PennyRSA { get; set; }
        public bool UmbrellaRaised { get; set; }
        public int GamesPlayed { get; set; }
        public Queue<WaterUnit> WaterUnits { get; set; }

        private Dictionary<Type, Type> types;
        protected static readonly ILog logger = LogManager.GetLogger(typeof(ConversationFactory));

        /// <summary>
        /// Creates a conversation based on the message type.
        /// </summary>
        public virtual Conversation CreateFromMessageType(Envelope env)
        {
            logger.DebugFormat("Attempting to create conversation from {0}.", env.ActualMessageType);

            Conversation convo = null;
            Type messageType = env.ActualMessageType;
            if (types.ContainsKey(messageType))
            {
                convo = Activator.CreateInstance(types[messageType]) as Conversation;

                convo.ProcessType = ProcessType;
                convo.Communicator = Communicator;
                convo.Identity = Identity;
                convo.MyProcess = Process;
                convo.Game = Game;
                convo.Games = Games;
                convo.Dictionary = Dictionary;
                convo.PennyKey = PennyKey;
                convo.PennyBankId = PennyBankId;
                convo.BalloonStoreId = BalloonStoreId;
                convo.WaterSourceId = WaterSourceId;
                convo.Pennies = Pennies;
                convo.Balloons = Balloons;
                convo.Factory = this;
                convo.Alive = Alive;
                convo.LifePoints = LifePoints;
                convo.CurrentGameId = CurrentGameId;
                convo.CurrentProcesses = CurrentProcesses;
                convo.CommProcess = CommProcess;
                convo.Balloons = Balloons;
                convo.FilledBalloons = FilledBalloons;
                convo.Umbrellas = Umbrellas;
                convo.NumIds = NumIds;
                convo.PennyRSA = PennyRSA;
                convo.UmbrellaRaised = UmbrellaRaised;
                convo.GamesPlayed = GamesPlayed;

                convo.Queue = Dictionary.CreateQueue(env.Message.ConvId);
                convo.Queue.Enqueue(env);

                logger.DebugFormat("Created conversation from {0} succesfully.", env.ActualMessageType);
            }
            return convo;
        }

        /// <summary>
        /// Creates a conversation based on the conversation type.
        /// </summary>
        public virtual Conversation CreateFromConversationType(Type type)
        {
            logger.DebugFormat("Attempting to create conversation from ", type);
            Conversation convo = null;
            convo = Activator.CreateInstance(type) as Conversation;

            convo.ProcessType = ProcessType;
            convo.Communicator = Communicator;
            convo.Identity = Identity;
            convo.MyProcess = Process;
            convo.Game = Game;
            convo.Games = Games;
            convo.Dictionary = Dictionary;
            convo.PennyKey = PennyKey;
            convo.PennyBankId = PennyBankId;
            convo.WaterSourceId = WaterSourceId;
            convo.BalloonStoreId = BalloonStoreId;
            convo.Pennies = Pennies;
            convo.Balloons = Balloons;
            convo.Factory = this;
            convo.Alive = Alive;
            convo.LifePoints = LifePoints;
            convo.CurrentGameId = CurrentGameId;
            convo.CurrentProcesses = CurrentProcesses;
            convo.CommProcess = CommProcess;
            convo.Balloons = Balloons;
            convo.FilledBalloons = FilledBalloons;
            convo.Umbrellas = Umbrellas;
            convo.NumIds = NumIds;
            convo.PennyRSA = PennyRSA;
            convo.GamesPlayed = GamesPlayed;

            logger.DebugFormat("Created conversation from {0} successfully.", type);
            return convo;
        }

        /// <summary>
        /// Adds a message type to the list of message that the process should respond to.
        /// </summary>
        public void Add(Type messageType, Type conversationType)
        {
            types.Add(messageType, conversationType);
        }

        /// <summary>
        /// Initializes the ConversationFactory.
        /// </summary>
        public void Initialize()
        {
            types = new Dictionary<Type, Type>();
            DefaultMaxTries = 3;
            DefaultTimeout = 3000;
        }

        /// <summary>
        /// States whether an incoming message should start a conversation.
        /// </summary>
        public bool IncomingMessageCanStartConversation(Type messageType)
        {
            return types.ContainsKey(messageType);
        }

        public abstract void InitializeTypes();
    }
}
