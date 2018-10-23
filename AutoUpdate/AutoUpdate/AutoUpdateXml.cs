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
    public class AutoUpdateXml
    {

        /// <summary>
        /// Check version if need update return true 
        /// </summary>
        /// <returns>True: Need update False :Not need</returns>
        public static bool IsNeedUpdate(Element Server,Element local)
        {
            DateTime dateServer = DateTime.Parse(Server._Date);
            DateTime dateLocal = DateTime.Parse(local._Date);
            return (dateServer > dateLocal);
            
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
                return (response.StatusCode == HttpStatusCode.OK);
                

            }
            catch
            {
                return false;
            }
        }
        public static bool IsExistLocation(string local)
        {
            return File.Exists(local);
        }


        public static Element XmlParse(Uri server)
        {
            try{
                ServicePointManager.ServerCertificateValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true;//ignore ssl Certificate 
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(server.AbsoluteUri);

                return GetXmlInformation(xmldoc);
            }
            catch{
                return null;
            }

        }

        public static Element XmlParse(string local)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(local);

                return GetXmlInformation(xmldoc);
            }
            catch
            {
                return null;
            }

        }

        private static Element GetXmlInformation(XmlNode node)
        {
            Element ele = new Element();
            XmlNode nn = node.FirstChild;
            XmlNodeList list =  nn.ChildNodes;
            foreach (XmlNode n in list)
            {
                switch (n.Name)
                {
                    case "Date":
                        ele._Date = n.InnerText;
                        break;
                    case "Uri":
                        ele._Uri = n.InnerText;
                        break;
                    case "FileName":
                        ele._FileName = n.InnerText;
                        break;

                }
            }
            return ele;
        }


    }
}
