using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Web;
using CefSharp.WinForms;

namespace RdpView
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class MainWindow : Form
    {
        private ChromiumWebBrowser _browser;
        public MainWindow()
        {
            //MessageBox.Show(GetPublicIPAddress());
            InitializeComponent();
            Client.f = this;
            CheckForIllegalCrossThreadCalls = false; 
            LoadBrowser();
        }


        private void LoadBrowser()
        {
            CefSettings settings = new CefSettings();
            settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF";
            Clipboard.SetText(settings.CachePath);
            CefSharp.Cef.Initialize(settings);
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            //_browser = new ChromiumWebBrowser("https://api.raindroprdp.com/windows_login.html");
            _browser = new ChromiumWebBrowser("https://api.raindroprdp.com/auth0windowsconnect/login");
            //_browser.LoadHtml(File.ReadAllText("K:\\Projects\\Fiverr\\Test\\htmlx.html"));
            _browser.ConsoleMessage += BrowserOnConsoleMessage;
            _browser.JavascriptObjectRepository.Register("client", new Client(), true, BindingOptions.DefaultBinder);
            _browser.Dock = DockStyle.Fill;
             this.Controls.Add(_browser);
        }

        private string d = "<script>" +
                           "var local_ip = null;window.local_ip = client.get_ip();" +
"</script>";
        private void BrowserOnConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Console.Write(e.Message);
        }


        public class Client
        {
            public static Form f;
            public string get_ip()
            {
                Console.WriteLine("Get_IP Called!");
                return GetPublicIPAddress();
            }

            public void set(string s)
            {
                MessageBox.Show(s.ToString());
            }

            public void start_rdp(string ip, string username, string password)
            {
                System.Console.WriteLine($"IP: {ip}\nUsername: {username}\nPassword: {password}");
                MessageBox.Show(ip + "\n" + password, username);
            }
        }

        public static string GetPublicIPAddress()
        {
            var ipAddress = "";

            try
            {
                using (var client = new WebClient())
                {
                    ipAddress = client.DownloadString("http://ipinfo.io/ip");
                }
            }
            catch (Exception e)
            {
            }

            return ipAddress.Trim();
        }
    }
}