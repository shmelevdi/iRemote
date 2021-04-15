﻿using System;
using System.Net;
using System.Net.NetworkInformation;
using Shmelevdi.iRemoteProject.Networking;
using System.Windows.Forms;
using Shmelevdi.iRemoteProject.Settings;

namespace Shmelevdi.iRemoteProject
{
    public class iRemote
    {
        public static string version = "1.0";
        /// <summary>
        /// Information about the found device
        /// </summary>
        public class iRemoteData : EventArgs
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

        /// <summary>
        /// If the device is found
        /// </summary>
        public event iRemoteEventHandler DeviceFound;

        /// <summary>
        /// If the device is not found
        /// </summary>
        public event iRemoteEventHandler DeviceNotFound;

        /// <summary>
        /// If the device is dead
        /// </summary>
        public event iRemoteEventHandler DeviceIsDead;

        /// <summary>
        /// If the device is alive
        /// </summary>
        public event iRemoteEventHandler DeviceIsAlive;

        /// <summary>
        /// Network Device Search Event Handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public delegate void iRemoteEventHandler(iRemote i, iRemoteData e);

        public IPAddress irdeviceip;
        public SearchDevice SearchDevice;
        public ServerStatus ServerStatus;
        public iRemoteData irdata;
        public Configuration conf;
        public WebSocketHandler WSHandler;
        public string MAC_String;

        /// <summary>
        /// Accepts a MAC address without colons as a string parameter.For example: "5CCF7F2CC60B"
        /// </summary>
        /// <param name="MAC_String"></param>
        public iRemote()
        {
            this.irdata = new iRemoteData();
            this.conf = new Configuration();
        }

        public void Start()
        {
            SearchDevice = new SearchDevice(conf.Device.MAC);
            SearchDevice.Found += (sender, e) => {
                irdata.ip = e.ip;
                DeviceFound(this, irdata);
                ServerStatus = new ServerStatus(e.ip);//server pinger
                WSHandler = new WebSocketHandler(irdata.ip, conf.Device.ManualPort); //websocket client

                ServerStatus.Die += (sndr, eargs) => {
                    irdata.status = eargs.status;
                    if (DeviceIsDead != null) DeviceIsDead(this, irdata);
                };
                ServerStatus.Alive += (sndr, eargs) => {
                    irdata.status = eargs.status;
                   if(DeviceIsAlive != null) DeviceIsAlive(this, irdata);
                };

            };

            SearchDevice.NotFound += (sender, e) => {
                irdata.ip = e.ip;
                if (DeviceNotFound != null) DeviceNotFound(this, irdata);
            };        
        }

    }

}
