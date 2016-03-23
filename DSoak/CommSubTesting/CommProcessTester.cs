using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;
using log4net;
using SharedObjects;

namespace CommSubTesting
{
    [TestClass]
    public class CommProcessTester
    {
        private int _shutdownEventCount;
        [TestMethod]
        public void CommProcess_TestPublicProperties()
        {
            // Test communication properties and acceessors properties

            DummyCommProcess p = new DummyCommProcess()
            {
                RegistryEndPoint = new PublicEndPoint() {HostAndPort = "10.12.14.16:12001"}
            };
            Assert.AreEqual("10.12.14.16:12001", p.RegistryEndPoint.ToString());

            p.RegistryEndPoint = new PublicEndPoint() { HostAndPort = "127.0.0.1:12001" };
            Assert.AreEqual("127.0.0.1:12001", p.RegistryEndPoint.ToString());

            Assert.IsNull(p.CommSubsystem);
            Assert.IsNull(p.MyCommunicator);
            Assert.IsNull(p.MyDispatcher);

            // Test process-related properties and assessors
            RuntimeOptions options = new DummyRuntimeOptions();
            options.SetDefaults();
            p.Options = options;
            Assert.AreSame(options, p.Options);

            p.Options = new DummyRuntimeOptions();
            p.Options.SetDefaults();
            Assert.AreNotSame(options, p.Options);

            Assert.IsNotNull(p.ErrorHistory);

            Assert.IsNull(p.MyProcessInfo);
            Assert.IsFalse(p.IsInitialized);

            // Note that a login converation would typically receive the process information back in a login reply
            p.MyProcessInfo = new ProcessInfo() {EndPoint = new PublicEndPoint() {HostAndPort = "127.0.0.1:12002"}, Type = ProcessInfo.ProcessType.Player, Status = ProcessInfo.StatusCode.PlayingGame};
            Assert.AreEqual("127.0.0.1:12002", p.MyProcessInfo.EndPoint.ToString());
            Assert.AreEqual(ProcessInfo.ProcessType.Player, p.MyProcessInfo.Type);
            Assert.AreEqual(ProcessInfo.StatusCode.PlayingGame, p.MyProcessInfo.Status);
            Assert.IsTrue(p.IsInitialized);
            Assert.AreEqual("Playing Game", p.StatusString);

            // Note that a conversation would typically be the component to change the process's status
            p.MyProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;
            Assert.IsFalse(p.IsInitialized);
            Assert.AreEqual("Not Initialized", p.StatusString);


        }

        [TestMethod]
        public void CommProcess_TestSetupStartingStopping()
        {
            RuntimeOptions options = new DummyRuntimeOptions();
            options.SetDefaults();

            DummyCommProcess p = new DummyCommProcess()
            {
                Options = options
            };

            p.SetupCommSubsystem(new DummyConversationFactory());
            Assert.IsNotNull(p.CommSubsystem);
            Assert.IsNotNull(p.MyDispatcher);
            Assert.IsNotNull(p.MyCommunicator);
            Assert.IsTrue(p.MyCommunicator.Port>=options.MinPort && p.MyCommunicator.Port<=options.MaxPort);

            p.RegistryEndPoint = new PublicEndPoint() { HostAndPort = "127.0.0.1:12001" };
            p.MyProcessInfo = new ProcessInfo()
            {
                EndPoint = new PublicEndPoint() { Host = "127.0.0.1", Port = p.MyCommunicator.Port },
                Type = ProcessInfo.ProcessType.Player,
                Status = ProcessInfo.StatusCode.PlayingGame
            };

            p.Shutdown += ShutdownEventHandler;

            _shutdownEventCount = 0;
            p.ProcessLoopCount = 0;
            p.CleanupSessionCalled = false;
            p.CleanupProcessCalled = false;

            p.Start();

            Thread.Sleep(1000);
            
            Assert.IsTrue(p.ProcessLoopCount > 9);

            // Begin a shutdown. This would typically be down in a Shutdown conversation
            p.BeginShutdown();
            Assert.AreEqual(ProcessInfo.StatusCode.Terminating, p.MyProcessInfo.Status);

            // Wait for shutdown to complete
            p.WaitToCloseDown(1000);

            Assert.AreEqual(1, _shutdownEventCount);
            Assert.IsTrue(p.CleanupProcessCalled);

            // Make sure the Process method is not running any more
            int loopcount = p.ProcessLoopCount;
            Thread.Sleep(1000);
            Assert.AreEqual(loopcount, p.ProcessLoopCount);
        }

        void ShutdownEventHandler(StateChange changeInfo)
        {
            Assert.AreEqual(StateChange.ChangeType.Shutdown, changeInfo.Type);
            _shutdownEventCount++;
        }

    }

    public class DummyCommProcess : CommProcess
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DummyCommProcess));

        protected override void Process(object state)
        {
            while (KeepGoing && MyProcessInfo.Status != ProcessInfo.StatusCode.Terminating)
            {
                Logger.Info("Processing...");
                ProcessLoopCount++;
                Thread.Sleep(100);
            }

            Stop();
        }


        protected override void CleanupProcess()
        {
            CleanupProcessCalled = true;
        }

        public override void CleanupSession()
        {
            CleanupSessionCalled = true;
        }

        public int ProcessLoopCount { get; set; }
        public bool CleanupProcessCalled { get; set; }
        public bool CleanupSessionCalled { get; set; }
    }
}
