using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator.Models.VirtualMemoryPaging
{
    public class PFrame
    {
        public PFrame()
        {

        }

        public int Id { get; set; }

        public VPage VPage { get; set; }
    }
}
