using System.Threading;

using log4net;

using SharedObjects;
using Utils;

namespace CommSub
{
    public abstract class CommProcess : BackgroundThread
    {
        #region Private Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommProcess));

        protected const int MainLoopSleep = 200;
        protected object MyLock = new object();
        #endregion

        #region Public Events
        public event StateChange.Handler Shutdown;
        #endregion

        #region Public Properties
        public RuntimeOptions Options { get; set; }
        public PublicEndPoint ProxyEndPoint { get; set; }
        #endregion

        #region Accessors
        public CommSubsystem CommSubsystem { get; protected set; }
        public bool IsInitialized { get { return MyProcessInfo!=null && MyProcessInfo.Status != ProcessInfo.StatusCode.NotInitialized; } }
        public Communicator MyCommunicator { get { return CommSubsystem.Communicator; } }
        public Dispatcher MyDispatcher { get { return CommSubsystem.Dispatcher; } }
        #endregion

        #region Constructors, Initializers, Destructors
        public virtual void SetupCommSubsystem(ConversationFactory conversationFactory)
        {
            CommSubsystem = new CommSubsystem() { ConversationFactory = conversationFactory, MaxPort = Options.MaxPort, MinPort = Options.MinPort };
            CommSubsystem.Initialize();
            CommSubsystem.Start();
        }

        public override void Stop()
        {
            Logger.DebugFormat("Entering Stop, with Status={0}", (MyProcessInfo==null) ? "null" : MyProcessInfo.Status.ToString());
            if (MyProcessInfo!=null)
                MyProcessInfo.Status = ProcessInfo.StatusCode.Terminating;

            Logger.DebugFormat("Status={0}", (MyProcessInfo == null) ? "null" : MyProcessInfo.Status.ToString());

            base.Stop();
            CommSubsystem.Stop();
            Cleanup();

            if (MyProcessInfo != null)
                MyProcessInfo.Status = ProcessInfo.StatusCode.Unknown;

            Logger.DebugFormat("Leaving Stop, with Status={0}", (MyProcessInfo == null) ? "null" : MyProcessInfo.Status.ToString());
        }

        public virtual void Cleanup()
        {
            MyProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;
        }

        #endregion

        #region Event Raising Methods

        public void RecordError(Error error)
        {
            // TODO: Record error
        }

        public void RaiseShutdownEvent()
        {
            Logger.Debug("Enter RaiseShutdownEvent");
            if (Shutdown != null)
            {
                Logger.Debug("Raise Shutdown event");
                Shutdown(new StateChange() { Type = StateChange.ChangeType.Shutdown, Subject = this });
                Logger.Debug("Back from raising Shutdown event");
            }
            else
                Logger.Debug("Nobody is registered for the shutdown");
            Logger.Debug("Leave RaiseShutdownEvent");
        }

        #endregion

        #region Public Process Stuff
        public bool Quit { get; set; }
        public ProcessInfo MyProcessInfo { get; set; }
        public PublicEndPoint RegistryEndPoint { get; set; }

        public string StatusString
        {
            get
            {
                return (MyProcessInfo == null) ? string.Empty : MyProcessInfo.StatusString;
            }
        }

        public void ChangeStatus(ProcessInfo.StatusCode newStatus)
        {
            MyProcessInfo.Status = newStatus;
        }

        public void DoShutdown()
        {
            MyProcessInfo.Status = ProcessInfo.StatusCode.Terminating;
            Quit = true;
            RaiseShutdownEvent();
        }

        public void WaitToCloseDown(int timeout)
        {
            Logger.Debug("Enter WaitToCloseDown");
            while (timeout > 0 && IsInitialized)
            {
                Thread.Sleep(500);
                timeout -= 500;
                if (IsInitialized)
                    Logger.DebugFormat("Waiting for another {0} ms", timeout);
                else
                    Logger.Debug("Shutdown complete");
            }
        }

        public virtual void Reset()
        {
            
        }
        #endregion
    }
}
