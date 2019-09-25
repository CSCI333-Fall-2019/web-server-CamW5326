using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamWebServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1.Text = "Start";
            button2.Text = "Stop";
            button2.Enabled = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button1.Enabled = false;

            ThreadPool.QueueUserWorkItem(BackgroundFunc, this);
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
        }
        private void BackgroundFunc(object state)
        {
            try
            {
                Thread.Sleep(5000);
                SetForegroundData("");
            }
            catch (Exception Ex)
            {
                SetForegroundData(Ex.Message);
            }
        }
        //Calls itself recursivley until it finds parent
        public void SetForegroundData(string value)
        {
            if (InvokeRequired) //if background thread
            {
                this.BeginInvoke(new Action<string>(SetForegroundData), new object[] { value });
                return;
            }
            button1.Text = value;
        }
    }
}
