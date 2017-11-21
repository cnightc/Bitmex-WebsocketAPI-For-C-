using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace websocketapi
{
    public class orderbook
    {
        public string table { get; set; }
        public string action { get; set; }
        public data[] data { get; set; }

    }
}
