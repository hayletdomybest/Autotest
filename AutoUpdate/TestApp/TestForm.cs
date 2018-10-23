using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using AutoUpdate;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml;
namespace TestApp
{
    public partial class TestForm : Form
    {
        /// <summary>
        /// Download information Server address
        /// </summary>
        private const string Server = @"https://raw.githubusercontent.com/t628x7600/Autotest/master/updateInfo.xml";
        
        /// <summary>
        /// Previous update information address
        /// </summary>
        private readonly string local = Environment.CurrentDirectory + "/UpdateVersion.xml";
        
        /// <summary>
        /// Download information Local address  
        /// </summary>
        private readonly string DownLoadInfoPath = Environment.CurrentDirectory + @"/DownLoad_Information/info.xml";

        /// <summary>
        /// Download application address
        /// </summary>
        private readonly string DownLoadFormPath = Environment.CurrentDirectory + @"/DownLoadForm.exe";
        
        
        
        public TestForm()
        {
            InitializeComponent();
        }

        private void btn_check_Click(object sender, EventArgs e)
        {
            AutoUpdate.AutoUpdate UpdateInterface;
            Assembly currAsm = Assembly.GetExecutingAssembly();
            Uri uri = new Uri(Server);
            UpdateInterface = new AutoUpdate.AutoUpdate(uri,local);
            UpdateInterface.UpdateComplete += this.UpdateComplete;
            UpdateInterface.DoUpdate();
        }
        
        private  void UpdateComplete(bool update)
        {
            if (!update)
                return;
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => true;//ignore ssl Certificate
            
            //Current process file address
            string cur_process = Process.GetCurrentProcess().MainModule.FileName;

            //Get uri bind Download information Server address
            Uri uri = new Uri(Server);
            
            //Parameter for passing to DownloadForm 
            ArgsBuilder args = new ArgsBuilder();

            //For start process application information 
            ProcessStartInfo pInfo = new ProcessStartInfo(DownLoadFormPath);

            //Parameter[0] = DownLoadInfoPath
            args.Add(DownLoadInfoPath);

            //Parameter[1] = current process name
            args.Add(Path.GetFileName(cur_process));
            pInfo.Arguments = args.ToString();
            if (!Directory.Exists(Path.GetDirectoryName(DownLoadInfoPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(DownLoadInfoPath));
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(uri.AbsoluteUri);
                doc.Save(DownLoadInfoPath);

                using (Process p = new Process())
                {
                    p.StartInfo = pInfo;
                    p.Start();
                }
                Application.Exit();

            }
            catch
            {
                MessageBox.Show("網路異常");
            
            }        
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "當前版本號碼" + "\n     " + ProductVersion; 
        }
    }
}
