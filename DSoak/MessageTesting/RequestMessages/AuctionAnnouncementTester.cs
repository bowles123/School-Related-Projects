using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class AuctionAnnouncementTester
    {
        [TestMethod]
        public void AuctionAnnoucement_TestEverything()
        {
            AuctionAnnouncement r1 = new AuctionAnnouncement() { MinimumBid = 10};
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.MinimumBid);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            AuctionAnnouncement r2 = m2 as AuctionAnnouncement;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.MinimumBid, r2.MinimumBid);
        }
    }
}