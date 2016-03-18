using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class BuyBalloonRequestTester
    {
        [TestMethod]
        public void BuyBallooonRequest_TestEverything()
        {
            BuyBalloonRequest r1 = new BuyBalloonRequest() { Penny = new Penny() { Id = 10 } };
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.Penny.Id);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            BuyBalloonRequest r2 = m2 as BuyBalloonRequest;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.Penny.Id, r2.Penny.Id);
        }
    }
}