using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator.Models.MemoryAllocation
{
    public class Memory : INotifyPropertyChanged
    {
        private string color = "Transparent";

        public string Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }

        public bool Occupied { get; set; } = false;

        private DoTask task;

        public DoTask Task
        {
            get
            {
                return task;
            }
            set
            {
                task = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
