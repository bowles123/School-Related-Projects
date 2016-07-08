using System;
using SharedObjects;
using Utils;
using log4net;
using System.Threading;

namespace CommunicationSubsystem
{
    public abstract class CommunicationProcess: BackgroundThread
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommunicationProcess));

        protected const int MainLoopSleep = 200;

        public PublicKey PublicKey { get; set; }
        public PublicEndPoint RegistryEndPoint { get; set; }
        public PublicEndPoint ProxyEndPoint { get; set; }
        public PublicEndPoint PennyBankEndPoint { get; set; }
        public CommSubsystem CommSubsystem { get; protected set; }
        public Communicator MyCommunicator { get { return (CommSubsystem == null) ? null : CommSubsystem.Communicator; } }
        public Dispatcher MyDispatcher { get { return (CommSubsystem == null) ? null : CommSubsystem.Dispatcher; } }
        public ProcessInfo MyProcessInfo { get; set; }
        public bool IsInitialized { get { return MyProcessInfo != null && MyProcessInfo.Status != ProcessInfo.StatusCode.NotInitialized; } }
        public event StateChange.Handler Shutdown;
        public string StatusString
        {
            get
            {
                return (MyProcessInfo == null) ? string.Empty : MyProcessInfo.StatusString;
            }
        }

        /// <summary>
        /// Sets up the communication process's communication subsystem.
        /// </summary>
        public virtual void SetupCommSubsystem(ConversationFactory conversationFactory)
        {
            CommSubsystem = new CommSubsystem()
            {
                Factory = conversationFactory,
                Communicator = conversationFactory.Communicator,
                Dictionary = conversationFactory.Dictionary,
                Dispatcher = new Dispatcher()
                {
                    Communicator = conversationFactory.Communicator,
                    Factory = conversationFactory,
                    Dictionary = conversationFactory.Dictionary
                }
            };
        }

        /// <summary>
        /// Stops the communication process's processing.
        /// </summary>
        public override void Stop()
        {
            Logger.DebugFormat("Entering Stop, with Status={0}", (MyProcessInfo == null) ? "null" : MyProcessInfo.Status.ToString());
            if (MyProcessInfo != null)
                MyProcessInfo.Status = ProcessInfo.StatusCode.Terminating;

            Logger.DebugFormat("Status={0}", (MyProcessInfo == null) ? "null" : MyProcessInfo.Status.ToString());

            base.Stop();
            CleanupProcess();

            Logger.DebugFormat("Leaving Stop, with Status={0}", (MyProcessInfo == null) ? "null" : MyProcessInfo.Status.ToString());
        }

        /// <summary>
        /// Begins the shutdown process for the communication process.
        /// </summary>
        public void BeginShutdown()
        {
            if (MyProcessInfo != null)
                MyProcessInfo.Status = ProcessInfo.StatusCode.Terminating;
            RaiseShutdownEvent();
        }

        /// <summary>
        /// Raises shutdown event for the communication process.
        /// </summary>
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
        /// Cleans up communcation process.
        /// </summary>
        protected virtual void CleanupProcess()
        {
            if (MyProcessInfo != null)
                MyProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;
        }

        public virtual void CleanupSession() { }
    }
}
