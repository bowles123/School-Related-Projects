using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectsTesting
{
    [TestClass]
    public class IdentityInfoTester
    {
        [TestMethod]
        public void IdentityInfo_TestEverything()
        {
            IdentityInfo i1 = new IdentityInfo();
            Assert.IsNull(i1.ANumber);
            Assert.IsNull(i1.FirstName);
            Assert.IsNull(i1.LastName);
            Assert.IsNull(i1.Alias);

            IdentityInfo i2 = new IdentityInfo() { ANumber = "123", FirstName = "Joe", LastName = "Franks", Alias = "Joey" };
            Assert.AreEqual("123", i2.ANumber);
            Assert.AreEqual("Joe", i2.FirstName);
            Assert.AreEqual("Franks", i2.LastName);
            Assert.AreEqual("Joey", i2.Alias);

            IdentityInfo i3 = i2.Clone();
            Assert.AreNotSame(i2, i3);
            Assert.AreEqual("123", i3.ANumber);
            Assert.AreEqual("Joe", i3.FirstName);
            Assert.AreEqual("Franks", i3.LastName);
            Assert.AreEqual("Joey", i3.Alias);

            IdentityInfo i4 = new IdentityInfo();
            i4.ANumber = "124145";
        }
    }
}
