using System.Collections.Generic;

namespace CommSub
{
    public class ErrorList
    {
        private readonly List<Error> _errors = new List<Error>();
        private readonly object _myLock = new object();
        private const int DefaultMax = 100;
        private int _max = 100;

        public int Max
        {
            get { return _max;  }
            set
            {
                _max = value;
                if (Max <= 0) Max = DefaultMax;
            }
        }

        public void Clear()
        {
            lock (_myLock)
            {
                _errors.Clear();
            }
        }

        public void Add(Error err)
        {
            if (err != null)
            {
                lock (_myLock)
                {
                    _errors.Add(err);

                    while (_errors.Count>Max)
                        _errors.RemoveAt(0);
                }
            }
        }

        public int Count
        {
            get
            {
                int result;
                lock (_myLock)
                {
                    result = _errors.Count;
                }
                return result;
            }
        }

        public Error[] All
        {
            get
            {
                Error[] result;
                lock (_myLock)
                {
                    result = _errors.ToArray();
                }
                return result;
            }
        }

        public Error this[int index]
        {
            get
            {
                Error result = null;
                lock (_myLock)
                {
                    if (index < _errors.Count)
                        result = _errors[index];
                }
                return result;
            }
        }
    }

}
