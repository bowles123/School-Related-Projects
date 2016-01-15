using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.StatusMessages
{
    [DataContract]
    public class EndGame : StatusMessage
    {
        [DataMember]
        public int WinnerId { get; set; }           // 0 if there was no winner

        [DataMember]
        public List<int> PlayersWhoTied { get; set; }
                                                    // Empty if there was a a winner
    }
}
