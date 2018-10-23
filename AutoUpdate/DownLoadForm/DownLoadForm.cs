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
using System.IO;
using System.Threading;
using Ionic.Zip;
using AutoUpdate;
namespace DownLoadForm
{
    public partial class DownLoadForm : Form
    {
        private readonly string currentPath = Environment.CurrentDirectory;
        private readonly string tempRoot = Path.Combine(Environment.CurrentDirectory, "Temp");
        private string tempPath;
        private const string updateInfo = "update.xml";
        private const string previousInfo = "UpdateVersion.xml";
        /// <summary>
        /// Will update application name
        /// </summary>
        private string UpdateExeName;
        
        /// <summary>
        /// Download information Path
        /// </summary>
        private string Download_Info;

        /// <summary>
        /// Download data date
        /// </summary>
        private string Download_Date;

        /// <summary>
        /// Download data uri
        /// </summary>
        private string Download_Uri;

        /// <summary>
        /// Download data file name
        /// </summary>
        private string Download_FileName;
        
        /// <summary>
        /// Download data file size
        /// </summary>
        private long Down_load_FileSize = 0;


        private bool IsUpdate = false;


        internal DownLoadForm(string[] sender)
        {
            InitializeComponent();
            Download_Info = sender[0];
            UpdateExeName = sender[1];
        }

        private void DownLoadForm_Load(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(Download_Info);

                XmlNode xmlUpdate_node = xmldoc.FirstChild;
                XmlNodeList xmlUpdate_list = xmlUpdate_node.ChildNodes;
                foreach (XmlNode n in xmlUpdate_list)
                {
                    switch (n.Name)
                    { 
                        case "Date":
                            Download_Date = n.InnerText;
                            break;
                        case "Uri":
                            Download_Uri = n.InnerText;
                            break;
                        case "FileName":
                            Download_FileName = n.InnerText;
                            break; 
                    }
                }
                DownLoadFile();
            }
            catch{
                Debug_Error("更新失敗");
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
                Httpreques  = (HttpWebRequest)HttpWebRequest.Create(Download_Uri);
                Httpresponse = (HttpWebResponse)Httpreques.GetResponse();
                Down_load_FileSize = Httpresponse.ContentLength;
                if (!Directory.Exists(tempRoot))
                    Directory.CreateDirectory(tempRoot);
                
                lab_FiileName.Text = Download_FileName;
                DownloadClient.DownloadFileAsync(new Uri(Download_Uri),
                    Path.Combine(tempRoot, Download_FileName));
            }
            catch
            {
                Debug_Error("下載失敗");
            }
        
        }
        void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            string DisplayBytes = FormatBytes(e.BytesReceived, 1, false) + "/" + FormatBytes(Down_load_FileSize, 1, true);
            lab_FileSize.Text = DisplayBytes;
            bar_rate.Value = (int)((e.ProgressPercentage) * 0.8);

        }
        void DownloadCompletedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            IsUpdate = true;
            btn_Cancel.Text = "離開";
            lab_Title.Text = "更新檔下載完成";
            tempPath = Path.Combine(tempRoot, Download_FileName).Replace(".zip","");
            BackgroundWorker unzip_bg = new BackgroundWorker();
            unzip_bg.DoWork += new DoWorkEventHandler(unzip);
            unzip_bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(unzip_compelete);
            unzip_bg.RunWorkerAsync();
        }

        private void unzip(object sender, DoWorkEventArgs e)
        {
            lab_Title.Text = "更新檔解壓縮";
            UnZipFiles(Path.Combine(tempRoot, Download_FileName),null);
        
        }
        private void unzip_compelete(object sender, RunWorkerCompletedEventArgs e)
        {
            bar_rate.Value = 90;
            BackgroundWorker mvFile_bg = new BackgroundWorker();
            mvFile_bg.DoWork += new DoWorkEventHandler(mvFile);
            mvFile_bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(mvFile_compelete);
            mvFile_bg.RunWorkerAsync();

        }

        private void mvFile(object sender, DoWorkEventArgs e)
        {
            lab_Title.Text = "安裝更新檔";
            UpdateItem[] Items = ReadXml();
            try
            {
                foreach (UpdateItem n in Items)
                {
                    string source = Path.Combine(tempPath, n._Name);
                    string dir = (n._Path != "") ?
                        Path.Combine(Environment.CurrentDirectory, n._Path) : Environment.CurrentDirectory;
                    string dirfull = Path.Combine(dir, n._Name);
                    File.Copy(source, dirfull,true);
                }
            }
            catch {
                Debug_Error("安裝更新檔失敗");
            }
        }

        private void mvFile_compelete(object sender, RunWorkerCompletedEventArgs e)
        {
            bar_rate.Value = 100;
            lab_Title.Text = "更新完成";
            btn_Cancel.Text = "完成";
            string SaveInfo = Path.Combine(Environment.CurrentDirectory, previousInfo);
            XmlDocument doc = new XmlDocument();
            doc.Load(SaveInfo);
            XmlNode n = doc.SelectSingleNode("AutoUpdate");
            n["Date"].InnerText = Download_Date;
            doc.Save(SaveInfo);
            /*
            File.Delete(tempPath);
            File.Delete(Path.Combine(tempRoot, Download_FileName));*/
        }

        private AutoUpdate.UpdateItem[] ReadXml()
        {
            List<UpdateItem> items = new List<UpdateItem>();
            XmlDocument doc = new XmlDocument();
            try
            {

                doc.Load(Path.Combine(tempPath,updateInfo));
                XmlNodeList list = doc.SelectNodes("update/Item");
                foreach (XmlNode n in list)
                {
                    UpdateItem temp = new UpdateItem();
                    temp._Name = n["Name"].InnerText;
                    temp._Path = n["Path"].InnerText;
                    items.Add(temp);
                }
            }
            catch {
                Debug_Error("讀取更新資料失敗");
            }
            return items.ToArray();        
        }

        //解壓縮檔案
        //path: 解壓縮檔案目錄路徑
        //password: 密碼
        private static void UnZipFiles(string path, string password)
        {
            ZipFile unzip = ZipFile.Read(path);
            if (password != null && password != string.Empty) unzip.Password = password;
            string unZipPath = Path.GetDirectoryName(path);

            foreach (ZipEntry e in unzip)
            {
                e.Extract(unZipPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }


        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType;

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
            else if (newBytes > 1073741824)
            {
                // Best size in GB
                newBytes /= 1073741824;
                byteType = "GB";
            }
            else
                byteType = "B";

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
                    string startExe = Path.Combine(Environment.CurrentDirectory, UpdateExeName);
                    using (Process p = new Process())
                    {
                        p.StartInfo.FileName = startExe;
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

        private void Debug_Error(string err)
        {
            MessageBox.Show(err);
            Application.Exit();
        }
    }
}
