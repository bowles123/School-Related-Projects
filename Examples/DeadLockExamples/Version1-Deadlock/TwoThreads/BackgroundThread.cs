using System;
using System.Threading;

namespace TwoThreads
{
    public abstract class BackgroundThread
    {
        #region Private data members
        protected bool KeepGoing { get; set; }
        #endregion

        #region Public Properties and Methods
        public virtual void Start()
        {

            try
            {
                KeepGoing = true;
                ThreadPool.QueueUserWorkItem(Process, null);
            }
            catch (Exception err)
            {
                Console.WriteLine("Aborted exception caught", err.ToString());
            }
        }

        public virtual void Stop()
        {
            KeepGoing = false;                              // Clear the flag that keep the background
            Thread.Sleep(0);                                // Give up the processor so other threads will run
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
