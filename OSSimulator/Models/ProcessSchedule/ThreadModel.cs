﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator.Models.ProcessSchedule
{
    public class ThreadModel : IComparable<ThreadModel>, INotifyPropertyChanged
    {
        private int pid;

        public int Pid
        {
            get
            {
                return pid;
            }
            set
            {
                pid = value;
                OnPropertyChanged();
            }
        }

        private long priority;

        public long Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
                OnPropertyChanged();
            }
        }

        private long time;

        public long Time
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

        private long value;

        public long Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        private State state;

        public State ProcState
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int CompareTo(ThreadModel other)
        {
            return other.Priority.CompareTo(Priority);
        }

        public enum State
        {
            READY,
            RUNNING,
            FINISHED,
            BLOCKED
        }
    }
}