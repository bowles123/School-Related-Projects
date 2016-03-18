using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class AuctionAnnouncement : Request
    {
        static AuctionAnnouncement() { Register(typeof(AuctionAnnouncement)); }

        [DataMember]
        public int MinimumBid { get; set; }

    }
}
