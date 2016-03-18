using System;
using SharedObjects;

using Messages;
using log4net;

namespace CommSub.Conversations.ResponderConversations
{
    public abstract class RequestReply : Conversation
    {
        #region Private Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RequestReply));

        private Routing _routingMessage;
        private Message _request;
        private int _fromProcessId;
        #endregion

        protected Message Request { get { return _request; } }
        protected int FromProcessId { get { return _fromProcessId; } }

        /// <summary>
        /// Execute the responder side of a request-reply conversation
        /// 
        /// Note that this is a temple method, four override parts:
        ///     - AllowTypes
        ///     - RemoteProcessMustBeKnown
        ///     - RemoteProcessMustBeInGame
        ///     - IsConverstationStateValid
        ///     - IsProcessStateValid
        ///     - CreateReply
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(object context = null)
        {
            Done = false;
            Logger.DebugFormat("Start {0} Conversation", GetType().Name);

            if (IsEnvelopeValid(IncomingEnvelope, AllowedTypes))
            {
                if (!IsConversationStateValid())
                    Error = Error.Get(Error.StandardErrorNumbers.InvalidConversationSetup);
                else if (!IsProcessStateValid())
                    Error = Error.Get(Error.StandardErrorNumbers.InvalidProcessStateForConversation);
                else
                {
                    _routingMessage = IncomingEnvelope.Message as Routing;
                    if (_routingMessage!=null)
                    {
                        _request = _routingMessage.InnerMessage;
                        _fromProcessId = _routingMessage.FromProcessId;
                    }
                    else
                    {
                        _request = IncomingEnvelope.Message;
                        _fromProcessId = _request.MsgId.Pid;
                    }

                    Logger.DebugFormat("Reply to {0} message from {1}", _request.GetType().Name, _fromProcessId);

                    Message reply = CreateReply();
                    reply.SetMessageAndConversationNumbers(MessageNumber.Create(), _request.ConvId);

                    Envelope replyEnv = new Envelope();
                    if (_routingMessage != null)
                    {
                        replyEnv.Message = new Routing()
                        {
                            InnerMessage = reply,
                            ToProcessIds = new[] { _fromProcessId }
                        };
                        replyEnv.EndPoint = Process.ProxyEndPoint;
                    }
                    else
                    {
                        replyEnv.Message = reply;
                        replyEnv.EndPoint = IncomingEnvelope.EndPoint;
                    }

                    UnreliableSend(replyEnv);

                    Logger.DebugFormat("Replied with a {0}", reply.GetType().Name);
                }
            }

            if (Error != null)
            {
                Logger.Warn(Error.Message);
                Process.RaiseErrorOccurredEvent(Error);
            }

            Done = true;
            Logger.DebugFormat("End {0}", GetType().Name);
        }

        /// <summary>
        /// This method returns an array of types of valid requests.  The concrete conversation must implement this method.
        /// </summary>
        protected abstract Type[] AllowedTypes { get; }

        /// <summary>
        /// This method should return true when the conversation object contains the necessary information to execute.
        /// Override this method in concrete specializations, when necessary.
        /// </summary>
        /// <returns>true is the conversation contains all of the necessary information</returns>
        protected virtual bool IsConversationStateValid() { return true; }

        /// <summary>
        /// This method should return true when the process is in a valid state for this conversation to execute.
        /// Override this method in concrete specializations, when necessary.
        /// </summary>
        /// <returns>true if the process is in a state where it is okay for this conversation to execute</returns>
        protected virtual bool IsProcessStateValid() { return true; }

        /// <summary>
        /// This method creates a reply to the request.  It must be implemented in the concrete conversation
        /// </summary>
        /// <returns></returns>
        protected abstract Message CreateReply();
    }
}
