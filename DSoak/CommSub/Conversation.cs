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

        /// <summary>
        /// This in the process in which the conversation is taking place.
        /// </summary>
        public CommProcess Process { get; set; }

        public CommSubsystem CommSubsystem { get { return (Process==null) ? null : Process.CommSubsystem; } }
        public Communicator MyCommunicator { get { return (CommSubsystem == null) ? null : CommSubsystem.Communicator; } }
        public Envelope IncomingEnvelope { get; set; }
        public int Timeout { get; set; }
        public int MaxRetries { get; set; }
        public bool Done { get; set; }
        public Error ConvError { get { return Error; } }

        public void Launch(object context = null)
        {
            bool result = ThreadPool.QueueUserWorkItem(Execute, context);
            Logger.DebugFormat("Launch result = {0}", result);
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
                Type messageType = env.Message.GetType();
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

        protected void UnreliableSend(Envelope envelope)
        {
            if (MyCommunicator!=null && envelope!=null)
                MyCommunicator.Send(envelope);
        }

        #endregion
    }
}
