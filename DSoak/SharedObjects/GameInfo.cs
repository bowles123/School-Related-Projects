using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Policy;
using log4net.Repository.Hierarchy;

namespace SharedObjects
{
    [DataContract]
    public class GameInfo
    {
        private object _myLock;
        private bool _isDirty;

        private StatusCode _status;
        private int[] _startingPlayers;
        private List<GameProcessData> _currentProcesses;
        private int[] _winners;

        public enum StatusCode { NotInitialized=1, Initializing=2, Available=4,  Starting=8, InProgress=16, Ending=32, Complete=64, Cancelled=128 };

        [DataMember]
        public Int32 GameId { get; set; }
        [DataMember]
        public int GameManagerId { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public int MinPlayers { get; set; }
        [DataMember]
        public int MaxPlayers { get; set; }

        [DataMember]
        public StatusCode Status
        {
            get { return _status; }
            set
            {
                if (_myLock == null)
                    _myLock = new object();

                lock (_myLock)
                {
                    if (_status != value)
                    {
                        _status = value;
                        _isDirty = true;
                    }
                }
            }
        }

        [DataMember]
        public int[] StartingPlayers
        {
            get { return _startingPlayers; }
            set
            {
                if (_myLock == null)
                    _myLock = new object();

                lock (_myLock)
                {
                    if (StartingPlayersAreDifferent(value))
                    {
                        _isDirty = true;
                        _startingPlayers = value;
                    }
                }
            }
            
        }

        private bool StartingPlayersAreDifferent(int[] newStartingPlayers)
        {
            bool result = (_startingPlayers == null && newStartingPlayers != null) ||
                          (_startingPlayers != null && newStartingPlayers == null) ||
                          (_startingPlayers != null && newStartingPlayers !=null &&
                            _startingPlayers.Length != newStartingPlayers.Length);

            if (!result && _startingPlayers != null && newStartingPlayers != null)
            {
                result = _startingPlayers.Any(p => !newStartingPlayers.Contains(p));
            }

            return result;
        }

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
                    _isDirty = true;
                }
            }
        }

        [DataMember]
        public int[] Winners
        {
            get { return _winners; }
            set
            {
                if (_myLock == null)
                    _myLock = new object();

                lock (_myLock)
                {
                    _winners = value;
                    _isDirty = true;
                }                
            }
        }

        public bool IsDirty { get { return _isDirty; } }
        public void ClearDirtyFlag() { _isDirty = false;  }

        public GameInfo()
        {
            SetupDefaults();
        }

        public GameInfo Clone()
        {
            GameInfo clone = MemberwiseClone() as GameInfo;
            return clone;
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
