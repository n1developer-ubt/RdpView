using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class MainWindow : Form
    {
        private ChromiumWebBrowser _browser;
        public MainWindow()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            WindowState = FormWindowState.Maximized;
            try
            {
                LoadBrowser();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void LoadBrowser()
        {
            CefSettings settings = new CefSettings
            {
                CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF"
            };
            //Clipboard.SetText(settings.CachePath);
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

        private void BrowserOnConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Console.Write(e.Message);
        }


        public class Client
        {
            public string get_ip()
            {
                Console.WriteLine("Get_IP Called!");
                return GetPublicIpAddress();
            }

            public void set(string s)
            {
                MessageBox.Show(s.ToString());
            }

            public void start_rdp(string ip, string username, string password)
            {
                string cmd = $"\"{Application.StartupPath}\\freerdp\\wfreerdp.exe\" /f /cert-ignore /u:{username} /p:{password} /v:{ip} /admin /multimon";
                Console.Write($"\n\n\n{cmd}\n\n\n");
                System.Console.WriteLine($"IP: {ip}\nUsername: {username}\nPassword: {password}");
                ExecuteCommand(cmd);
                //File.WriteAllText(Path.Combine(Path.GetTempPath(), Path.GetTempFileName()),cmd);
            }

            static void ExecuteCommand(string command)
            {
                var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                var process = Process.Start(processInfo);

                process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                    Console.WriteLine("output>>" + e.Data);
                process.BeginOutputReadLine();

                process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs args)
                {
                    MessageBox.Show(args.Data);
                    //Console.WriteLine("error>>" + );
                };
                process.BeginErrorReadLine();

                process.WaitForExit();

                Console.WriteLine("ExitCode: {0}", process.ExitCode);
                process.Close();
            }
        }

        public static string GetPublicIpAddress()
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