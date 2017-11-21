using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Data;
namespace websocketapi
{
    class BuissnesServiceImpl : WebSocketService
    {
        public static Dictionary<long, data> dicdata = new Dictionary<long, data>();

        public void onReceive(string msg)
        {
            try
            {
                if (msg.Contains("error"))
                    Console.WriteLine(msg);
                if (msg.Contains("partial"))
                {
                    List<orderbook> orderbooklist = JsonConvert.DeserializeObject<List<orderbook>>("[" + msg + "]");
                    foreach (var datas in orderbooklist)
                    {
                        foreach (var item in datas.data)
                        {
                            if (item.symbol.Equals("XBTUSD") && !dicdata.Keys.Contains(item.id))
                                dicdata.Add(item.id, item);
                        }
                    }
                    //Console.WriteLine("读取到数据条数：" + dicdata.Keys.Count);
                }
                if (dicdata.Keys.Count <= 0) return;
                if (msg.Contains("update"))
                {
                    var rrr = JsonConvert.DeserializeObject<List<orderbook>>("[" + msg + "]");
                    foreach (var item in rrr)
                    {
                        //Console.WriteLine("table:" + item.table + "     " + "action:" + item.action);
                        foreach (var t in item.data)
                        {
                            var temp = dicdata[t.id];
                            //Console.WriteLine("更新前：" + "symbol:" + temp.symbol + "     " + "id:" + temp.id + "     " + "side:" + temp.side + "     " + "size:" + temp.size);
                            //Console.WriteLine("更新后：" + "symbol:" + t.symbol + "     " + "id:" + t.id + "     " + "side:" + t.side + "     " + "size:" + t.size);
                            dicdata.Remove(temp.id);
                            dicdata.Add(t.id, t);
                        }
                    }
                }

                if (msg.Contains("insert"))
                {
                    var rrr = JsonConvert.DeserializeObject<List<orderbook>>("[" + msg + "]");
                    foreach (var item in rrr)
                    {
                        //Console.WriteLine("table:" + item.table + "     " + "action:" + item.action);
                        foreach (var t in item.data)
                        {
                            //Console.WriteLine("新增数据：    " + "symbol:" + t.symbol + "     " + "id:" + t.id + "     " + "side:" + t.side + "     " + "size:" + t.size);
                            dicdata.Add(t.id, t);
                        }
                    }
                }
                if (msg.Contains("delete"))
                {
                    var rrr = JsonConvert.DeserializeObject<List<orderbook>>("[" + msg + "]");
                    foreach (var item in rrr)
                    {
                        //Console.WriteLine("table:" + item.table + "     " + "action:" + item.action);
                        foreach (var t in item.data)
                        {
                            var temp = dicdata[t.id];
                            //Console.WriteLine("symbol:" + t.symbol + "     " + "id:" + t.id + "     " + "side:" + t.side + "     " + "size:" + t.size);
                            dicdata.Remove(t.id);
                        }
                    }
                }
                //foreach (var t in dicdata.Values.Take(5))
                //{
                   // Console.WriteLine("symbol:" + t.symbol + "     " + "id:" + t.id + "     " + "side:" + t.side + "     " + "size:" + t.size + "     price:" + t.price);
                //}
                //Console.WriteLine("--------------------");
               // Console.WriteLine(dicdata.Values.Count.ToString());
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(msg);
                Console.WriteLine(ex.Message);
            }
        }
    }
    public class orderbook
    {
        public string table { get; set; }
        public string action { get; set; }
        public data[] data { get; set; }

    }
    public class data
    {
        public string symbol { get; set; }
        public long id { get; set; }
        public string side { get; set; }
        public long size { get; set; }
        public float price { get; set; }
    }
    #region MyRegion
    public class data2
    {
        public string high { get; set; }
        public string vol { get; set; }
        public string last { get; set; }
        public string low { get; set; }
        public string buy { get; set; }
        public string change { get; set; }
        public string sell { get; set; }
        public string dayLow { get; set; }
        public string close { get; set; }
        public string dayHigh { get; set; }
        public string open { get; set; }
        public string timestamp { get; set; }
    }

    public class table1
    {
        public string table { get; set; }
        public string action { get; set; }
        //public insertdata insertdata { get; set; }
        //public updatedata updatedata { get; set; }
        //public deletedata deletedata { get; set; }
    }
    public class updatetable : table1
    {
        public updatedata updatedata { get; set; }
    }
    public class deletetable : table1
    {
        public deletedata deletedata { get; set; }
    }

    public class insertdata
    {
        public string symbol { get; set; }
        public long id { get; set; }
        public string side { get; set; }
        public long size { get; set; }
        public float price { get; set; }
    }

    public class updatedata
    {
        public string symbol { get; set; }
        public long id { get; set; }
        public string side { get; set; }
        public long size { get; set; }

    }
    public class deletedata
    {
        public string symbol { get; set; }
        public long id { get; set; }
        public string side { get; set; }
    }














    public class Info
    {
        public string phantom { get; set; }
        public string id { get; set; }
        public data data { get; set; }
    }
    #endregion





}
