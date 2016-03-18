using System;
using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class GameProcessData
    {
        private DateTime _lastChanged = DateTime.Now;
        private int _lifePoints;
        private int _hitPoints;
        private bool _hasUmbrellaRaised;

        private object _myLock;

        public GameProcessData() {}

        public GameProcessData(ProcessInfo processInfo)
        {
            ProcessId = processInfo.ProcessId;
            Type = processInfo.Type;
        }

        [DataMember]
        public Int32 ProcessId { get; set; }

        [DataMember]
        public ProcessInfo.ProcessType Type { get; set; }

        [DataMember]
        public int LifePoints
        {
            get
            {
                int result;
                if (_myLock==null) _myLock = new object();               
                lock (_myLock)
                {
                    result = _lifePoints;
                }
                return result;
            }
            set
            {
                if (_myLock == null) _myLock = new object();
                lock (_myLock)
                {
                    _lifePoints = value;
                    _lastChanged = DateTime.Now;
                }
            }
        }

        [DataMember]
        public int HitPoints
        {
            get
            {
                int result;

                if (_myLock == null) _myLock = new object();
                lock (_myLock)
                {
                    result = _hitPoints;
                }
                return result;
            }
            set
            {
                if (_myLock == null) _myLock = new object();
                lock (_myLock)
                {
                    _hitPoints = value;
                    _lastChanged = DateTime.Now;
                }
            }
        }

        [DataMember]
        public bool HasUmbrellaRaised
        {
            get
            {
                bool result;

                if (_myLock == null) _myLock = new object();
                lock (_myLock)
                {
                    result = _hasUmbrellaRaised;
                }
                return result;
            }
            set
            {
                if (_myLock == null) _myLock = new object();
                lock (_myLock)
                {
                    _hasUmbrellaRaised = value;
                    _lastChanged = DateTime.Now;
                }
            }
        }

        public DateTime LastChanged
        {
            get
            {
                DateTime result;
                if (_myLock == null) _myLock = new object();
                lock (_myLock)
                {
                    result = _lastChanged;
                }
                return result;
                
            }
        }

        public void ChangeLifePoints(int delta)
        {
            if (_myLock == null) _myLock = new object();
            lock (_myLock)
            {
                _lifePoints = Math.Max(0, _lifePoints + delta);
                _lastChanged = DateTime.Now;
            }            
        }

        public void ChangeHitPoints(int delta)
        {
            if (_myLock == null) _myLock = new object();
            lock (_myLock)
            {
                _hitPoints = Math.Max(0, _hitPoints + delta);
                _lastChanged = DateTime.Now;
            }
        }

        public GameProcessData Clone()
        {
            return MemberwiseClone() as GameProcessData;
        }

        public override string ToString()
        {
            return string.Format(
                "Id={0}, Type={1}, LifePoints={2}, HitPoints={3}, HasUmbrellaRaised={4}",
                ProcessId, Type, LifePoints, HitPoints, HasUmbrellaRaised);
        }
    }
}
