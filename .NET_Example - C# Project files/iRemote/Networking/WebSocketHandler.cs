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
        }

        public string getCurrentHost()
        {
            return ws.Url.Host.ToString();
        }
       
    }
}
