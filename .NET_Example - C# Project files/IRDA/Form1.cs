using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;

namespace IRDA
{
   
    public partial class Form1 : Form
    {
        public Form exem;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        [StructLayout(LayoutKind.Sequential)]
        struct MIB_IPNETROW
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwIndex;
            [MarshalAs(UnmanagedType.U4)]
            public int dwPhysAddrLen;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac0;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac1;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac2;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac3;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac4;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac5;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac6;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac7;
            [MarshalAs(UnmanagedType.U4)]
            public int dwAddr;
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
        }

        [DllImport("IpHlpApi.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int GetIpNetTable(IntPtr pIpNetTable,
              [MarshalAs(UnmanagedType.U4)] ref int pdwSize, bool bOrder);

        private static Dictionary<IPAddress, PhysicalAddress> GetAllDevicesOnLAN()
        {
            Dictionary<IPAddress, PhysicalAddress> all = new Dictionary<IPAddress, PhysicalAddress>();
            all.Add(GetIPAddress(), GetMacAddress());
            int spaceForNetTable = 0;
            GetIpNetTable(IntPtr.Zero, ref spaceForNetTable, false);
            IntPtr rawTable = IntPtr.Zero;
            try
            {
                rawTable = Marshal.AllocCoTaskMem(spaceForNetTable);
                int errorCode = GetIpNetTable(rawTable, ref spaceForNetTable, false);
                if (errorCode != 0)
                {
                    throw new Exception(string.Format(
                      "Unable to retrieve network table. Error code {0}", errorCode));
                }
                int rowsCount = Marshal.ReadInt32(rawTable);
                IntPtr currentBuffer = new IntPtr(rawTable.ToInt64() + Marshal.SizeOf(typeof(Int32)));
                MIB_IPNETROW[] rows = new MIB_IPNETROW[rowsCount];
                for (int index = 0; index < rowsCount; index++)
                {
                    rows[index] = (MIB_IPNETROW)Marshal.PtrToStructure(new IntPtr(currentBuffer.ToInt64() +
                                                (index * Marshal.SizeOf(typeof(MIB_IPNETROW)))
                                               ),
                                                typeof(MIB_IPNETROW));
                }
                PhysicalAddress virtualMAC = new PhysicalAddress(new byte[] { 0, 0, 0, 0, 0, 0 });
                PhysicalAddress broadcastMAC = new PhysicalAddress(new byte[] { 255, 255, 255, 255, 255, 255 });
                foreach (MIB_IPNETROW row in rows)
                {
                    IPAddress ip = new IPAddress(BitConverter.GetBytes(row.dwAddr));
                    byte[] rawMAC = new byte[] { row.mac0, row.mac1, row.mac2, row.mac3, row.mac4, row.mac5 };
                    PhysicalAddress pa = new PhysicalAddress(rawMAC);
                    if (!pa.Equals(virtualMAC) && !pa.Equals(broadcastMAC) && !IsMulticast(ip))
                    {
                        if (!all.ContainsKey(ip))
                        {
                            all.Add(ip, pa);
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(rawTable);
            }
            return all;
        }

        private static IPAddress GetIPAddress()
        {
            String strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            foreach (IPAddress ip in addr)
            {
                if (!ip.IsIPv6LinkLocal)
                {
                    return (ip);
                }
            }
            return addr.Length > 0 ? addr[0] : null;
        }

        private static PhysicalAddress GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress();
                }
            }
            return null;
        }

        private static bool IsMulticast(IPAddress ip)
        {
            bool result = true;
            if (!ip.IsIPv6Multicast)
            {
                byte highIP = ip.GetAddressBytes()[0];
                if (highIP < 224 || highIP > 239)
                {
                    result = false;
                }
            }
            return result;
        }


        public Form1()
        {
            InitializeComponent();
            exem = this;

        }
        Dictionary<string, string> codes = new Dictionary<string, string>();

        private void Form1_Load(object sender, EventArgs e)
        {          
            // - codes: HEX - IRDA Hex command, {X} - key on keyboard
            codes.Add("FE807F", "{1}");
            codes.Add("FE40BF", "{2}");
            codes.Add("FEC03F", "{3}");
            codes.Add("FE20DF", "{4}");
            codes.Add("FEA05F", "{5}");
            codes.Add("FE609F", "{6}");
            codes.Add("FEE01F", "{7}");
            codes.Add("FE10EF", "{8}");
            codes.Add("FE906F", "{9}");
            codes.Add("FE00FF", "{0}");
            codes.Add("FE30CF", "{UP}");
            codes.Add("FEB04F", "{DOWN}");
            codes.Add("FEF00F", "{LEFT}");
            codes.Add("FE708F", "{RIGHT}");
            codes.Add("FE08F7", "{ENTER}");
            codes.Add("FEC837", "{ESC}");

            codes.Add("FE0AF5", "iRemote"); // ------------- random name for HEX (if using w/o keyboard bind)
            Thread search = new Thread(SearchIRDevice);
            search.Start();
         
        }

        private IPAddress irdeviceip;
        private void SearchIRDevice()
        {
            irdeviceip = null;
            Dictionary<IPAddress, PhysicalAddress> all = GetAllDevicesOnLAN();
            foreach (KeyValuePair<IPAddress, PhysicalAddress> kvp in all)
            {
                label6.Invoke(new Action(() => { label6.Text = "Поиск устройств iRemote..."; }));
                if (kvp.Value.ToString() == "5CCF7F2CC60B") // ------------------------------ MAC-Address ESP8266
                  {                  
                    Ping ping = new Ping();
                    PingReply pr = ping.Send(kvp.Key.ToString());
                    if(pr.Status == IPStatus.Success)
                    {
                        label6.Invoke(new Action(() => { label6.Text = "Устройство обнаружено!"; }));
                        irdeviceip = kvp.Key;
                        WSHandler(kvp.Key);
                    }
                    else
                    {
                        label6.Invoke(new Action(() => { label6.Text = "Устройств iRemote не обнаружено!"; }));                      
                        button1.Invoke(new Action(() => { button1.Visible = true; }));
                    }
                  }              
            }
            if(irdeviceip == null)
            {
                label6.Invoke(new Action(() => { label6.Text = "Устройств iRemote не обнаружено!"; }));
                button1.Invoke(new Action(() => { button1.Visible = true; }));
            }
        }

        public WebSocket ws;
        private void WSHandler(IPAddress deviceip)
        {
           
            ws = new WebSocket("ws://"+deviceip.ToString()+":8080");
            ws.WaitTime = TimeSpan.FromSeconds(10);
            ws.Connect();
            label6.Invoke(new Action(() => { label6.Text = "Подключаюсь к " + ws.Url.Host.ToString(); }));
            ws.OnOpen += (sender, e) =>
            {
                label6.Invoke(new Action(() => { label6.Text = "Жду " + ws.Url.Host.ToString(); }));

            };

            ws.OnMessage += (sender, e) =>
            {

                label6.Invoke(new Action(() => { label6.Text = "Подключено к " + ws.Url.Host.ToString(); }));
                JObject json = JObject.Parse(e.Data);
                if(json.ContainsKey("status"))
                {
                    if (json.GetValue("status").ToString() == "connected") 
                    {
                        label6.Invoke(new Action(() => { label6.Text = "Подключено к " + ws.Url.Host.ToString(); }));
                        Thread pinger = new Thread(statusServer);
                        pinger.Start();
                    }                  
                }

                if (json.ContainsKey("ir_code"))
                {
                    Thread animation = new Thread(animationHandler);
                    animation.Start();
                    //FE0Af5 - minimize\maximize to tray
                    if(json.GetValue("ir_code").ToString().ToUpper()== "FE0AF5")
                    {
                        exem.Invoke(new Action(() => {
                           
                            if (exem.WindowState == FormWindowState.Normal)
                            {
                                exem.WindowState = FormWindowState.Minimized;
                                exem.TopMost = false;
                            }
                            else
                            {
                                notifyIcon1_MouseDoubleClick(null, null);
                                exem.TopMost = true;
                            }

                        }));

                    }
                    else
                    {
                       
                        label4.Invoke(new Action(() => {
                            try
                            {
                                label4.Text = codes[json.GetValue("ir_code").ToString().ToUpper()];
                                SendKeys.Send(codes[json.GetValue("ir_code").ToString().ToUpper()]);
                            }
                            catch
                            { label4.Text = "Err"; }
                        }));

                        label1.Invoke(new Action(() => {
                            try
                            {
                                label1.Text = json.GetValue("ir_code").ToString().ToUpper();
                            }
                            catch
                            { label1.Text = "Err"; }
                        }));
                    } 
                }
            };

            ws.OnClose += (sender, e) =>
            {
                ws.Connect();
                label6.Invoke(new Action(() => { label6.Text = "Соединение потеряно... " + ws.Url.Host.ToString(); }));
            };


            ws.OnError += (sender, e) =>
            {
                label6.Invoke(new Action(() => { label6.Text = "Ошибка подключения " + ws.Url.Host.ToString(); }));
            };
                
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void TitleNouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            ActiveForm.WindowState = FormWindowState.Minimized;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread search = new Thread(SearchIRDevice);
            search.Start();
            button1.Visible = false;
        }

        
        private void animationHandler()
        {
            notifyIcon1.Icon = Properties.Resources.trayblue;
            pictureBox2.Invoke(new Action(() => { pictureBox2.Image = Properties.Resources.irda_green; }));
            Thread.Sleep(300);
            pictureBox2.Invoke(new Action(() => { pictureBox2.Image = Properties.Resources.irda_gray; }));
            notifyIcon1.Icon = Properties.Resources.traygreen;
        }

        private void statusServer()
        {
            while(true)
            {
                Ping ping = new Ping();
                PingReply pr = ping.Send(irdeviceip.ToString());
                if (pr.Status != IPStatus.Success)
                {
                    label6.Invoke(new Action(() => { label6.Text = "Ошибка соединения с iRemote!"; }));
                    button1.Invoke(new Action(() => { button1.Visible = true; }));
                    ws.Close();
                    break;
                }
                Thread.Sleep(10000);
            }

        }

    }
}
