using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using log4net;

namespace Utils
{
    public abstract class BackgroundThread : IDisposable
    {
        #region Private data members
        private static readonly ILog log = LogManager.GetLogger(typeof(BackgroundThread));

        protected Thread myThread = null;
        protected bool keepGoing = false;
        protected bool suspended = false;
        #endregion

        #region Constructors and destruction
        public BackgroundThread() { }

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
                keepGoing = true;
                suspended = false;
                log.Info("Starting " + Label);
                ThreadPool.QueueUserWorkItem(Process, null);
            }
            catch (Exception err)
            {
                log.Fatal("Aborted exception caught", err);
            }
        }

        public virtual void Stop()
        {
            log.Info("Stopping " + Label);
            keepGoing = false;                      // Clear the flag that keep the background
            Thread.Sleep(0);

            // thread in its main loop
            if (IsRunning)
                myThread.Join();                        // Wait for background thread to terminate

            myThread = null;                            // deference the background thread so it will be
                                                        // garabage collected
            log.Debug("Leaving Stop");
        }

        public bool IsRunning
        {
            get { return (myThread != null && myThread.IsAlive); }
        }

        public virtual string Status
        {
            get
            {
                string result = "Not running";
                if (keepGoing)
                    result = (suspended) ? "Suspended" : "Running";
                return result;
            }
        }

        public virtual void Suspend()
        {
            if (Suspended == false)
                Suspended = true;
        }

        public virtual void Resume()
        {
            if (Suspended == true)
                Suspended = false;
        }

        public virtual bool Suspended
        {
            get { return suspended; }
            set
            {
                suspended = value;
                log.Info(Label + " - " + Status);
            }
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Main process method for background thread
        /// 
        /// This method should stop whatever it is doing and terminate whenever keepGoing becomes false. 
        /// Also, it should not actually do any process anything but stay alive, if suspend becomes true.
        /// </summary>
        protected abstract void Process(Object state);


        #endregion


    }
}
