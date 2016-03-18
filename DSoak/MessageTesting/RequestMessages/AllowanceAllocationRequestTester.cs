using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class AllowanceAllocationRequestTester
    {
        [TestMethod]
        public void AllowanceAllocationRequest_TestEverything()
        {
            AllowanceAllocationRequest r1 = new AllowanceAllocationRequest() { ToProcessId = 10, Amount = 100};
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.ToProcessId);
            Assert.AreEqual(100, r1.Amount);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            AllowanceAllocationRequest r2 = m2 as AllowanceAllocationRequest;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.ToProcessId, r2.ToProcessId);
            Assert.AreEqual(r1.Amount, r2.Amount);
        }
    }
}