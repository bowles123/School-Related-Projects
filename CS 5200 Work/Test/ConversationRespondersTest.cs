using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationSubsystem;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;
using System.Net.Sockets;
using System.Net;
using PlayerProcess;
using Messages;
using System.Collections.Generic;
using System.Threading;
using BalloonStoreProcess;
using System.Security.Cryptography;
using CommunicationSubsystem.Conversations.Responders;

namespace CommunicationSubsystemTesting
{ 
    [TestClass]
    public class ConversationRespondersTest
    {
        private ConversationFactory factory;
        private ConversationDictionary dictionary;
        private Communicator mockCommunicator;
        private int mockCommunicatorPort;
        private PublicEndPoint mockCommunicatorEp;
        private ProcessInfo player;
        private UdpClient mockProxyClient;
        private int mockProxyClientPort;
        private PublicEndPoint mockProxyEP;
        private Dispatcher dispatcher;
        private CommunicationProcess myProcess;
        private GameInfo game;
        private TcpClient mockTCP;
        private UdpClient mockPennyBankClient;
        private int mockPennyBankClientPort;
        private PublicEndPoint mockPennBankEndPoint;
        private TcpListener mockTCPListener;
        private NetworkStream stream;
        private Communicator mockPennyBankCommunicator;
        private Penny penny;
        protected RSACryptoServiceProvider rsa;
        protected RSAPKCS1SignatureFormatter rsaSigner;
        private SHA1Managed hasher;

        private void Initialize()
        {
            // Initialize needed components of responders.
            factory = new PlayerConversationFactory();
            dictionary = new ConversationDictionary();
            mockCommunicator = new Communicator() { MySocket = new UdpClient(0) };
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            player = new ProcessInfo() { ProcessId = 10 };

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

            // Set up communication process.
            myProcess = new Player()
            {
                RegistryEndPoint = mockCommunicatorEp,
                ProxyEndPoint = mockCommunicatorEp
            };

            // Assign values to the needed attributes of the conversation factory.
            factory.Initialize();
            factory.InitializeTypes();
            factory.Communicator = mockCommunicator;
            factory.Dictionary = dictionary;
            factory.ProcessType = ProcessInfo.ProcessType.Player;
            factory.Process = player;
            factory.CommProcess = myProcess;
        }

        private void InitializeForBalloonStore()
        {
            // Initialize needed components of responders.
            factory = new BalloonStoreConversationFactory();
            dictionary = new ConversationDictionary();
            mockCommunicator = new Communicator() { MySocket = new UdpClient(0) };
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            player = new ProcessInfo();

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

            // Set up communication process.
            myProcess = new BalloonStore()
            {
                RegistryEndPoint = mockCommunicatorEp,
                ProxyEndPoint = mockCommunicatorEp,
                PennyBankEndPoint = mockCommunicatorEp,
            };

            // Assign values to the needed attributes of the conversation factory.
            factory.Initialize();
            factory.InitializeTypes();
            factory.Communicator = mockCommunicator;
            factory.Dictionary = dictionary;
            factory.ProcessType = ProcessInfo.ProcessType.Player;
            factory.Process = player;
            factory.CommProcess = myProcess;
        }

        private void InitializeGameInformation()
        {
            GameProcessData[] games = new GameProcessData[1];
            games[0] = new GameProcessData()
            {
                ProcessId = player.ProcessId,
                Type = player.Type,
                HasUmbrellaRaised = false,
                HitPoints = 0,
                LifePoints = 0
            };

            game = new GameInfo()
            {
                GameId = 1,
                Label = "Game # 1",
                GameManagerId = 2,
                MaxPlayers = 6,
                MinPlayers = 2,
                Status = GameInfo.StatusCode.Initializing
            };

            factory.Game = new GameInfo();
        }

        private void InitializeAllowanceDistribution()
        {
            // Set up the mock penny bank. 
            mockPennyBankClient = new UdpClient(0);
            mockPennyBankClientPort = ((IPEndPoint)mockPennyBankClient.Client.LocalEndPoint).Port;
            mockPennBankEndPoint = new PublicEndPoint() { Host = "127.0.0.1", Port = mockPennyBankClientPort };
            mockPennyBankCommunicator = new Communicator() { MySocket = mockPennyBankClient };

            factory.Pennies = new Queue<Penny>();
            factory.CommProcess.PennyBankEndPoint = mockPennBankEndPoint;
        }
        
        private void InitializeBalloonResponder()
        {
            Queue<Balloon> balloons = new Queue<Balloon>();
            RSAParameters parameters;
            RSAParameters pennyParameters = new RSAParameters();
            penny = new Penny();
            rsa = new RSACryptoServiceProvider();
            rsaSigner = new RSAPKCS1SignatureFormatter(rsa);
            rsaSigner.SetHashAlgorithm("SHA1");
            hasher = new SHA1Managed();
            parameters = rsa.ExportParameters(false);

            balloons.Enqueue(new Balloon() { Id = 2, SignedBy = 1, IsFilled = false });
            balloons.Enqueue(new Balloon() { Id = 3, SignedBy = 1, IsFilled = false });
            balloons.Enqueue(new Balloon() { Id = 4, SignedBy = 1, IsFilled = false });
            factory.Balloons = balloons;

            byte[] bytes = penny.DataBytes();
            byte[] hash = hasher.ComputeHash(bytes);
            penny.DigitalSignature = rsaSigner.CreateSignature(hash);
            factory.PennyKey = new PublicKey()
            {
                Exponent = parameters.Exponent,
                Modulus = parameters.Modulus
            };

            pennyParameters.Modulus = factory.PennyKey.Modulus;
            pennyParameters.Exponent = factory.PennyKey.Exponent;
            factory.Game = new GameInfo()
            { GameId = 12, MinPlayers = 2, MaxPlayers = 4, Status = GameInfo.StatusCode.InProgress };
            factory.PennyRSA = new RSACryptoServiceProvider();
            factory.PennyRSA.ImportParameters(pennyParameters);
            factory.CurrentProcesses = new GameProcessData[5] { new GameProcessData()
            { ProcessId =  player.ProcessId, Type = ProcessInfo.ProcessType.Player },
            new GameProcessData() { ProcessId = 1, Type = ProcessInfo.ProcessType.GameManager },
            new GameProcessData() { ProcessId = 2, Type = ProcessInfo.ProcessType.BalloonStore },
            new GameProcessData() { ProcessId = 3, Type = ProcessInfo.ProcessType.UmbrellaSupplier },
            new GameProcessData() { ProcessId = 4, Type = ProcessInfo.ProcessType.WaterServer } };
        }

        [TestMethod]
        public void TestAliveResponder()
        {
            // Initialize components needed for the alive responsder.
            Initialize();

            // Set up alive request and send it. 
            AliveRequest request = new AliveRequest();
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Recieve reply back from the sender.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Reply));

            // Assert that the response was successfull and that the note says I'm Alive!!
            Reply response = envelope.Message as Reply;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.AreEqual("I'm Alive!!", response.Note);
        }

        [TestMethod]
        public void TestGameStatusResponder()
        {
            // Initialize components needed for the game status responder.
            Initialize();
            InitializeGameInformation();

            // Set up game status notification and send it. 
            GameStatusNotification alive = new GameStatusNotification() { Game = game };
            alive.InitMessageAndConversationNumbers();
            Envelope send = new Envelope()
            { Message = new Routing() { InnerMessage = alive }, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);
            Thread.Sleep(1000);

            // Assert that the conversation updated information properly
            Assert.AreEqual(game.GameId, dispatcher.Factory.CurrentGameId);
            Assert.AreEqual(game.CurrentProcesses.Length, dispatcher.Factory.CurrentProcesses.Length);
            Assert.AreEqual(game.Label, dispatcher.Factory.Game.Label);
            Assert.AreEqual(game.Status, dispatcher.Factory.GameStatus);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestHitResponder()
        {
            // Initialize components needed for the hit responsder.
            Initialize();

            // Set up hit request and send it. 
            HitNotification request = new HitNotification() { ByPlayerId = 2 };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Recieve reply back from the sender.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was successfull and that the note says Owww!!
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(Reply));
            Reply response = reply.InnerMessage as Reply;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.AreEqual("Owww!", response.Note);
        }

        [TestMethod]
        public void TestShutdownResponder()
        {
            // Initialize components needed for the shutdown responsder.
            Initialize();

            // Set up shutdown request and send it. 
            ShutdownRequest request = new ShutdownRequest();
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);
            Thread.Sleep(1000);

            // Assert that the process was shutdown correctly.
            Assert.AreEqual(ProcessInfo.StatusCode.Terminating, dispatcher.Factory.Process.Status);
            Assert.IsFalse(dispatcher.Factory.Alive);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestReadyResponder()
        {
            // Initialize components needed for the ready responsder.
            Initialize();
            InitializeGameInformation();

            // Set up start request and run it. 
            ReadyToStart request = new ReadyToStart();
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Recieve reply back from the sender.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was successfull and that the note says Ready!
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(Reply));
            Reply response = reply.InnerMessage as Reply;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.AreEqual("Ready!", response.Note);

            // Send start game reply.
            StartGame start = new StartGame() { Success = true, Note = "Starting Game." };
            start.InitMessageAndConversationNumbers();
            send = new Envelope() { Message = start, Endpoint = mockCommunicatorEp };
            mockCommunicator.Send(send);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestAllowanceDistributionResponder()
        {
            // Initialize components needed for the allowance distribution responder.
            Initialize();
            InitializeAllowanceDistribution();

            // Set up allowance delivery request and send it. 
            AllowanceDeliveryRequest request = new AllowanceDeliveryRequest()
            { NumberOfPennies = 1,  PortNumber = mockPennyBankClientPort };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockPennyBankCommunicator.Send(send);

            // Set up the TCP listener.
            mockTCPListener = new TcpListener(mockPennBankEndPoint.IPEndPoint.Address, 
                mockPennyBankClientPort);
            mockTCPListener.Start();
            mockTCP = mockTCPListener.AcceptTcpClient();
            stream = mockTCP.GetStream();
            NetworkStreamExtensions.WriteStreamMessage(stream, new Penny() { Id = 1 });

            // Recieve reply back from the sender.
            Envelope envelope = null;
            envelope = mockPennyBankCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Reply));

            // Assert that a reply was sent back and the pennies were successfully sent.
            Reply reply = envelope.Message as Reply;
            Assert.IsTrue(reply.Success);
            Assert.AreEqual(request.ConvId, reply.ConvId);
            Assert.IsTrue(dispatcher.Factory.Pennies.Count > 0);
            Assert.AreEqual(request.NumberOfPennies, dispatcher.Factory.Pennies.Count);
            Assert.IsNotNull(dispatcher.Factory.Pennies.Dequeue());
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestValidPennyBuyBalloonResponder()
        {
            // Initialize components needed for balloon responder.
            InitializeForBalloonStore();
            InitializeBalloonResponder();

            // Set up buy balloon request and send it. 
            BuyBalloonRequest request = new BuyBalloonRequest() { Penny = penny };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            Envelope validationReply = null;
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Recieve penny validation.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(5000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.ActualMessage, typeof(PennyValidation));
            PennyValidation validationRequest = envelope.ActualMessage as PennyValidation;
            Assert.AreEqual(penny.Id, validationRequest.Pennies[0].Id);

            // Send back penny validation.
            validationReply = new Envelope()
            {
                Message = new Reply() { Success = true, ConvId = validationRequest.ConvId },
                Endpoint = mockCommunicatorEp
            };
            mockCommunicator.Send(validationReply);

            // Receive reply back from the penny bank.
            envelope = null;
            envelope = mockCommunicator.Retrieve(5000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was successfull and that the balloon is not null.
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(BalloonReply));
            BalloonReply response = reply.InnerMessage as BalloonReply;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.IsNotNull(response.Balloon);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestInvalidPennySignatureBuyBalloonResponder()
        {
            // Initialize components needed for balloon responder.
            InitializeForBalloonStore();
            InitializeBalloonResponder();
            penny.DigitalSignature = new byte[2];

            // Set up buy balloon request and send it. 
            BuyBalloonRequest request = new BuyBalloonRequest() { Penny = penny };
            MessageNumber.LocalProcessId = player.ProcessId;
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Receive reply back from the penny bank.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(5000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was unsuccessfull because of an invalid signature
            // and that the balloon is null.
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(BalloonReply));
            BalloonReply response = reply.InnerMessage as BalloonReply;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("The penny's signature is not valid.", response.Note);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.IsNull(response.Balloon);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestInvalidPennyBuyBalloonResponder()
        {
            // Initialize components needed for balloon responder.
            InitializeForBalloonStore();
            InitializeBalloonResponder();

            // Set up buy balloon request and send it. 
            BuyBalloonRequest request = new BuyBalloonRequest() { Penny = penny };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            Envelope validationReply = null;
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Recieve penny validation.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(5000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.ActualMessage, typeof(PennyValidation));
            PennyValidation validationRequest = envelope.ActualMessage as PennyValidation;
            Assert.AreEqual(penny.Id, validationRequest.Pennies[0].Id);

            // Send back penny validation.
            validationReply = new Envelope()
            {
                Message = new Reply() { Success = false, ConvId = validationRequest.ConvId },
                Endpoint = mockCommunicatorEp
            };
            mockCommunicator.Send(validationReply);

            // Receive reply back from the penny bank.
            envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was unsuccessfull because the penny didn't come from the penny bank
            // and that the balloon is null.
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(BalloonReply));
            BalloonReply response = reply.InnerMessage as BalloonReply;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("Penny is not a valid penny from the penny bank.", response.Note);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.IsNull(response.Balloon);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestUsedPennyBuyBalloonResponder()
        {
            // Initialize components needed for balloon responder.
            InitializeForBalloonStore();
            InitializeBalloonResponder();

            // Set up buy balloon request and send it. 
            BuyBalloonRequest request = new BuyBalloonRequest() { Penny = penny };
            MessageNumber.LocalProcessId = player.ProcessId;
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Receive reply back from the penny bank.
            Envelope envelope = null;
            BuyBalloonResponder.usedPennies = new List<int>();
            BuyBalloonResponder.usedPennies.Add(penny.Id);
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was unsuccessfull because of an invalid signature
            // and that the balloon is null.
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(BalloonReply));
            BalloonReply response = reply.InnerMessage as BalloonReply;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("The penny has already been used.", response.Note);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.IsNull(response.Balloon);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestInvalidProcessBuyBalloonResponder()
        {
            // Initialize components needed for balloon responder.
            InitializeForBalloonStore();
            InitializeBalloonResponder();

            // Set up buy balloon request and send it. 
            BuyBalloonRequest request = new BuyBalloonRequest() { Penny = penny };
            MessageNumber.LocalProcessId = 27;
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Receive reply back from the penny bank.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was unsuccessfull because of an invalid signature
            // and that the balloon is null.
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(BalloonReply));
            BalloonReply response = reply.InnerMessage as BalloonReply;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("You are not part of this game.", response.Note);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.IsNull(response.Balloon);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestNoBalloonsBuyBalloonResponder()
        {
            // Initialize components needed for balloon responder.
            InitializeForBalloonStore();
            factory.Balloons = new Queue<Balloon>();
            factory.Game = new GameInfo() { Status = GameInfo.StatusCode.InProgress };

            // Set up buy balloon request and send it. 
            BuyBalloonRequest request = new BuyBalloonRequest() { Penny = penny };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Receive reply back from the penny bank.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was unsuccessfull because of an invalid signature
            // and that the balloon is null.
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(BalloonReply));
            BalloonReply response = reply.InnerMessage as BalloonReply;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("No more balloons remaining the the inventory.", response.Note);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.IsNull(response.Balloon);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestNotInProgressBuyBalloonResponder()
        {
            // Initialize components needed for balloon responder.
            InitializeForBalloonStore();
            InitializeBalloonResponder();
            factory.Balloons = new Queue<Balloon>();
            factory.Game.Status = GameInfo.StatusCode.Cancelled;

            // Set up buy balloon request and send it. 
            BuyBalloonRequest request = new BuyBalloonRequest() { Penny = penny };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Receive reply back from the penny bank.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was unsuccessfull because the game is not in progress.
            // and that the balloon is null.
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(BalloonReply));
            BalloonReply response = reply.InnerMessage as BalloonReply;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("Cannot buy a balloon unless the game is in progress.", response.Note);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.IsNull(response.Balloon);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestWinAuctionResponder()
        {
            // Initialize components needed for auction responder.
            Initialize();

            // Set up auction announcement and send it. 
            AuctionAnnouncement request = new AuctionAnnouncement() { MinimumBid = 3 };
            Queue<Penny> pennies = new Queue<Penny>();
            BidAck acknowledge = new BidAck() { Success = true, Won = true,
                Umbrella = new Umbrella() { Id = 1, SignedBy = 2} };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };

            // Set up pennies and start up send request.
            pennies.Enqueue(new Penny() { Id = 1 });
            pennies.Enqueue(new Penny() { Id = 2 });
            pennies.Enqueue(new Penny() { Id = 3 });
            pennies.Enqueue(new Penny() { Id = 4 });
            pennies.Enqueue(new Penny() { Id = 5 });
            factory.Pennies = pennies;
            factory.Umbrellas = new Queue<Umbrella>();
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Recieve reply back from the sender.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was successful..
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(Bid));
            Bid response = reply.InnerMessage as Bid;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.AreEqual(response.Pennies.Length, request.MinimumBid);

            // Send bid acknowledge over.
            send = new Envelope()
            {
                Message = new Routing()
                { InnerMessage = acknowledge, ToProcessIds = new int[1] { 3 } },
                Endpoint = mockCommunicatorEp
            };
            acknowledge.SetMessageAndConversationNumbers(MessageNumber.Create(), reply.ConvId);
            mockCommunicator.Send(send);
            Thread.Sleep(1000);

            // Assert that the umbrella information and the number of pennies are correct.
            Assert.AreEqual(2, dispatcher.Factory.Pennies.Count);
            Assert.IsNotNull(dispatcher.Factory.Umbrellas);
            Assert.AreEqual(1, dispatcher.Factory.Umbrellas.Count);
            Umbrella umbrella = dispatcher.Factory.Umbrellas.Dequeue();
            Assert.AreEqual(umbrella.Id, acknowledge.Umbrella.Id);
            Assert.AreEqual(umbrella.SignedBy, acknowledge.Umbrella.SignedBy);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestLoseAuctionResponder()
        {
            // Initialize components needed for auction responder.
            Initialize();

            // Set up auction announcement and send it. 
            AuctionAnnouncement request = new AuctionAnnouncement() { MinimumBid = 3 };
            Queue<Penny> pennies = new Queue<Penny>();
            BidAck acknowledge = new BidAck()
            { Success = true, Won = false, Umbrella = new Umbrella() { Id = 1, SignedBy = 2 } };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };

            // Set up pennies and start up send request.
            pennies.Enqueue(new Penny() { Id = 1 });
            pennies.Enqueue(new Penny() { Id = 2 });
            pennies.Enqueue(new Penny() { Id = 3 });
            pennies.Enqueue(new Penny() { Id = 4 });
            pennies.Enqueue(new Penny() { Id = 5 });
            factory.Pennies = pennies;
            factory.Umbrellas = new Queue<Umbrella>();
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Recieve reply back from the sender.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was successful..
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(Bid));
            Bid response = reply.InnerMessage as Bid;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.AreEqual(response.Pennies.Length, request.MinimumBid);

            // Send bid acknowledge over.
            send = new Envelope()
            {
                Message = new Routing()
                { InnerMessage = acknowledge, ToProcessIds = new int[1] { 3 } },
                Endpoint = mockCommunicatorEp
            };
            acknowledge.SetMessageAndConversationNumbers(MessageNumber.Create(), reply.ConvId);
            mockCommunicator.Send(send);
            Thread.Sleep(1000);

            // Assert that the umbrella information and the number of pennies are correct.
            Assert.AreEqual(5, dispatcher.Factory.Pennies.Count);
            Assert.IsNotNull(dispatcher.Factory.Umbrellas);
            Assert.AreEqual(0, dispatcher.Factory.Umbrellas.Count);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestExitGameResponder()
        {
            // Initialize components needed for exit game responder.
            Initialize();
            factory.Game = new GameInfo() { GameManagerId = 1 };

            // Set up exit game request and send it. 
            ExitGameRequest request = new ExitGameRequest() { GameId = 1 };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };

            dispatcher.Start();
            mockCommunicator.Send(send);

            // Recieve reply back from the sender.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was successful.
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(Reply));
            Reply response = reply.InnerMessage as Reply;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(request.ConvId, response.ConvId);
            Assert.IsNull(dispatcher.Factory.Game);
            Assert.AreEqual(0, dispatcher.Factory.LifePoints);
            Assert.AreEqual(0, dispatcher.Factory.CurrentGameId);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestUmbrellaLoweredResponder()
        {
            // Initialize components needed for exit game responder.
            Initialize();
            factory.Game = new GameInfo() { GameManagerId = 1 };

            // Set up exit game request and send it. 
            UmbrellaLoweredNotification request = new UmbrellaLoweredNotification() { UmbrellaId = 1 };
            request.InitMessageAndConversationNumbers();
            Envelope send = new Envelope() { Message = request, Endpoint = mockCommunicatorEp };
            dispatcher.Start();
            mockCommunicator.Send(send);

            // Recieve reply back from the sender.
            Envelope envelope = null;
            envelope = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(envelope);
            Assert.IsNotNull(envelope.Message);
            Assert.IsInstanceOfType(envelope.Message, typeof(Routing));
            Routing reply = envelope.Message as Routing;

            // Assert that the response was successful.
            Assert.IsInstanceOfType(reply.InnerMessage, typeof(Reply));
            Reply response = reply.InnerMessage as Reply;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(request.ConvId, response.ConvId);
            dispatcher.Stop();
        }
    }
}
