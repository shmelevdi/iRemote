using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Threading;
using Shmelevdi.iRemoteProject;
using Shmelevdi.iRemoteProject.Settings;

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
            iRemote ir = new iRemote();
            ir.Start();
            ir.DeviceFound += (sndr, args) =>
            {
                label6.Invoke(new Action(() =>
                {
                    label6.Text = "Устройство найдено!";
                }));
            };

            ir.DeviceNotFound += (sndr, args) =>
            {
                label6.Invoke(new Action(() =>
                {
                    label6.Text = "Устройство не найдено!";
                }));
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
