using System;

namespace Utils
{
    public class ObjectIdGenerator
    {
        private static ObjectIdGenerator _instance;
        private static readonly object MyLock = new object();

        public static ObjectIdGenerator Instance
        {
            get
            {
                if (_instance == null)
                    lock (MyLock)
                    {
                        _instance = new ObjectIdGenerator();
                    }
                return _instance;
            }
        }

        private Int32 _nextId = 1;

        public Int32 GetNextIdNumber()
        {
            if (_nextId == Int32.MaxValue)
                _nextId = 1;
            return _nextId++;
        }

        public Int32 GetNextBlock(Int32 blockSize)
        {
            if (_nextId == Int32.MaxValue)
                _nextId = 1;
            Int32 result = _nextId;
            _nextId += blockSize;
            return result;
        }

    }
}
