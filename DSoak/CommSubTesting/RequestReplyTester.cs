using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;
using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace CommSubTesting
{
    [TestClass]
    public class RequestReplyTester
    {
        [TestMethod]
        public void RequestReply_SimplyTestWithEndPoints()
        {
            // Create some array of bool tracks for tracking execution of conversations
            bool[] conversationStarted = new bool[2];
            bool[] conversationFinished = new bool[2];

            // Setup three processes for testing, each with pre and post actions on the conversations that will set the above status flags.
            DummyCommProcess p1 = TestUtilities.SetupDummyCommProcess(1,
                (object context) => { conversationStarted[0] = true; },
                (object context) => { conversationFinished[0] = true; });

            DummyCommProcess p2 = TestUtilities.SetupDummyCommProcess(2,
                (object context) => { conversationStarted[1] = true; },
                (object context) => { conversationFinished[1] = true; });

            // Create the initiator Conversation for p1, using it's factory
            Conversations.InitiatorConversations.SimpleRequestReply initiatorConv =
                p1.CommSubsystem.ConversationFactory.CreateFromConversationType<Conversations.InitiatorConversations.SimpleRequestReply>();

            // Set the target of this conversation to be p2, using an end point
            initiatorConv.TargetEndPoint = p2.MyProcessInfo.EndPoint;

            Assert.IsFalse(conversationStarted[0]);
            Assert.IsFalse(conversationFinished[0]);

            Assert.IsFalse(conversationStarted[1]);
            Assert.IsFalse(conversationFinished[1]);

            // Run the Conversation directly with a context of 0 for testing purposes.  Note that the .Launch method would run it on its own thread.
            initiatorConv.Execute(0);

            Assert.IsTrue(conversationStarted[0]);
            Assert.IsTrue(conversationFinished[0]);

            Assert.IsTrue(conversationStarted[1]);
            Assert.IsTrue(conversationFinished[1]);
        }

        [TestMethod]
        public void RequestReply_SimplyTestThroughProxy()
        {
            // Create some array of bool tracks for tracking execution of conversations
            bool[] conversationStarted = new bool[2];
            bool[] conversationFinished = new bool[2];

            // Setup three processes for testing, each with pre and post actions on the conversations that will set the above status flags.
            DummyCommProcess p1 = TestUtilities.SetupDummyCommProcess(1,
                (object context) => { conversationStarted[0] = true; },
                (object context) => { conversationFinished[0] = true; });

            DummyCommProcess p2 = TestUtilities.SetupDummyCommProcess(2,
                (object context) => { conversationStarted[1] = true; },
                (object context) => { conversationFinished[1] = true; });

            // Setup a dictionary that make process ids to end points.  Note that the processes
            // really don't exist for this test case, just the communicators
            Dictionary<int, PublicEndPoint> idsToEndPoints = new Dictionary<int, PublicEndPoint>
            {
                {1, p1.MyProcessInfo.EndPoint },
                {2, p2.MyProcessInfo.EndPoint }
            };

            // Create and the proxy.  This proxy is a light-weight simulution of the real proxy.
            DummyProxy proxy = new DummyProxy()
            {
                IdsToEndpoints = idsToEndPoints
            };
            proxy.Start();

            // Setup the proxy end points.  Note that this would typically be done in the login conversation
            // using the proxy end point returned in the reply
            p1.ProxyEndPoint = proxy.EndPoint;
            p2.ProxyEndPoint = proxy.EndPoint;

            // Create the initiator Conversation for p1, using it's factory
            Conversations.InitiatorConversations.SimpleRequestReply initiatorConv =
                p1.CommSubsystem.ConversationFactory.CreateFromConversationType<Conversations.InitiatorConversations.SimpleRequestReply>();

            // Set the target of this conversation to be p2, using a process id
            initiatorConv.ToProcessId = 2;

            Assert.IsFalse(conversationStarted[0]);
            Assert.IsFalse(conversationFinished[0]);

            Assert.IsFalse(conversationStarted[1]);
            Assert.IsFalse(conversationFinished[1]);

            // Run the Conversation directly with a context of 0 for testing purposes.  Note that the .Launch method would run it on its own thread.
            initiatorConv.Execute(0);

            Assert.IsTrue(conversationStarted[0]);
            Assert.IsTrue(conversationFinished[0]);

            Assert.IsTrue(conversationStarted[1]);
            Assert.IsTrue(conversationFinished[1]);           
        }

        public class DummyProxy
        {
            private Communicator _communicator;
            private bool _keepGoing;
            private Thread _myThread;
            private PublicEndPoint _myEndPoint;

            public PublicEndPoint EndPoint { get { return _myEndPoint; } }
            public Dictionary<int, PublicEndPoint> IdsToEndpoints { get; set; }

            public void Start()
            {
                _communicator = new Communicator()
                {
                    MinPort = 15000,
                    MaxPort = 15999
                };
                _communicator.Start();
                _myEndPoint = new PublicEndPoint() {Host = "127.0.0.1", Port = _communicator.Port};

                _keepGoing = true;
                _myThread = new Thread(Process);
                _myThread.Start();

            }

            public void Stop()
            {
                _keepGoing = false;
                _myThread.Join();
                _communicator.Stop();
            }

            private void Process()
            {
                while (_keepGoing)
                {
                    Envelope env = _communicator.Receive(1000);
                    if (env != null)
                    {
                        Routing routing = env.Message as Routing;
                        Assert.IsNotNull(routing);
                        Assert.IsTrue(routing.ToProcessIds.Length>0);

                        Dictionary<int, PublicEndPoint>.Enumerator iterator = IdsToEndpoints.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            env.EndPoint = iterator.Current.Value;
                            _communicator.Send(env);
                        }
                    }
                }
            }
        }
    }
}
