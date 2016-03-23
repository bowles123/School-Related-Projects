using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace CommSubTesting
{
    /// <summary>
    /// Summary description for ListenerTester
    /// </summary>
    [TestClass]
    public class DispatcherTester
    {
 
        [TestMethod]
        public void Dispatcher_TestEverything()
        {
            // Setup a fake process id
            MessageNumber.LocalProcessId = 10;

            // Create a commSubsystem with a Listener
            CommSubsystem commSubsystem = TestUtilities.SetupTestCommSubsystem(new DummyConversationFactory());

            // Get the EndPoint for the commSubsystem, so we can send messages to it
            PublicEndPoint targetEP = new PublicEndPoint()
                            {
                              Host = "127.0.0.1",
                              Port = commSubsystem.Communicator.Port
                            };

            // Create another Communicator to send stuff to the listener.
            Communicator remoteComm = new Communicator() { MinPort = 12000, MaxPort = 12099 };
            remoteComm.Start();

            // Check initial state
            Assert.AreEqual(0, commSubsystem.QueueDictionary.ConversationQueueCount);
            Assert.AreEqual(0, DummyConversation.CreatedInstances.Count);

            // Case 1 - Send a request message that is acceptable and will create a
            //          new conversation

            // TODO: Rewrite for new dispatcher
            Thread.Sleep(3000);

            MessageNumber nr1 = MessageNumber.Create();
            AliveRequest request = new AliveRequest() { MsgId = nr1, ConvId = nr1 };
            Envelope env1 = new Envelope() { Message = request, EndPoint = targetEP };
            remoteComm.Send(env1);


            Thread.Sleep(3000);

            Assert.AreEqual(1, DummyConversation.CreatedInstances.Count);
            Assert.IsTrue(DummyConversation.LastCreatedInstance.ExecuteWasCalled);

            // Case 2 - Send a message as part of an existing conversation

            // TODO: Rewrite for new dispatcher

            MessageNumber convId = new MessageNumber() { Pid = 15, Seq = 20 };
            EnvelopeQueue convQueue = commSubsystem.QueueDictionary.CreateQueue(convId);
                                                    // Simulate an existing conversation that able
                                                    // to receive message.
            Assert.AreEqual(0, convQueue.Count);

            MessageNumber nr2 = MessageNumber.Create();
            Reply reply = new Reply() { MsgId = nr2, ConvId = convId };
            Envelope env2 = new Envelope() { Message = reply, EndPoint = targetEP };
            remoteComm.Send(env2);

            Thread.Sleep(1000);
            Assert.AreEqual(1, convQueue.Count);

            // Case 3 - Send a message of a type that can't start a conversation and isn't
            //          part of an existing conversation
            Thread.Sleep(3000);

            MessageNumber nr3 = MessageNumber.Create();
            LoginRequest request3 = new LoginRequest() { MsgId = nr3, ConvId = nr3 };
            Envelope env3 = new Envelope() { Message = request, EndPoint = targetEP };
            remoteComm.Send(env2);

            Thread.Sleep(3000);

            Assert.AreEqual(1, DummyConversation.CreatedInstances.Count);
        }



    }
}
