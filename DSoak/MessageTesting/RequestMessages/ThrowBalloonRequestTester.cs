using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class ThrowBalloonRequestTester
    {
        [TestMethod]
        public void ThrowBallooonRequest_TestEverything()
        {
            ThrowBalloonRequest r1 = new ThrowBalloonRequest()
            {
                Balloon = new Balloon() { Id = 10 }
            };
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.Balloon.Id);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            ThrowBalloonRequest r2 = m2 as ThrowBalloonRequest;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.Balloon.Id, r2.Balloon.Id);
        }
    }
}