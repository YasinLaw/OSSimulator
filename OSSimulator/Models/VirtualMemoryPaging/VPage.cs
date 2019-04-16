using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator.Models.VirtualMemoryPaging
{
    public class VPage : INotifyPropertyChanged
    {
        public VPage()
        {
            Time = 0;
            isEntered = false;
        }

        public int Id { get; set; }

        private int time;

        public int Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                OnPropertyChanged();
            }
        }

        private bool isEntered;

        public bool IsEntered
        {
            get
            {
                return isEntered;
            }
            set
            {
                isEntered = value;
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
