using System;
using System.Net;
using System.Net.NetworkInformation;
using Shmelevdi.iRemoteProject.Networking;
using System.Windows.Forms;
using Shmelevdi.iRemoteProject.Settings;
using Newtonsoft.Json.Linq;

namespace Shmelevdi.iRemoteProject
{
    public class iRemote
    {
        public static string version = "1.0";
        /// <summary>
        /// Information about the found device
        /// </summary>
        public class iRemoteStatusData : EventArgs
        {
            private IPAddress ipdata;
            private IPStatus state;
            private ushort portd;

            /// <summary>
            /// Returns the IP of the found device
            /// </summary>
            public IPAddress ip
            {
                set
                {
                    ipdata = value;
                }
                get
                {
                    return this.ipdata;
                }
            }

            /// <summary>
            /// Returns the server ping status
            /// </summary>
            public IPStatus status
            {
                set
                {
                    state = status;
                }
                get
                {
                    return this.state;
                }
            }

            public ushort port
            {
                set
                {
                    portd = port;
                }
                get
                {
                    return this.portd;
                }
            }
        }

        public class iRemoteCommData : EventArgs
        {
            private JObject irerr;
            private JObject irstatus;
            private JObject ircmd;

            /// <summary>
            /// Returns a JObject with the received signal
            /// </summary>
            public JObject IR_Command
            {
                set
                {
                    ircmd = value;
                }
                get
                {
                    return this.ircmd;
                }
            }

            /// <summary>
            /// Returns a JObject with the received error about the device status
            /// </summary>
            public JObject IR_Status
            {
                set
                {
                    irstatus = value;
                }
                get
                {
                    return this.irstatus;
                }
            }

            /// <summary>
            /// Returns a JObject with the received error from the device
            /// </summary>
            public JObject IR_Error
            {
                set
                {
                    irerr = value;
                }
                get
                {
                    return this.irerr;
                }
            }


        }

        /// <summary>
        /// If the device is found
        /// </summary>
        public event iRemoteStatusEventHandler DeviceFound;

        /// <summary>
        /// If the device is not found
        /// </summary>
        public event iRemoteStatusEventHandler DeviceNotFound;

        /// <summary>
        /// If the device is dead
        /// </summary>
        public event iRemoteStatusEventHandler DeviceIsDead;

        /// <summary>
        /// If the device is alive
        /// </summary>
        public event iRemoteStatusEventHandler DeviceIsAlive;

        /// <summary>
        /// When receiving IR data
        /// </summary>
        public event iRemoteCommEventHandler OnIRData;

        /// <summary>
        /// When getting the device status
        /// </summary>
        public event iRemoteCommEventHandler OnStatus;

        /// <summary>
        /// When receiving a device error
        /// </summary>
        public event iRemoteCommEventHandler OnError;

        /// <summary>
        /// Network Device Search Event Handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public delegate void iRemoteStatusEventHandler(iRemote i, iRemoteStatusData e);

        /// <summary>
        /// Event handler for incoming signals from the device
        /// </summary>
        /// <param name="i"></param>
        /// <param name="e"></param>
        public delegate void iRemoteCommEventHandler(iRemote i, iRemoteCommData e);

        public IPAddress irdeviceip;
        public SearchDevice SearchDevice;
        public ServerStatus ServerStatus;
        public iRemoteStatusData irdata;
        public Configuration conf;
        public WebSocketHandler WSHandler;
        public string MAC_String;

        /// <summary>
        /// Accepts a MAC address without colons as a string parameter.For example: "5CCF7F2CC60B"
        /// </summary>
        /// <param name="MAC_String"></param>
        public iRemote()
        {
            this.irdata = new iRemoteStatusData();
            this.conf = new Configuration();
        }

        public void Start()
        {
            SearchDevice = new SearchDevice(conf.Device.MAC);
            SearchDevice.Found += (sender, e) =>
            {
                irdata.ip = e.ip;
                DeviceFound(this, irdata);
                ServerStatus = new ServerStatus(e.ip);//server pinger
                WSHandler = new WebSocketHandler(irdata.ip, conf.Device.ManualPort); //websocket client
                this.WSHandle();
                ServerStatus.Die += (sndr, eargs) =>
                {
                    irdata.status = eargs.status;
                    if (DeviceIsDead != null) DeviceIsDead(this, irdata);
                };
                ServerStatus.Alive += (sndr, eargs) =>
                {
                    irdata.status = eargs.status;
                    if (DeviceIsAlive != null) DeviceIsAlive(this, irdata);
                };

            };

            SearchDevice.NotFound += (sender, e) =>
            {
                irdata.ip = e.ip;
                if (DeviceNotFound != null) DeviceNotFound(this, irdata);
            };
        }

        /// <summary>
        /// Initializing WebSocket event handlers
        /// </summary>
        private void WSHandle()
        {
            this.CommHandle();
            WSHandler.ws.OnOpen += (sender, e) =>
            {
                //Waiting for connection processing

            };

            WSHandler.ws.OnMessage += (sender, e) =>
            {
                iRemoteCommData commdata = new iRemoteCommData();
                JObject json = JObject.Parse(e.Data);
                if (json.ContainsKey("status"))
                {
                    if (json.GetValue("status").ToString() == "connected")
                    {
                        //Connected
                        commdata.IR_Status = json;
                        if (OnStatus != null) OnStatus(this, commdata);
                    }
                }
                if (json.ContainsKey("ir_code"))
                {
                    //Received an IR signal
                    commdata.IR_Command = json;
                    if (OnIRData != null) OnIRData(this, commdata);
                }
                else
                {
                    //Received something another
                    commdata.IR_Error = json;
                    if (OnError != null) OnError(this, commdata);
                }
            };

            WSHandler.ws.OnClose += (sender, e) =>
            {
                //When closing. Autoreconnect, etc.
            };


            WSHandler.ws.OnError += (sender, e) =>
            {
                //Connection error
            };

        }

        /// <summary>
        /// Initializing event handlers from the device
        /// </summary>
        private void CommHandle()
        {
            OnIRData += (sender, e) =>
            {
                MessageBox.Show(conf.Codes.Storage[e.IR_Command.GetValue("ir_code").ToString().ToUpper()].Name);
                
            };

            OnStatus += (sender, e) =>
            {
                MessageBox.Show(e.IR_Status.GetValue("status").ToString());

            };
        }

    }

}
