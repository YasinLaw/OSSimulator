using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator.Models.VirtualMemoryPaging
{
    public class VM
    {
        public VM()
        {
            VPages = new List<VPage>();
            PFrames = new List<PFrame>();
        }

        public List<VPage> VPages { get; set; }

        public List<PFrame> PFrames { get; set; }
    }
}
