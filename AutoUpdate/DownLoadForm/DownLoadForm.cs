using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.Diagnostics;
namespace DownLoadForm
{
    public partial class DownLoadForm : Form
    {
        private readonly string currentPath = Environment.CurrentDirectory;
        private readonly string ExeName = "TestApp.exe";
        private string arg;

        private Version version = null;
        
        private Uri FileUri = null;
        
        private string FilePath = "";

        private string FileName = "";

        private long FileSize = 0;

        private bool IsUpdate = false;
        internal DownLoadForm(string sender)
        {
            InitializeComponent();
            arg = sender;
        }

        private void DownLoadForm_Load(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(arg);
                
                XmlNodeList xmlUpdate = xmldoc.SelectNodes("AutoUpdate/update");
                XmlNode node = xmlUpdate.Item(0);
                version  = new Version(node["version"].InnerText);
                FileUri  = new Uri(node["url"].InnerText);
                FilePath = node["filePath"].InnerText;
                FileName = node["FileName"].InnerText;
                DownLoadFile();
            }
            catch{
                MessageBox.Show("更新失敗");
                Application.Exit();
            }
        }
        private void DownLoadFile()
        {
            WebClient DownloadClient;
            HttpWebRequest  Httpreques;
            HttpWebResponse Httpresponse;
            ServicePointManager.ServerCertificateValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true;//ignore ssl Certificate
            DownloadClient = new WebClient();
            DownloadClient.DownloadProgressChanged += DownloadProgressChanged;
            DownloadClient.DownloadFileCompleted += DownloadCompletedEventHandler;
            try
            {
                Httpreques  = (HttpWebRequest)HttpWebRequest.Create(FileUri);
                Httpresponse = (HttpWebResponse)Httpreques.GetResponse();

                FileSize = Httpresponse.ContentLength;
                string SavePath = (this.FilePath.Length == 0) ? currentPath :
                    GetDir(currentPath, FilePath);
                SavePath = GetDir(SavePath, this.FileName);
                lab_FiileName.Text = FileName;
                DownloadClient.DownloadFileAsync(FileUri, SavePath);
            }
            catch
            {
                MessageBox.Show("下載失敗");
                Application.Exit();
            }
        
        }
        void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            string DisplayBytes = FormatBytes(e.BytesReceived, 1, false) + "/" + FormatBytes(FileSize, 1, true);
            lab_FileSize.Text = DisplayBytes;
            bar_rate.Value = e.ProgressPercentage;

        }
        void DownloadCompletedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            IsUpdate = true;
            btn_Cancel.Text = "離開";
            lab_Title.Text = "更新完成";
            MessageBox.Show("更新成功");
        }

        
        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "B";

            // Check if best size in KB
            if (newBytes > 1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "KB";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                // Check if best size in MB
                newBytes /= 1048576;
                byteType = "MB";
            }
            else
            {
                // Best size in GB
                newBytes /= 1073741824;
                byteType = "GB";
            }

            // Show decimals
            if (decimalPlaces > 0)
                formatString += ":0.";

            // Add decimals
            for (int i = 0; i < decimalPlaces; i++)
                formatString += "0";

            // Close placeholder
            formatString += "}";

            // Add byte type
            if (showByteType)
                formatString += byteType;

            return string.Format(formatString, newBytes);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (!this.IsUpdate)
                Application.Exit();
            else
            {
                try
                {
                    //client.DownloadFileAsync(uri,SavePath);
                    string target = GetDir(currentPath, ExeName);
                    using (Process p = new Process())
                    {
                        p.StartInfo.FileName = target;
                        p.Start();
                    }
                    Application.Exit();
                }
                catch
                {
                    MessageBox.Show("Setting Tool 開啟失敗");

                }
            }
        }
        private string GetDir(string str1, string str2)
        {
            return str1 + @"\" + str2;
        }
    }
}
