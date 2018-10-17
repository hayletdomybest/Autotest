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
        /// Parent form
        /// </summary>
        private Form ParentForm;

        /// <summary>
        /// Parent assembly
        /// </summary>
        private Assembly ParentAssembly;

        /// <summary>
        /// Parent name
        /// </summary>
        private string ParentPath;

        /// <summary>
        /// For save xml parse array
        /// </summary>
        private AutoUpdateXml JobsFromXML;



        /// <summary>
        /// Thread to find update
        /// </summary>
        private BackgroundWorker BgWorker;

        /// <summary>
        /// Uri of the update xml on the server
        /// </summary>
        private Uri UpdateXmlLocation;


        /// <summary>
        /// Creates a new SharpUpdater object
        /// </summary>
        /// <param name="a">Parent ssembly to be attached</param>
        /// <param name="owner">Parent form to be attached</param>
        /// <param name="XMLOnServer">Uri of the update xml on the server</param>
        public AutoUpdate(Assembly assm, Form owner, Uri XMLOnServer)
        {
            ParentForm = owner;
            ParentAssembly = assm;
            ParentPath = assm.Location;

            UpdateXmlLocation = XMLOnServer;

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
            if (!AutoUpdateXml.IsExistServer(UpdateXmlLocation))
                e.Cancel = true;
            else // Parse update xml     
                e.Result = AutoUpdateXml.XmlParse(UpdateXmlLocation);

           
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
            JobsFromXML = (AutoUpdateXml)e.Result;
            // Check if the update is not null and is a newer version than the current application
            if (JobsFromXML == null)
                return;


            if (JobsFromXML.IsNeedUpdate(ParentAssembly.GetName().Version))
            {
                AcceptForm acceptform = new AcceptForm(JobsFromXML);
                acceptform.Location = (this.ParentForm.Location);
                acceptform.ShowDialog();
                IsUpdate = (acceptform.DialogResult == DialogResult.Yes);
            }
            if (UpdateComplete != null)
                UpdateComplete(IsUpdate);
            
        }
    }
}
