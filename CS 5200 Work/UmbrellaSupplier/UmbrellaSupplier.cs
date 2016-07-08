using System;
using CommunicationSubsystem;
using SharedObjects;
using log4net;
using System.Threading;
using System.Security.Cryptography;
using System.Collections.Generic;
using CommunicationSubsystem.Conversations.Initiators;

namespace UmbrellaSupplierProcess
{
    public class UmbrellaSupplier : CommunicationProcess
    {
        // Public data members.
        public SHA1Managed Hasher { get; set; }
        public UmbrellaSupplierOptions Options { get { return options; } }
        public GameInfo[] Games { get { return games; } set { games = value; } }
        public Queue<Umbrella> Umbrellas { get { return umbrellas; } set { umbrellas = value; } }
        public int NextId { get; set; }
        public int NumIds { get { return numIds; } }

        // Protected data memebers.
        protected IdentityInfo myIdentity;
        protected GameInfo[] games;
        protected Thread displayThread;
        protected UmbrellaSupplierConversationFactory factory;
        protected RSACryptoServiceProvider rsa;
        protected RSAPKCS1SignatureFormatter rsaSigner;
        protected Queue<Umbrella> umbrellas;
        protected int numIds;

        // Private data members.
        private UmbrellaSupplierOptions options;
        private static readonly ILog logger = LogManager.GetLogger(typeof(UmbrellaSupplier));

        // Default constructor.
        public UmbrellaSupplier()
        {
            options = new UmbrellaSupplierOptions();
        }

        /// <summary>
        /// Initializes the umbrella supplier.
        /// </summary>
        public void initialize()
        {
            factory = new UmbrellaSupplierConversationFactory();
            factory.Dictionary = new ConversationDictionary();
            factory.Communicator = new Communicator() { Dictionary = factory.Dictionary };
            umbrellas = new Queue<Umbrella>();
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
        /// Starts the umbrella supplier up.
        /// </summary>
        public void startUmbrellaSupplier()
        {
            displayThread = new Thread(new ThreadStart(updateInfo));

            logger.Debug("Start Umbrella Supplier and Dispatcher threads.");
            MyDispatcher.Start();
            Start();
        }

        /// <summary>
        /// Starts the umbrella supplier's processing.
        /// </summary>
        protected override void Process(object state)
        {
            MyDispatcher.Factory.Alive = true;

            while (MyDispatcher.Factory.Alive)
            {
                if (MyProcessInfo == null)
                {
                    MyProcessInfo = new ProcessInfo() { Type = ProcessInfo.ProcessType.BalloonStore };
                    tryLogin();
                    tryGetNextId();
                    createUmbrellas();
                    displayThread.Start();
                }
                else if (MyProcessInfo.Status == ProcessInfo.StatusCode.Registered &&
                    MyDispatcher.Factory.CurrentGameId <= 0)
                {
                    if (umbrellas == null)
                        createUmbrellas();

                    tryGetGameList();
                    tryJoinGame();
                }
                else if (MyDispatcher.Factory.CurrentGameId > 0 &&
                    MyDispatcher.Factory.GameStatus == GameInfo.StatusCode.InProgress)
                {
                    trySendAuctionAnnouncement();
                }
                else if (umbrellas.Count == 0)
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
        /// Gets the next ids for the umbrellas that will be created.
        /// </summary>
        public void tryGetNextId()
        {
            MyDispatcher.Factory.NumIds = Options.NumUmbrellas;
            logger.Debug("Attempting to get the next id from the registry.");

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(NextIdInitiator));
            conv.Launch();

            while (conv.Status == "Running") Thread.Sleep(0);
            NextId = conv.NextId;
            numIds = conv.NumIds;
        }

        /// <summary>
        /// Creates the allotted number of umbrellas for the umbrella supplier.
        /// </summary>
        public void createUmbrellas()
        {
            logger.Debug("Attempting to create balloons.");
            int i = NextId;

            while (umbrellas.Count < Options.NumUmbrellas)
            {
                Umbrella umbrella = new Umbrella()
                {
                    Id = i,
                    SignedBy = MyProcessInfo.ProcessId
                };

                byte[] bytes = umbrella.DataBytes();
                byte[] hash = Hasher.ComputeHash(bytes);

                umbrella.DigitalSignature = rsaSigner.CreateSignature(hash);
                umbrellas.Enqueue(umbrella);
                i++;
                logger.DebugFormat("Created balloon with id of {0}.", i);
            }
            logger.DebugFormat("Successfully created {0} balloons.", Options.NumUmbrellas);
            MyDispatcher.Factory.Umbrellas = Umbrellas;
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
        /// Attempts to send an auction announcement to all players of the game it's currently a part of.
        /// </summary>
        public void trySendAuctionAnnouncement()
        {
            logger.Debug("Attempting to send an auction announcement.");
            MyDispatcher.Factory.Umbrellas = umbrellas;

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                AuctionAnnouncementInitiator));
            conv.Launch();
            umbrellas = MyDispatcher.Factory.Umbrellas;
        }

        /// <summary>
        /// Attempts to leave a game the umbrella supplier is currently a part of.
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
        /// Updates the console with the current amount of umbrellas.
        /// </summary>
        protected void updateInfo()
        {
            while (MyDispatcher.Factory.Alive)
            {
                if (MyProcessInfo != null && MyDispatcher.Factory.CurrentGameId > 0)
                {
                    logger.Debug("Update number of Umbrellas.");
                    Console.WriteLine(string.Format("Umbrellas: {0}", MyDispatcher.Factory.Umbrellas.Count));
                }
                Thread.Sleep(100);
            }
        }
    }
}
