using Messages;

namespace CommunicationSubsystem
{
    public abstract class TimeoutRetryConversation: Conversation
    {
        /// <summary>
        /// Envelope containing the conversation's response message.
        /// </summary>
        public Envelope Response { get; set; }
        public int RetryAmount { get; }
        protected int tries;
        private int retryAmount = 5;

        public TimeoutRetryConversation()
        {
            Timeout = 3000;
            RetryAmount = 5;
        }

        public TimeoutRetryConversation(int timeoutSeconds, int retries)
        {
            if (timeoutSeconds > 0)
            {
                Timeout = timeoutSeconds;
            }
            else
            {
                Timeout = 3000;
            }

            if (retries > 0)
            {
                RetryAmount = retries;
            }
            else
            {
                RetryAmount = 5;
            }
        }
    }
}
