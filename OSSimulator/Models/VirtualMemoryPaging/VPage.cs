using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator.Models.VirtualMemoryPaging
{
    public class VPage
    {
        public VPage()
        {
            Time = 0;
        }

        public int Id { get; set; }

        public int Time { get; set; }
    }
}
