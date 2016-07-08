using System;
using CommunicationSubsystem;
using SharedObjects;
using log4net;
using System.Threading;
using System.Security.Cryptography;
using System.Collections.Generic;
using CommunicationSubsystem.Conversations.Initiators;

namespace WaterServerProcess
{
    public class WaterServer : CommunicationProcess
    {
        // Public data members.
        public SHA1Managed Hasher { get; set; }
        public WaterServerOptions Options { get { return options; } }
        public GameInfo[] Games { get { return games; } set { games = value; } }
        public Queue<WaterUnit> WaterUnits { get { return waterUnits; } set { waterUnits = value; } }
        public int NextId { get; set; }
        public int NumIds { get { return numIds; } }

        // Protected data memebers.
        protected IdentityInfo myIdentity;
        protected GameInfo[] games;
        protected Thread displayThread;
        protected WaterServerConversationFactory factory;
        protected RSACryptoServiceProvider rsa;
        protected RSAPKCS1SignatureFormatter rsaSigner;
        protected Queue<WaterUnit> waterUnits;
        protected int numIds;

        // Private data members.
        private WaterServerOptions options;
        private static readonly ILog logger = LogManager.GetLogger(typeof(WaterServer));

        // Default constructor.
        public WaterServer()
        {
            options = new WaterServerOptions();
        }

        /// <summary>
        /// Initializes the water server.
        /// </summary>
        public void initialize()
        {
            factory = new WaterServerConversationFactory();
            factory.Dictionary = new ConversationDictionary();
            factory.Communicator = new Communicator() { Dictionary = factory.Dictionary };
            waterUnits = new Queue<WaterUnit>();
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
        /// Starts the water server up.
        /// </summary>
        public void startWaterServer()
        {
            displayThread = new Thread(new ThreadStart(updateInfo));

            logger.Debug("Start Water Server and Dispatcher threads.");
            MyDispatcher.Start();
            Start();
        }

        /// <summary>
        /// Starts the water server's processing.
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
                    createWaterUnits();
                    displayThread.Start();
                }
                else if (MyProcessInfo.Status == ProcessInfo.StatusCode.Registered &&
                    MyDispatcher.Factory.CurrentGameId <= 0)
                {
                    if (waterUnits == null)
                        createWaterUnits();

                    tryGetGameList();
                    tryJoinGame();
                }
                else if (waterUnits.Count == 0)
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
        }

        /// <summary>
        /// Gets the next ids for the water units that will be created.
        /// </summary>
        public void tryGetNextId()
        {
        }

        /// <summary>
        /// Creates the allotted number of water units for the water server.
        /// </summary>
        public void createWaterUnits()
        {
        }

        /// <summary>
        /// Attempts to get the list of available games from the registry.
        /// </summary>
        public void tryGetGameList()
        {
        }

        /// <summary>
        /// Attempts to join a particular game.
        /// </summary>
        public void tryJoinGame()
        {
        }

        /// <summary>
        /// Attempts to leave a game the water server is currently a part of.
        /// </summary>
        public void tryLeaveGame()
        {
        }

        /// <summary>
        /// Attempts to log process out of the registry.
        /// </summary>
        public void tryLogout()
        {
        }

        /// <summary>
        /// Updates the console with the current amount of water units.
        /// </summary>
        protected void updateInfo()
        {
            while (MyDispatcher.Factory.Alive)
            {
                if (MyProcessInfo != null && MyDispatcher.Factory.CurrentGameId > 0)
                {
                    logger.Debug("Update number of Water Units.");
                    Console.WriteLine(string.Format("Water Units: {0}", MyDispatcher.Factory.WaterUnits.Count));
                }
                Thread.Sleep(100);
            }
        }
    }
}
