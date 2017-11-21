using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace websocketapi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "WebsocketForBitmex";
        }

        private WebSocket webSocketClient = null;
        private Boolean isRecon = false;
        private System.Timers.Timer t = new System.Timers.Timer(2000);
        private string url = "wss://www.bitmex.com/realtime?subscribe=orderBookL2:XBTUSD,trade:XBTUSD";
        public static Dictionary<long, data> dicdata = new Dictionary<long, data>();
        
        private static float tradeprice;

        public void start()
        {
            webSocketClient = new WebSocket(url);

            webSocketClient.OnError += new EventHandler<WebSocketSharp.ErrorEventArgs>(webSocketClient_Error);
            webSocketClient.OnOpen += new EventHandler(webSocketClient_Opened);
            webSocketClient.OnClose += new EventHandler<WebSocketSharp.CloseEventArgs>(webSocketClient_Closed);
            webSocketClient.OnMessage += new EventHandler<MessageEventArgs>(webSocketClient_MessageReceived);
            webSocketClient.ConnectAsync();
            while (!webSocketClient.IsAlive)
            {
                Console.WriteLine("Waiting WebSocket connnet......");
                Thread.Sleep(1000);
            }
            t.Elapsed += new System.Timers.ElapsedEventHandler(heatBeat);
            t.Start();
        }

        private void heatBeat(object sender, System.Timers.ElapsedEventArgs e)
        {

            this.send("{'event':'ping'}");
        }
        private void webSocketClient_Error(object sender, WebSocketSharp.ErrorEventArgs e)
        {

        }
        private void webSocketClient_MessageReceived(object sender, MessageEventArgs e)
        {
            onReceive(e.Data);
        }

        private void webSocketClient_Closed(object sender, WebSocketSharp.CloseEventArgs e)
        {
            if (!webSocketClient.IsAlive)
            {
                isRecon = true;
                webSocketClient.ConnectAsync();
            }
        }
        public string Decompress(byte[] baseBytes)
        {
            string resultStr = string.Empty;
            using (MemoryStream memoryStream = new MemoryStream(baseBytes))
            {
                using (InflaterInputStream inf = new InflaterInputStream(memoryStream))
                {
                    using (MemoryStream buffer = new MemoryStream())
                    {
                        byte[] result = new byte[1024];

                        int resLen;
                        while ((resLen = inf.Read(result, 0, result.Length)) > 0)
                        {
                            buffer.Write(result, 0, resLen);
                        }
                        resultStr = Encoding.Default.GetString(result);
                    }
                }
            }
            return resultStr;
        }
        private void webSocketClient_Opened(object sender, EventArgs e)
        {
            this.send("{'event':'ping'}");
        }

        public Boolean isReconnect()
        {
            if (isRecon)
            {
                if (webSocketClient.IsAlive)
                {
                    isRecon = false;
                }
                return true;
            }
            return false;
        }
        public void send(string channle)
        {
            webSocketClient.Send(channle);
        }
        public void stop()
        {
            if (webSocketClient != null)
                webSocketClient.Close();
        }

        public void onReceive(string msg)
        {
            try
            {
                if (msg.Contains("error"))
                {
                    return;
                }

                if (msg.Contains("success"))
                {
                    return;
                }

                if (msg.Contains("trade"))
                {
                    #region table=trade
                    #region buyong
                    #endregion
                    if (msg.Contains("insert"))
                    {
                        List<trade> tradelist = JsonConvert.DeserializeObject<List<trade>>("[" + msg + "]");
                        if (tradelist.Count <= 0)
                            return;
                        var tradedata = tradelist[tradelist.Count - 1].data;
                        if (tradedata.Length <= 0)
                            return;


                        tradeprice = tradedata[tradedata.Length - 1].price;
                        tradeSide.Text = tradedata[tradedata.Length - 1].side;

                        price.Text = tradeprice.ToString();



                        #region MyRegion




                        #endregion

                        #region MyRegion



                        #endregion
                    }

                    #endregion
                }
                if (msg.Contains("orderBookL2"))
                {
                    #region table=orderbookL2
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
                    }
                    if (dicdata.Keys.Count <= 0) return;
                    if (msg.Contains("update"))
                    {
                        var rrr = JsonConvert.DeserializeObject<List<orderbook>>("[" + msg + "]");
                        foreach (var item in rrr)
                        {
                            foreach (var t in item.data)
                            {
                                var temp = dicdata[t.id];
                                dicdata[t.id].size = t.size;
                            }
                        }
                    }

                    if (msg.Contains("insert"))
                    {
                        var rrr = JsonConvert.DeserializeObject<List<orderbook>>("[" + msg + "]");
                        foreach (var item in rrr)
                        {
                            foreach (var t in item.data)
                            {
                                dicdata.Add(t.id, t);
                            }
                        }
                    }
                    if (msg.Contains("delete"))
                    {
                        var rrr = JsonConvert.DeserializeObject<List<orderbook>>("[" + msg + "]");
                        foreach (var item in rrr)
                        {
                            foreach (var t in item.data)
                            {
                                var temp = dicdata[t.id];
                                dicdata.Remove(t.id);
                            }
                        }
                    }
                    #endregion

                    if (tradeprice > 0)
                    {
                        var newtemp = dicdata.Values.OrderBy(c => c.price).ToList();
                        var temp = newtemp.SingleOrDefault(c => c.symbol == "XBTUSD" && c.price == tradeprice);
                        if (temp == null) return;
                        int p = newtemp.IndexOf(temp);

                        if (tradeSide.Text == "Sell")
                        {
                            this.thighprice1.Text = newtemp[p + 5].price.ToString();
                            this.thighprice2.Text = newtemp[p + 4].price.ToString();
                            this.thighprice3.Text = newtemp[p + 3].price.ToString();
                            this.thighprice4.Text = newtemp[p + 2].price.ToString();
                            this.thighprice5.Text = newtemp[p + 1].price.ToString();

                            this.thighsize1.Text = newtemp[p + 5].size.ToString();
                            this.thighsize2.Text = newtemp[p + 4].size.ToString();
                            this.thighsize3.Text = newtemp[p + 3].size.ToString();
                            this.thighsize4.Text = newtemp[p + 2].size.ToString();
                            this.thighsize5.Text = newtemp[p + 1].size.ToString();

                            this.tlowprice1.Text = newtemp[p - 0].price.ToString();
                            this.tlowprice2.Text = newtemp[p - 1].price.ToString();
                            this.tlowprice3.Text = newtemp[p - 2].price.ToString();
                            this.tlowprice4.Text = newtemp[p - 3].price.ToString();
                            this.tlowprice5.Text = newtemp[p - 4].price.ToString();

                            this.tlowsize1.Text = newtemp[p - 0].size.ToString();
                            this.tlowsize2.Text = newtemp[p - 1].size.ToString();
                            this.tlowsize3.Text = newtemp[p - 2].size.ToString();
                            this.tlowsize4.Text = newtemp[p - 3].size.ToString();
                            this.tlowsize5.Text = newtemp[p - 4].size.ToString();
                        }
                        if (tradeSide.Text == "Buy")
                        {
                            this.thighprice1.Text = newtemp[p + 4].price.ToString();
                            this.thighprice2.Text = newtemp[p + 3].price.ToString();
                            this.thighprice3.Text = newtemp[p + 2].price.ToString();
                            this.thighprice4.Text = newtemp[p + 1].price.ToString();
                            this.thighprice5.Text = newtemp[p + 0].price.ToString();

                            this.thighsize1.Text = newtemp[p + 4].size.ToString();
                            this.thighsize2.Text = newtemp[p + 3].size.ToString();
                            this.thighsize3.Text = newtemp[p + 2].size.ToString();
                            this.thighsize4.Text = newtemp[p + 1].size.ToString();
                            this.thighsize5.Text = newtemp[p + 0].size.ToString();

                            this.tlowprice1.Text = newtemp[p - 1].price.ToString();
                            this.tlowprice2.Text = newtemp[p - 2].price.ToString();
                            this.tlowprice3.Text = newtemp[p - 3].price.ToString();
                            this.tlowprice4.Text = newtemp[p - 4].price.ToString();
                            this.tlowprice5.Text = newtemp[p - 5].price.ToString();

                            this.tlowsize1.Text = newtemp[p - 1].size.ToString();
                            this.tlowsize2.Text = newtemp[p - 2].size.ToString();
                            this.tlowsize3.Text = newtemp[p - 3].size.ToString();
                            this.tlowsize4.Text = newtemp[p - 4].size.ToString();
                            this.tlowsize5.Text = newtemp[p - 5].size.ToString();
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(msg);
                Console.WriteLine(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            stop();
            this.Close();
        }


    }
}
