using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class AllowancDeliveryRequestTester
    {
        [TestMethod]
        public void AllowanceDeliveryRequest_TestEverything()
        {
            AllowanceDeliveryRequest r1 = new AllowanceDeliveryRequest() { PortNumber = 1230, NumberOfPennies = 10};
            Assert.IsNotNull(r1);
            Assert.AreEqual(1230, r1.PortNumber);
            Assert.AreEqual(10, r1.NumberOfPennies);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            AllowanceDeliveryRequest r2 = m2 as AllowanceDeliveryRequest;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.PortNumber, r2.PortNumber);
            Assert.AreEqual(r1.NumberOfPennies, r2.NumberOfPennies);
        }
    }
}