using System.Threading;
using UnityEngine.Serialization;

namespace RoadArchitect.Threading {
    public class ThreadedJob {
        [FormerlySerializedAs("m_Handle")] private readonly object handle = new object();

        [FormerlySerializedAs("m_IsDone")] private bool isDone;

        [FormerlySerializedAs("m_Thread")] private Thread thread;


        public bool IsDone
        {
            get
            {
                bool tmp;
                lock (handle) {
                    tmp = isDone;
                }

                return tmp;
            }
            set
            {
                lock (handle) {
                    isDone = value;
                }
            }
        }


        public virtual void Start() {
            thread = new Thread(Run);
            thread.Start();
        }


        public virtual void Abort() {
            thread.Abort();
        }


        protected virtual void ThreadFunction() { }


        protected virtual void OnFinished() { }


        public virtual bool Update() {
            if (IsDone) {
                OnFinished();
                return true;
            }

            return false;
        }


        private void Run() {
            ThreadFunction();
            IsDone = true;
        }
    }
}