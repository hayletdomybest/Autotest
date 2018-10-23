using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AutoUpdate
{
    public delegate void _UpdateComplete(bool update);
    public class AutoUpdate
    {
        public _UpdateComplete UpdateComplete = null; 

        /// <summary>
        /// Local Xml information 
        /// </summary>
        private Element Local;

        /// <summary>
        /// Server Xml information
        /// </summary>
        private Element Server;



        /// <summary>
        /// Thread to find update
        /// </summary>
        private BackgroundWorker BgWorker;

        /// <summary>
        /// Uri of the update xml on the server
        /// </summary>
        private Uri UpdateXmlServer;

        /// <summary>
        /// Path of the update xml on the location
        /// </summary>
        private string UpdateXmlLocation;

        /// <summary>
        /// Creates a new AutoUpdate object
        /// </summary>
        /// <param name="a">Parent ssembly to be attached</param>
        /// <param name="owner">Parent form to be attached</param>
        /// <param name="XMLOnServer">Uri of the update xml on the server</param>
        public AutoUpdate(Uri server, string location)
        {
            UpdateXmlServer = server;
            UpdateXmlLocation = location;

            // Set up backgroundworker
            BgWorker = new BackgroundWorker();
            BgWorker.DoWork += new DoWorkEventHandler(BgWorker_DoWork);
            BgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);
        }

        /// <summary>
        /// Update data
        /// </summary>
        public void DoUpdate()
        {
            if (!BgWorker.IsBusy)
                BgWorker.RunWorkerAsync();
        }


        /// <summary>
        /// Checks for/parses update.xml on server
        /// </summary>
        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Check for update on server
            e.Cancel = (!AutoUpdateXml.IsExistServer(UpdateXmlServer) ||
                        !AutoUpdateXml.IsExistLocation(UpdateXmlLocation));

            if (e.Cancel)
                return;

            Server = AutoUpdateXml.XmlParse(UpdateXmlServer);
            Local = AutoUpdateXml.XmlParse(UpdateXmlLocation);
        }


        /// <summary>
        /// After the background worker is done, prompt to update if there is one
        /// </summary>
        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool IsUpdate = false;
            // If there is a file on the server
            if (e.Cancelled)
                return;

            
            if (AutoUpdateXml.IsNeedUpdate(Server,Local))
            {
                AcceptForm acceptform = new AcceptForm(Server);
                acceptform.ShowDialog();
                IsUpdate = (acceptform.DialogResult == DialogResult.Yes);
            }
            if (UpdateComplete != null)
                UpdateComplete(IsUpdate);
            
        }
    }
}
