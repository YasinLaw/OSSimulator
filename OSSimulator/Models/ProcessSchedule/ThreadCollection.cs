using System.Collections.Generic;

namespace OSSimulator.Models.ProcessSchedule
{
    public class ThreadCollection
    {
        public ThreadCollection()
        {
            RunningThreads = new List<ThreadModel>();
            BlockedThreads = new List<ThreadModel>();
            Threads = new List<ThreadModel>();
        }

        public List<ThreadModel> RunningThreads { get; set; }

        public List<ThreadModel> BlockedThreads { get; set; }

        public List<ThreadModel> Threads { get; set; }
    }
}