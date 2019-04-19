using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator.Models.MemoryAllocation
{
    public class DoTask : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private State taskState = State.UNINSTALLED;

        public State TaskState
        {
            get
            {
                return taskState;
            }
            set
            {
                taskState = value;
                OnPropertyChanged();
            }
        }

        private string color;
    
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

        private int memoryId = -1;

        public int MemoryId
        {
            get
            {
                return memoryId;
            }
            set
            {
                memoryId = value;
                OnPropertyChanged();
            }
        }

        public int Length { get; set; }

        public enum State
        {
            INSTALLED,
            UNINSTALLED
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
