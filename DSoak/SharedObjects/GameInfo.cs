using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class GameInfo
    {
        private List<ProcessInfo> currentProcesses = null;
        private object myLock;

        public enum StatusCode { NotInitialized=1, Initializing=2, Available=4,  Starting=8, InProgress=16, Ending=32, Complete=64, Cancelled=128 };

        [DataMember]
        public Int32 GameId { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public StatusCode Status { get; set; }
        [DataMember]
        public ProcessInfo GameManager { get; set; }
        [DataMember]
        public Int32 MinPlayers { get; set; }
        [DataMember]
        public Int32 MaxPlayers { get; set; }
        [DataMember]
        public Int32 StartingNumberOfPlayers { get; set; }

        public ProcessInfo[] CurrentProcesses
        {
            get
            {
                if (currentProcesses == null)
                    currentProcesses = new List<ProcessInfo>();
                if (myLock == null)
                    myLock = new object();

                ProcessInfo[] result = null;
                lock (myLock)
                {
                    result = currentProcesses.ToArray();
                }
                return result;
            }
            set
            {
                if (myLock == null)
                    myLock = new object();

                lock (myLock)
                {
                    if (value == null)
                        currentProcesses = new List<ProcessInfo>();
                    else
                        currentProcesses = value.ToList();
                }
            }
        }

        [DataMember]
        public Int32 Winner { get; set; }

        public GameInfo()
        {
            SetupDefaults();
        }

        public GameInfo Clone()
        {
            GameInfo clone = MemberwiseClone() as GameInfo;
            clone.GameManager = GameManager.Clone();
            return clone;
        }

        public ProcessInfo FindCurrentProcess(int processId)
        {
            ProcessInfo process = null;
            if (currentProcesses != null)
            {
                if (myLock == null)
                    myLock = new object();

                lock (myLock)
                {
                    process = currentProcesses.Find(p => p.ProcessId == processId);
                }
            }
            return process;
        }

        public void AddCurrentProcess(ProcessInfo process)
        {
            if (myLock == null)
                myLock = new object();

            lock (myLock)
            {
                if (process != null)
                {
                    if (currentProcesses == null)
                        currentProcesses = new List<ProcessInfo>();
                    currentProcesses.Add(process);
                }
            }
        }

        public void RemoveCurrentProcess(int processId)
        {
            if (currentProcesses != null)
            {
                if (myLock == null)
                    myLock = new object();

                lock (myLock)
                {
                    currentProcesses.RemoveAll(p => p.ProcessId == processId);
                }
            }
        }

        private void SetupDefaults()
        {
            if (MinPlayers == 0) MinPlayers = 2;
            if (MaxPlayers == 0) MaxPlayers = 20;
            if ((int) Status == 0) Status = StatusCode.NotInitialized;
        }
    }
}
