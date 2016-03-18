using System.Collections.Generic;

using Messages;
using Messages.RequestMessages;

using log4net;

namespace CommSub.Conversations.InitiatorConversations
{
    public abstract class UnreliableMulticastThruProxy : Conversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UnreliableMulticastThruProxy));

        public List<int> TargetProcessIds { get; set; }

        public override void Execute(object context = null)
        {
            Done = false;

            Logger.DebugFormat("In Execute for {0}", GetType().Name);

            if (IsConversationStateValid())
            {
                if (IsProcessStateValid())
                {
                    Request request = CreateRequest();
                    if (request != null)
                    {
                        Logger.DebugFormat("Sending message of type {0} to {1} processes", request.GetType().Name, TargetProcessIds.Count);
                        request.InitMessageAndConversationNumbers();
                        Envelope env = new Envelope()
                        {
                            Message = new Routing()
                            {
                                InnerMessage = request,
                                ToProcessIds = TargetProcessIds.ToArray()
                            },
                            EndPoint = Process.ProxyEndPoint
                        };

                        MyCommunicator.Send(env);
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
        protected virtual bool IsConversationStateValid() { return (TargetProcessIds != null && TargetProcessIds.Count > 0); }

        /// <summary>
        /// This method should return true when the process is in a valid state for this conversation to execute.
        /// Override this method in concrete specializations, when necessary.
        /// </summary>
        /// <returns>true if the process is in a state where it is okay for this conversation to execute</returns>
        protected virtual bool IsProcessStateValid() { return true; }


        public abstract Request CreateRequest();
    }
}
