using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Objects
{
    // Note, This is the list of available dashboard panels.
    // It is serialized as text to the xml stored in the repository for dashboard layouts
    //  so new ones can be added in alphabetic order, but don't change the names
    public enum DashboardPanel
    {
        Cache,
        Cpu,
        CustomCounters,
        Databases,
        Disk,
        Empty,
        FileActivity,
        LockWaits,
        Memory,
        Network,
        ServerWaits,
        Sessions,
        TempDB,
        VM,
        SQLServerPhysicalIO,
        AzureDisk//6.2.4
    }

    [Serializable]
    public class DashboardConfiguration : ISerializable
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public List<DashboardPanelConfiguration> Panels;

        internal DashboardConfiguration()
        {
            Columns = 0;
            Rows = 0;
            Panels = new List<DashboardPanelConfiguration>();
        }

        #region Custom Serialization

        public DashboardConfiguration(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Owner = info.GetString("Owner");
            Columns = info.GetInt32("Columns");
            Rows = info.GetInt32("Rows");
            Panels = (List<DashboardPanelConfiguration>)info.GetValue("Panels", typeof(List<DashboardPanelConfiguration>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Owner", Owner);
            info.AddValue("Columns", Columns);
            info.AddValue("Rows", Rows);
            info.AddValue("Panels", Panels);
        }

        internal string Serialize()
        {
            return Serialize(this);
        }

        /// <summary>
        ///  Serializes a DashboardConfiguration as XML
        /// </summary>
        /// <param name="config">object to be serialized</param>
        /// <returns>serialized object</returns>
        internal static string Serialize(DashboardConfiguration config)
        {
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(DashboardConfiguration), new Type[] { typeof(DashboardPanelConfiguration) });
            StringBuilder buffer = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, config);
                writer.Flush();
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Deserializes a DashboardConfiguration object from XML.
        /// </summary>
        /// <param name="xml">string to be deserialized</param>
        /// <returns>deserialized DashboardConfiguration object</returns>
        internal static DashboardConfiguration Deserialize(string xml)
        {
            Type type = typeof(DashboardConfiguration);
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(type, new Type[] { typeof(DashboardPanelConfiguration) });

            DashboardConfiguration config = null;

            StringReader stream = new StringReader(xml);
            using (XmlReader xmlReader = XmlReader.Create(stream))
            {
                config = (DashboardConfiguration)serializer.Deserialize(xmlReader);
            }

            return config;
        }

        #endregion

        internal void SetSize(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
            List<DashboardPanelConfiguration> deletepanels = new List<DashboardPanelConfiguration>();
            foreach (var panelConfig in Panels)
            {
                if (panelConfig.Column > Columns || panelConfig.Row > Rows)
                    deletepanels.Add(panelConfig);
            }
            foreach (var panelConfig in deletepanels)
            {
                Panels.Remove(panelConfig);
            }
        }
    }

    [Serializable]
    public class DashboardPanelConfiguration
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public DashboardPanel Panel { get; set; }
        public List<DashboardPanelOption> Options { get; set; }

        internal DashboardPanelConfiguration() { }

        internal DashboardPanelConfiguration(int row, int column, DashboardPanel panel)
        {
            Column = column;
            Row = row;
            Panel = panel;
            Options = new List<DashboardPanelOption>();
        }

        internal void SetPosition(int column, int row)
        {
            Column = column;
            Row = row;
        }

        internal void SetOptions(List<DashboardPanelOption> options)
        {
            Options = options;
        }

        internal string GetDashboardDescription()
        {
            return DashboardHelper.GetDashboardDescription(Panel);
        }

        internal string GetDashboardHelpTopic()
        {
            return DashboardHelper.GetDashboardHelpTopic(Panel);
        }

        internal DashboardControl GetNewDashboardControl(ViewContainer vHost)
        {
            return DashboardHelper.GetNewDashboardControl(Panel, vHost);
        }
    }

    [Serializable]
    public class DashboardPanelOption
    {
        public string Type { get; set; }
        public List<string> Values { get; set; }


        internal DashboardPanelOption() { }

        internal DashboardPanelOption(string type, string value)
        {
            Type = type;
            Values = new List<string>{ value };
        }

        internal DashboardPanelOption(string type, IEnumerable<string> values)
        {
            Type = type;
            Values = new List<string>();
            Values.AddRange(values);
        }
    }

    internal class DashboardPanelSelector
    {
        public int Position { get; set; }
        public DashboardPanel DashboardPanel { get; private set; }

        public Image DesignerImage
        {
            get { return GetPanelImage(); }
        }

        public DashboardPanelSelector (int position, DashboardPanel panel)
        {
            Position = position;
            DashboardPanel = panel;
        }

        public Image GetPanelImage()
        {
            return DashboardHelper.GetPanelImage(DashboardPanel);
        }
    }

    internal class DashboardLayoutSelector
    {
        private int rowNumber = 1;

        public int DashboardLayoutID { get; private set; }
        public string CurrentUser { get; private set; }
        public string Owner { get; private set; }
        public string Name { get; private set; }
        public DashboardConfiguration DashboardConfiguration { get; private set; }
        public DateTime? LastUpdated { get; private set; }
        public string LastUpdatedText
        {
            get { return LastUpdated.HasValue ? string.Format("updated {0}", LastUpdated) : string.Empty; }
        }
        public DateTime? LastViewed { get; private set; }
        public string LastViewedText
        {
            get { return LastViewed.HasValue ? string.Format("last used {0}", LastViewed) : string.Empty; }
        }
        public Byte[] ImageBytes { get; private set; }
        public bool IsSystem { get; private set; }
        public bool CanDelete { get; private set; }
        public int RowNumber
        {
            get { return rowNumber; }
            set { rowNumber = value; }
        }

        public DashboardLayoutSelector(string currentUser, int id, string owner, string name, DashboardConfiguration configuration, DateTime? lastUpdated, DateTime? lastViewed, Image image)
        {
            loadValues(currentUser, id, owner, configuration, lastUpdated, lastViewed, ImageHelper.ConvertImageToByteArray(image));
            Name = name;
        }

        public DashboardLayoutSelector(string currentUser, int id, string owner, DashboardConfiguration configuration, DateTime? lastUpdated, DateTime? lastViewed, Image image)
        {
            loadValues(currentUser, id, owner, configuration, lastUpdated, lastViewed, ImageHelper.ConvertImageToByteArray(image));
        }

        public DashboardLayoutSelector(string currentUser, int id, string owner, DashboardConfiguration configuration, DateTime? lastUpdated, DateTime? lastViewed, Byte[] imageBytes)
        {
            loadValues(currentUser, id, owner, configuration, lastUpdated, lastViewed, imageBytes);
        }

        private void loadValues(string currentUser, int id, string owner, DashboardConfiguration configuration, DateTime? lastUpdated, DateTime? lastViewed, Byte[] imageBytes)
        {
            DashboardLayoutID = id;
            CurrentUser = currentUser;
            if (owner == null)
            {
                IsSystem = true;
                CanDelete = false;
                Owner = "SQLDM Default";
            }
            else
            {
                IsSystem = false;
                CanDelete = (owner == currentUser);
                Owner = owner;
            }
            DashboardConfiguration = configuration;
            Name = configuration.Name;
            LastUpdated = lastUpdated;
            LastViewed = lastViewed;
            ImageBytes = imageBytes;
        }
    }
}
