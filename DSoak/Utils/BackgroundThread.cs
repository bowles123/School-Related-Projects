using System;
using System.Threading;
using log4net;

namespace Utils
{
    public abstract class BackgroundThread : IDisposable
    {
        #region Private data members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackgroundThread));

        private Thread _processThread;
        private bool _suspended;

        protected bool KeepGoing { get; set; }

        #endregion

        #region Constructors and destruction
        public void Dispose()
        {
            Stop();
        }
        #endregion

        #region Public Properties and Methods
        public string Label { get; set; }

        public virtual void Start()
        {

            try
            {
                KeepGoing = true;
                _suspended = false;
                Logger.Info("Starting " + Label);
                ThreadPool.QueueUserWorkItem(Process, null);
            }
            catch (Exception err)
            {
                Logger.Fatal("Aborted exception caught", err);
            }
        }

        public virtual void Stop()
        {
            Logger.Info("Stopping " + Label);
            KeepGoing = false;                      // Clear the flag that keep the background
            Thread.Sleep(0);

            // thread in its main loop
            if (IsRunning)
                _processThread.Join();                        // Wait for background thread to terminate

            _processThread = null;                            // deference the background thread so it will be
                                                        // garabage collected
            Logger.Debug("Leaving Stop");
        }

        public bool IsRunning
        {
            get { return (_processThread != null && _processThread.IsAlive); }
        }

        public virtual string Status
        {
            get
            {
                string result = "Not running";
                if (KeepGoing)
                    result = (_suspended) ? "Suspended" : "Running";
                return result;
            }
        }

        public virtual bool Suspended
        {
            get { return _suspended; }
            set
            {
                _suspended = value;
                Logger.Info(Label + " - " + Status);
            }
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Main process method for background thread
        /// 
        /// This method should stop whatever it is doing and terminate whenever KeepGoing becomes false. 
        /// Also, it should not actually do any process anything but stay alive, if suspend becomes true.
        /// </summary>
        protected abstract void Process(Object state);

        #endregion


    }
}
