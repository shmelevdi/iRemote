using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Shmelevdi.iRemoteProject.Networking
{
    /// <summary>
    /// Information about the found device
    /// </summary>
    public class ServerStatusArgs : EventArgs
    {
        private IPStatus data;
        /// <summary>
        /// Returns the server status object
        /// </summary>
        public IPStatus status
        {
            set
            {
                data = value;
            }
            get
            {
                return this.data;
            }
        }
    }
    public class ServerStatus
    {

        /// <summary>
        /// When ServerStatus is die
        /// </summary>
        public event ServerStatusEventHandler Die;

        /// <summary>
        /// When ServerStatus is alive
        /// </summary>
        public event ServerStatusEventHandler Alive;

        public delegate void ServerStatusEventHandler(ServerStatus s, ServerStatusArgs e);

        /// <summary>
        /// IP of the found device
        /// </summary>
        private IPAddress device_ip;

        /// <summary>
        /// Time of Thread.Sleep between ping events
        /// </summary>
        private int sleep;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device_ip"></param>
        public ServerStatus(IPAddress device_ip, int sleep=10000)
        {
           
            this.device_ip = device_ip;
            this.sleep = sleep;
            Thread ss = new Thread(Listen);
            ss.Start();
        }

        /// <summary>
        /// PING-PONG with the transmitted device
        /// </summary>
        private void Listen()
        {          
            while (true)
            {
                ServerStatusArgs e = new ServerStatusArgs();
                Ping ping = new Ping();
                PingReply pr = ping.Send(device_ip.ToString());
                e.status = pr.Status;
                if (pr.Status != IPStatus.Success)
                {
                    if (Die != null) Die(this, e);
                    break;
                }
                else
                     if (Alive != null) Alive(this, e);
                Thread.Sleep(sleep);
            }

        }
    }
}
