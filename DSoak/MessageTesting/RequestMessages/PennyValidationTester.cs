using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class PennyValidationTester
    {
        [TestMethod]
        public void PennyValidation_TestEverything()
        {
            PennyValidation r1 = new PennyValidation() { Pennies = new Penny[] { new Penny() { Id = 10 }}};
            Assert.IsNotNull(r1);
            Assert.AreEqual(10,r1.Pennies[0].Id);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            PennyValidation r2 = m2 as PennyValidation;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.Pennies[0].Id, r2.Pennies[0].Id);
        }
    }
}