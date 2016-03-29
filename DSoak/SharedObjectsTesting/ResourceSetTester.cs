using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectsTesting
{
    [TestClass]
    public class ResourceSetTester
    {
        [TestMethod]
        public void ResoureSet_TestEverything()
        {
            ResourceSet<Penny> s = new ResourceSet<Penny>();

            Assert.AreEqual(0, s.AvailableCount);
            Assert.AreEqual(0, s.UsedCount);

            Penny p1 = new Penny() { Id = 100};
            Penny p2 = new Penny() { Id = 101 };
            Penny p3 = new Penny() { Id = 102 };
            s.AddOrUpdate(p1);
            s.AddOrUpdate(p2);
            s.AddOrUpdate(p3);
            Assert.AreEqual(3, s.AvailableCount);
            Assert.AreEqual(0, s.UsedCount);

            Assert.IsNotNull(s.Get(100));
            Assert.IsNotNull(s.Get(101));
            Assert.IsNotNull(s.Get(102));
            Assert.IsNull(s.Get(103));
            Assert.IsNull(s.Get(0));

            s.AddOrUpdate(new Penny() { Id = 100 });
            Assert.AreEqual(3, s.AvailableCount);
            Assert.AreEqual(0, s.UsedCount);

            Penny r1 = s.Get(100);
            Assert.AreEqual(p1.Id, r1.Id);

            Penny a1 = s.GetAvailable();
            Assert.IsNotNull(a1);

            s.MarkAsUsed(101);
            Assert.AreEqual(2, s.AvailableCount);
            Assert.AreEqual(1, s.UsedCount);

            s.MarkAsUsed(100);
            Assert.AreEqual(1, s.AvailableCount);
            Assert.AreEqual(2, s.UsedCount);

            s.MarkAsUsed(100);
            Assert.AreEqual(1, s.AvailableCount);
            Assert.AreEqual(2, s.UsedCount);

            Assert.IsTrue(s.AreAnyUsed(new [] { new Penny() { Id = 100 } }));
            Assert.IsTrue(s.AreAnyUsed(new [] { new Penny() { Id = 101 }}));
            Assert.IsFalse(s.AreAnyUsed(new[] { new Penny() { Id = 102 } }));

            Penny a2 = s.GetAvailable();
            Assert.AreSame(a2, p3);

            s.Clear();
            Assert.AreEqual(0, s.AvailableCount);
            Assert.AreEqual(0, s.UsedCount);

            // Test reserving and unreserving
            s.AddOrUpdate(p1);
            s.AddOrUpdate(p2);
            s.AddOrUpdate(p3);
            Assert.AreEqual(3, s.AvailableCount);
            Assert.AreEqual(0, s.UsedCount);

            Penny p10 = s.ReserveOne();
            Assert.AreEqual(2, s.AvailableCount);
            Assert.AreEqual(0, s.UsedCount);

            Penny p20 = s.ReserveOne();
            Assert.AreNotSame(p10, p20);
            Assert.AreEqual(1, s.AvailableCount);
            Assert.AreEqual(0, s.UsedCount);

            s.Unreserve(p10.Id);
            Assert.AreEqual(2, s.AvailableCount);
            Assert.AreEqual(0, s.UsedCount);

            s.Unreserve(p10.Id);
            Assert.AreEqual(2, s.AvailableCount);
            Assert.AreEqual(0, s.UsedCount);

            s.MarkAsUsed(p20.Id);
            Assert.AreEqual(2, s.AvailableCount);
            Assert.AreEqual(1, s.UsedCount);

        }
    }
}
