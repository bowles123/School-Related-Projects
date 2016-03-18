using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class GameInfo
    {
        private List<GameProcessData> _currentProcesses;
        private object _myLock;

        public enum StatusCode { NotInitialized=1, Initializing=2, Available=4,  Starting=8, InProgress=16, Ending=32, Complete=64, Cancelled=128 };

        [DataMember]
        public Int32 GameId { get; set; }
        [DataMember]
        public int GameManagerId { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public StatusCode Status { get; set; }
        [DataMember]
        public int MinPlayers { get; set; }
        [DataMember]
        public int MaxPlayers { get; set; }
        [DataMember]
        public int[] StartingPlayers { get; set; }

        [DataMember]
        public GameProcessData[] CurrentProcesses
        {
            get
            {
                if (_myLock == null)
                    _myLock = new object();
                
                lock (_myLock)
                {
                    if (_currentProcesses == null)
                        _currentProcesses = new List<GameProcessData>();
                }

                GameProcessData[] result;
                lock (_myLock)
                {
                    result = _currentProcesses.ToArray();
                }
                return result;
            }
            set
            {
                if (_myLock == null)
                    _myLock = new object();

                lock (_myLock)
                {
                    _currentProcesses = (value == null) ? new List<GameProcessData>() : value.ToList();
                }
            }
        }

        [DataMember]
        public int[] Winners { get; set; }

        public GameInfo()
        {
            SetupDefaults();
        }

        public GameInfo Clone()
        {
            GameInfo clone = MemberwiseClone() as GameInfo;
            return clone;
        }

        public GameProcessData FindCurrentProcess(int processId)
        {
            GameProcessData process = null;
            if (_currentProcesses != null)
            {
                if (_myLock == null)
                    _myLock = new object();

                lock (_myLock)
                {
                    process = _currentProcesses.Find(p => p.ProcessId == processId);
                }
            }
            return process;
        }

        public void AddProcess(GameProcessData process)
        {
            if (_myLock == null)
                _myLock = new object();

            lock (_myLock)
            {
                if (process != null)
                {
                    if (_currentProcesses == null)
                        _currentProcesses = new List<GameProcessData>();
                    _currentProcesses.Add(process);
                }
            }
        }

        public void RemoveProcess(int processId)
        {
            if (_currentProcesses != null)
            {
                if (_myLock == null)
                    _myLock = new object();

                lock (_myLock)
                {
                    _currentProcesses.RemoveAll(p => p.ProcessId == processId);
                }
            }
        }

        public void ComputeStartingPlayers()
        {
            if (_myLock == null)
                _myLock = new object();

            lock (_myLock)
            {
                if (_currentProcesses == null)
                    StartingPlayers = new int[0];
                else
                {
                    List<GameProcessData> players =
                        _currentProcesses.FindAll(p => p.Type == ProcessInfo.ProcessType.Player);
                    StartingPlayers = players.Select(p => p.ProcessId).ToArray();
                }
            }
        }

        private void SetupDefaults()
        {
            if (MinPlayers == 0) MinPlayers = 2;
            if (MaxPlayers == 0) MaxPlayers = 20;
            if ((int) Status == 0) Status = StatusCode.NotInitialized;
            if (StartingPlayers == null) StartingPlayers = new int[0];
        }
    }
}
