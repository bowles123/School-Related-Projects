using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class FillBalloonRequestTester
    {
        [TestMethod]
        public void FillBallooonRequest_TestEverything()
        {
            FillBalloonRequest r1 = new FillBalloonRequest()
            {
                Balloon = new Balloon() { Id = 10 },
                Pennies = new Penny[] { new Penny() { Id = 11 } }
            };
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.Balloon.Id);
            Assert.AreEqual(11, r1.Pennies[0].Id);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            FillBalloonRequest r2 = m2 as FillBalloonRequest;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.Balloon.Id, r2.Balloon.Id);
            Assert.AreEqual(r1.Pennies[0].Id, r2.Pennies[0].Id);
        }
    }
}