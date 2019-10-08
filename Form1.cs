using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamWebServer
{
    public partial class Form1 : Form
    {
        private bool _isRunning;
        static readonly object _isRunningLock = new object();
        public bool IsRunning
        {
            get
            {
                lock (_isRunningLock)
                {
                    return this._isRunning;
                }
            }
            set
            {
                lock (_isRunningLock)
                {
                    this._isRunning = value;
                }
                button1.Enabled = !value;
                button2.Enabled = value;
            }
        }
        public Form1()
        {
            InitializeComponent();
            button1.Text = "Start";
            button2.Text = "Stop";
            IsRunning = false;

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            IsRunning = true;

            ThreadPool.QueueUserWorkItem(StartTcpListener, this.IsRunning);
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            IsRunning = false;
        }
        private void StartTcpListener(object arg)
        {
            TcpListener listener;
            int port = 5326;
            bool IsRunning = (bool)arg;

            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (IsRunning)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();

                    ThreadPool.QueueUserWorkItem(GetRequestedItem, client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error received: " + ex.Message);
                }
            }
        }

        private void GetRequestedItem(object arg)
        {
            TcpClient client = (TcpClient)arg;
            NetworkStream ns = client.GetStream();

            MemoryStream memStream = new MemoryStream();
            string outString;
            Byte[] output = new Byte[1024];

            ns.Read(output, 0, output.Length);
            outString = System.Text.Encoding.ASCII.GetString(output);

            if (outString.StartsWith("GET"))
            {
                Byte[] fileContent = File.ReadAllBytes("../../Content/default.html");
                memStream.Write(fileContent, 0, fileContent.Length);

                Byte[] header = System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\nServer: TopSecretServer\nContent-Type: text/html\nAccept-Ranges: bytes\n\n");
                ns.Write(header, 0, header.Length);

                ns.Write(memStream.ToArray(), 0, memStream.ToArray().Length);
            }
        }
    }
}
/*
        private void GetWebData(object input) {
            try
            {
                string strInput = input.ToString();
                // Response Data init
                Byte[] output = new Byte[1024];
                string response = String.Empty;

                // Create a new TCP Client
                var tcpClient = new TcpClient(strInput, 80);
                NetworkStream ns = tcpClient.GetStream();

                Console.WriteLine("Connected");

                // Write a request to the web server
                Byte[] cmd = System.Text.Encoding.ASCII.GetBytes("GET / HTTP/1.0 \nHost: " + strInput + "\n\n");
                ns.Write(cmd, 0, cmd.Length);

                // Get the output
                Int32 bytes = ns.Read(output, 0, output.Length);

                while (bytes > 0)
                {
                    response += System.Text.Encoding.ASCII.GetString(output);
                    bytes = ns.Read(output, 0, output.Length);
                }
                Thread.Sleep(5000);
                SetForegroundData(response);
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
            //if parent,
            txtOutput.Text = value;
        }
    }
}
*/