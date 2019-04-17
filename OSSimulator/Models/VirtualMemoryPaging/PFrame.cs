using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator.Models.VirtualMemoryPaging
{
    public class PFrame : INotifyPropertyChanged, IComparable<PFrame>
    {
        public PFrame()
        {

        }

        public int Id { get; set; }

        private VPage vPage;

        public VPage VPage
        {
            get
            {
                return vPage;
            }
            set
            {
                vPage = value;
                OnPropertyChanged();
            }
        }

        public int Occupied { get; set; } = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int CompareTo(PFrame other)
        {
            return other.Occupied.CompareTo(Occupied);
        }
    }
}
