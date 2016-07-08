using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using CommunicationSubsystem;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;
using System.Net.Sockets;
using System.Net;
using Messages;
using System.Collections.Generic;
using PlayerProcess;
using CommunicationSubsystem.Conversations.Initiators;

namespace CommunicationSubsystemTesting
{
    [TestClass]
    public class ConversationInitiatorsTest
    {
        private ConversationFactory factory;
        private ConversationDictionary dictionary;
        private Communicator mockCommunicator;
        private int mockCommunicatorPort;
        private PublicEndPoint mockCommunicatorEp;
        private ProcessInfo player;
        private PublicKey pennyBankKey;
        private IdentityInfo identity;
        private GameInfo[] games;
        private ProcessInfo gameManager;
        private GameInfo game1;
        private GameInfo game2;
        private GameInfo game3;
        private GameInfo[] sendGames;
        private UdpClient mockProxyClient;
        private int mockProxyClientPort;
        private PublicEndPoint mockProxyEP;
        private GameInfo game;
        private Queue<Penny> myPennies;
        private Queue<Balloon> myBalloons;
        private Queue<Balloon> myFilledBalloons;
        private PublicEndPoint mockBalloonStoreEp;
        private PublicEndPoint mockWaterSupplierEp;
        private PublicEndPoint mockUmbrellaSupplierEp;
        private ProcessInfo umbrellaSupplier;
        private ProcessInfo balloonStore;
        private ProcessInfo waterSupplier;
        private CommunicationProcess myProcess;
        private ProcessInfo throwAt;
        private Dispatcher dispatcher;
        private Queue<Umbrella> myUmbrellas;
        private Umbrella umbrella;
        private int initializeCalls = 0;

        private void Initialize()
        {
            initializeCalls++;

            // Initialize needed components of initiators.
            factory = new PlayerConversationFactory();
            dictionary = new ConversationDictionary();
            mockCommunicator = new Communicator();
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            player = new ProcessInfo();
            pennyBankKey = new PublicKey();
            myBalloons = new Queue<Balloon>();
            myFilledBalloons = new Queue<Balloon>();
            myUmbrellas = new Queue<Umbrella>();

            // Set up dispatcher.
            dispatcher = new Dispatcher();
            dispatcher.Factory = factory;
            dispatcher.Dictionary = dictionary;
            dispatcher.Communicator = mockCommunicator;

            // Initialize proxy information.
            mockProxyClient = new UdpClient(0);
            mockProxyClientPort = ((IPEndPoint)mockProxyClient.Client.LocalEndPoint).Port;
            mockProxyEP = new PublicEndPoint()
            {
                Host = "127.0.0.1",
                Port = mockProxyClientPort
            };

            myProcess = new Player()
            {
                RegistryEndPoint = mockCommunicatorEp,
                ProxyEndPoint = mockProxyEP,
                PennyBankEndPoint = mockCommunicatorEp
            };

            // Assign values to the needed attributes of the conversation factory.
            factory.Initialize();
            factory.InitializeTypes();
            factory.Communicator = mockCommunicator;
            factory.Dictionary = dictionary;
            factory.ProcessType = ProcessInfo.ProcessType.Player;
            factory.Process = player;
            factory.Balloons = myBalloons;
            factory.FilledBalloons = myFilledBalloons;
            factory.CommProcess = myProcess;
        }

        private void InitializeLogin()
        {
            identity = new IdentityInfo()
            {
                ANumber = "A01667582",
                FirstName = "Brian",
                LastName = "Bowles",
                Alias = "Brian"
            };

            player = new ProcessInfo()
            {
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.Unknown,
                Type = ProcessInfo.ProcessType.Player,
                Label = "Brian"
            };

            // Assign values to the needed attributes of the conversation factory.
            factory.Identity = identity;
            factory.PennyKey = pennyBankKey;
        }

        private void InitializeGetGameList()
        {
            games = new GameInfo[3];
            gameManager = new ProcessInfo()
            {
                ProcessId = 1,
                Type = ProcessInfo.ProcessType.GameManager,
                EndPoint = mockCommunicatorEp
            };

            // Set up games and add them to the games array.
            game1 = new GameInfo()
            {
                GameManagerId = gameManager.ProcessId,
                GameId = 1,
                Status = GameInfo.StatusCode.Available,
                MaxPlayers = 5,
                MinPlayers = 3,
                Label = "Game #1"
            };

            game2 = new GameInfo()
            {
                GameManagerId = gameManager.ProcessId,
                GameId = 2,
                Status = GameInfo.StatusCode.Available,
                MaxPlayers = 5,
                MinPlayers = 3,
                Label = "Game #2"
            };

            game3 = new GameInfo()
            {
                GameManagerId = gameManager.ProcessId,
                GameId = 3,
                Status = GameInfo.StatusCode.Available,
                MaxPlayers = 6,
                MinPlayers = 3,
                Label = "Game #3"
            };
            sendGames = new GameInfo[3] { game1, game2, game3 };

            // Assign values to the needed attributes of the conversation factory.
            factory.Games = games;
        }

        private void InitializeJoinGame()
        {
            // Set up game information.
            gameManager = new ProcessInfo()
            {
                ProcessId = 1,
                Type = ProcessInfo.ProcessType.GameManager,
                EndPoint = mockCommunicatorEp
            };

            game = new GameInfo()
            {
                GameId = 1,
                GameManagerId = gameManager.ProcessId,
                Label = "Game #1",
                MaxPlayers = 6,
                MinPlayers = 3,
                Status = GameInfo.StatusCode.Available
            };

            // Initialize factory and set needed attributes.
            factory.Game = game;
        }

        private void InitializeBuyBalloon()
        {
            Penny penny1 = new Penny() { Id = 3 };
            Penny penny2 = new Penny() { Id = 4 };
            Penny penny3 = new Penny() { Id = 5 };

            myPennies = new Queue<Penny>();
            mockBalloonStoreEp = mockCommunicatorEp;

            // Set up need processes and games.
            balloonStore = new ProcessInfo()
            {
                Type = ProcessInfo.ProcessType.BalloonStore,
                EndPoint = mockBalloonStoreEp,
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.JoinedGame,
                Label = "Balloon store #1"
            };
            player = new ProcessInfo()
            {
                Type = ProcessInfo.ProcessType.Player,
                Status = ProcessInfo.StatusCode.JoinedGame,
                Label = "Brian",
                ProcessId = 3,
                EndPoint = new PublicEndPoint()
            };

            // Initialize my pennies.
            myPennies.Enqueue(penny2);
            myPennies.Enqueue(penny3);
            myPennies.Enqueue(penny1);

            // Initialize factory and set needed attributes
            factory.Pennies = myPennies;
            factory.BalloonStoreId = balloonStore.ProcessId;
        }

        private void InitializeFillBalloon()
        {
            Penny penny1 = new Penny() { Id = 3 };
            Penny penny2 = new Penny() { Id = 4 };
            Penny penny3 = new Penny() { Id = 5 };
            Balloon balloon1 = new Balloon() { Id = 1, IsFilled = false };
            Balloon balloon2 = new Balloon() { Id = 2, IsFilled = false };
            Balloon balloon3 = new Balloon() { Id = 3, IsFilled = false };

            myBalloons = new Queue<Balloon>();
            myPennies = new Queue<Penny>();
            mockWaterSupplierEp = mockCommunicatorEp;

            // Set up need processes and games.
            waterSupplier = new ProcessInfo()
            {
                Type = ProcessInfo.ProcessType.WaterServer,
                EndPoint = mockBalloonStoreEp,
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.JoinedGame,
                Label = "Water supplier #1"
            };
            player = new ProcessInfo()
            {
                Type = ProcessInfo.ProcessType.Player,
                Status = ProcessInfo.StatusCode.JoinedGame,
                Label = "Brian",
                ProcessId = 3,
                EndPoint = new PublicEndPoint()
            };

            // Initialize my pennies and balloons.
            myPennies.Enqueue(penny2);
            myPennies.Enqueue(penny3);
            myPennies.Enqueue(penny1);
            myBalloons.Enqueue(balloon1);
            myBalloons.Enqueue(balloon2);
            myBalloons.Enqueue(balloon3);

            factory.Balloons = myBalloons;
            factory.Pennies = myPennies;
            factory.WaterSourceId = waterSupplier.ProcessId;
        }

        private void InitializeThrowBalloon()
        {
            Balloon balloon1 = new Balloon() { Id = 1, IsFilled = true };
            Balloon balloon2 = new Balloon() { Id = 2, IsFilled = true };
            Balloon balloon3 = new Balloon() { Id = 3, IsFilled = true };

            myFilledBalloons = new Queue<Balloon>();
            mockBalloonStoreEp = mockCommunicatorEp;

            // Set up need processes and games.
            balloonStore = new ProcessInfo()
            {
                Type = ProcessInfo.ProcessType.BalloonStore,
                EndPoint = mockBalloonStoreEp,
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.JoinedGame,
                Label = "Balloon store #1"
            };
            player = new ProcessInfo()
            {
                Type = ProcessInfo.ProcessType.Player,
                Status = ProcessInfo.StatusCode.JoinedGame,
                Label = "Brian",
                ProcessId = 3,
                EndPoint = new PublicEndPoint()
            };
            throwAt = new ProcessInfo()
            {
                Type = ProcessInfo.ProcessType.Player,
                Status = ProcessInfo.StatusCode.JoinedGame,
                Label = "Enemy",
                ProcessId = 5,
                EndPoint = new PublicEndPoint() { Host = "127.0.0.1", Port = 3000 }
            };

            // Add filled balloons to my filled balloon array.
            myFilledBalloons.Enqueue(balloon1);
            myFilledBalloons.Enqueue(balloon2);
            myFilledBalloons.Enqueue(balloon3);

            int[] players = new int[1] { throwAt.ProcessId };
            game = new GameInfo() { StartingPlayers = players };
            factory.Game = game;
            MessageNumber.LocalProcessId = player.ProcessId;
            factory.FilledBalloons = myFilledBalloons;
            factory.CurrentProcesses = new GameProcessData[3]
            {
                new GameProcessData()
                { Type = ProcessInfo.ProcessType.BalloonStore, ProcessId = balloonStore.ProcessId },
                new GameProcessData()
                { Type = ProcessInfo.ProcessType.Player, ProcessId = player.ProcessId },
                new GameProcessData()
                { Type = ProcessInfo.ProcessType.Player, ProcessId = throwAt.ProcessId }
            };
        }

        public void InitializeRaiseUmbrella()
        {
            umbrella = new Umbrella() { Id = 1, DigitalSignature = new byte[32] };
            mockUmbrellaSupplierEp = mockCommunicatorEp;
            myUmbrellas.Enqueue(umbrella);

            umbrellaSupplier = new ProcessInfo()
            {
                Type = ProcessInfo.ProcessType.UmbrellaSupplier,
                EndPoint = mockUmbrellaSupplierEp,
                ProcessId = 10,
                Status = ProcessInfo.StatusCode.JoinedGame,
                Label = "Umbrella Supplier"
            };

            factory.Umbrellas = myUmbrellas;
            factory.Game = new GameInfo() { GameManagerId = 1 };
        }

        public void InitializeLeaveGame()
        {
            Penny penny = new Penny();
            Balloon balloon = new Balloon();

            factory.Game = new GameInfo() { Status = GameInfo.StatusCode.InProgress, GameId = 1 };
            factory.CurrentGameId = factory.Game.GameId;
            factory.Pennies = new Queue<Penny>();
            factory.Balloons = new Queue<Balloon>();
            factory.WaterSourceId = 2;
            factory.BalloonStoreId = 3;
            factory.PennyBankId = 4;
            factory.FilledBalloons = new Queue<Balloon>();

            factory.Process = new ProcessInfo()
            {
                Status = ProcessInfo.StatusCode.PlayingGame,
                Label = "Brian",
                ProcessId = 1
            };

            factory.Pennies.Enqueue(penny);
            factory.Balloons.Enqueue(balloon);
        }

        public void InitializeLogout()
        {
            factory.CommProcess.PennyBankEndPoint = new PublicEndPoint();
            factory.CommProcess.ProxyEndPoint = mockProxyEP;
            factory.Game = new GameInfo() { };
            factory.Games = new GameInfo[1] { factory.Game };
            factory.PennyKey = new PublicKey();

            factory.Process = new ProcessInfo()
            {
                Label = "Brian",
                Status = ProcessInfo.StatusCode.Registered,
                ProcessId = 1
            };
        }

        [TestMethod]
        public void TestLoginInitiator()
        {
            // Set up components needed for logging in.
            Initialize();
            InitializeLogin();

            // Initiate login.
            Conversation conv = factory.CreateFromConversationType(typeof(LoginInitiator));
            Assert.IsNotNull(conv);
            conv.Launch();

            // Receive LoginRequest.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that envelope and message are not null and that the message is a login request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(LoginRequest));
            LoginRequest request = envelope.Message as LoginRequest;
            Assert.IsNotNull(request.Identity);
            Assert.AreEqual(identity.FirstName, request.Identity.FirstName);
            Assert.AreEqual(identity.ANumber, request.Identity.ANumber);
            Assert.AreEqual(identity.LastName, request.Identity.LastName);

            // Send back a LoginReply
            LoginReply reply = new LoginReply()
            {
                Success = true,
                ProcessInfo = player,
                Note = "Welcome!",
                PennyBankEndPoint = new PublicEndPoint(),
                PennyBankPublicKey = new PublicKey(),
                ProxyEndPoint = mockProxyEP,
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create()
            };
            byte[] rply = reply.Encode();
            mockCommunicator.Send(new Envelope() { Message = reply, Endpoint = mockCommunicatorEp });
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that the conversation's player information is now the same as the player's information.
            Assert.AreEqual(player.Label, conv.MyProcess.Label);
            Assert.AreEqual(player.ProcessId, conv.MyProcess.ProcessId);
            Assert.AreEqual(player.Status, conv.MyProcess.Status);
            Assert.AreEqual(player.Type, conv.MyProcess.Type);

            // Assert that the login initiator set up the proxy and registry endpoints correctly.
            Assert.AreEqual(mockProxyEP, conv.CommProcess.ProxyEndPoint);
            Assert.AreEqual(mockCommunicatorEp, conv.CommProcess.RegistryEndPoint);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestGetGameListInitiator()
        {
            // Set up needed components for the game list initiator to use.
            Initialize();
            InitializeGetGameList();

            // Initiate game list conversation.
            Conversation conv = factory.CreateFromConversationType(typeof(GetGameListInitiator));
            Assert.IsNotNull(conv);
            conv.Launch();

            // Receive game list request.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that we received something and that it was a game list request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(GameListRequest));
            GameListRequest request = envelope.Message as GameListRequest;
            Assert.AreEqual((int)GameInfo.StatusCode.Available, request.StatusFilter);

            // Send back a GameListReply
            GameListReply reply = new GameListReply()
            {
                Success = true,
                GameInfo = sendGames,
                Note = "Available games.",
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create()
            };
            byte[] rply = reply.Encode();
            mockCommunicator.Send(new Envelope() { Message = reply, Endpoint = mockCommunicatorEp });
            dispatcher.Start();
            Thread.Sleep(1000);

            // Assert that the game list information is correct.
            Assert.AreEqual(games.Length, conv.Games.Length);
            Assert.AreEqual(game1.GameId, conv.Games[0].GameId);
            Assert.AreEqual(game2.GameId, conv.Games[1].GameId);
            Assert.AreEqual(game3.GameId, conv.Games[2].GameId);
            Assert.AreEqual(game1.Label, conv.Games[0].Label);
            Assert.AreEqual(game2.Label, conv.Games[1].Label);
            Assert.AreEqual(game3.Label, conv.Games[2].Label);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestJoinGameInitiator()
        {
            // Set up needed components for the join game initiator to use.
            Initialize();
            InitializeJoinGame();

            // Initiate join game.
            Conversation conv = factory.CreateFromConversationType(typeof(JoinGameInitiator));
            Assert.IsNotNull(conv);
            conv.Launch();

            // Have proxy receive join game request.
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = null;
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, gameManager.EndPoint.IPEndPoint);

            // Receive join game request from the proxy.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that the envelope and message are not null and the message is a join game request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing route = envelope.Message as Routing;
            Assert.IsInstanceOfType(route.InnerMessage, typeof(JoinGameRequest));
            JoinGameRequest request = route.InnerMessage as JoinGameRequest;
            Assert.AreEqual(player.ProcessId, request.Process.ProcessId);
            Assert.AreEqual(player.Type, request.Process.Type);
            Assert.AreEqual(game.GameId, request.GameId);

            // Send back join game reply through the proxy.
            JoinGameReply reply = new JoinGameReply()
            {
                Success = true,
                GameId = game.GameId,
                InitialLifePoints = 10,
                Note = "Available games.",
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create()
            };
            byte[] rply = reply.Encode();

            // Send join game reply to the proxy.
            Routing response = new Routing() { InnerMessage = reply };
            envelope = new Envelope() { Message = response, Endpoint = mockProxyEP };
            mockCommunicator.Send(envelope);
            bytes = null;

            // Forward message to the initiator of the join game request.
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, mockCommunicatorEp.IPEndPoint);
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that the conversations game information is the same as the game set up.
            Assert.AreEqual(game.GameId, conv.CurrentGameId);
            Assert.AreEqual(game.Status, conv.GameStatus);
            Assert.AreEqual(reply.InitialLifePoints, conv.LifePoints);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestBuyBalloonInitiator()
        {
            // Set up needed components for the buy balloon initiator to use.
            Initialize();
            InitializeBuyBalloon();

            // Initiate buy balloon.
            Conversation conv = factory.CreateFromConversationType(typeof(BuyBalloonInitiator));
            Assert.IsNotNull(conv);
            Assert.IsNotNull(conv.Pennies);
            Assert.IsTrue(conv.Pennies.Count > 0);
            conv.Launch();

            // Have proxy receive buy balloon request.
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = null;
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, mockBalloonStoreEp.IPEndPoint);

            // Receive buy balloon request from the proxy.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that the envelope and message are not null and the message is a buy balloon request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing route = envelope.Message as Routing;
            Assert.IsInstanceOfType(route.InnerMessage, typeof(BuyBalloonRequest));
            BuyBalloonRequest request = route.InnerMessage as BuyBalloonRequest;
            Assert.IsNotNull(request.Penny);
            Assert.AreEqual(4, request.Penny.Id);

            // Send back balloon reply through the proxy.
            BalloonReply reply = new BalloonReply()
            {
                Success = true,
                Note = "Balloon",
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create(),
                Balloon = new Balloon() { IsFilled = false, Id = 1 }
            };
            byte[] rply = reply.Encode();

            envelope = new Envelope() { Message = reply, Endpoint = mockProxyEP };
            mockCommunicator.Send(envelope);
            bytes = null;

            // Forward message to the initiator of the buy balloon request.
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, mockCommunicatorEp.IPEndPoint);
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that the conversations balloon informamtion is correct.
            Assert.IsTrue(conv.Balloons.Count > 0);
            Balloon balloon = conv.Balloons.Dequeue();
            Assert.IsFalse(balloon.IsFilled);
            Assert.AreEqual(reply.Balloon.Id, balloon.Id);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestFillBalloonInitiator()
        {
            // Set up needed components for the fill balloon initiator to use.
            Initialize();
            InitializeFillBalloon();

            // Initiate fill balloon.
            Conversation conv = factory.CreateFromConversationType(typeof(FillBalloonInitiator));
            Assert.IsNotNull(conv);
            Assert.IsNotNull(conv.Pennies);
            Assert.IsNotNull(conv.Balloons);
            Assert.IsTrue(conv.Pennies.Count > 0);
            Assert.IsTrue(conv.Balloons.Count > 0);
            conv.Launch();

            // Have proxy receive fill balloon request.
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = null;
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, mockWaterSupplierEp.IPEndPoint);

            // Receive fill balloon request from the proxy.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that the envelope and message are not null and the message is a fill balloon request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing route = envelope.Message as Routing;
            Assert.IsInstanceOfType(route.InnerMessage, typeof(FillBalloonRequest));
            FillBalloonRequest request = route.InnerMessage as FillBalloonRequest;
            Assert.IsNotNull(request.Balloon);
            Assert.AreEqual(2, request.Pennies.Length);
            Assert.AreEqual(4, request.Pennies[0].Id);
            Assert.AreEqual(5, request.Pennies[1].Id);

            // Send back balloon reply through the proxy.
            BalloonReply reply = new BalloonReply()
            {
                Success = true,
                Note = "Filled Balloon",
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create(),
                Balloon = request.Balloon
            };
            reply.Balloon.IsFilled = true;
            byte[] rply = reply.Encode();

            envelope = new Envelope() { Message = reply, Endpoint = mockProxyEP };
            mockCommunicator.Send(envelope);
            bytes = null;

            // Forward message to the initiator of the buy balloon request.
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, mockCommunicatorEp.IPEndPoint);
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that the conversations balloon information is correct and that the balloon is filled.
            Assert.IsTrue(conv.FilledBalloons.Count > 0);
            Balloon balloon = conv.FilledBalloons.Dequeue();
            Assert.IsTrue(balloon.IsFilled);
            Assert.AreEqual(reply.Balloon.Id, balloon.Id);
        }

        [TestMethod]
        public void TestThrowBalloonInitiator()
        {
            // Set up needed components for the throw balloon initiator to use.
            Initialize();
            InitializeJoinGame();
            InitializeThrowBalloon();

            // Initiate throw balloon.
            Conversation conv = factory.CreateFromConversationType(typeof(ThrowBalloonInitiator));
            Assert.IsNotNull(conv);
            conv.Launch();

            // Have proxy receive throw balloon request.
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = null;
            mockProxyClient.Client.ReceiveTimeout = 1500;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, gameManager.EndPoint.IPEndPoint);

            // Receive throw balloon request from the proxy.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that the envelope and message are not null and the message is a throw balloon request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing route = envelope.Message as Routing;
            Assert.IsInstanceOfType(route.InnerMessage, typeof(ThrowBalloonRequest));
            ThrowBalloonRequest request = route.InnerMessage as ThrowBalloonRequest;
            Assert.IsNotNull(request.Balloon);
            Assert.IsTrue(request.Balloon.IsFilled);
            Assert.AreEqual(5, request.TargetPlayerId);
            Assert.AreEqual(myFilledBalloons.Count, conv.FilledBalloons.Count);

            // Send back reply through the proxy.
            Reply reply = new Reply()
            {
                Success = true,
                Note = "Hit!",
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create(),
            };
            byte[] rply = reply.Encode();

            envelope = new Envelope() { Message = reply, Endpoint = mockProxyEP };
            mockCommunicator.Send(envelope);
            dispatcher.Start();
            Thread.Sleep(100);

            Assert.AreEqual(myFilledBalloons.Count, conv.FilledBalloons.Count);
        }

        [TestMethod]
        public void TestLogoutInitiator()
        {
            // Set up needed components for the logout initiator to use.
            Initialize();
            InitializeLogout();

            // Initiate logout.
            Conversation conv = factory.CreateFromConversationType(typeof(LogoutInitiator));
            Assert.IsNotNull(conv);
            conv.Launch();

            // Receive LogoutRequest.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that envelope and message are not null and that the message is a logout request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(LogoutRequest));
            LogoutRequest request = envelope.Message as LogoutRequest;

            // Assert that player is known and game and games are not null
            Assert.IsNotNull(conv.Game);
            Assert.IsNotNull(conv.Games);
            Assert.AreEqual(ProcessInfo.StatusCode.Registered, conv.MyProcess.Status);

            // Send back a reply to the logout initiator.
            Reply reply = new Reply()
            {
                Success = true,
                Note = "Goodbye",
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create()
            };
            byte[] rply = reply.Encode();
            mockCommunicator.Send(new Envelope() { Message = reply, Endpoint = mockCommunicatorEp });
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that process logged out correctly.
            Assert.IsNull(conv.Game);
            Assert.IsNull(conv.Games);
            Assert.IsNull(conv.PennyKey);
            Assert.IsNull(conv.CommProcess.ProxyEndPoint);
            Assert.IsNull(conv.CommProcess.PennyBankEndPoint);
            Assert.AreEqual(ProcessInfo.StatusCode.Unknown, conv.MyProcess.Status);
        }

        [TestMethod]
        public void TestLeaveGameInitiator()
        {
            // Set up needed components for the leave game initiator to use.
            Initialize();
            InitializeLeaveGame();

            // Initiate leave game.
            Conversation conv = factory.CreateFromConversationType(typeof(LeaveGameInitiator));
            Assert.IsNotNull(conv);
            conv.Launch();

            // Have proxy receive leave game request.
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = null;
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, mockCommunicatorEp.IPEndPoint);

            // Receive leave game request from the proxy.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that envelope and message are not null and that the message is a leave game request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing route = envelope.Message as Routing;
            Assert.IsInstanceOfType(route.InnerMessage, typeof(LeaveGameRequest));
            LeaveGameRequest request = route.InnerMessage as LeaveGameRequest;

            // Assert that game information is correct before discarding it.
            Assert.IsNotNull(conv.Game);
            Assert.IsNotNull(conv.Balloons);
            Assert.IsNotNull(conv.Pennies);
            Assert.IsNotNull(conv.FilledBalloons);
            Assert.AreNotEqual(0, conv.CurrentGameId);
            Assert.AreNotEqual(0, conv.PennyBankId);
            Assert.AreNotEqual(0, conv.WaterSourceId);
            Assert.AreNotEqual(0, conv.BalloonStoreId);
            Assert.AreEqual(ProcessInfo.StatusCode.PlayingGame, conv.MyProcess.Status);

            // Send back a reply to the leave game initiator.
            Reply reply = new Reply()
            {
                Success = true,
                Note = "Goodbye.",
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create()
            };
            byte[] rply = reply.Encode();

            envelope = new Envelope() { Message = reply, Endpoint = mockProxyEP };
            mockCommunicator.Send(envelope);
            bytes = null;

            // Forward message to the initiator of the leave game request.
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, mockCommunicatorEp.IPEndPoint);
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that all game information is discarded correctly.
            Assert.IsNull(conv.Game);
            Assert.IsNull(conv.Balloons);
            Assert.IsNull(conv.FilledBalloons);
            Assert.AreEqual(0, conv.CurrentGameId);
            Assert.AreEqual(0, conv.WaterSourceId);
            Assert.AreEqual(0, conv.BalloonStoreId);
            Assert.AreEqual(ProcessInfo.StatusCode.LeavingGame, conv.MyProcess.Status);
        }

        [TestMethod]
        public void TestRaiseUmbrellaInitiator()
        {
            // Set up components needed for raising an umbrella.
            Initialize();
            InitializeRaiseUmbrella();

            // Initiate raise umbrella.
            Conversation conv = factory.CreateFromConversationType(typeof(RaiseUmbrellaInitiator));
            Assert.IsNotNull(conv);
            conv.Launch();

            // Have proxy receive raise umbrella request.
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = null;
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, mockCommunicatorEp.IPEndPoint);

            // Receive raise umbrella request from the proxy.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that envelope and message are not null and that the message is an umbrella request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing route = envelope.Message as Routing;
            Assert.IsInstanceOfType(route.InnerMessage, typeof(RaiseUmbrellaRequest));
            RaiseUmbrellaRequest request = route.InnerMessage as RaiseUmbrellaRequest;

            // Assert that umbrella request contains correct information.
            Assert.IsNotNull(conv.Umbrellas);
            Assert.AreEqual(umbrella.Id, request.Umbrella.Id);
            Assert.IsNotNull(request.Umbrella.DigitalSignature);

            // Send back a reply to the raise umbrella initiator.
            Reply reply = new Reply()
            {
                Success = true,
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create()
            };
            byte[] rply = reply.Encode();

            envelope = new Envelope() { Message = reply, Endpoint = mockProxyEP };
            mockCommunicator.Send(envelope);
            bytes = null;

            // Forward message to the initiator of the raise umbrella request.
            mockProxyClient.Client.ReceiveTimeout = 1000;
            bytes = mockProxyClient.Receive(ref endpoint);
            Assert.IsNotNull(bytes);
            mockProxyClient.Send(bytes, bytes.Length, mockCommunicatorEp.IPEndPoint);
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that umbrella initiator worked correctly.
            Assert.AreEqual(myUmbrellas.Count, conv.Umbrellas.Count);
        }

        [TestMethod]
        public void TestGetPublicKeyInitiator()
        {
            // Set up components needed for getting a public key.
            Initialize();

            // Initiate get public key.
            Conversation conv = factory.CreateFromConversationType(typeof(GetPublicKeyInitiator));
            GetPublicKeyInitiator.Id = 1;
            Assert.IsNotNull(conv);
            conv.Launch();

            // Receive GetKeyRequest.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that envelope and message are not null and that the message is a get key request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(GetKeyRequest));
            GetKeyRequest request = envelope.Message as GetKeyRequest;

            // Assert that key request has correct information.
            Assert.IsNotNull(request.ConvId);
            Assert.IsNotNull(request.MsgId);
            Assert.IsTrue(request.ProcessId > 0);

            // Send back a reply to the get key initiator.
            PublicKeyReply reply = new PublicKeyReply()
            {
                Success = true,
                ProcessId = request.ProcessId,
                Key = new PublicKey() { Exponent = new byte[3], Modulus = new byte[10] },
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create()
            };
            byte[] rply = reply.Encode();
            mockCommunicator.Send(new Envelope() { Message = reply, Endpoint = mockCommunicatorEp });
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that reply was received successfully and information is correct.
            Assert.AreEqual(reply.ConvId, request.ConvId);
            Assert.IsTrue(reply.Success);
            Assert.AreEqual(reply.ProcessId, GetPublicKeyInitiator.Id);
            Assert.AreEqual(reply.Key.Exponent.Length, GetPublicKeyInitiator.Key.Exponent.Length);
            Assert.AreEqual(reply.Key.Modulus.Length, GetPublicKeyInitiator.Key.Modulus.Length);
        }

        [TestMethod]
        public void TestNextIdInitiator()
        {
            // Set up components needed for getting a next id.
            Initialize();

            // Initiate next id.
            Conversation conv = factory.CreateFromConversationType(typeof(NextIdInitiator));
            conv.NumIds = 3;
            Assert.IsNotNull(conv);
            conv.Launch();

            // Receive NextIdRequest.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that envelope and message are not null and that the message is a next id request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.ActualMessage, typeof(NextIdRequest));
            NextIdRequest request = envelope.ActualMessage as NextIdRequest;

            // Assert that next id request has correct information.
            Assert.IsNotNull(request.ConvId);
            Assert.IsNotNull(request.MsgId);
            Assert.IsTrue(request.NumberOfIds > 0);
            Assert.AreEqual(3, request.NumberOfIds);

            // Send back a reply to the get key initiator.
            NextIdReply reply = new NextIdReply()
            {
                Success = true,
                NumberOfIds = factory.NumIds,
                NextId = 10,
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create()
            };
            byte[] rply = reply.Encode();
            mockCommunicator.Send(new Envelope() { Message = reply, Endpoint = mockCommunicatorEp });
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that reply was received successfully and information is correct.
            Assert.AreEqual(reply.ConvId, request.ConvId);
            Assert.IsTrue(reply.Success);
            Assert.AreEqual(reply.NumberOfIds, conv.NumIds);
            Assert.AreEqual(reply.NextId, conv.NextId);
        }

        [TestMethod]
        public void TestPennyValidationInitiator()
        {
            // Set up components needed for validating pennies.
            Initialize();

            // Initiate next id.
            Conversation conv = factory.CreateFromConversationType(typeof(PennyValidationInitiator));
            conv.PennyToValidate = new Penny { Id = 1, SignedBy = 4 };
            Assert.IsNotNull(conv);
            conv.Launch();

            // Receive PennyValidation request.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);

            // Assert that envelope and message are not null and that the message is a penny validation request.
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.ActualMessage, typeof(PennyValidation));
            PennyValidation request = envelope.ActualMessage as PennyValidation;

            // Assert that penny validation request has correct information.
            Assert.IsNotNull(request.ConvId);
            Assert.IsNotNull(request.MsgId);
            Assert.AreEqual(1, request.Pennies[0].Id);
            Assert.AreEqual(4, request.Pennies[0].SignedBy);

            // Send back a reply to the penny validation initiator.
            Reply reply = new Reply()
            {
                Success = true,
                ConvId = request.ConvId,
                MsgId = MessageNumber.Create()
            };
            byte[] rply = reply.Encode();
            mockCommunicator.Send(new Envelope() { Message = reply, Endpoint = mockCommunicatorEp });
            dispatcher.Start();
            Thread.Sleep(100);

            // Assert that reply was received successfully and information is correct.
            Assert.AreEqual(reply.ConvId, request.ConvId);
            Assert.IsTrue(reply.Success);
            Assert.IsTrue(PennyValidationInitiator.ValidPenny);
        }
    }
}
