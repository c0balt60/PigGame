using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigProject
{
    class BroadcastArgs: EventArgs
    {
        public string? Player { get; set; }
        public int? Turn { get; set; }

    }
}
