using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WebSocketSharp;

namespace Shmelevdi.iRemoteProject.Networking
{
    public class WebSocketHandler
    {

        public WebSocket ws;

        private ushort port;

        public WebSocketHandler(IPAddress deviceip, ushort port)
        {
            ws = new WebSocket("ws://" + deviceip.ToString() + ":" + port.ToString());
            ws.WaitTime = TimeSpan.FromSeconds(10);
            ws.Connect();
            Start();
        }

        public string getCurrentHost()
        {
            return ws.Url.Host.ToString();
        }

        private void Start()
        {
            ws.OnOpen += (sender, e) =>
            {
                //Waiting for connection processing

            };

            ws.OnMessage += (sender, e) =>
            {
                JObject json = JObject.Parse(e.Data);
                if (json.ContainsKey("status"))
                {
                    if (json.GetValue("status").ToString() == "connected")
                    {
                        //Connected
                        MessageBox.Show("Conected");
                    }
                }
                if (json.ContainsKey("ir_code"))
                {
                    //Received an IR signal

                }
                else
                {
                    //Received something another

                }
            };

            ws.OnClose += (sender, e) =>
            {
                //When closing. Autoreconnect, etc.
            };


            ws.OnError += (sender, e) =>
            {
                //Connection error
            };

        }
}
}
