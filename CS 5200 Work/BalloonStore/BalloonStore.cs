using System;
using CommunicationSubsystem;
using SharedObjects;
using log4net;
using System.Threading;
using System.Security.Cryptography;
using System.Collections.Generic;
using CommunicationSubsystem.Conversations.Initiators;

namespace BalloonStoreProcess
{
    public class BalloonStore: CommunicationProcess
    {
        // Public data members.
        public SHA1Managed Hasher { get; set; }
        public BalloonStoreOptions Options { get { return options; } }
        public GameInfo[] Games { get { return games; } set { games = value; } }
        public Queue<Balloon> Balloons { get { return balloons; } set { balloons = value; } }
        public int NextId { get; set; }
        public int NumIds { get { return numIds; } }

        // Protected data memebers.
        protected IdentityInfo myIdentity;
        protected GameInfo[] games;
        protected Thread displayThread;
        protected BalloonStoreConversationFactory factory;
        protected RSACryptoServiceProvider rsa;
        protected RSAPKCS1SignatureFormatter rsaSigner;
        protected Queue<Balloon> balloons;
        protected int numIds;

        // Private data members.
        private BalloonStoreOptions options;
        private static readonly ILog logger = LogManager.GetLogger(typeof(BalloonStore));

        // Default constructor.
        public BalloonStore()
        {
            options = new BalloonStoreOptions();
        }

        /// <summary>
        /// Initializes the balloon store.
        /// </summary>
        public void initialize()
        {
            factory = new BalloonStoreConversationFactory();
            factory.Dictionary = new ConversationDictionary();
            factory.Communicator = new Communicator() { Dictionary = factory.Dictionary };
            balloons = new Queue<Balloon>();
            games = new GameInfo[1];
            RegistryEndPoint = new PublicEndPoint("127.0.0.1:12000");
            ProxyEndPoint = new PublicEndPoint();

            factory.Initialize();
            factory.InitializeTypes();
            factory.CommProcess = this;
            factory.Pennies = new Queue<Penny>();
            SetupCommSubsystem(factory);

            myIdentity = new IdentityInfo()
            {
                Alias = Options.Alias,
                ANumber = Options.ANumber,
                FirstName = Options.FirstName,
                LastName = Options.LastName
            };

            rsa = new RSACryptoServiceProvider();
            rsaSigner = new RSAPKCS1SignatureFormatter(rsa);
            rsaSigner.SetHashAlgorithm("SHA1");
            Hasher = new SHA1Managed();
            RSAParameters parameters = rsa.ExportParameters(false);

            PublicKey = new PublicKey()
            {
                Exponent = parameters.Exponent,
                Modulus = parameters.Modulus
            };
        }

        /// <summary>
        /// Starts the balloon store up.
        /// </summary>
        public void startBalloonStore()
        {
            displayThread = new Thread(new ThreadStart(updateInfo));

            logger.Debug("Start Balloon Store and Dispatcher threads.");
            MyDispatcher.Start();
            Start();
        }

        /// <summary>
        /// Starts the balloon store's processing.
        /// </summary>
        protected override void Process(object state)
        {
            MyDispatcher.Factory.Alive = true;

            while(MyDispatcher.Factory.Alive)
            {
                if (MyProcessInfo == null)
                {
                    MyProcessInfo = new ProcessInfo() { Type = ProcessInfo.ProcessType.BalloonStore };
                    tryLogin();
                    tryGetNextId();
                    createBalloons();
                    displayThread.Start();
                }
                else if (MyProcessInfo.Status == ProcessInfo.StatusCode.Registered && 
                    MyDispatcher.Factory.CurrentGameId <= 0)
                {
                    if (balloons == null)
                        createBalloons();

                    tryGetGameList();
                    tryJoinGame();
                }
                else if (balloons.Count == 0)
                {
                    tryLeaveGame();
                }
            }
        }

        /// <summary>
        /// Attempts to log the process in with the registry.
        /// </summary>
        public void tryLogin()
        {
            logger.Debug("Attempting to login.");

            MyDispatcher.Factory.Identity = myIdentity;
            MyDispatcher.Factory.Process = MyProcessInfo;
            MyDispatcher.Factory.ProcessType = MyProcessInfo.Type;

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(LoginInitiator));
            conv.Launch();

            while (conv.Status == "Running") Thread.Sleep(0);
            MyProcessInfo = conv.MyProcess;
            ProxyEndPoint = conv.CommProcess.ProxyEndPoint;
            MyDispatcher.Factory.Process = MyProcessInfo;
            MyDispatcher.Factory.PennyKey = conv.PennyKey;

            RSAParameters pennyParameters = new RSAParameters();
            pennyParameters.Modulus = MyDispatcher.Factory.PennyKey.Modulus;
            pennyParameters.Exponent = MyDispatcher.Factory.PennyKey.Exponent;
            MyDispatcher.Factory.PennyRSA = new RSACryptoServiceProvider();
            MyDispatcher.Factory.PennyRSA.ImportParameters(pennyParameters);
        }

        /// <summary>
        /// Gets the next ids for the balloons that will be created.
        /// </summary>
        public void tryGetNextId()
        {
            MyDispatcher.Factory.NumIds = Options.NumBalloons;
            logger.Debug("Attempting to get the next id from the registry.");

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(NextIdInitiator));
            conv.Launch();

            while (conv.Status == "Running") Thread.Sleep(0);
            NextId = conv.NextId;
            numIds = conv.NumIds;
        }

        /// <summary>
        /// Creates the allotted number of balloons for the balloon store.
        /// </summary>
        public void createBalloons()
        {
            logger.Debug("Attempting to create balloons.");
            int i = NextId;

            while (balloons.Count < Options.NumBalloons)
            {
                Balloon balloon = new Balloon()
                {
                    Id = i,
                    IsFilled = false,
                    SignedBy = MyProcessInfo.ProcessId
                };

                byte[] bytes = balloon.DataBytes();
                byte[] hash = Hasher.ComputeHash(bytes);

                balloon.DigitalSignature = rsaSigner.CreateSignature(hash);
                balloons.Enqueue(balloon);
                i++;
                logger.DebugFormat("Created balloon with id of {0}.", i);
            }
            logger.DebugFormat("Successfully created {0} balloons.", Options.NumBalloons);
            MyDispatcher.Factory.Balloons = balloons;
        }

        /// <summary>
        /// Attempts to get the list of available games from the registry.
        /// </summary>
        public void tryGetGameList()
        {
            logger.Debug("Attempting to get the game list.");
            MyDispatcher.Factory.Games = games;

            do
            {
                Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                    GetGameListInitiator));
                conv.Launch();

                while (conv.Status == "Running") Thread.Sleep(0);
                games = conv.Games;
            }
            while (games.Length == 0);
        }

        /// <summary>
        /// Attempts to join a particular game.
        /// </summary>
        public void tryJoinGame()
        {
            logger.Debug("Attempting to join a game.");
            MyDispatcher.Factory.Process = MyProcessInfo;
            MyDispatcher.Factory.Pennies = factory.Pennies;

            for (int i = 0; i < games.Length; i++)
            {
                MyDispatcher.Factory.Game = games[i];
                Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                    JoinGameInitiator));
                conv.Launch();

                while (conv.Status == "Running") Thread.Sleep(0);
                MyDispatcher.Factory.Game = conv.Game;
                MyDispatcher.Factory.CurrentGameId = conv.CurrentGameId;
                return;
            }
        }

        /// <summary>
        /// Attempts to leave a game the balloon store is currently a part of.
        /// </summary>
        public void tryLeaveGame()
        {
            logger.Debug("Attempting to leave game.");
            MyDispatcher.Factory.Process = MyProcessInfo;

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                LeaveGameInitiator));
            conv.Launch();

            while (conv.Status == "Running") Thread.Sleep(0);
            MyDispatcher.Factory.Game = conv.Game;
            MyDispatcher.Factory.CurrentGameId = conv.CurrentGameId;
            MyDispatcher.Factory.CurrentProcesses = conv.CurrentProcesses;
            MyDispatcher.Factory.GameStatus = conv.GameStatus;
            MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
        }

        /// <summary>
        /// Attempts to log process out of the registry.
        /// </summary>
        public void tryLogout()
        {
            logger.Debug("Attempting to logout.");
            MyDispatcher.Factory.Process = MyProcessInfo;

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(LogoutInitiator));
            conv.Launch();

            while (conv.Status == "Running") Thread.Sleep(0);
            MyDispatcher.Factory.Alive = false;
            MyDispatcher.Factory.Game = conv.Game;
            MyDispatcher.Factory.Games = conv.Games;
            MyDispatcher.Factory.PennyKey = conv.PennyKey;
            MyProcessInfo.Status = conv.MyProcess.Status;
        }

        /// <summary>
        /// Updates the console with the current amount of balloons.
        /// </summary>
        protected void updateInfo()
        {
            while (MyDispatcher.Factory.Alive)
            {
                if (MyProcessInfo != null && MyDispatcher.Factory.CurrentGameId > 0)
                {
                    logger.Debug("Update number of Balloons.");
                    Console.WriteLine(string.Format("Balloons: {0}", MyDispatcher.Factory.Balloons.Count));
                }
                Thread.Sleep(100);
            }
        }
    }
}
