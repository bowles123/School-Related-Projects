using System.Threading;

using CommSub;
using SharedObjects;

using log4net;

namespace CommSubTesting
{
    public class DummyCommProcess : CommProcess
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DummyCommProcess));

        public int AssignedProcessId { get; set; }

        // Callback method for testing purposes
        public delegate void Callback(Envelope env);

        public override void Start()
        {
            Logger.Debug("Enter Start");

            MyProcessInfo = new ProcessInfo
            {
                ProcessId = AssignedProcessId,
                Label = Options.Label,
                Type = ProcessInfo.ProcessType.Player,
                Status = ProcessInfo.StatusCode.NotInitialized
            };

            DummyConversationFactory conversationFactory = new DummyConversationFactory()
            {
                Process = this,
                DefaultMaxRetries = Options.Retries,
                DefaultTimeout = Options.Timeout
            };

            SetupCommSubsystem(conversationFactory, Options.MinPort, Options.MaxPort);

            MyProcessInfo.EndPoint = new PublicEndPoint() {Host = "127.0.0.1", Port = MyCommunicator.Port};
            Logger.DebugFormat("Process {0}'s End Point = {1}", MyProcessInfo.ProcessId, MyProcessInfo.EndPoint);

            base.Start();
        }

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
