using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class AuctionAnnouncement : Request
    {
        [DataMember]
        public int MinimumBid { get; set; }

    }
}
