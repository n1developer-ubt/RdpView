using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RdpView
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            GetPostResponse();
            webBrowser1.Url = new Uri("Https://api.raindroprdp.com/auth0windowsconnect/login");
            //Task.Run(() => { lblIp.Text = GetPublicIPAddress(); });
        }

        private void GetPostResponse()
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var HtmlResult =
                    wc.UploadString("https://api.raindroprdp.com/managemachine/raindropconnect?ip=103.255.7.14",
                        "POST");
                MessageBox.Show(HtmlResult);
            }
        }

        public static string GetPublicIPAddress()
        {
            var ipAddress = "";

            try
            {
                using (var client = new WebClient())
                {
                    ipAddress = client.DownloadString("http://icanhazip.com/");
                }
            }
            catch (Exception e)
            {
            }

            return ipAddress;
        }
    }
}