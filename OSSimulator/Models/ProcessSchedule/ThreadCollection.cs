using System.Collections.Generic;

namespace OSSimulator.Models.ProcessSchedule
{
    public class ThreadCollection
    {
        public ThreadCollection()
        {
            Threads = new List<ThreadModel>();
            BlockedThreads = new List<ThreadModel>();
        }

        public List<ThreadModel> Threads { get; set; }

        public List<ThreadModel> BlockedThreads { get; set; }
    }
}