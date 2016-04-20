using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using SharedObjects;

using log4net;

namespace CommSub
{
    public abstract class Conversation
    {
        #region Private and Protected Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Conversation));

        /// <summary>
        /// Hold the identitifier for this conversation's queue
        /// </summary>
        protected MessageNumber QueueId;

        /// <summary>
        /// If an error occurs during the conversation, it should be saved in this data member
        /// </summary>
        protected Error Error;
        #endregion

        #region Public Properties and Methods
        public delegate void CallbackWithRegistryEntries(List<ProcessInfo> data);
        public delegate void CallbackWithGameInfo(List<GameInfo> data);
        public delegate void ActionHandler(object context);

        public ActionHandler PreExecuteAction { get; set; }
        public ActionHandler PostExecuteAction { get; set; }


        /// <summary>
        /// This in the process in which the conversation is taking place.
        /// </summary>
        public CommProcess Process { get; set; }
        
        /// <summary>
        /// This is the process's communication subsystem
        /// </summary>
        public CommSubsystem CommSubsystem { get { return (Process == null) ? null : Process.CommSubsystem; } }
        
        /// <summary>
        /// This is the communication subsystem's communicator
        /// </summary>
        public Communicator MyCommunicator { get { return (CommSubsystem == null) ? null : CommSubsystem.Communicator; } }
        
        /// <summary>
        /// For conversations started by an incoming message, this is the message
        /// </summary>
        public Envelope IncomingEnvelope { get; set; }
        
        /// <summary>
        /// For conversations that will have a timeout, this is the timeout value in milliseconds
        /// </summary>
        public int Timeout { get; set; }
        
        /// <summary>
        /// For conversation that can resend and retry the waiting for a reply, this is the maximum number of retries
        /// </summary>
        public int MaxRetries { get; set; }
        
        /// <summary>
        /// This is set to true when the conversation finishes
        /// </summary>
        public bool Done { get; set; }

        /// <summary>
        /// This hold an error, if the conversation resulted in an error
        /// </summary>
        public Error ConvError { get { return Error; } }

        public void Launch(object context = null)
        {
            bool result = ThreadPool.QueueUserWorkItem(Execute, context);
            Logger.DebugFormat("Launch of {0}, result = {1}", GetType().Name, result);
        }

        public abstract void Execute(object context = null);
        #endregion

        #region Private and Protected Methods
        protected bool IsEnvelopeValid(Envelope env, params Type[] allowedTypes)
        {
            bool result = false;
            Error = null;
            Logger.Debug("Checking to see if envelope is valid and message of appropriate type");
            if (env == null || env.Message == null)
                Error = Error.Get(Error.StandardErrorNumbers.NullEnvelopeOrMessage);
            else if (env.Message.MsgId == null)
                Error = Error.Get(Error.StandardErrorNumbers.NullMessageNumber);
            else if (env.Message.ConvId == null)
                Error = Error.Get(Error.StandardErrorNumbers.NullConversationId);
            else if (!IsValidRemoteProcess(env.Message.MsgId.Pid))
            {
                Error = Error.Get(Error.StandardErrorNumbers.ProcessIdInMessageNumberIsNotAProcessId);
                Error.Message += string.Format(": LocalProcessId in MsgId = {0}", env.Message.MsgId.Pid);
            }
            else
            {
                Type messageType = env.ActualMessageType;
                
                Logger.DebugFormat("See if {0} is valid for a {1} conversation", messageType, GetType().Name);
                if (!allowedTypes.Contains(messageType))
                {
                    Error = Error.Get(Error.StandardErrorNumbers.InvalidTypeOfMessage);
                    Error.Message += ". Allowed type(s): " +
                                     allowedTypes.Aggregate(string.Empty, (current, t) => current + t.ToString());
                }
                else
                    result = true;
                    
            }

            if (Error!=null)
                Logger.Error(Error.Message);

            return result;
        }

        protected virtual bool IsValidRemoteProcess(int processId)
        {
            return true;
        }

        protected bool UnreliableSend(Envelope envelope)
        {
            return (MyCommunicator != null && envelope != null) && MyCommunicator.Send(envelope);
        }

        #endregion
    }
}
