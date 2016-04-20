using System.Collections.Generic;
using System.Linq;

namespace SharedObjects
{
    public class ResourceSet<T> where T : SharedResource
    {
        private readonly Dictionary<int, T> _resources = new Dictionary<int, T>(); 
        private readonly List<int> _available = new List<int>();
        private readonly List<int> _reserved = new List<int>();
        private readonly List<int> _used = new List<int>(); 
        private readonly object _myLock = new object();
        private int _lastRecentlyUsedIndex;

        public void Clear()
        {
            lock (_myLock)
            {
                _resources.Clear();
                _reserved.Clear();
                _available.Clear();
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
                    result = _available.Count;
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
                    if (_resources.ContainsKey(sharedResource.Id))
                        _resources[sharedResource.Id] = sharedResource;
                    else
                    {
                        _resources.Add(sharedResource.Id, sharedResource);
                        _available.Add(sharedResource.Id);
                    }
                }
            }
        }

        public T Get(int id)
        {
            T result = null;
            lock (_myLock)
            {
                if (_resources.ContainsKey(id))
                    result = _resources[id];
            }
            return result;
        }

        public T GetAvailable()
        {
            T result = null;
            lock (_myLock)
            {
                if (_available.Count > 0)
                    result = _resources[_available[0]];
            }
            return result;
        }

        public T ReserveOne()
        {
            T result = null;
            lock (_myLock)
            {
                if (_available.Count > 0)
                {
                    result = _resources[_available[0]];
                    _available.RemoveAt(0);
                    _reserved.Add(result.Id);
                }
            }
            return result;
            
        }

        public T[] ReserveMany(int n)
        {
            T[] result = null;
            lock (_myLock)
            {
                if (n>0 && _available.Count >= n)
                {
                    result = new T[n];
                    for (int i = 0; i < n; i++)
                    {
                        result[i] = _resources[_available[0]];
                        _available.RemoveAt(0);
                        _reserved.Add(result[i].Id);
                    }
                }
            }
            return result;

        }

        public void Unreserve(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                lock (_myLock)
                {
                    foreach (int id in ids)
                        Unreserve(id);
                }
            }
        }

        public void Unreserve(int id)
        {
            lock (_myLock)
            {
                if (_reserved.Contains(id))
                {
                    _reserved.Remove(id);
                    _available.Add(id);
                }
            }
        }

        public void MarkAsUsed(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                lock (_myLock)
                {
                    foreach (int id in ids)
                        MarkAsUsed(id);
                }
            }
        }

        public void MarkAsUsed(int id)
        {
            lock (_myLock)
            {
                if (_reserved.Contains(id))
                    _reserved.Remove(id);
                else if (_available.Contains(id))
                    _available.Remove(id);

                if (!_used.Contains(id))
                    _used.Add(id);
            }
        }

        public bool AreAllAvailable(Penny[] pennies, bool markAsUsedIfValid)
        {
            bool result;
            lock (_myLock)
            {
                result = true;
                if (pennies != null && pennies.Length > 0)
                {
                    if (!pennies.All(p => _available.Contains(p.Id)))
                        result = false;

                    if (result && markAsUsedIfValid)
                        MarkAsUsed(pennies.Select(p => p.Id).ToArray());
                }
            }
            return result;
        }

        public bool AreAnyUsed(Penny[] pennies)
        {
            bool result = false;
            lock (_myLock)
            {
                if (pennies != null && pennies.Length>0 && pennies.Any(p => _used.Contains(p.Id)))
                    result = true;
            }
            return result;
        }

        public int[] GetRecentlyUsed()
        {
            int[] recentlyUsed;
            lock (_myLock)
            {
                recentlyUsed = new int[_used.Count - _lastRecentlyUsedIndex];
                _used.CopyTo(_lastRecentlyUsedIndex, recentlyUsed, 0, _used.Count - _lastRecentlyUsedIndex);
                _lastRecentlyUsedIndex = _used.Count;
            }
            return recentlyUsed;
        }
    }
}
