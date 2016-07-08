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
using CommunicationSubsystem.Conversations.Responders;

namespace CommunicationSubsystemTesting
{
    [TestClass]
    public class QueueTest
    {
        [TestMethod]
        public void TestEnvelopeQueueEnqueueDequeue()
        {
            EnvelopeQueue queue = new EnvelopeQueue();
            int Timeout = 1000;

            Envelope envelope1 = new Envelope()
            {
                Message = new LoginRequest() { },
                Endpoint = new PublicEndPoint()
            };
            queue.Enqueue(envelope1);

            Envelope envelope2 = new Envelope()
            {
                Message = new LoginReply() { },
                Endpoint = new PublicEndPoint()
            };
            queue.Enqueue(envelope2);

            Envelope envelope3 = queue.Dequeue(Timeout);
            Assert.AreSame(envelope1, envelope3);

            Envelope envelope4 = queue.Dequeue(Timeout);
            Assert.AreSame(envelope2, envelope4);

            Envelope envelope5 = queue.Dequeue(Timeout);
            Assert.IsNull(envelope5);
        }
    }

    [TestClass]
    public class DictionaryTest
    {
        [TestMethod]
        public void GetConversationCreateDictionaryTest()
        {
            EnvelopeQueue queue1, queue2, queue3, queue4, queue5, queue6, queue7, queue8;
            ConversationDictionary dictionary = new ConversationDictionary();
            Dictionary<MessageNumber, EnvelopeQueue> conversations;

            Message mssg1 = new LoginRequest();
            Message mssg2 = new ShutdownRequest();
            Message mssg3 = new GameListRequest();
            Message mssg4 = new JoinGameRequest();
            Message mssg5 = new LogoutRequest();
            Message mssg6 = new BuyBalloonRequest();
            Message mssg7 = new FillBalloonRequest();
            Message mssg8 = new ThrowBalloonRequest();

            mssg1.InitMessageAndConversationNumbers();
            mssg2.InitMessageAndConversationNumbers();
            mssg3.InitMessageAndConversationNumbers();
            mssg4.InitMessageAndConversationNumbers();
            mssg5.InitMessageAndConversationNumbers();
            mssg6.InitMessageAndConversationNumbers();
            mssg7.InitMessageAndConversationNumbers();
            mssg8.InitMessageAndConversationNumbers();

            Assert.IsNotNull(mssg1.ConvId);
            Assert.IsNotNull(mssg2.ConvId);
            Assert.IsNotNull(mssg3.ConvId);
            Assert.IsNotNull(mssg4.ConvId);
            Assert.IsNotNull(mssg5.ConvId);
            Assert.IsNotNull(mssg6.ConvId);
            Assert.IsNotNull(mssg7.ConvId);
            Assert.IsNotNull(mssg8.ConvId);

            Assert.IsNotNull(mssg1.MsgId);
            Assert.IsNotNull(mssg2.MsgId);
            Assert.IsNotNull(mssg3.MsgId);
            Assert.IsNotNull(mssg4.MsgId);
            Assert.IsNotNull(mssg5.MsgId);
            Assert.IsNotNull(mssg6.MsgId);
            Assert.IsNotNull(mssg7.MsgId);
            Assert.IsNotNull(mssg8.MsgId);

            queue1 = dictionary.CreateQueue(mssg1.ConvId);
            queue2 = dictionary.CreateQueue(mssg2.ConvId);
            queue3 = dictionary.CreateQueue(mssg3.ConvId);
            queue4 = dictionary.CreateQueue(mssg4.ConvId);
            queue5 = dictionary.CreateQueue(mssg5.ConvId);
            queue6 = dictionary.CreateQueue(mssg6.ConvId);
            queue7 = dictionary.CreateQueue(mssg7.ConvId);
            queue8 = dictionary.CreateQueue(mssg8.ConvId);
            conversations = dictionary.getConversations();

            Assert.AreEqual(8, conversations.Count);
            Assert.AreEqual(conversations[mssg1.ConvId], queue1);
            Assert.AreEqual(conversations[mssg2.ConvId], queue2);
            Assert.AreEqual(conversations[mssg3.ConvId], queue3);
            Assert.AreEqual(conversations[mssg4.ConvId], queue4);
            Assert.AreEqual(conversations[mssg5.ConvId], queue5);
            Assert.AreEqual(conversations[mssg6.ConvId], queue6);
            Assert.AreEqual(conversations[mssg7.ConvId], queue7);
            Assert.AreEqual(conversations[mssg8.ConvId], queue8);

            queue1 = dictionary.GetByConversation(mssg1.ConvId);
            queue2 = dictionary.GetByConversation(mssg2.ConvId);
            queue3 = dictionary.GetByConversation(mssg3.ConvId);
            queue4 = dictionary.GetByConversation(mssg4.ConvId);
            queue5 = dictionary.GetByConversation(mssg5.ConvId);
            queue6 = dictionary.GetByConversation(mssg6.ConvId);
            queue7 = dictionary.GetByConversation(mssg7.ConvId);
            queue8 = dictionary.GetByConversation(mssg8.ConvId);

            Assert.AreEqual(conversations[mssg1.ConvId], queue1);
            Assert.AreEqual(conversations[mssg2.ConvId], queue2);
            Assert.AreEqual(conversations[mssg3.ConvId], queue3);
            Assert.AreEqual(conversations[mssg4.ConvId], queue4);
            Assert.AreEqual(conversations[mssg5.ConvId], queue5);
            Assert.AreEqual(conversations[mssg6.ConvId], queue6);
            Assert.AreEqual(conversations[mssg7.ConvId], queue7);
            Assert.AreEqual(conversations[mssg8.ConvId], queue8);
        }

        [TestMethod]
        public void CloseCreateQueueTest()
        {
            EnvelopeQueue queue1, queue2, queue3, queue4, queue5;
            ConversationDictionary dictionary = new ConversationDictionary();
            Dictionary<MessageNumber, EnvelopeQueue> conversations;
            Message mssg1 = new LoginRequest();
            Message mssg2 = new AliveRequest();
            Message mssg3 = new GameListRequest();
            Message mssg4 = new JoinGameRequest();
            Message mssg5 = new LogoutRequest();

            mssg1.InitMessageAndConversationNumbers();
            mssg2.InitMessageAndConversationNumbers();
            mssg3.InitMessageAndConversationNumbers();
            mssg4.InitMessageAndConversationNumbers();
            mssg5.InitMessageAndConversationNumbers();

            Assert.IsNotNull(mssg1.ConvId);
            Assert.IsNotNull(mssg2.ConvId);
            Assert.IsNotNull(mssg3.ConvId);
            Assert.IsNotNull(mssg4.ConvId);
            Assert.IsNotNull(mssg5.ConvId);

            queue1 = dictionary.CreateQueue(mssg1.ConvId);
            queue2 = dictionary.CreateQueue(mssg2.ConvId);
            queue3 = dictionary.CreateQueue(mssg3.ConvId);
            queue4 = dictionary.CreateQueue(mssg4.ConvId);
            queue5 = dictionary.CreateQueue(mssg5.ConvId);
            conversations = dictionary.getConversations();

            Assert.AreEqual(5, conversations.Count);
            Assert.AreEqual(conversations[mssg1.ConvId], queue1);
            Assert.AreEqual(conversations[mssg2.ConvId], queue2);
            Assert.AreEqual(conversations[mssg3.ConvId], queue3);
            Assert.AreEqual(conversations[mssg4.ConvId], queue4);
            Assert.AreEqual(conversations[mssg5.ConvId], queue5);

            dictionary.CloseQueue(mssg1.ConvId);
            dictionary.CloseQueue(mssg2.ConvId);
            dictionary.CloseQueue(mssg3.ConvId);
            dictionary.CloseQueue(mssg4.ConvId);
            dictionary.CloseQueue(mssg5.ConvId);

            Assert.AreEqual(0, conversations.Count);
        }
    }

    [TestClass]
    public class ConversationFactoryTest
    {
        [TestMethod]
        public void CreateConversationTest()
        {
            PlayerConversationFactory factory = new PlayerConversationFactory();
            Conversation conv1, conv2, conv3, conv4, conv5;
            Envelope env1 = new Envelope() { Message = new LoginRequest() { } };
            Envelope env2 = new Envelope() { Message = new AliveRequest() { } };
            Envelope env3 = new Envelope() { Message = new GameListRequest() { } };
            Envelope env4 = new Envelope() { Message = new JoinGameRequest() { } };
            Envelope env5 = new Envelope() { Message = new LogoutRequest() { } };

            env1.Message.InitMessageAndConversationNumbers();
            env2.Message.InitMessageAndConversationNumbers();
            env3.Message.InitMessageAndConversationNumbers();
            env4.Message.InitMessageAndConversationNumbers();
            env5.Message.InitMessageAndConversationNumbers();

            Assert.IsNotNull(env1.Message.ConvId);
            Assert.IsNotNull(env2.Message.ConvId);
            Assert.IsNotNull(env3.Message.ConvId);
            Assert.IsNotNull(env4.Message.ConvId);
            Assert.IsNotNull(env5.Message.ConvId);

            factory.Initialize();
            factory.InitializeTypes();
            factory.Dictionary = new ConversationDictionary();

            conv1 = factory.CreateFromConversationType(typeof(LoginInitiator));
            conv2 = factory.CreateFromConversationType(typeof(AliveResponder));
            conv3 = factory.CreateFromConversationType(typeof(GetGameListInitiator));
            conv4 = factory.CreateFromConversationType(typeof(JoinGameInitiator));
            conv5 = factory.CreateFromConversationType(typeof(LogoutInitiator));

            Assert.IsInstanceOfType(conv1, typeof(LoginInitiator));
            Assert.IsInstanceOfType(conv2, typeof(AliveResponder));
            Assert.IsInstanceOfType(conv3, typeof(GetGameListInitiator));
            Assert.IsInstanceOfType(conv4, typeof(JoinGameInitiator));
            Assert.IsInstanceOfType(conv5, typeof(LogoutInitiator));

            conv1 = factory.CreateFromMessageType(env1);
            conv2 = factory.CreateFromMessageType(env2);
            conv3 = factory.CreateFromMessageType(env3);
            conv4 = factory.CreateFromMessageType(env4);
            conv5 = factory.CreateFromMessageType(env5);

            Assert.IsNull(conv1);
            Assert.IsNotNull(conv2);
            Assert.IsInstanceOfType(conv2, typeof(AliveResponder));
            Assert.IsNull(conv3);
            Assert.IsNull(conv4);
            Assert.IsNull(conv5);
        }
    }

    [TestClass]
    public class DispatcherTest
    {
        [TestMethod]
        public void TestDispatcherCorrectness()
        {
            Communicator mockCommunicator = new Communicator() { MySocket = new UdpClient(0) };
            int mockClientPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            PublicEndPoint mockClientEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockClientPort };
            Dispatcher dispatcher = new Dispatcher();
            ConversationDictionary dictionary = new ConversationDictionary();
            ConversationFactory factory = new PlayerConversationFactory();
            ProcessInfo GameManager = new ProcessInfo() { EndPoint = mockClientEp, ProcessId = 1 };
            GameProcessData player = new GameProcessData() { Type = ProcessInfo.ProcessType.Player };
            GameProcessData gameManager = new GameProcessData() { Type = ProcessInfo.ProcessType.GameManager };
            GameProcessData[] processes = new GameProcessData[2] { player, gameManager };
            CommunicationProcess process = new Player();
            process.PennyBankEndPoint = new PublicEndPoint();
            GameInfo Game = new GameInfo()
            {
                GameManagerId = GameManager.ProcessId,
                GameId = 1,
                CurrentProcesses = processes
            };

            AliveRequest request1 = new AliveRequest() { };
            GameStatusNotification request2 = new GameStatusNotification() { Game = Game };
            HitNotification request3 = new HitNotification() { };
            ReadyToStart request4 = new ReadyToStart() { };
            ShutdownRequest request5 = new ShutdownRequest() { };

            request1.InitMessageAndConversationNumbers();
            request2.InitMessageAndConversationNumbers();
            request3.InitMessageAndConversationNumbers();
            request4.InitMessageAndConversationNumbers();
            request5.InitMessageAndConversationNumbers();

            factory.Initialize();
            factory.InitializeTypes();
            factory.Dictionary = dictionary;
            factory.Communicator = mockCommunicator;
            factory.LifePoints = 10;
            factory.Game = Game;
            factory.CurrentGameId = Game.GameId;
            factory.CommProcess = process;
            factory.Process = GameManager;

            dispatcher.Factory = factory;
            dispatcher.Dictionary = dictionary;
            dispatcher.Communicator = mockCommunicator;
            dispatcher.Start();

            mockCommunicator.Send(new Envelope() { Message = request1, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope()
            { Message = new Routing() { InnerMessage = request2 }, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope() { Message = request3, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope() { Message = request4, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope() { Message = request5, Endpoint = mockClientEp });
            Thread.Sleep(1000);
            Assert.AreEqual(5, Conversation.LaunchCount);
            dispatcher.Stop();
        }

        [TestMethod]
        public void TestDispatcherIncorrectness()
        {
            Communicator mockCommunicator = new Communicator() { MySocket = new UdpClient(0) };
            int mockClientPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            PublicEndPoint mockClientEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockClientPort };
            Dispatcher dispatcher = new Dispatcher();
            ConversationDictionary dictionary = new ConversationDictionary();
            ConversationFactory factory = new PlayerConversationFactory();
            ProcessInfo GameManager = new ProcessInfo() { EndPoint = mockClientEp, ProcessId = 1 };
            GameProcessData player = new GameProcessData() { Type = ProcessInfo.ProcessType.Player };
            GameProcessData gameManager = new GameProcessData() { Type = ProcessInfo.ProcessType.GameManager };
            GameProcessData[] processes = new GameProcessData[2] { player, gameManager };
            CommunicationProcess process = new Player();
            process.PennyBankEndPoint = new PublicEndPoint();
            GameInfo Game = new GameInfo()
            {
                GameManagerId = GameManager.ProcessId,
                GameId = 1,
                CurrentProcesses = processes
            };

            LoginRequest request6 = new LoginRequest() { };
            LogoutRequest request7 = new LogoutRequest() { };
            GameListRequest request8 = new GameListRequest() { };
            JoinGameRequest request9 = new JoinGameRequest() { };
            BuyBalloonRequest request10 = new BuyBalloonRequest() { };
            FillBalloonRequest request11 = new FillBalloonRequest() { };
            ThrowBalloonRequest request12 = new ThrowBalloonRequest() { };

            request6.InitMessageAndConversationNumbers();
            request7.InitMessageAndConversationNumbers();
            request8.InitMessageAndConversationNumbers();
            request9.InitMessageAndConversationNumbers();
            request10.InitMessageAndConversationNumbers();
            request11.InitMessageAndConversationNumbers();
            request12.InitMessageAndConversationNumbers();

            factory.Initialize();
            factory.InitializeTypes();
            factory.Dictionary = dictionary;
            factory.Communicator = mockCommunicator;
            factory.LifePoints = 10;
            factory.Game = Game;
            factory.CurrentGameId = Game.GameId;
            factory.CommProcess = process;
            factory.Process = GameManager;

            dispatcher.Factory = factory;
            dispatcher.Dictionary = dictionary;
            dispatcher.Communicator = mockCommunicator;
            dispatcher.Start();

            mockCommunicator.Send(new Envelope() { Message = request6, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope() { Message = request7, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope() { Message = request8, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope() { Message = request9, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope() { Message = request10, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope() { Message = request11, Endpoint = mockClientEp });
            mockCommunicator.Send(new Envelope() { Message = request12, Endpoint = mockClientEp });

            Thread.Sleep(3000);
            dispatcher.Stop();
            Assert.AreEqual(0, Conversation.LaunchCount);
        }
    }

    [TestClass]
    public class CommunicatorTest
    {
        [TestMethod]
        public void TestSendReceive()
        {
            Communicator mockCommunicator = new Communicator() { MySocket = new UdpClient(0) };
            int mockClientPort = ((IPEndPoint)mockCommunicator.MySocket.Client.LocalEndPoint).Port;
            PublicEndPoint mockClientEp = new PublicEndPoint() { Host = "127.0.0.1", Port = mockClientPort };
            LoginRequest login = new LoginRequest() { ProcessLabel = "Brian",
                ProcessType = ProcessInfo.ProcessType.Player };
            Envelope envelope = new Envelope() { Message = login, Endpoint = mockClientEp };

            mockCommunicator.Send(envelope);

            IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);
            Envelope response = null;
            response = mockCommunicator.Retrieve(1000);

            Assert.IsNotNull(response);
            Message msg = response.Message;
            Assert.IsInstanceOfType(msg, typeof(LoginRequest));
        }
    }
}
