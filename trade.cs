using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace websocketapi
{
    public class trade
    {
        public string table { get; set; }
        public string action { get; set; }
        public tradedata[] data { get; set; }

        //{"table":"trade","keys":[],"types":{"timestamp":"timestamp","symbol":"symbol","side":"symbol","size":"long","price":"float","tickDirection":"symbol","trdMatchID":"guid","grossValue":"long","homeNotional":"float","foreignNotional":"float"},"foreignKeys":{"symbol":"instrument","side":"side"},"attributes":{"timestamp":"sorted","symbol":"grouped"},"action":"partial","data":[{"timestamp":"2017-11-12T07:57:10.264Z","symbol":"XBTUSD","side":"Buy","size":632,"price":6134,"tickDirection":"PlusTick","trdMatchID":"2b3bac52-d670-6c20-20d7-ef3afd1851ae","grossValue":10303496,"homeNotional":0.10303496,"foreignNotional":632}],"filter":{"symbol":"XBTUSD"}}
    }
}
