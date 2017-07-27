using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaStreams
{
    abstract class Messages
    {
        public class StartSending
        { }

        public class StreamComplete
        { }
    }
}
