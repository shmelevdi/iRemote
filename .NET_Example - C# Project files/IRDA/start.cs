using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Threading;
using Shmelevdi.iRemote;

namespace IRDA
{
   
    public partial class start : Form
    {
        public Form exem;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();



        

       


        public start()
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

            iRemote ir = new iRemote("5CCF7F2CC60B");
            ir.Start();
            ir.DeviceFound += (sndr, args) => {
                label6.Text = "Found!";
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
          //  Thread search = new Thread(SearchIRDevice);
         //   search.Start();
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



    }
}
