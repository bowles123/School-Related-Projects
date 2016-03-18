using System.Collections.Generic;

using Messages.RequestMessages;

using SharedObjects;
using log4net;

namespace CommSub.Conversations.InitiatorConversations
{
    public abstract class UnreliableMulticast : Conversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UnreliableMulticast));

        public List<PublicEndPoint> TargetEndPoints { get; set; }

        public override void Execute(object context = null)
        {
            Logger.Debug("In Execute");
            Done = false;

            if (IsConversationStateValid())
            {
                if (IsProcessStateValid())
                {
                    Request request = CreateRequest();
                    if (request != null)
                    {
                        Logger.DebugFormat("Sending message of type {0} to {1} processes", request.GetType().Name, TargetEndPoints.Count);
                        request.InitMessageAndConversationNumbers();
                        Envelope env = new Envelope() { Message = request };

                        foreach (PublicEndPoint ep in TargetEndPoints)
                        {
                            env.EndPoint = ep;
                            MyCommunicator.Send(env);
                        }
                    }
                    else
                        Error = new Error() { Message = "Cannot create message for unreliable multicast" };
                }
                else
                    Error = Error.Get(Error.StandardErrorNumbers.InvalidProcessStateForConversation);
            }
            else
                Error = Error.Get(Error.StandardErrorNumbers.InvalidConversationSetup);

            if (Error != null)
            {
                Logger.Warn(Error.Message);
                Process.RaiseErrorOccurredEvent(Error);
            }

            Done = true;
            Logger.DebugFormat("End {0}", GetType().Name);
        }

        /// <summary>
        /// This method should return true when the conversation object contains the necessary information to execute.
        /// Override this method in concrete specializations, when necessary.
        /// </summary>
        /// <returns>true is the conversation contains all of the necessary information</returns>
        protected virtual bool IsConversationStateValid() { return (TargetEndPoints != null && TargetEndPoints.Count > 0); }

        /// <summary>
        /// This method should return true when the process is in a valid state for this conversation to execute.
        /// Override this method in concrete specializations, when necessary.
        /// </summary>
        /// <returns>true if the process is in a state where it is okay for this conversation to execute</returns>
        protected virtual bool IsProcessStateValid() { return true; }

        public abstract Request CreateRequest();
    }
}
