using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ObjectIdGenerator
    {
        private static ObjectIdGenerator instance = null;
        private static object myLock = new object();

        public static ObjectIdGenerator Instance
        {
            get
            {
                if (instance == null)
                    lock (myLock)
                    {
                        instance = new ObjectIdGenerator();
                    }
                return instance;
            }
        }

        private Int32 nextId = 1;

        public Int32 GetNextIdNumber()
        {
            if (nextId == Int32.MaxValue)
                nextId = 1;
            return nextId++;
        }

        public Int32 GetNextBlock(Int32 blockSize)
        {
            if (nextId == Int32.MaxValue)
                nextId = 1;
            Int32 result = nextId;
            nextId += blockSize;
            return result;
        }

    }
}
