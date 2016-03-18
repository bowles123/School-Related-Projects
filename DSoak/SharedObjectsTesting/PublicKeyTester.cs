using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectsTesting
{
    [TestClass]
    public class PublicKeyTester
    {
        [TestMethod]
        public void PublicKey_TestEverything()
        {
            PublicKey pk1 = new PublicKey();
            Assert.IsNull(pk1.Exponent);
            Assert.IsNull(pk1.Modulus);

            PublicKey pk2 = new PublicKey()
            {
                Exponent = new byte[] { 10,  20, 30},
                Modulus = new  byte[] {40, 50, 60}
            };

            Assert.IsNotNull(pk2.Exponent);
            Assert.AreEqual(3, pk2.Exponent.Length);
            Assert.AreEqual(10, pk2.Exponent[0]);
            Assert.AreEqual(20, pk2.Exponent[1]);
            Assert.AreEqual(30, pk2.Exponent[2]);
            Assert.IsNotNull(pk2.Modulus);
            Assert.AreEqual(3, pk2.Modulus.Length);
            Assert.AreEqual(40, pk2.Modulus[0]);
            Assert.AreEqual(50, pk2.Modulus[1]);
            Assert.AreEqual(60, pk2.Modulus[2]);
        }
    }
}
