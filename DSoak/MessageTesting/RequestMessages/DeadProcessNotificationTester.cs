using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class DeadProcessNotificationTester
    {
        [TestMethod]
        public void DeadProcessNotification_TestEverything()
        {
            DeadProcessNotification r1 = new DeadProcessNotification() { ProcessId = 10};
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.ProcessId);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            DeadProcessNotification r2 = m2 as DeadProcessNotification;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.ProcessId, r2.ProcessId);
        }
    }
}