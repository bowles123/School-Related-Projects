using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using SharedObjects;

namespace CommSub
{
    public class KnownGameProcesses
    {
        #region Private Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(KnownGameProcesses));

        private readonly Dictionary<Int32, GameProcessData> _processes = new Dictionary<Int32, GameProcessData>();
        private readonly object _myLock = new object();
        #endregion

        public void Clear()
        {
            lock (_myLock)
            {
                _processes.Clear();
            }
        }

        public void UpdateProcesses(GameProcessData[] currentProcesses)
        {
            lock (_myLock)
            {
                Logger.DebugFormat("Clear list of processes");
                _processes.Clear();
                if (currentProcesses != null)
                {
                    foreach (GameProcessData process in currentProcesses)
                        AddOrUpdate(process);
                }
            }
        }

        public Error AddOrUpdate(GameProcessData processInfo)
        {
            Error error = null;
            if (processInfo == null)
                error = Error.Get(Error.StandardErrorNumbers.InvalidProcessInfo);
            else if (processInfo.ProcessId <= 0)
                error = Error.Get(Error.StandardErrorNumbers.InvalidProcessId);
            else
            {
                int id = processInfo.ProcessId;

                Logger.DebugFormat("Add/Update ({0}, {1}) to ProcessAddressBook", id, processInfo.Type);
                lock (_myLock)
                {
                    if (!_processes.ContainsKey(id))
                        _processes.Add(id, processInfo);
                    else
                        _processes[id] = processInfo;

                }
            }
            return error;
        }

        public Error Remove(Int32 processId, bool errorIfUnknown = true)
        {
            Error error = null;
            if (processId < 0)
                error = Error.Get(Error.StandardErrorNumbers.InvalidProcessId);
            else
            {
                Logger.DebugFormat("Remove Id={0} from ProcessAddressBook", processId);
                lock (_myLock)
                {
                    if (_processes.ContainsKey(processId))
                    {
                        string ep = _processes[processId].ToString();
                        _processes.Remove(processId);
                    }
                    else if (errorIfUnknown)
                        error = Error.Get(Error.StandardErrorNumbers.UnknownProcessId);
                }
            }
            return error;
        }

        public GameProcessData this[Int32 processId]
        {
            get
            {
                GameProcessData result = null;
                lock (_myLock)
                {
                    if (_processes.ContainsKey(processId))
                        result = _processes[processId];
                }
                return result;
            }
        }

        public List<GameProcessData> FilterProcesses(ProcessInfo.ProcessType type)
        {
            List<GameProcessData> result = new List<GameProcessData>();
            lock (_myLock)
            {
                Dictionary<int, GameProcessData>.Enumerator iterator = _processes.GetEnumerator();
                while (iterator.MoveNext())
                    if (iterator.Current.Value.Type == type)
                        result.Add(iterator.Current.Value);
            }
            return result;
        }

        public List<GameProcessData> Processes
        {
            get
            {
                List<GameProcessData> result;
                lock (_myLock)
                {
                    result = _processes.Values.ToList();
                }
                return result;
            }
        }

        public void LogContents()
        {
            lock (_myLock)
            {
                Logger.Debug("Known Game Processes:");
                Dictionary<Int32, GameProcessData>.Enumerator iterator = _processes.GetEnumerator();
                while (iterator.MoveNext())
                    Logger.DebugFormat("{0,10} {1}", iterator.Current.Key, iterator.Current.Value);
            }
        }

    }
}
