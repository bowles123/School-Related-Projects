using System.Collections.Generic;

using Messages;

using SharedObjects;
using log4net;

namespace CommSub.Conversations.InitiatorConversations
{
    public abstract class UnreliableMulticast : Conversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UnreliableMulticast));

        public List<PublicEndPoint> TargetEndPoints { get; set; }
        public List<int> TargetProcessIds { get; set; }

        public override void Execute(object context = null)
        {
            Logger.Debug("In Execute");
            Done = false;

            if (PreExecuteAction != null)
                PreExecuteAction(context);

            if (IsConversationStateValid())
            {
                if (IsProcessStateValid())
                {
                    Message message = CreateMessage();
                    if (message != null)
                    {
                        if (TargetEndPoints!=null && TargetEndPoints.Count>0)
                            DirectMulticast(message);

                        if (TargetProcessIds!=null && TargetProcessIds.Count>0)
                            IndirectMulticast(message);
                    }
                    else
                        Error = new Error() {Message = "Cannot create message for unreliable multicast"};
                }
                else
                    Error = Error.Get(Error.StandardErrorNumbers.InvalidProcessStateForConversation);
            }
            else
                Error = Error.Get(Error.StandardErrorNumbers.InvalidConversationSetup);

            if (Error != null)
            {
                Logger.Warn(Error.Message);
            }

            Done = true;

            if (PostExecuteAction != null)
                PostExecuteAction(context);

            Logger.DebugFormat("End {0}", GetType().Name);
        }

        private void DirectMulticast(Message message)
        {
            Logger.DebugFormat("Sending message of type {0} to {1} processes", message.GetType().Name, TargetEndPoints.Count);
            message.InitMessageAndConversationNumbers();
            Envelope env = new Envelope() {Message = message};

            foreach (PublicEndPoint ep in TargetEndPoints)
            {
                env.EndPoint = ep;
                MyCommunicator.Send(env);
            }
        }

        private void IndirectMulticast(Message message)
        {
            Logger.DebugFormat("Sending message of type {0} to {1} processes", message.GetType().Name, TargetProcessIds.Count);
            message.InitMessageAndConversationNumbers();
            Envelope env = new Envelope()
            {
                Message = new Routing()
                {
                    InnerMessage = message,
                    ToProcessIds = TargetProcessIds.ToArray()
                },
                EndPoint = Process.ProxyEndPoint
            };

            MyCommunicator.Send(env);
        }

        /// <summary>
        /// This method should return true when the conversation object contains the necessary information to execute.
        /// Override this method in concrete specializations, when necessary.
        /// </summary>
        /// <returns>true is the conversation contains all of the necessary information</returns>
        protected virtual bool IsConversationStateValid()
        {
            return (TargetEndPoints != null && TargetEndPoints.Count > 0) || 
                    (TargetProcessIds!=null && TargetProcessIds.Count>0);
        }

        /// <summary>
        /// This method should return true when the process is in a valid state for this conversation to execute.
        /// Override this method in concrete specializations, when necessary.
        /// </summary>
        /// <returns>true if the process is in a state where it is okay for this conversation to execute</returns>
        protected virtual bool IsProcessStateValid() { return true; }

        protected abstract Message CreateMessage();
    }
}
