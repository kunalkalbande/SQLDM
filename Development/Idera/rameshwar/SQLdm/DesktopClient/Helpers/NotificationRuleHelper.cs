using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Notification.Providers;
using System.Threading;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    /// <summary>
    /// SQLdm 10.0 (Swati Gogia) Import/Export Wizard helper class
    /// </summary>
    public static class NotificationRuleHelper
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Message");
        public static string CreateXML(NotificationRule rule)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(NotificationRule));
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, rule);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                    return xmlDoc.InnerXml;
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured while exporting NotificationRule {0}. Detailed error : {1}", rule.Description, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
        }

        static public string SerializeNotificationRule(Idera.SQLdm.Common.Notification.NotificationRule rule)
        {
            try
            {
                return CreateXML(rule);
            }
            catch
            {
                throw;
            }

        }


        static public NotificationRule DeserializeNotificationRule(string xmlPath)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(NotificationRule));
                TextReader reader = new StreamReader(xmlPath);
                object obj = deserializer.Deserialize(reader);
                NotificationRule XmlData = (NotificationRule)obj;
                reader.Close();
                return XmlData;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in parsing custom counter xml ({0}) during import. Detailed error : {1}", xmlPath, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
        }

    }
}
  

