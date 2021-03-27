using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Shmelevdi.iRemoteProject.Networking
{

    /// <summary>
    /// Information about the found device
    /// </summary>
    public class DeviceData : EventArgs
    {
        private IPAddress data;
        public IPAddress ip
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

    /// <summary>
    /// Search device in LAN class
    /// </summary>
    public class SearchDevice
    {
        /// <summary>
        /// If the device is found
        /// </summary>
        public event SearchEventHandler Found;

        /// <summary>
        /// If the device is not found
        /// </summary>
        public event SearchEventHandler NotFound;

        /// <summary>
        /// Network Device Search Event Handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public delegate void SearchEventHandler(SearchDevice d, DeviceData e);

        /// <summary>
        /// The mac-address used
        /// </summary>
        private string MAC_ADDRESS;
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="MAC_ADDRESS"></param>
        public SearchDevice(string MAC_ADDRESS)
        {
            this.MAC_ADDRESS = MAC_ADDRESS;
            Thread search = new Thread(Start);
            search.Start();
        }

        /// <summary>
        /// Start of search
        /// </summary>
        private void Start()
        {
            DeviceData dd = new DeviceData();
            Dictionary<IPAddress, PhysicalAddress> all = SearchDeviceSupport.GetLANDevices();
            foreach (KeyValuePair<IPAddress, PhysicalAddress> kvp in all)
            {
                if (kvp.Value.ToString() == MAC_ADDRESS) // ------------------------------ MAC-Address ESP8266 ///"5CCF7F2CC60B"
                {                 
                    Ping ping = new Ping();
                    PingReply pr = ping.Send(kvp.Key.ToString());
                    if (pr.Status == IPStatus.Success)
                    {          
                        //if found
                        dd.ip = kvp.Key;
                        if (Found != null)  Found(this, dd);
                    }
                    else
                    {
                        //If not found or err
                        if (NotFound != null) NotFound(this, dd);
                    }
                }
            }
            if (dd.ip == null)
            {
                if (NotFound != null)  NotFound(this, dd);
                //if not found
            }
        }
    }

}
