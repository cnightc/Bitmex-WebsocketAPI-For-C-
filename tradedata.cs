using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace websocketapi
{
    public class tradedata
    {
        public string symbol { get; set; }
        public string side { get; set; }
        public long size { get; set; }
        public float price { get; set; }        
        public string tickDirection { get; set; }
        public Guid trdMatchID { get; set; }
        public long grossValue { get; set; }
        public float homeNotional { get; set; }
        public float foreignNotional { get; set; }
        //public DateTimeOffset timestamp { get; set; }
       // public DateTime timestamp { get; set; }
        public string timestamp { get; set; }
        //"timestamp":"timestamp","symbol":"symbol","side":"symbol","size":"long","price":"float","tickDirection":"symbol","trdMatchID":"guid","grossValue":"long","homeNotional":"float","foreignNotional":"float"
    }
}
