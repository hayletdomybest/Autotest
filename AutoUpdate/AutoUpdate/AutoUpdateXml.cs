using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace AutoUpdate
{
    /// <summary>
    /// This class is implement for handle XML file
    /// </summary>
    class AutoUpdateXml
    {
        /// <summary>
        /// The update version #
        /// </summary>
        public Version Version { get{return _Version;} }

        /// <summary>
        /// The location of the update binary
        /// </summary>
        public Uri Uri { get{return _Uri;} }

        /// <summary>
        /// The file path of the binary
        /// for use on local computer
        /// </summary>
        public string FilePath { get {return _FilePath;} }

        public string Description{get {return _Description;}}

        /// <summary>
        /// The arguments to pass to the updated application on startup
        /// </summary>
        public string LaunchArgs { get {return _LaunchArgs; } }

        /// <summary>
        /// Tag to distinguish types of updates
        /// </summary>
        public JobType Tag;

        //Private variable announce
        private Version _Version;
        private Uri _Uri;
        private string _FilePath;
        private string _Description;
        private string _LaunchArgs;

        public AutoUpdateXml(Version version, Uri uri, string filePath, 
            string description, string launchArgs, JobType type)
        {
            _Version = version;
            _Uri = uri;
            _FilePath = filePath;
            _Description = description;
            _LaunchArgs = launchArgs;
            Tag = type;
        }

        /// <summary>
        /// Check version if need update return true 
        /// </summary>
        /// <param name="version">Appliction's current version</param>
        /// <returns>True: Need update False :Not need</returns>
        public bool IsNeedUpdate(Version version)
        {
            return _Version > version;
        }
        /// <summary>
        /// Check uri whether or not Exist
        /// </summary>
        /// <param name="local">Uniform resource identifier</param>
        /// <returns>True: Exist False: dosen't Exist</returns>
        public static bool IsExistServer(Uri local)
        {
            try
            {
                
                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true; //ignore ssl Certificate
                HttpWebRequest request =  (HttpWebRequest)WebRequest.Create(local);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Stream ftpStream = response.GetResponseStream();
                response.Close();
                //ftpStream.Close();
                return response.StatusCode == HttpStatusCode.OK;
                

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parse Xml documet from Internet
        /// </summary>
        /// <param name="uri">Xml uri </param>
        /// <returns>Parse Xml which save in AutoUpdateXml array</returns>
        public static AutoUpdateXml XmlParse(Uri uri)
        {
            try{
                ServicePointManager.ServerCertificateValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true;//ignore ssl Certificate 
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(uri.AbsoluteUri);
                XmlNodeList xmlUpdate = xmldoc.SelectNodes("AutoUpdate/update");
                //XmlNodeList xmlAdd = xmldoc.SelectNodes("AutoUpdate/Add");
                //XmlNodeList xmlRemove = xmldoc.SelectNodes("AutoUpdate/Remove");            
                //AddList(result, xmlAdd, JobType.ADD);
                //AddList(result, xmlRemove, JobType.REMOVE);
                return TransmitXml(xmlUpdate, JobType.UPDATE);
            }
            catch{
                return null;
            }

        }

        static AutoUpdateXml TransmitXml(XmlNodeList list, JobType type)
        {
            Version version = null;
            string url = "", filePath = "", description = "", launchArgs = "";
            foreach (XmlNode node in list)
            {
                // If the node doesn't exist, there is no add
                if (node == null)
                    return null;
                // Parse data
                version = new Version(node["version"].InnerText);
                url = node["url"].InnerText;
                filePath = node["filePath"].InnerText;
                description = node["description"].InnerText;
                launchArgs = node["launchArgs"].InnerText;
                return (new AutoUpdateXml
                    (version, new Uri(url), filePath, description, launchArgs,type));
            }
            return null;
        }
    }
}
