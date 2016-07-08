using System;
using SharedObjects;
using System.Net;
using System.Threading;
using log4net;
using CommunicationSubsystem;
using System.Collections.Generic;
using CommunicationSubsystem.Conversations.Initiators;

namespace PlayerProcess
{
    public class Player : CommunicationProcess
    {
        // Public properties.
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ANumber { get; set; }
        public string Nickname { get; set; }
        public GameInfo[] Games { get { return games; } set { games = value; } }
        public PublicEndPoint MyEndpoint { get; set; }
        public Queue<Penny> Pennies { get; set; }
        public Queue<Balloon> Balloons { get; set; }
        public Queue<Balloon> FilledBalloons { get; set; }
        public Queue<Umbrella> Umbrellas { get; set; }
        public bool UmbrellaRaised { get { return umbrellaRaised; } }
        public PlayerOptions Options { get { return options; } }

        public int myPort
        {
            get
            {
                int port = 0;
                if (MyCommunicator.MySocket != null && MyCommunicator.MySocket.Client != null)
                {
                    IPEndPoint myEndpoint = MyCommunicator.MySocket.Client.LocalEndPoint as IPEndPoint;
                    if (myEndpoint != null)
                        port = myEndpoint.Port;
                }
                return port;
            }
        }

        // Protected data members.
        protected int lifepoints;
        protected bool umbrellaRaised;
        protected IdentityInfo myIdentity;
        protected Thread displayThread;
        protected GameInfo[] games;
        protected PlayerConversationFactory factory;

        // Private data members.
        private static readonly ILog logger = LogManager.GetLogger(typeof(Player));
        private PlayerOptions options;
        private int gamesPlayed;

        // Default constructor.
        public Player()
        {
            options = new PlayerOptions();
        }

        /// <summary>
        /// Initializes a player with default values for its data members.
        /// </summary>
        public void initialize()
        {
            logger.Debug("Initialize Player.");

            myIdentity = new IdentityInfo()
            {
                ANumber = Options.ANumber,
                FirstName = Options.FirstName,
                LastName = Options.LastName,
                Alias = Options.Alias
            };

            gamesPlayed = 0;
            lifepoints = 0;
            MyProcessInfo = null;
            //RegistryEndPoint = new PublicEndPoint("52.3.213.61:12000");
            RegistryEndPoint = new PublicEndPoint("127.0.0.1:12000");
            ProxyEndPoint = new PublicEndPoint();
            PennyBankEndPoint = new PublicEndPoint();
            Pennies = new Queue<Penny>();
            Balloons = new Queue<Balloon>();
            FilledBalloons = new Queue<Balloon>();
            Umbrellas = new Queue<Umbrella>();
            PennyBankEndPoint = new PublicEndPoint();

            factory = new PlayerConversationFactory();
            factory.Dictionary = new ConversationDictionary();
            factory.Communicator = new Communicator() { Dictionary = factory.Dictionary };
            factory.Initialize();
            factory.InitializeTypes();
            factory.LifePoints = 0;
            factory.ProcessType = ProcessInfo.ProcessType.Player;
            factory.Pennies = Pennies;
            factory.Balloons = Balloons;
            factory.FilledBalloons = FilledBalloons;
            factory.Umbrellas = Umbrellas;
            factory.CommProcess = this;

            umbrellaRaised = false;
            SetupCommSubsystem(factory);
        }

        /// <summary>
        /// Starts the player processing and updating.
        /// </summary>
        public void startPlayer()
        {
            // Create separate threads for the player process, listening, and displaying player's point information.
            displayThread = new Thread(new ThreadStart(updateInfo));

            logger.Debug("Start player and Dispatcher threads.");
            MyDispatcher.Start();
            Start();
        }

        /// <summary>
        /// Loops through sending different requests as long as the process is alive.
        /// </summary>
        protected override void Process(object objState)
        {
            // Variables.
            MyDispatcher.Factory.Alive = true;

            logger.Debug("Start player processing.");

            while (MyDispatcher.Factory.Alive)
            {
                if (MyProcessInfo == null)
                {
                    MyProcessInfo = new ProcessInfo() { Type = ProcessInfo.ProcessType.Player };
                    tryLogin();

                    // Start display thread.
                    displayThread.Start();
                }
                else if (MyProcessInfo.Status == ProcessInfo.StatusCode.Registered && 
                    MyDispatcher.Factory.CurrentGameId <= 0)
                {
                    tryGetGameList();
                    tryJoinGame();
                    MyDispatcher.Factory.LifePoints = lifepoints;
                }
                else
                {
                    if (MyDispatcher.Factory.CurrentGameId > 0 && 
                        MyDispatcher.Factory.GameStatus == GameInfo.StatusCode.InProgress)
                    {
                        Console.WriteLine("Game ID: {0}", MyDispatcher.Factory.CurrentGameId);

                        tryRaiseUmbrella();
                        tryMove('B');
                        tryMove('F');
                        tryMove('T');

                        if (MyDispatcher.Factory.Umbrellas.Count > 0)
                            tryRaiseUmbrella();
                    }

                    if (gamesPlayed > 100)
                        tryLogout();
                    Thread.Sleep(100);
                }
            }
            Stop();
        }

        /// <summary>
        /// Tries to make a move with a balloon; either tries to buy one, fill one, or throw one.
        /// </summary>
        public void tryMove(char move)
        {
            if (char.ToUpper(move) == 'T' && MyDispatcher.Factory.FilledBalloons.Count > 0)
                tryThrowBalloon();
            if (char.ToUpper(move) == 'B' && Pennies.Count > 0)
                tryBuyBalloon();
            if (char.ToUpper(move) == 'F' && MyDispatcher.Factory.Balloons.Count > 0)
                tryFillBalloon();
        }

        /// <summary>
        /// Tries to throw a balloon if a filled one is available.
        /// </summary>
        public void tryThrowBalloon()
        {
            logger.Debug("Attempting to throw balloon.");

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                ThrowBalloonInitiator));
            conv.Launch();

            while (conv.Status == "Running") Thread.Sleep(0);
            MyDispatcher.Factory.Balloons = conv.Balloons;
            MyDispatcher.Factory.FilledBalloons = conv.FilledBalloons;
        }

        /// <summary>
        /// Tries to buy a baloon.
        /// </summary>
        public void tryBuyBalloon()
        {
            logger.Debug("Attempting to buy balloon.");

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                BuyBalloonInitiator));
            conv.Launch();

            while (conv.Status == "Running") Thread.Sleep(0);
            MyDispatcher.Factory.Balloons = conv.Balloons;
            MyDispatcher.Factory.Pennies = conv.Pennies;
        }

        /// <summary>
        /// Tries to fill a balloon if one is availabe.
        /// </summary>
        public void tryFillBalloon()
        {
            logger.Debug("Attempting to fill balloon.");

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                FillBalloonInitiator));
            conv.Launch();

            while (conv.Status == "Running") Thread.Sleep(0);
            MyDispatcher.Factory.FilledBalloons = conv.FilledBalloons;
            MyDispatcher.Factory.Balloons = conv.Balloons;
            MyDispatcher.Factory.Pennies = conv.Pennies;
        }

        /// <summary>
        /// Tries to raise an umbrella if one is available.
        /// </summary>
        public void tryRaiseUmbrella()
        {
            logger.Debug("Attempting to raise umbrella.");
            if (MyDispatcher.Factory.Umbrellas.Count <= 0)
                return;

            Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                RaiseUmbrellaInitiator));
            conv.Launch();

            while (conv.Status == "Running") Thread.Sleep(0);
            MyDispatcher.Factory.Umbrellas = conv.Umbrellas;
            MyDispatcher.Factory.UmbrellaRaised = conv.UmbrellaRaised;
            umbrellaRaised = conv.UmbrellaRaised;
        }

        /// <summary>
        /// Tries to log the process in with the registry.
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
            PennyBankEndPoint = conv.CommProcess.PennyBankEndPoint;
            MyDispatcher.Factory.Process = MyProcessInfo;
        }

        /// <summary>
        /// Tries to get the list of available games from the registry.
        /// </summary>
        public void tryGetGameList()
        {
            logger.Debug("Attempting to get the game list.");

            do
            {
                games = new GameInfo[1];
                Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                    GetGameListInitiator));
                conv.Launch();

                while (conv.Status == "Running") Thread.Sleep(0);
                games = conv.Games;
            }
            while (games == null);
        }

        /// <summary>
        /// Tries to join an available game from the list of games received from the registry.
        /// </summary>
        public void tryJoinGame()
        {
            logger.Debug("Attempting to join a game.");
            MyDispatcher.Factory.Process = MyProcessInfo;
            MyDispatcher.Factory.Pennies = factory.Pennies;
            MyDispatcher.Factory.LifePoints = lifepoints;

            for (int i = 0; i < games.Length; i++)
            {
                MyDispatcher.Factory.Game = games[i];
                Conversation conv = MyDispatcher.Factory.CreateFromConversationType(typeof(
                    JoinGameInitiator));
                conv.Launch();

                while (conv.Status == "Running") Thread.Sleep(0);
                MyDispatcher.Factory.Game = conv.Game;
                MyDispatcher.Factory.CurrentGameId = conv.Game.GameId;
                MyDispatcher.Factory.CurrentGameId = conv.CurrentGameId;
                lifepoints = conv.LifePoints;
                return;
            }
        }

        /// <summary>
        /// Tries to logout with the registry.
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
        /// Attempts to leave a game that the process is currently a part of.
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
            MyDispatcher.Factory.Balloons = conv.Balloons;
            MyDispatcher.Factory.BalloonStoreId = conv.BalloonStoreId;
            MyDispatcher.Factory.FilledBalloons = conv.FilledBalloons;
            MyDispatcher.Factory.WaterSourceId = conv.WaterSourceId;
            MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
        }

        /// <summary>
        /// Updates the console with the player's information.
        /// </summary>
        protected void updateInfo()
        {
            while (MyDispatcher.Factory.Alive)
            {
                if (MyDispatcher.Factory.Game != null &&
                    MyDispatcher.Factory.Game.Status == GameInfo.StatusCode.Complete)
                {
                    logger.DebugFormat("Games played: {0}", gamesPlayed);
                    MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
                    MyDispatcher.Factory.CurrentGameId = 0;
                    gamesPlayed++;
                    MyDispatcher.Factory.Game = null;
                }

                if (MyProcessInfo != null && MyDispatcher.Factory.CurrentGameId > 0)
                {
                    logger.Debug("Update player information.");

                    if (MyDispatcher.Factory.Game.Status == GameInfo.StatusCode.Cancelled ||
                        MyDispatcher.Factory.Game.Status == GameInfo.StatusCode.Ending)
                    {
                        MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
                        MyDispatcher.Factory.CurrentGameId = 0;
                    }
                    umbrellaRaised = MyDispatcher.Factory.UmbrellaRaised;
                    gamesPlayed = MyDispatcher.Factory.GamesPlayed;

                    Console.WriteLine(string.Format(
                        "Process Id: {6}, Life Points: {0}, Pennies {1}, Balloons: {2}, Filled Balloons: {3}, Umbrellas: {4}, Umbrella Raised: {5}", 
                        MyDispatcher.Factory.LifePoints, MyDispatcher.Factory.Pennies.Count,
                        MyDispatcher.Factory.Balloons.Count, MyDispatcher.Factory.Umbrellas.Count,
                        MyDispatcher.Factory.FilledBalloons.Count, umbrellaRaised,
                        MyProcessInfo.ProcessId));
                }
                Thread.Sleep(100);
            }
        }
    }
}
