using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonteCarlo
{
    class LoopThreadState
    {
        Random random = new Random();

        public long Count { get; set; }

        public Random RandomNumberGenerator { get { return random; } }
    }
}
