using SharedObjects;
using System.Collections.Generic;
using Utils;
using log4net;
using System.Security.Cryptography;

namespace CommunicationSubsystem
{
    public abstract class Conversation : BackgroundThread
    {
        protected static readonly ILog logger = LogManager.GetLogger(typeof(Conversation));

        /// <summary>
        /// Envelope containing the conversation's request message.
        /// </summary>
        public Envelope Request { get; set; }
        public ProcessInfo.ProcessType ProcessType { get; set; }
        public Communicator Communicator { get; set; }
        public EnvelopeQueue Queue { get; set; }
        public IdentityInfo Identity { get; set; }
        public ProcessInfo MyProcess { get; set; }
        public GameInfo Game { get; set; }
        public GameInfo[] Games { get; set; }
        public ConversationDictionary Dictionary { get; set; }
        public static int LaunchCount { get; set; }
        public Queue<Penny> Pennies { get; set; }
        public PublicKey PennyKey { get; set; }
        public int BalloonStoreId { get; set; }
        public int WaterSourceId { get; set; }
        public int PennyBankId { get; set; }
        public int CurrentGameId { get; set; }
        public int LifePoints { get; set; }
        public Queue<Balloon> Balloons { get; set; }
        public Queue<Balloon> FilledBalloons { get; set; }
        public int Timeout { get; set; }
        public ConversationFactory Factory { get; set; }
        public bool Alive { get; set; }
        public GameProcessData[] CurrentProcesses { get; set; }
        public CommunicationProcess CommProcess { get; set; }
        public GameInfo.StatusCode GameStatus { get; set; }
        public Queue<Umbrella> Umbrellas { get; set; }
        public bool UmbrellaRaised { get; set; }
        public int NextId { get; set; }
        public int NumIds { get; set; }
        public Penny PennyToValidate { get; set; }
        public RSACryptoServiceProvider PennyRSA { get; set; }
        public int GamesPlayed { get; set; }

        public void Launch()
        {
            LaunchCount++;
            Start();
        }
    }
}
