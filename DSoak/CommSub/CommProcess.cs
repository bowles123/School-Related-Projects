using System.ServiceModel.Security;
using System.Threading;

using log4net;

using SharedObjects;
using Utils;

namespace CommSub
{
    /// <summary>
    /// This class can be a base class for any process that needs to use the communication subsystem.  Such a
    /// process is an active object that run with behavior that needs to run on a background thread.  So, this
    /// class inherits from BackgroundThread.
    /// 
    /// A specialization of this class will need to implement the active behavior by overriding the Process method.
    /// Before exit, the Process method should call Stop() to stop communication subsystem.
    /// 
    /// This class contains a communicationsubsystem, runtime options, the Proxy's end point, an error history, and
    /// some convenient methods for working with these things.
    /// 
    /// It also has a Shutdown event that could be used to wire up the hanlding of a shutdown message.  A
    /// shutdown would raise the event.  The GUI and other active objects could be listening for that event
    /// and stop one it occurs.
    /// </summary>
    public abstract class CommProcess : BackgroundThread
    {
        #region Private Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommProcess));

        protected const int MainLoopSleep = 200;
        protected object MyLock = new object();
        private readonly ErrorList _myErrorHistory = new ErrorList();
        #endregion

        #region Communication Subsystem Accessors
        public PublicEndPoint RegistryEndPoint { get; set; }
        public PublicEndPoint ProxyEndPoint { get; set; }
        public PublicEndPoint PennyBankEndPoint { get; set; }
        public CommSubsystem CommSubsystem { get; protected set; }
        public Communicator MyCommunicator { get { return (CommSubsystem==null) ? null : CommSubsystem.Communicator; } }
        public Dispatcher MyDispatcher { get { return (CommSubsystem==null) ? null : CommSubsystem.Dispatcher; } }
        #endregion


        #region Constructors, Initializers, Destructors
        public virtual void SetupCommSubsystem(ConversationFactory conversationFactory, int minPort=12000, int maxPort=12999)
        {
            CommSubsystem = new CommSubsystem() { ConversationFactory = conversationFactory, MaxPort = maxPort, MinPort = minPort };
            CommSubsystem.Initialize(this);
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
            CleanupProcess();

            Logger.DebugFormat("Leaving Stop, with Status={0}", (MyProcessInfo == null) ? "null" : MyProcessInfo.Status.ToString());
        }

        /// <summary>
        /// This method cleans up a process after logging out and stopping it communication subsystem.  Specialization is this class
        /// should override this method if they need to any special clean up before terminating.
        /// 
        /// Note that this method is called by the Stop method
        /// </summary>
        protected virtual void CleanupProcess()
        {
            if (MyProcessInfo!=null)
                MyProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;
        }

        #endregion

        #region Public Process Stuff
        public RuntimeOptions Options { get; set; }
        public ErrorList ErrorHistory { get { return _myErrorHistory; } }
        public ProcessInfo MyProcessInfo { get; set; }
        public bool IsInitialized { get { return MyProcessInfo != null && MyProcessInfo.Status != ProcessInfo.StatusCode.NotInitialized; } }

        public string StatusString
        {
            get
            {
                return (MyProcessInfo == null) ? string.Empty : MyProcessInfo.StatusString;
            }
        }

        public void BeginShutdown()
        {
            if (MyProcessInfo!=null)
                MyProcessInfo.Status = ProcessInfo.StatusCode.Terminating;
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

        /// <summary>
        /// This method should clean up a session, like the playing of one game.  Specialization of this
        /// class should override this method if they need to do any special cleanup between sessions.
        /// 
        /// This method is not automatically by anything in this base classes.  Specializations or
        /// conversations should call it as needed.
        /// </summary>
        public virtual void CleanupSession()
        {
            MyProcessInfo.Status = ProcessInfo.StatusCode.Pausing;

            if (ErrorHistory != null)
                ErrorHistory.Clear();
        }

        #endregion


        #region Public Events and Methods for Raising Events
        public event StateChange.Handler Shutdown;

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

    }

}
