
namespace CommSub
{
    public class CommSubsystem
    {
        // A CommSubsystem is a facade that encapsulates the following objects.
        private EnvelopeQueueDictionary _queueDictionary;
        private Communicator _myCommunicator;
        private Dispatcher _myDispatcher;

        // A process (e.g., something that specializes CommProcess) should set the following
        // properties when it creates a new subsystem.
        public int MinPort { get; set; }
        public int MaxPort { get; set; }
        public ConversationFactory ConversationFactory { get; set; }

        // The following properties are conveniences for working with the ComSubsystem.
        // They are part of the abstraction that this facade provides
        public EnvelopeQueueDictionary QueueDictionary { get { return _queueDictionary; } }
        public Communicator Communicator { get { return _myCommunicator; } }
        public Dispatcher Dispatcher { get { return _myDispatcher; } }

        /// <summary>
        /// Initialize
        /// 
        /// This methods setup up all of the components in a CommSubsystem.  Call this method
        /// sometime after setting the MinPort, MaxPort, and ConversationFactory
        /// </summary>
        public void Initialize()
        {
            _queueDictionary = new EnvelopeQueueDictionary();
            
            ConversationFactory.Initialize();
            _myCommunicator = new Communicator() { MinPort = MinPort, MaxPort = MaxPort };
            _myDispatcher = new Dispatcher() { Label = "Dispatcher", CommSubsystem = this};
        }
        
        /// <summary>
        /// Start
        /// 
        /// This method starts up all active components in the CommSubsystem.  Call this method
        /// sometime after calling Initalize.
        /// </summary>
        public void Start()
        {
            _myCommunicator.Start();
            _myDispatcher.Start();
        }

        /// <summary>
        /// Stop
        /// 
        /// This method stops all of the active components of a CommSubsystem and release the
        /// releases (or at least allows them to be garabage collected.  Once stop is called,
        /// a CommSubsystem cannot be restarted with setting it up from scratch.
        /// </summary>
        public void Stop()
        {
            if (_myDispatcher != null)
            {
                _myDispatcher.Stop();
                _myDispatcher = null;
            }

            if (_myCommunicator != null)
            {
                _myCommunicator.Stop();
                _myCommunicator = null;
            }

        }


    }
}
