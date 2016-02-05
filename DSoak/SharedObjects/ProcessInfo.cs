using System;
using System.Text;
using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class ProcessInfo
    {
        public enum ProcessType { Unknown = 0, Registry, GameManager, Player, BalloonStore, WaterServer, UmbrellaManager };
        public enum StatusCode { Unknown = 0, NotInitialized = 1, Initializing = 2, Registered = 3, JoiningGame = 4, JoinedGame = 5, Working = 6, PlayingGame = 7, HostingGame = 8, LeavingGame = 9, Won = 10, Lost = 11, Terminating = 12, Tied=13 };

        private StatusCode status;
        private static readonly string[] statusNames = new string[] { "Unknown", "Not Initialized", "Initializing", "Registered", "Joining Game", "Joined Game", "Working", "Playing Game", "Hosting Game", "Leaving Game", "Won Game", "Lost Game", "Terminating", "Tied in a Draw" };
        private object myLock = new object();

        [DataMember]
        public Int32 ProcessId { get; set; }
        [DataMember]
        public ProcessType Type { get; set; }
        [DataMember]
        public PublicEndPoint EndPoint { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public StatusCode Status
        {
            get
            {
                StatusCode result;
                if (myLock != null)
                    lock (myLock) { result = status; }
                else
                    result = status;
                return result;
            }
            set
            {
                if (myLock != null)
                    lock (myLock) { status = value; }
                else
                    status = value;
            }
        }
        public string StatusString { get { return statusNames[(int) Status]; } }
        [DataMember]
        public int Wins { get; set; }
        [DataMember]
        public int Losses { get; set; }
        [DataMember]
        public int Draws { get; set; }
        [DataMember]
        public short LifePoints { get; set; }
        [DataMember]
        public short HitPoints { get; set; }
        [DataMember]
        public short NumberOfPennies { get; set; }
        [DataMember]
        public short NumberOfUnfilledBalloon { get; set; }
        [DataMember]
        public short NumberOfFilledBalloon { get; set; }
        [DataMember]
        public short NumberOfUnraisedUmbrellas { get; set; }
        [DataMember]
        public bool HasUmbrellaRaised { get; set; }

        [DataMember]
        public byte[] DigitalSignature { get; set; }

        public DateTime? AliveTimestamp { get; set; }
        public Int32 AliveReties { get; set; }

        public ProcessInfo Clone()
        {
            return MemberwiseClone() as ProcessInfo;
        }

        public string LabelAndId
        {
            get
            {
                string result = (!string.IsNullOrWhiteSpace(Label)) ? Label : string.Empty;
                result = string.Format("{0}  ({1})", result, ProcessId);
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("Id={0}, Label={1}, Type={2}, EndPoint={3}, Status={4}, Wins={5}, Losses={6}, Draws={7}",
                    ProcessId, Label, Type,
                    (EndPoint == null) ? string.Empty : EndPoint.ToString(),
                    StatusString,
                    Wins, Losses, Draws);
        }
    }
}
