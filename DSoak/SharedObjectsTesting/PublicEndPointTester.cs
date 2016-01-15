using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectTesting
{
    [TestClass]
    public class PublicEndPointTester
    {
        [TestMethod]
        public void PublicEndPoint_Constructor()
        {
            PublicEndPoint ep1 = new PublicEndPoint();
            Assert.AreEqual("0.0.0.0", ep1.Host);
            Assert.AreEqual(0, ep1.Port);
            Assert.IsNotNull(ep1.IPEndPoint);
            Assert.AreEqual("0.0.0.0:0", ep1.IPEndPoint.ToString());
            Assert.AreEqual(0, ep1.IPEndPoint.Port);

            PublicEndPoint ep2 = new PublicEndPoint() { Host = "swcwin.serv.usu.edu", Port = 12001 };
            Assert.AreEqual("swcwin.serv.usu.edu", ep2.Host);
            Assert.AreEqual(12001, ep2.Port);
            Assert.AreEqual("129.123.41.13:12001", ep2.IPEndPoint.ToString());
        }

        [TestMethod]
        public void PublicEndPoint_PublicProperties()
        {
            PublicEndPoint ep1 = new PublicEndPoint() { Host = "www.usu.edu", Port = 12010 };
            Assert.AreEqual("www.usu.edu", ep1.Host);
            Assert.AreEqual(12010, ep1.Port);
            Assert.AreEqual("129.123.54.210:12010", ep1.IPEndPoint.ToString());

            ep1.HostAndPort = "swcwin.serv.usu.edu:12001";
            Assert.AreEqual("swcwin.serv.usu.edu", ep1.Host);
            Assert.AreEqual(12001, ep1.Port);
            Assert.AreEqual("129.123.41.13:12001", ep1.IPEndPoint.ToString());

            ep1.Host = "www.usu.edu";
            Assert.AreEqual("www.usu.edu", ep1.Host);
            Assert.AreEqual(12001, ep1.Port);
            Assert.AreEqual("129.123.54.210:12001", ep1.IPEndPoint.ToString());

            ep1.Port = 12010;
            Assert.AreEqual("www.usu.edu", ep1.Host);
            Assert.AreEqual(12010, ep1.Port);
            Assert.AreEqual("129.123.54.210:12010", ep1.IPEndPoint.ToString());

            ep1.Host = null;
            Assert.AreEqual("0.0.0.0", ep1.Host);
            Assert.AreEqual(12010, ep1.Port);
            Assert.AreEqual("0.0.0.0:12010", ep1.IPEndPoint.ToString());

            ep1.HostAndPort = "swcwin.serv.usu.edu:12001";
            Assert.AreEqual("swcwin.serv.usu.edu", ep1.Host);
            Assert.AreEqual(12001, ep1.Port);
            Assert.AreEqual("129.123.41.13:12001", ep1.IPEndPoint.ToString());

            ep1.Host = "";
            Assert.AreEqual("0.0.0.0", ep1.Host);
            Assert.AreEqual(12001, ep1.Port);
            Assert.AreEqual("0.0.0.0:12001", ep1.IPEndPoint.ToString());

            ep1.Port = 0;
            Assert.AreEqual("0.0.0.0", ep1.Host);
            Assert.AreEqual(0, ep1.Port);
            Assert.AreEqual("0.0.0.0:0", ep1.IPEndPoint.ToString());
        }

        [TestMethod]
        public void PublicEndPoint_Clone()
        {
            PublicEndPoint ep1 = new PublicEndPoint();
            PublicEndPoint ep1_copy = ep1.Clone();

            Assert.AreNotSame(ep1, ep1_copy);
            Assert.AreEqual("0.0.0.0", ep1_copy.Host);
            Assert.AreEqual(0, ep1_copy.Port);
            Assert.IsNotNull(ep1_copy.IPEndPoint);
            Assert.AreEqual("0.0.0.0:0", ep1_copy.IPEndPoint.ToString());
            Assert.AreEqual(0, ep1_copy.IPEndPoint.Port);

            PublicEndPoint ep2 = new PublicEndPoint() { Host = "swcwin.serv.usu.edu", Port = 12001 };
            PublicEndPoint ep2_copy = ep2.Clone() as PublicEndPoint;

            Assert.AreNotSame(ep2, ep2_copy);
            Assert.AreEqual("swcwin.serv.usu.edu", ep2_copy.Host);
            Assert.AreEqual(12001, ep2_copy.Port);
            Assert.AreEqual("129.123.41.13:12001", ep2_copy.IPEndPoint.ToString());
        }

        [TestMethod]
        public void PublicEndPoint_OtherPublicMethods()
        {
            PublicEndPoint ep1 = new PublicEndPoint();
            PublicEndPoint ep2 = new PublicEndPoint() { Host = "swcwin.serv.usu.edu", Port = 12001 };
            PublicEndPoint ep3 = new PublicEndPoint() { Host = "swcwin.serv.usu.edu", Port = 12001 };
            PublicEndPoint ep4 = new PublicEndPoint() { HostAndPort = "www.usu.edu:2300" };

            Assert.IsTrue(ep3.Equals(ep2));
            Assert.IsTrue(ep2.Equals(ep3));
            Assert.IsFalse(ep3.Equals(null));
            Assert.IsFalse(ep3.Equals(ep1));
            Assert.IsFalse(ep1.Equals(ep3));
            Assert.IsFalse(ep3.Equals(ep4));
            Assert.IsFalse(ep4.Equals(ep3));

            Assert.IsFalse(ep3.Equals(new PublicEndPoint() { Host = "127.0.0.1", Port = 12001 }));
            Assert.IsFalse(ep3.Equals(new PublicEndPoint() { Host = "swcwin.serv.usu.edu", Port = 23324 }));
            Assert.IsFalse(ep3.Equals(new PublicEndPoint() { Host = "", Port = 12001 }));
            Assert.IsFalse(ep3.Equals(new PublicEndPoint() { Host = null, Port = 12001 }));
        }

    }
}
