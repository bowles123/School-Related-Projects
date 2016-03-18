
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectsTesting
{
    [TestClass]
    public class PennyTester
    {
        [TestMethod]
        public void Penny_Constructor()
        {
            Penny p1 = new Penny();
            Assert.AreEqual(0, p1.Id);
            Assert.IsNull(p1.DigitalSignature);
        }

        [TestMethod]
        public void Penny_PublicProperties()
        {
            Penny p1 = new Penny() { Id = 100, DigitalSignature = new byte[] { 10, 20 } };
            Assert.AreEqual(100, p1.Id);
            Assert.AreEqual(2, p1.DigitalSignature.Length);
            Assert.AreEqual(10, p1.DigitalSignature[0]);
            Assert.AreEqual(20, p1.DigitalSignature[1]);

            p1.Id = 0;
            Assert.AreEqual(0, p1.Id);
            Assert.AreEqual(2, p1.DigitalSignature.Length);
            Assert.AreEqual(10, p1.DigitalSignature[0]);
            Assert.AreEqual(20, p1.DigitalSignature[1]);

            p1.Id = 113;
            Assert.AreEqual(113, p1.Id);

            p1.DigitalSignature = null;
            Assert.IsNull(p1.DigitalSignature);

            Penny p2 = new Penny();
            p2.Id = 900;
            Assert.AreEqual(900, p2.Id);
        }

        [TestMethod]
        public void Penny_TestClone()
        {
            Penny p1 = new Penny() { Id = 100, DigitalSignature = new byte[] { 10, 20 } };
            Penny p2 = p1.Clone();

            Assert.AreNotSame(p1, p2);

            Assert.AreEqual(100, p2.Id);
            Assert.AreEqual(2, p2.DigitalSignature.Length);
            Assert.AreEqual(10, p2.DigitalSignature[0]);
            Assert.AreEqual(20, p2.DigitalSignature[1]);
        }

        [TestMethod]
        public void Penny_TestSign()
        {
        }

    }
}
