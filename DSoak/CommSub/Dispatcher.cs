using System;

using Utils;

using log4net;

namespace CommSub
{
    public class Dispatcher : BackgroundThread
    {
        #region Private Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Dispatcher));
        private const int TimeoutMs = 1000;
        #endregion

        public CommSubsystem CommSubsystem { get; set; }

        protected override void Process(Object state)
        {
            while (KeepGoing)
            {
                Envelope e = CommSubsystem.Communicator.Receive(TimeoutMs);
                if (e != null && e.Message!=null)
                {
                    Logger.DebugFormat("Received message: Type={0}, From={1}", e.Message.GetType().Name, e.IPEndPoint);
                    EnqueueEnvelope(e);
                }
            }
        }

        private void EnqueueEnvelope(Envelope e)
        {
            EnvelopeQueue queue = CommSubsystem.QueueDictionary.GetByConvId(e.Message.ConvId);

            if (queue != null)
            {
                Logger.DebugFormat("Placing message in the queue for conversation {0}", e.Message.ConvId);
                queue.Enqueue(e);
            }
            else
            {
                if (CommSubsystem.ConversationFactory.IncomingMessageCanStartConversation(e.Message.GetType()))
                    Dispatch(e);
                else
                    Logger.WarnFormat("Unexcepted incoming message of type {0}", e.Message.GetType().Name);
            }
        }


        private void Dispatch(Envelope incomingRequestEnvelope)
        {
            Conversation conversation = CommSubsystem.ConversationFactory.CreateFromMessageType(incomingRequestEnvelope.Message.GetType(), incomingRequestEnvelope);
            if (conversation == null)
                Logger.WarnFormat("Cannot find strategy for {0}", incomingRequestEnvelope.Message.GetType().Name);
            else
            {
                Logger.DebugFormat("Dispatch request to strategy, type={0}, message from={1}", conversation.GetType().Name, incomingRequestEnvelope.IPEndPoint);
                conversation.Launch();
            }
        }
    }
}
