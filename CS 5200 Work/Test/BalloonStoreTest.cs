using Microsoft.VisualStudio.TestTools.UnitTesting;
using BalloonStoreProcess;
using SharedObjects;
using Messages;
using System.Net;
using System.Threading;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using CommunicationSubsystem;

namespace Test
{
    [TestClass]
    public class BalloonStoreTest
    {
        [TestMethod]
        public void BalloonStoreLoginTest()
        {
            // Set components needed to test the login.
            BalloonStore store = new BalloonStore();
            Envelope request = null, response = null;
            LoginRequest message = null;
            LoginReply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, storeCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to login.
            store.initialize();
            store.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Balloon Store",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.Unknown,
                Type = ProcessInfo.ProcessType.BalloonStore
            };

            mockCommunicator = new Communicator();
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            storeCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = store.MyCommunicator.Port };
            store.RegistryEndPoint = mockCommunicatorEp;
            Thread run = new Thread(store.tryLogin);
            store.MyDispatcher.Start();
            run.Start();

            // Recieve login request from the balloon store and Assert that it is such.
            request = mockCommunicator.Retrieve(10000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(LoginRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as LoginRequest;
            Assert.AreEqual(0, message.ConvId.Pid);
            Assert.AreEqual(ProcessInfo.ProcessType.BalloonStore, store.MyProcessInfo.Type);
            Assert.AreEqual(store.PublicKey.Exponent.Length, message.PublicKey.Exponent.Length);
            Assert.AreEqual(store.PublicKey.Modulus.Length, message.PublicKey.Modulus.Length);

            for (int i = 0; i < store.PublicKey.Exponent.Length; i++)
                Assert.AreEqual(store.PublicKey.Exponent[i], message.PublicKey.Exponent[i]);
            for (int i = 0; i < store.PublicKey.Modulus.Length; i++)
                Assert.AreEqual(store.PublicKey.Modulus[i], message.PublicKey.Modulus[i]);

            // Set up login reply and send it.
            ProcessInfo info = store.MyProcessInfo;
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
                PennyBankPublicKey = store.PublicKey
            };
            response = new Envelope() { Message = reply, Endpoint =  storeCommunicatorEp};
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the balloon's login set up the process info and proxy correctly.
            Assert.AreEqual(reply.ProcessInfo.ProcessId, store.MyProcessInfo.ProcessId);
            Assert.AreEqual(ProcessInfo.ProcessType.BalloonStore, store.MyProcessInfo.Type);
            Assert.AreEqual(reply.ProcessInfo.Label, store.MyProcessInfo.Label);
            Assert.AreEqual(ProcessInfo.StatusCode.Registered, store.MyProcessInfo.Status);
            Assert.IsNotNull(store.MyDispatcher.Factory.PennyRSA);
            Assert.AreEqual(proxy, store.ProxyEndPoint);

            // Assert that the penny bank's public key was set up correctly.
            Assert.AreEqual(store.PublicKey.Exponent.Length, 
                store.MyDispatcher.Factory.PennyKey.Exponent.Length);
            Assert.AreEqual(store.PublicKey.Modulus.Length, 
                store.MyDispatcher.Factory.PennyKey.Modulus.Length);
            for (int i = 0; i < store.PublicKey.Exponent.Length; i++)
                Assert.AreEqual(store.PublicKey.Exponent[i], store.MyDispatcher.Factory.PennyKey.Exponent[i]);
            for (int i = 0; i < store.PublicKey.Modulus.Length; i++)
                Assert.AreEqual(store.PublicKey.Modulus[i], store.MyDispatcher.Factory.PennyKey.Modulus[i]);
        }

        [TestMethod]
        public void BalloonStoreGetNextIdTest()
        {
            // Set components needed to test getting the next id.
            BalloonStore store = new BalloonStore();
            Envelope request = null, response = null;
            NextIdRequest message = null;
            NextIdReply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, storeCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to get the next id.
            store.initialize();
            store.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Balloon Store",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.Registered,
                Type = ProcessInfo.ProcessType.BalloonStore
            };
            store.Options.NumBalloons = 5;

            mockCommunicator = new Communicator();
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            storeCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = store.MyCommunicator.Port };
            store.RegistryEndPoint = mockCommunicatorEp;
            Thread run = new Thread(store.tryGetNextId);
            store.MyDispatcher.Start();
            run.Start();

            // Recieve next id request from the balloon store and Assert that it is such.
            request = mockCommunicator.Retrieve(10000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(NextIdRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as NextIdRequest;
            Assert.AreEqual(0, message.ConvId.Pid);
            Assert.AreEqual(ProcessInfo.ProcessType.BalloonStore, store.MyProcessInfo.Type);
            Assert.AreEqual(store.Options.NumBalloons, message.NumberOfIds);

            // Set up next id reply and send it.
            reply = new NextIdReply()
            {
                ConvId = message.ConvId,
                MsgId = MessageNumber.Create(),
                Success = true,
                Note = "Welcome",
                NextId = 10,
                NumberOfIds = message.NumberOfIds
            };
            response = new Envelope() { Message = reply, Endpoint = storeCommunicatorEp };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the balloon's login set up the process info and proxy correctly.
            Assert.IsFalse(store.NextId == 0);
            Assert.AreEqual(reply.NextId, store.NextId);
            Assert.IsFalse(reply.NumberOfIds == 0);
            Assert.AreEqual(reply.NumberOfIds, store.NumIds);
        }

        [TestMethod]
        public void BalloonStoreGetGameListTest()
        {
            // Set components needed to test the getting of the game list.
            BalloonStore store = new BalloonStore();
            Envelope request = null, response = null;
            GameListRequest message = null;
            GameListReply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, storeCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to get the game list.
            store.initialize();
            store.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Balloon Store",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.Registered,
                Type = ProcessInfo.ProcessType.BalloonStore
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = store.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            storeCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = store.MyCommunicator.Port };
            store.RegistryEndPoint = mockCommunicatorEp;
            Thread run = new Thread(store.tryGetGameList);
            store.MyDispatcher.Start();
            run.Start();

            // Recieve get game request from the balloon store and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(GameListRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as GameListRequest;
            Assert.AreEqual(store.MyProcessInfo.ProcessId, message.ConvId.Pid);
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
            response = new Envelope() { Message = reply, Endpoint = storeCommunicatorEp };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the balloon store set up the game list correctly.
            Assert.AreEqual(reply.GameInfo[0].GameId, store.Games[0].GameId);
            Assert.AreEqual(reply.GameInfo[0].Status, store.Games[0].Status);
            Assert.AreEqual(reply.GameInfo[0].MinPlayers, store.Games[0].MinPlayers);
            Assert.AreEqual(reply.GameInfo[0].MaxPlayers, store.Games[0].MaxPlayers);
        }

        [TestMethod]
        public void BalloonStoreJoinGameTest()
        {
            // Set components needed to test the joining of a game.
            BalloonStore store = new BalloonStore();
            Envelope request = null, response = null;
            JoinGameRequest message = null;
            JoinGameReply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, storeCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to join a game.
            store.initialize();
            store.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Balloon Store",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.JoiningGame,
                Type = ProcessInfo.ProcessType.BalloonStore
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = store.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            storeCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = store.MyCommunicator.Port };
            store.RegistryEndPoint = mockCommunicatorEp;
            store.ProxyEndPoint = mockCommunicatorEp;
            store.Games = new GameInfo[1]
            {
                new GameInfo()
                { GameId = 1, Status = GameInfo.StatusCode.Available, MinPlayers = 2, MaxPlayers = 4 }
            };

            Thread run = new Thread(store.tryJoinGame);
            store.MyDispatcher.Start();
            run.Start();

            // Recieve join game request from the balloon store and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(JoinGameRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as JoinGameRequest;
            Assert.AreEqual(store.MyProcessInfo.ProcessId, message.ConvId.Pid);
            Assert.AreEqual(store.Games[0].GameId, message.GameId);
            Assert.AreEqual(store.MyProcessInfo.Status, message.Process.Status);
            Assert.AreEqual(store.MyProcessInfo.ProcessId, message.Process.ProcessId);
            Assert.AreEqual(store.MyProcessInfo.Type, message.Process.Type);
            Assert.AreEqual(store.MyProcessInfo.Label, message.Process.Label);

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
            { Message = new Routing()
            { InnerMessage = reply, ToProcessIds = new int[1] { 2 } }, Endpoint = storeCommunicatorEp };
            mockCommunicator.Send(response);
            response = null;
            Thread.Sleep(1000);

            // Assert that the balloon store set up the join game correctly.
            Assert.AreEqual(reply.GameId, store.MyDispatcher.Factory.CurrentGameId);
            Assert.IsNotNull(store.MyDispatcher.Factory.Game);
            Assert.AreEqual(reply.GameId, store.MyDispatcher.Factory.Game.GameId);
        }
        
        [TestMethod]
        public void BalloonStoreLogoutTest()
        {
            // Set components needed to test the logout.
            BalloonStore store = new BalloonStore();
            Envelope request = null, response = null;
            LogoutRequest message = null;
            Reply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, storeCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to logout.
            store.initialize();
            store.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Balloon Store",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.Registered,
                Type = ProcessInfo.ProcessType.BalloonStore
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = store.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            storeCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = store.MyCommunicator.Port };
            store.RegistryEndPoint = mockCommunicatorEp;
            store.ProxyEndPoint = new PublicEndPoint();
            store.PennyBankEndPoint = new PublicEndPoint();
            Thread run = new Thread(store.tryLogout);
            store.MyDispatcher.Start();
            run.Start();

            // Recieve logout request from the balloon store and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(LogoutRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as LogoutRequest;
            Assert.AreEqual(store.MyProcessInfo.ProcessId, message.ConvId.Pid);

            // Set up reply to the logout request and send it.
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

            // Assert that the balloon store set up the join game correctly.
            Assert.IsNull(store.MyDispatcher.Factory.Game);
            Assert.IsNull(store.MyDispatcher.Factory.Games);
            Assert.IsNull(store.MyDispatcher.Factory.PennyKey);
            Assert.IsNull(store.ProxyEndPoint);
            Assert.IsNull(store.PennyBankEndPoint);
        }
        
        [TestMethod]
        public void CreateBalloonsTest()
        {
            // Initialize components needed for creating balloons and create them.
            BalloonStore store = new BalloonStore();
            store.initialize();
            store.Options.NumBalloons = 5;
            store.NextId = 1;

            store.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Balloon Store",
                Status = ProcessInfo.StatusCode.Registered,
                Type = ProcessInfo.ProcessType.BalloonStore,
                EndPoint = new PublicEndPoint("127.0.0.1:4000"),
                ProcessId = 1
            };
            store.createBalloons();

            // Assert that the appropriate amount of balloons were created.
            Assert.AreEqual(store.Options.NumBalloons, store.Balloons.Count);

            int i = 1;
            foreach (Balloon balloon in store.Balloons)
            {
                // Assert that the balloon is a valid balloon created by the balloon store.
                Assert.AreEqual(i, balloon.Id);
                Assert.IsFalse(balloon.IsFilled);
                Assert.AreEqual(store.MyProcessInfo.ProcessId , balloon.SignedBy);
                Assert.IsNotNull(balloon.DigitalSignature);

                i++;
            }
        }

        [TestMethod]
        public void BalloonStoreLeaveGameTest()
        {
            // Set components needed to test leaving a game.
            BalloonStore store = new BalloonStore();
            Envelope request = null, response = null;
            LeaveGameRequest message = null;
            Reply reply = null;
            Communicator mockCommunicator;
            PublicEndPoint mockCommunicatorEp, storeCommunicatorEp;
            int mockCommunicatorPort;

            // Intialize components and attempt to leave the game.
            store.initialize();
            store.MyDispatcher.Factory.Game = new GameInfo();
            store.MyDispatcher.Factory.CurrentProcesses = new GameProcessData[1];
            store.MyDispatcher.Factory.CurrentGameId = 1;
            store.MyProcessInfo = new ProcessInfo()
            {
                Label = "Brian's Balloon Store",
                ProcessId = 1,
                Status = ProcessInfo.StatusCode.LeavingGame,
                Type = ProcessInfo.ProcessType.BalloonStore
            };

            mockCommunicator = new Communicator();
            MessageNumber.LocalProcessId = store.MyProcessInfo.ProcessId;
            mockCommunicatorPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            mockCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockCommunicatorPort };
            storeCommunicatorEp = new PublicEndPoint() { Host = "127.0.0.1", Port = store.MyCommunicator.Port };
            store.RegistryEndPoint = mockCommunicatorEp;
            store.ProxyEndPoint = mockCommunicatorEp;
            store.PennyBankEndPoint = new PublicEndPoint();
            Thread run = new Thread(store.tryLeaveGame);
            store.MyDispatcher.Start();
            run.Start();

            // Recieve leave game request from the balloon store and Assert that it is such.
            request = mockCommunicator.Retrieve(1000);
            Assert.IsNotNull(request);
            Assert.IsInstanceOfType(request.ActualMessage, typeof(LeaveGameRequest));

            // Assert that the request has valid pieces.
            message = request.ActualMessage as LeaveGameRequest;
            Assert.AreEqual(store.MyProcessInfo.ProcessId, message.ConvId.Pid);

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
            Assert.IsNull(store.MyDispatcher.Factory.Game);
            Assert.IsNull(store.MyDispatcher.Factory.CurrentProcesses);
            Assert.AreEqual(0, store.MyDispatcher.Factory.CurrentGameId);
            Assert.AreEqual(ProcessInfo.StatusCode.Registered, store.MyProcessInfo.Status);
        }
    }
}
