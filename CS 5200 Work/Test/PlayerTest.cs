using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedObjects;
using System.Net;
using System.Threading;
using Messages;
using Messages.RequestMessages;
using PlayerProcess;
using Messages.ReplyMessages;
using CommunicationSubsystem;

namespace PlayerTesting
{
    [TestClass]
    public class PlayerTest
    {
        [TestMethod]
        public void PlayerLoginTest()
        {
            // Set components needed to test the login.
            Player player = new Player();
            Envelope request = null, response = null;
            LoginRequest message = null;
            LoginReply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, playerCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to login.
            player.initialize();
            player.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Player",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.Unknown,
                Type = ProcessInfo.ProcessType.Player
            };

            mockCommunicator = new Communicator();
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            playerCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = player.MyCommunicator.Port };
            player.RegistryEndPoint = mockCommunicatorEp;
            Thread run = new Thread(player.tryLogin);
            player.MyDispatcher.Start();
            run.Start();

            // Recieve login request from the player and Assert that it is such.
            request = mockCommunicator.Retrieve(10000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(LoginRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as LoginRequest;
            Assert.AreEqual(0, message.ConvId.Pid);
            Assert.AreEqual(ProcessInfo.ProcessType.Player, player.MyProcessInfo.Type);

            // Set up login reply and send it.
            ProcessInfo info = player.MyProcessInfo;
            info.Status = ProcessInfo.StatusCode.Registered;
            PublicEndPoint proxy = new PublicEndPoint("127.0.0.1:4000");
            reply = new LoginReply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true,
                Note = "Welcome",
                ProcessInfo = info,
                ProxyEndPoint = proxy,
                PennyBankEndPoint = new PublicEndPoint()
            };
            response = new Envelope() { Message = reply, Endpoint = playerCommunicatorEp };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the player's login set up the process info and proxy correctly.
            Assert.AreEqual(reply.ProcessInfo.ProcessId, player.MyProcessInfo.ProcessId);
            Assert.AreEqual(ProcessInfo.ProcessType.Player, player.MyProcessInfo.Type);
            Assert.AreEqual(reply.ProcessInfo.Label, player.MyProcessInfo.Label);
            Assert.AreEqual(ProcessInfo.StatusCode.Registered, player.MyProcessInfo.Status);
            Assert.AreEqual(reply.PennyBankEndPoint, player.PennyBankEndPoint);
            Assert.AreEqual(proxy, player.ProxyEndPoint);
        }

        [TestMethod]
        public void PlayerGetGameListTest()
        {
            // Set components needed to test the getting of the game list.
            Player player = new Player();
            Envelope request = null, response = null;
            GameListRequest message = null;
            GameListReply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, playerCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to get the game list.
            player.initialize();
            player.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Player",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.Registered,
                Type = ProcessInfo.ProcessType.Player
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = player.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            playerCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = player.MyCommunicator.Port };
            player.RegistryEndPoint = mockCommunicatorEp;
            Thread run = new Thread(player.tryGetGameList);
            player.MyDispatcher.Start();
            run.Start();

            // Recieve get game request from the player and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(GameListRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as GameListRequest;
            Assert.AreEqual(player.MyProcessInfo.ProcessId, message.ConvId.Pid);
            Assert.AreEqual((int)GameInfo.StatusCode.Available, message.StatusFilter);

            // Set up game list reply and send it.
            reply = new GameListReply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true,
                GameInfo = new GameInfo[1]
                { new GameInfo()
                { GameId = 1, Status = GameInfo.StatusCode.Available, MinPlayers = 2, MaxPlayers = 4 } }
            };
            response = new Envelope() { Message = reply, Endpoint = playerCommunicatorEp };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the player set up the game list correctly.
            Assert.AreEqual(reply.GameInfo[0].GameId, player.Games[0].GameId);
            Assert.AreEqual(reply.GameInfo[0].Status, player.Games[0].Status);
            Assert.AreEqual(reply.GameInfo[0].MinPlayers, player.Games[0].MinPlayers);
            Assert.AreEqual(reply.GameInfo[0].MaxPlayers, player.Games[0].MaxPlayers);
        }

        [TestMethod]
        public void PlayerJoinGameTest()
        {
            // Set components needed to test the joining of a game.
            Player player = new Player();
            Envelope request = null, response = null;
            JoinGameRequest message = null;
            JoinGameReply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, playerCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to join a game.
            player.initialize();
            player.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Player",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.JoiningGame,
                Type = ProcessInfo.ProcessType.Player
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = player.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            playerCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = player.MyCommunicator.Port };
            player.RegistryEndPoint = mockCommunicatorEp;
            player.ProxyEndPoint = mockCommunicatorEp;
            player.Games = new GameInfo[1]
            {
                new GameInfo()
                { GameId = 1, Status = GameInfo.StatusCode.Available, MinPlayers = 2, MaxPlayers = 4 }
            };

            Thread run = new Thread(player.tryJoinGame);
            player.MyDispatcher.Start();
            run.Start();

            // Recieve join game request from the player and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(JoinGameRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as JoinGameRequest;
            Assert.AreEqual(player.MyProcessInfo.ProcessId, message.ConvId.Pid);
            Assert.AreEqual(player.Games[0].GameId, message.GameId);
            Assert.AreEqual(player.MyProcessInfo.Status, message.Process.Status);
            Assert.AreEqual(player.MyProcessInfo.ProcessId, message.Process.ProcessId);
            Assert.AreEqual(player.MyProcessInfo.Type, message.Process.Type);
            Assert.AreEqual(player.MyProcessInfo.Label, message.Process.Label);

            // Set up join game reply and send it.
            reply = new JoinGameReply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true,
                GameId = message.GameId,
                InitialLifePoints = 10,
                Note = "Welcome to the game."
            };
            response = new Envelope()
            {
                Message = new Routing()
                { InnerMessage = reply, ToProcessIds = new int[1] { 2 } },
                Endpoint = playerCommunicatorEp
            };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the player set up the join game correctly.
            Assert.AreEqual(reply.GameId, player.MyDispatcher.Factory.CurrentGameId);
            Assert.IsNotNull(player.MyDispatcher.Factory.Game);
            Assert.AreEqual(reply.GameId, player.MyDispatcher.Factory.Game.GameId);
        }

        [TestMethod]
        public void PlayerBuyBalloonTest()
        {
            // Set components needed to test the buying of balloons.
            Player player = new Player();
            Penny penny = new Penny() { Id = 1, SignedBy = 4 };
            Envelope request = null, response = null;
            BuyBalloonRequest message = null;
            BalloonReply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, playerCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to buy a balloon.
            player.initialize();
            player.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Player",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.PlayingGame,
                Type = ProcessInfo.ProcessType.Player
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = player.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            playerCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = player.MyCommunicator.Port };
            player.RegistryEndPoint = mockCommunicatorEp;
            player.ProxyEndPoint = mockCommunicatorEp;
            player.Pennies.Enqueue(penny);
            player.MyDispatcher.Factory.BalloonStoreId = 1;

            Thread run = new Thread(player.tryBuyBalloon);
            player.MyDispatcher.Start();
            run.Start();

            // Recieve buy balloon request from the player and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(BuyBalloonRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as BuyBalloonRequest;
            Assert.AreEqual(player.MyProcessInfo.ProcessId, message.ConvId.Pid);
            Assert.AreEqual(penny.Id, message.Penny.Id);
            Assert.AreEqual(penny.SignedBy, message.Penny.SignedBy);

            // Set up balloon reply and send it.
            reply = new BalloonReply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true,
                Balloon = new Balloon() { Id = 1, SignedBy = 3, IsFilled = false }
            };
            response = new Envelope()
            {
                Message = new Routing()
                { InnerMessage = reply, ToProcessIds = new int[1] { 2 } },
                Endpoint = playerCommunicatorEp
            };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the player set up the balloon and pennies correctly.
            Balloon balloon;
            Assert.IsTrue(player.MyDispatcher.Factory.Pennies.Count == 0);
            Assert.IsTrue(player.MyDispatcher.Factory.Balloons.Count > 0);
            Assert.AreEqual(1, player.MyDispatcher.Factory.Balloons.Count);
            balloon = player.MyDispatcher.Factory.Balloons.Dequeue();

            Assert.IsNotNull(balloon);
            Assert.AreEqual(reply.Balloon.Id, balloon.Id);
            Assert.AreEqual(reply.Balloon.SignedBy, balloon.SignedBy);
            Assert.IsFalse(balloon.IsFilled);
        }

        [TestMethod]
        public void PlayerFillBalloonTest()
        {
            // Set components needed to test the filling of balloons.
            Player player = new Player();
            Penny penny1 = new Penny() { Id = 1, SignedBy = 4 }, 
                penny2 = new Penny() { Id = 2, SignedBy = 4 };
            Balloon balloon = new Balloon() { Id = 1, SignedBy = 3, IsFilled = false };
            Envelope request = null, response = null;
            FillBalloonRequest message = null;
            BalloonReply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, playerCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to fill a balloon.
            player.initialize();
            player.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Player",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.PlayingGame,
                Type = ProcessInfo.ProcessType.Player
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = player.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            playerCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = player.MyCommunicator.Port };
            player.RegistryEndPoint = mockCommunicatorEp;
            player.ProxyEndPoint = mockCommunicatorEp;
            player.Balloons.Enqueue(balloon);
            player.Pennies.Enqueue(penny1);
            player.Pennies.Enqueue(penny2);

            Thread run = new Thread(player.tryFillBalloon);
            player.MyDispatcher.Start();
            run.Start();

            // Recieve fill balloon request from the player and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(FillBalloonRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as FillBalloonRequest;
            Assert.AreEqual(player.MyProcessInfo.ProcessId, message.ConvId.Pid);
            Assert.AreEqual(balloon.Id, message.Balloon.Id);
            Assert.AreEqual(balloon.SignedBy, message.Balloon.SignedBy);
            Assert.IsFalse(message.Balloon.IsFilled);
            Assert.IsTrue(message.Pennies.Length > 0);
            Assert.AreEqual(2, message.Pennies.Length);

            Assert.AreEqual(penny1.Id, message.Pennies[0].Id);
            Assert.AreEqual(penny1.SignedBy, message.Pennies[0].SignedBy);
            Assert.AreEqual(penny2.Id, message.Pennies[1].Id);
            Assert.AreEqual(penny2.SignedBy, message.Pennies[1].SignedBy);

            // Set up balloon reply and send it.
            balloon.IsFilled = true;
            reply = new BalloonReply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true,
                Balloon = balloon
            };
            response = new Envelope()
            {
                Message = new Routing()
                { InnerMessage = reply, ToProcessIds = new int[1] { 2 } },
                Endpoint = playerCommunicatorEp
            };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the player set up the filled balloons, balloons, and pennies correctly.
            Assert.AreEqual(0, player.MyDispatcher.Factory.Pennies.Count);
            Assert.AreEqual(0, player.MyDispatcher.Factory.Balloons.Count);
            Assert.IsTrue(player.MyDispatcher.Factory.FilledBalloons.Count > 0);
            Assert.AreEqual(1, player.MyDispatcher.Factory.FilledBalloons.Count);
            balloon = player.MyDispatcher.Factory.FilledBalloons.Dequeue();

            Assert.IsNotNull(balloon);
            Assert.AreEqual(reply.Balloon.Id, balloon.Id);
            Assert.AreEqual(reply.Balloon.SignedBy, balloon.SignedBy);
            Assert.IsTrue(balloon.IsFilled);
        }

        [TestMethod]
        public void PlayerThrowBalloonTest()
        {
            // Set components needed to test the throwing of balloons.
            Player player = new Player();
            Balloon balloon = new Balloon() { Id = 1, SignedBy = 3, IsFilled = true };
            Envelope request = null, response = null;
            ThrowBalloonRequest message = null;
            Reply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, playerCommunicatorEp;
            int mockCommunicatorPort;
            GameInfo game = new GameInfo()
            {
                StartingPlayers = new int[2] { 5, 1 },
                GameId = 3,
                MinPlayers = 2,
                Status = GameInfo.StatusCode.InProgress,
                Label = "Game #3",
                GameManagerId = 2
            };

            // Intialize components and attempt to throw a balloon.
            player.initialize();
            player.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Player",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.PlayingGame,
                Type = ProcessInfo.ProcessType.Player
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = player.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            playerCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = player.MyCommunicator.Port };
            player.RegistryEndPoint = mockCommunicatorEp;
            player.ProxyEndPoint = mockCommunicatorEp;
            player.FilledBalloons.Enqueue(balloon);
            player.MyDispatcher.Factory.Game = game; // Might need to set up game in actually player.

            Thread run = new Thread(player.tryThrowBalloon);
            player.MyDispatcher.Start();
            run.Start();

            // Recieve throw balloon request from the player and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.Message, typeof(Routing));
            Routing route = request.Message as Routing;
            Assert.AreEqual(game.GameManagerId, route.ToProcessIds[0]);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(ThrowBalloonRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as ThrowBalloonRequest;
            Assert.AreEqual(player.MyProcessInfo.ProcessId, message.ConvId.Pid);
            Assert.AreEqual(balloon.Id, message.Balloon.Id);
            Assert.AreEqual(balloon.SignedBy, message.Balloon.SignedBy);
            Assert.AreEqual(game.StartingPlayers[0], message.TargetPlayerId);
            Assert.IsTrue(message.Balloon.IsFilled);

            // Set up reply to throw balloon request and send it.
            reply = new Reply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true
            };
            response = new Envelope()
            {
                Message = new Routing()
                { InnerMessage = reply, ToProcessIds = new int[1] { 2 } },
                Endpoint = playerCommunicatorEp
            };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the player set up the filled balloons correctly.
            Assert.IsTrue(player.MyDispatcher.Factory.FilledBalloons.Count == 0);
        }

        [TestMethod]
        public void PlayerRaiseUmbrellaTest()
        {
            // Set components needed to test the raising of umbrellas.
            Player player = new Player();
            Umbrella umbrella = new Umbrella() { Id = 10, SignedBy = 6 };
            Envelope request = null, response = null;
            RaiseUmbrellaRequest message = null;
            Reply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, playerCommunicatorEp;
            int mockCommunicatorPort;
            GameInfo game = new GameInfo()
            {
                StartingPlayers = new int[1] { 5 },
                GameId = 3,
                MinPlayers = 2,
                Status = GameInfo.StatusCode.InProgress,
                Label = "Game #3",
                GameManagerId = 2
            };

            // Intialize components and attempt to raising an umbrella.
            player.initialize();
            player.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Player",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.PlayingGame,
                Type = ProcessInfo.ProcessType.Player
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = player.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            playerCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = player.MyCommunicator.Port };
            player.RegistryEndPoint = mockCommunicatorEp;
            player.ProxyEndPoint = mockCommunicatorEp;
            player.Umbrellas.Enqueue(umbrella);
            player.MyDispatcher.Factory.Game = game; // Might need to set up game in actually player.

            Thread run = new Thread(player.tryRaiseUmbrella);
            player.MyDispatcher.Start();
            run.Start();

            // Recieve raise umbrella request from the player and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(RaiseUmbrellaRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as RaiseUmbrellaRequest;
            Assert.AreEqual(player.MyProcessInfo.ProcessId, message.ConvId.Pid);
            Assert.AreEqual(umbrella.Id, message.Umbrella.Id);
            Assert.AreEqual(umbrella.SignedBy, message.Umbrella.SignedBy);

            // Set up reply to raise umbrella request and send it.
            reply = new Reply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true
            };
            response = new Envelope()
            {
                Message = new Routing()
                { InnerMessage = reply, ToProcessIds = new int[1] { 2 } },
                Endpoint = playerCommunicatorEp
            };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the player set up the umbrellas and umbrellaRaised fields correctly.
            Assert.IsTrue(player.MyDispatcher.Factory.Umbrellas.Count == 0);
            Assert.IsTrue(player.UmbrellaRaised);
        }

        [TestMethod]
        public void PlayerLogoutTest()
        {
            // Set components needed to test the logout.
            Player player = new Player();
            Envelope request = null, response = null;
            LogoutRequest message = null;
            Reply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, playerCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to logout.
            player.initialize();
            player.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Player",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.Registered,
                Type = ProcessInfo.ProcessType.Player
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = player.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            playerCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = player.MyCommunicator.Port };
            player.RegistryEndPoint = mockCommunicatorEp;
            player.ProxyEndPoint = new PublicEndPoint();
            player.PennyBankEndPoint = new PublicEndPoint();
            Thread run = new Thread(player.tryLogout);
            player.MyDispatcher.Start();
            run.Start();

            // Recieve logout request from the player and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(LogoutRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as LogoutRequest;
            Assert.AreEqual(player.MyProcessInfo.ProcessId, message.ConvId.Pid);

            // Set up reply to the logout request and send it.
            reply = new Reply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true,
                Note = "Goodbye."
            };
            response = new Envelope() { Message = reply, Endpoint = playerCommunicatorEp };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the player set up the join game correctly.
            Assert.IsNull(player.MyDispatcher.Factory.Game);
            Assert.IsNull(player.MyDispatcher.Factory.Games);
            Assert.IsNull(player.MyDispatcher.Factory.PennyKey);
            Assert.IsNull(player.ProxyEndPoint);
            Assert.IsNull(player.PennyBankEndPoint);
        }

        [TestMethod]
        public void PlayerLeaveGameTest()
        {
            // Set components needed to test leaving a game.
            Player player = new Player();
            Envelope request = null, response = null;
            LeaveGameRequest message = null;
            Reply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, storeCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to leave the game.
            player.initialize();
            player.MyDispatcher.Factory.Game = new GameInfo();
            player.MyDispatcher.Factory.CurrentProcesses = new GameProcessData[1];
            player.MyDispatcher.Factory.CurrentGameId = 1;
            player.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Player",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.LeavingGame,
                Type = ProcessInfo.ProcessType.Player
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = player.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            storeCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = player.MyCommunicator.Port };
            player.RegistryEndPoint = mockCommunicatorEp;
            player.ProxyEndPoint = mockCommunicatorEp;
            player.PennyBankEndPoint = new PublicEndPoint();
            Thread run = new Thread(player.tryLeaveGame);
            player.MyDispatcher.Start();
            run.Start();

            // Recieve leave game request from the balloon store and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(LeaveGameRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as LeaveGameRequest;
            Assert.AreEqual(player.MyProcessInfo.ProcessId, message.ConvId.Pid);

            // Set up reply to the leave game request and send it.
            reply = new Reply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true,
                Note = "Goodbye."
            };
            response = new Envelope() { Message = reply, Endpoint = storeCommunicatorEp };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the balloon store set up the leave game correctly.
            Assert.IsNull(player.MyDispatcher.Factory.Game);
            Assert.IsNull(player.MyDispatcher.Factory.CurrentProcesses);
            Assert.AreEqual(0, player.MyDispatcher.Factory.CurrentGameId);
            Assert.AreEqual(ProcessInfo.StatusCode.Registered, player.MyProcessInfo.Status);
        }
    }
}

