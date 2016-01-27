using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using log4net;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace Player
{
    /// <summary>
    /// This is a the beginnings of a simple player, following a multi-threaded but sort-of brute-force architecture.  It is not the only
    /// possible design, by any means.  It is certainly not the best design.  However, it does deal with a couple of keep issues, like
    /// intra-process concurrency for the three main tasks of the player for HW1: playing, responding to alive messages, and displaying some
    /// basic status information.  It also shows how logging can work.
    /// 
    /// This version does not show how to deal with incoming messages in any order or some of the other design problems.
    /// </summary>
    public class Player
    {
        #region Private and Protected Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Player));

        protected UdpClient myClient;
        protected IdentityInfo myIdentity;
        protected ProcessInfo myProcessInfo;
        protected List<GameInfo> availableGames;
        protected GameInfo currentGameInfo;

        protected bool keepGoing = false;
        protected Thread processThread;
        protected Thread listenThread;
        protected Thread displayThread;
        #endregion

        #region Public Properties
        public PublicEndPoint RegistryEndPoint { get; set;  }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ANumber { get; set; }
        public string Alias { get; set; }
        public string ProcessLabel { get; set; }

        public ProcessInfo ProcessInfo { get { return myProcessInfo; } }

        public int MyPort { get
        {
            int port = 0;
            if (myClient != null && myClient.Client != null)
            {
                IPEndPoint myEndPoint = myClient.Client.LocalEndPoint as IPEndPoint;
                if (myEndPoint != null)
                    port = myEndPoint.Port;
            }
            return port;
        }}

        #endregion

        #region Constructors, Initializers, and Destructors
        public void Initialize()
        {
            Logger.Debug("Initialize");

            myClient = new UdpClient();

            myIdentity = new IdentityInfo()
            {
                ANumber = ANumber,
                FirstName = FirstName,
                LastName = LastName,
                Alias = Alias
            };

        }

        public void Start()
        {
            Logger.Debug("Start");
            keepGoing = true;

            processThread = new Thread(new ThreadStart(Process));
            processThread.Start();

            listenThread = new Thread(new ThreadStart(Listen));
            listenThread.Start();

            displayThread = new Thread(new ThreadStart(DisplayStatus));
            displayThread.Start();
        }

        public void Stop()
        {
            keepGoing = false;

            if (displayThread != null)
            {
                displayThread.Join();
                displayThread = null;
            }

            if (listenThread != null)
            {
                listenThread.Join();
                listenThread = null;
            }

            if (processThread != null)
            {
                processThread.Join();
                processThread = null;
            }

        }

        #endregion

        #region Thread Processing Methods
        protected void Process()
        {
            try
            {
                while (keepGoing)
                {
                    Logger.Debug("Top of Process loop");
                    if (ProcessInfo == null)
                        TryToLogin();
                    else if (ProcessInfo.Status == ProcessInfo.StatusCode.Registered)
                    {
                        TryToGetGameList();
                        TryToJoinGame();
                    }
                    else if (ProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame &&
                             currentGameInfo.Status == GameInfo.StatusCode.InProgress)
                    {
                        // MakeAPlay();
                    }
                    Thread.Sleep(10);
                }
                TryToLogout();
            }
            catch (Exception err)
            {
                Logger.Error(err.ToString());
            }

        }

        protected void Listen()
        {
            while (keepGoing)
            {
                Logger.Debug("Top of Listen loop");

                // Look for an incoming AliveRequest
                // if one came in, then reply back to send with a Reply message that has Success=true;
                
                Thread.Sleep(10);
            }
        }

        protected void DisplayStatus()
        {
            while (keepGoing)
            {
                Logger.Debug("Top of Display loop");

                // Display stuff about the status of the player that is interesting
                
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Other Private and Protected Methods

        protected void TryToLogin()
        {
            // try to complete a login conversation with the registry
            //      Create a LoginReguest message using myIdentity and ProcessLabel
            //      Send to RegistryEndPoint
            //      Wait for a LoginReply
            //
            // if there a LoginReply, them same ProcessInfo from the reply to myProcessInfo
        }

        protected void TryToGetGameList()
        {
            // try to complete a game-list conversation with the registry
            // save returned list in available games
        }

        protected void TryToJoinGame()
        {
            // if there is at least on available game
                // Pick a game from the avaiable game list
                // try to complete a join game conversation the game manager of the selected game

            // if successfully, update status and save game joined in currentGameInfo
        }

        protected void TryToLogout()
        {
            // try to complete a logout conversation with the registry
        }

        #endregion

    }
}
