using System.Collections.Generic;
using System.Linq;

namespace SharedObjects
{
    public class ResourceSet<T> where T : SharedResource
    {
        private readonly Dictionary<int, T> _availableSet = new Dictionary<int, T>();
        private readonly List<int> _used = new List<int>(); 
        private readonly object _myLock = new object();

        public void Clear()
        {
            lock (_myLock)
            {
                _availableSet.Clear();
                _used.Clear();
            }
        }

        public int AvailableCount
        {
            get
            {
                int result;
                lock (_myLock)
                {
                    result = _availableSet.Count;
                }
                return result;
            }
        }

        public int UsedCount
        {
            get
            {
                int result;
                lock (_myLock)
                {
                    result = _used.Count;
                }
                return result;
            }
        }

        public void AddOrUpdate(T sharedResource)
        {
            if (sharedResource != null)
            {
                lock (_myLock)
                {
                    if (_availableSet.ContainsKey(sharedResource.Id))
                        _availableSet[sharedResource.Id] = sharedResource;
                    else
                        _availableSet.Add(sharedResource.Id, sharedResource);                   
                }
            }
        }

        public T Get(int id)
        {
            T result = null;
            lock (_myLock)
            {
                if (_availableSet.ContainsKey(id))
                    result = _availableSet[id];
            }
            return result;
        }

        public T GetAvailable()
        {
            T result = null;
            lock (_myLock)
            {
                if (_availableSet.Count > 0)
                    result = _availableSet.First().Value;
            }
            return result;
        }

        public void MarkAsUsed(int id)
        {
            lock (_myLock)
            {
                if (_availableSet.ContainsKey(id))
                    _availableSet.Remove(id);

                if (!_used.Contains(id))
                    _used.Add(id);
            }
        }

        public bool ContainsAny(Penny[] pennies)
        {
            bool result;
            lock (_myLock)
            {
                result = true;
                if (pennies != null && pennies.Any(p => _availableSet.ContainsKey(p.Id)))
                    result = false;
            }
            return result;
        }

        public bool AreAnyUsed(Penny[] pennies)
        {
            bool result = false;
            lock (_myLock)
            {
                if (pennies != null && pennies.Any(p => _used.Contains(p.Id)))
                    result = true;
            }
            return result;
        }
    }
}
