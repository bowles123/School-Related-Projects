using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net;

namespace SharedObjects
{
    /// <summary>
    /// PublicEndPoint
    /// 
    /// This class is an Adapter (or Wrapper) for .Net's IPEndPoint that provides some additional
    /// functionality, including
    ///     - Keeping track of the IP hostname, not just the IPv4 Address
    ///     - Deferred, but automatic, DNS lookup of IP hostname and selection of an appropriate IPv4 Address
    /// </summary>
    [DataContract]
    public class PublicEndPoint
    {
        #region Private Data Members
        private string myHost = "0.0.0.0";               // String reprsentation of Hostname or IP Address
        private bool needToResolveHostname = false;      // If true, then the hostname needs to be resolved into an IP Address

        private IPEndPoint myEP;                         // Adaptee (object being adapted or "wrapped"
        #endregion

        #region Constructors
        public PublicEndPoint()
        {
            SetDefaults();
        }

        public PublicEndPoint(string hostnameAndPort) : this()
        {
            SetHostAndPort(hostnameAndPort);
        }
        #endregion

        #region Public properties and methods
        /// <summary>
        /// Host property
        /// 
        /// The property to access the host, which can be hostname or string representation of the IP Address
        /// </summary>
        [DataMember]
        public string Host
        {
            get
            {
                return myHost;
            }
            set
            {
                // If the IPEndPoint hasn't been set up, do so now.  Note that this might be the case when creating
                // the object via deserialization.  The properties will be called before the data member initializers
                // or the default constructor's body.
                if (myEP == null)
                    SetDefaults();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    myHost = value.Trim();
                    needToResolveHostname = true;
                }
                else
                {
                    myHost = "0.0.0.0";
                    myEP.Address = IPAddress.Any;
                    needToResolveHostname = false;
                }
            }
        }

        [DataMember]
        public Int32 Port
        {
            get { return myEP.Port; }
            set
            {
                // If the IPEndPoint hasn't been set up, do so now.  Note that this might be the case when creating
                // the object via deserialization.  The properties will be called before the data member initializers
                // or the default constructor's body.
                if (myEP == null)
                    SetDefaults();

                myEP.Port = value;
            }
        }

        public string HostAndPort
        {
            get { return this.ToString(); }
            set { SetHostAndPort(value); }
        }

        /// <summary>
        /// IPEndPoint Property
        /// 
        /// This property is for convenience in work with .Net IPEndPoint objects.
        /// </summary>
        public IPEndPoint IPEndPoint
        {
            get
            {
                if (needToResolveHostname)
                {
                    needToResolveHostname = false;
                    myEP.Address = LookupAddress(myHost);
                }
                return myEP;
            }
            set
            {
                myEP = (value != null) ?  myEP = value : myEP = new IPEndPoint(IPAddress.Any, 0);
                myHost = myEP.Address.ToString();
                needToResolveHostname = false;
            }
        }

        public static IPAddress LookupAddress(string host)
        {
            IPAddress result = null;
            if (!string.IsNullOrWhiteSpace(host))
            {
                IPAddress[] addressList = Dns.GetHostAddresses(host);
                for (int i = 0; i < addressList.Length && result == null; i++)
                    if (addressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        result = addressList[i];
            }
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", myHost, Port);
        }

        public override int GetHashCode()
        {
            return HostAndPort.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            PublicEndPoint other = obj as PublicEndPoint;
            if (other != null)
            {
                result = (HostAndPort == other.HostAndPort);
            }
            return result;
        }

        public PublicEndPoint Clone()
        {
            return MemberwiseClone() as PublicEndPoint;
        }
        #endregion

        #region Private Methods
        private void SetDefaults()
        {
            this.myHost = "0.0.0.0";
            this.needToResolveHostname = false;
            this.myEP = new IPEndPoint(IPAddress.Any, 0);
        }

        private void SetHostAndPort(string hostAndPort)
        {
            SetDefaults();

            if (!string.IsNullOrWhiteSpace(hostAndPort))
            {
                string[] tmp = hostAndPort.Split(':');
                if (tmp.Length == 1)
                {
                    Host = hostAndPort;
                    needToResolveHostname = true;
                }
                else if (tmp.Length >= 2)
                {
                    Host = tmp[0].Trim();

                    int port = 0;
                    Int32.TryParse(tmp[1].Trim(), out port);
                    Port = port;

                    needToResolveHostname = true;
                }
            }
        }
        #endregion

    }
}
