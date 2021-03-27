using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using WebSocketSharp;

namespace Shmelevdi.iRemoteProject.Networking
{
    class WebSocketHandler
    {

        public WebSocket ws;

        private ushort port;
        private void WSHandler(IPAddress deviceip)
        {

        //    ws = new WebSocket("ws://" + deviceip.ToString() + ":" + port);
        //    ws.WaitTime = TimeSpan.FromSeconds(10);
        //    ws.Connect();
        //  //  label6.Invoke(new Action(() => { label6.Text = "Подключаюсь к " + ws.Url.Host.ToString(); }));
        //    ws.OnOpen += (sender, e) =>
        //    {
        //  //      label6.Invoke(new Action(() => { label6.Text = "Жду " + ws.Url.Host.ToString(); }));

        //    };

        //    ws.OnMessage += (sender, e) =>
        //    {

        //   //     label6.Invoke(new Action(() => { label6.Text = "Подключено к " + ws.Url.Host.ToString(); }));
        //        JObject json = JObject.Parse(e.Data);
        //        if (json.ContainsKey("status"))
        //        {
        //            if (json.GetValue("status").ToString() == "connected")
        //            {
        //        //        label6.Invoke(new Action(() => { label6.Text = "Подключено к " + ws.Url.Host.ToString(); }));
        //                Thread pinger = new Thread(statusServer);
        //                pinger.Start();
        //            }
        //        }

        //        if (json.ContainsKey("ir_code"))
        //        {
        //            Thread animation = new Thread(animationHandler);
        //            animation.Start();
        //            //FE0Af5 - minimize\maximize to tray
        //            if (json.GetValue("ir_code").ToString().ToUpper() == "FE0AF5")
        //            {
        //                exem.Invoke(new Action(() => {

        //                    if (exem.WindowState == FormWindowState.Normal)
        //                    {
        //                        exem.WindowState = FormWindowState.Minimized;
        //                        exem.TopMost = false;
        //                    }
        //                    else
        //                    {
        //                        notifyIcon1_MouseDoubleClick(null, null);
        //                        exem.TopMost = true;
        //                    }

        //                }));

        //            }
        //            else
        //            {

        //                label4.Invoke(new Action(() => {
        //                    try
        //                    {
        //                        label4.Text = codes[json.GetValue("ir_code").ToString().ToUpper()];
        //                        SendKeys.Send(codes[json.GetValue("ir_code").ToString().ToUpper()]);
        //                    }
        //                    catch
        //                    { label4.Text = "Err"; }
        //                }));

        //                label1.Invoke(new Action(() => {
        //                    try
        //                    {
        //                        label1.Text = json.GetValue("ir_code").ToString().ToUpper();
        //                    }
        //                    catch
        //                    { label1.Text = "Err"; }
        //                }));
        //            }
        //        }
        //    };

        //    ws.OnClose += (sender, e) =>
        //    {
        //        ws.Connect();
        //        label6.Invoke(new Action(() => { label6.Text = "Соединение потеряно... " + ws.Url.Host.ToString(); }));
        //    };


        //    ws.OnError += (sender, e) =>
        //    {
        //        label6.Invoke(new Action(() => { label6.Text = "Ошибка подключения " + ws.Url.Host.ToString(); }));
        //    };

        }
    }
}
