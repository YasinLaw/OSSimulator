using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OSSimulator.Models.ProcessSchedule
{
    public class ThreadCollection : INotifyPropertyChanged
    {
        public ThreadCollection()
        {
            Threads = new BlockingCollection<ThreadModel>();
            BlockedThreads = new BlockingCollection<ThreadModel>();
        }

        private BlockingCollection<ThreadModel> threads;

        public BlockingCollection<ThreadModel> Threads
        {
            get
            {
                return threads;
            }
            set
            {
                threads = value;
                OnPropertyChanged();
            }
        }

        private BlockingCollection<ThreadModel> blockedThreads;

        public BlockingCollection<ThreadModel> BlockedThreads
        {
            get
            {
                return blockedThreads;
            }
            set
            {
                blockedThreads = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
