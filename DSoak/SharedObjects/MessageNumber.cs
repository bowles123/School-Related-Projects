using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class MessageNumber
    {
        #region Private Properties
        private static Int32 _nextSeqNumber;                     // Start with message #1
        private static readonly object MyLock = new object();
        #endregion

        #region Public Properties
        public static Int32 LocalProcessId { get; set; }            // Local process Id -- set once when the
                                                                    // process logins to the distributed application
        [DataMember]
        public Int32 Pid { get; set; }
        [DataMember]
        public Int32 Seq { get; set; }

        #endregion

        #region Constructors and Factories
        /// <summary>
        /// Factory method creates and new, unique message number.
        /// </summary>
        /// <returns>A new message number</returns>
        public static MessageNumber Create()
        {
            MessageNumber result = new MessageNumber()
                {
                    Pid = LocalProcessId,
                    Seq = GetNextSeqNumber()
                };
            return result;
        }

        public MessageNumber Clone()
        {
            return MemberwiseClone() as MessageNumber;
        }
        #endregion

        #region Public Methods for Testing
        public static void ResetSeqNumber() { _nextSeqNumber = 0; }
        public static void SetSeqNumber(int newValue) { _nextSeqNumber = Math.Max(0, newValue); }
        #endregion

        #region Overridden public methods of Object
        public override string ToString()
        {
            return Pid.ToString() + "." + Seq.ToString();
        }
        #endregion

        #region Private Methods
        private static Int32 GetNextSeqNumber()
        {
            lock (MyLock)
            {
                if (_nextSeqNumber == Int32.MaxValue)
                    _nextSeqNumber = 0;
                ++_nextSeqNumber;
            }
            return _nextSeqNumber;
        }
        #endregion

        #region Comparison Methods and Operators
        public override int GetHashCode()
        {
            return (Pid << 16) | (Seq & 0xFFFF);
        }

        public override bool Equals(object obj)
        {
            return Compare(this, obj as MessageNumber) == 0;
        }

        public static int Compare(MessageNumber a, MessageNumber b)
        {
            int result = 0;

            if (!ReferenceEquals(a, b))
            {
                if (((object) a == null) && ((object) b != null))
                    result = -1;                             
                else if (((object) a != null) && ((object) b == null))
                    result = 1;                             
                else
                {
                    if (a.Pid < b.Pid)
                        result = -1;
                    else if (a.Pid > b.Pid)
                        result = 1;
                    else if (a.Seq < b.Seq)
                        result = -1;
                    else if (a.Seq > b.Seq)
                        result = 1;
                }
            }
            return result;
        }

        public static bool operator ==(MessageNumber a, MessageNumber b)
        {
            return (Compare(a,b) == 0);
        }

        public static bool operator !=(MessageNumber a, MessageNumber b)
        {
            return (Compare(a,b) !=0 );
        }

        public static bool operator <(MessageNumber a, MessageNumber b)
        {
            return (Compare(a, b) < 0);
        }

        public static bool operator >(MessageNumber a, MessageNumber b)
        {
            return (Compare(a, b) > 0);
        }

        public static bool operator <=(MessageNumber a, MessageNumber b)
        {
            return (Compare(a, b) <= 0);
        }

        public static bool operator >= (MessageNumber a, MessageNumber b)
        {
            return (Compare(a, b) >= 0);
        }

        #endregion

        /// <summary>
        /// This is a class provide a comparer so MessageNumber can used as a dictionary key
        /// </summary>
        public class MessageNumberComparer : IEqualityComparer<MessageNumber>
        {
            public bool Equals(MessageNumber msgNumber1, MessageNumber msgNumber2)
            {
                return msgNumber1 == msgNumber2;
            }

            public int GetHashCode(MessageNumber msgNr)
            {
                return msgNr.GetHashCode();
            }
        }
    }


}
