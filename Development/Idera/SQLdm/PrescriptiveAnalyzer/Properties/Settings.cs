using System;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Idera.SQLdoctor.AnalysisEngine.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    [SettingsProviderAttribute(typeof(MySettingsProvider))] 
    public sealed partial class Settings {
        
        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }
    }

    public class MySettingsProvider : SettingsProvider
    {
        private static TracerX.Logger _logX = TracerX.Logger.GetLogger("MySettingsProvider");
        private Configuration _config;
        private object _configLock;

        public MySettingsProvider() : base()
        {
            _configLock = new Object();
        }

        public override string ApplicationName
        {
            get
            {
                string name = Assembly.GetExecutingAssembly().GetName().Name;
                if (string.IsNullOrEmpty(name))
                    name = Assembly.GetEntryAssembly().GetName().Name;
                return name;
            }
            set
            {
                // base.ApplicationName = value;
            }
        }

        public override void  Initialize(string name, System.Collections.Specialized.NameValueCollection values)
        {
 	        base.Initialize(ApplicationName, values);
        }

        private Configuration Configuration
        {
            get
            {
                lock (_configLock)
                {
                    if (_config != null)
                        return _config;

                    string path = Assembly.GetExecutingAssembly().CodeBase;
                    if (path.StartsWith("file:///"))
                        path = path.Remove(0,8);
                    path = path.Trim('/');
                   // path = Path.GetDirectoryName(path);
                    if (File.Exists(path + ".config"))
                    {
                        _config = OpenConfig(path);
                    }
                    if (_config == null)
                        _config = OpenConfig(null);
                }
                return _config;
            }
        }

        private Configuration OpenConfig(string path)
        {
            Configuration c = null;
            try
            {
                if (String.IsNullOrEmpty(path))
                    return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                return ConfigurationManager.OpenExeConfiguration(path);
            }
            catch (Exception e)
            {
               // Common.ExceptionLogger.Log(_logX, string.Format("OpenConfig({0}) Exception: ", path), e);
            }

            return c;
        }

    //Overridable Function GetAppSettingsPath() As String
    //    'Used to determine where to store the settings
    //    Dim fi As New System.IO.FileInfo(Application.ExecutablePath)
    //    Return fi.DirectoryName
    //End Function

    //Overridable Function GetAppSettingsFilename() As String
    //    'Used to determine the filename to store the settings
    //    Return ApplicationName & ".settings"
    //End Function

        public override void  SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            foreach (SettingsPropertyValue propval in values)
            {
                SetValue(propval);
            }
            if (values.Count > 0)
                SaveSettings();
        }

        private void SaveSettings()
        {

        }

        private void SetValue(SettingsPropertyValue propval)
        {

        }

        public override SettingsPropertyValueCollection  GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            using (_logX.DebugCall("AnalysisEngine Setting"))
            {
                SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
                Configuration config = Configuration;
                if (config != null)
                {
                    string sectionName = context["GroupName"] as string;
                    if (!String.IsNullOrEmpty(sectionName))
                    {
                        ApplicationSettingsGroup group = config.GetSectionGroup("applicationSettings") as ApplicationSettingsGroup;
                        ClientSettingsSection section = group.Sections[sectionName] as ClientSettingsSection;
                       
                        foreach (SettingsProperty prop in properties)
                        {
                            SettingsPropertyValue propertyValue = new SettingsPropertyValue(prop);
                            SettingElement element = section.Settings.Get(prop.Name);
                            if (element != null)
                            {
                                string svalue = element.Value.ValueXml.InnerText;
                                propertyValue.SerializedValue = svalue;
                                _logX.DebugFormat("{0}={1}", prop.Name, svalue);
                                values.Add(propertyValue);
                            }
                        }
                    }
                }
                return values;
            }
        }

        private object GetValue(SettingsProperty prop)
        {
            return 0;
        }

    //Private m_SettingsXML As Xml.XmlDocument = Nothing

    //Private ReadOnly Property SettingsXML() As Xml.XmlDocument
    //    Get
    //        'If we dont hold an xml document, try opening one.  
    //        'If it doesnt exist then create a new one ready.
    //        If m_SettingsXML Is Nothing Then
    //            m_SettingsXML = New Xml.XmlDocument

    //            Try
    //                m_SettingsXML.Load(IO.Path.Combine(GetAppSettingsPath, GetAppSettingsFilename))
    //            Catch ex As Exception
    //                'Create new document
    //                Dim dec As XmlDeclaration = m_SettingsXML.CreateXmlDeclaration("1.0", "utf-8", String.Empty)
    //                m_SettingsXML.AppendChild(dec)

    //                Dim nodeRoot As XmlNode

    //                nodeRoot = m_SettingsXML.CreateNode(XmlNodeType.Element, SETTINGSROOT, "")
    //                m_SettingsXML.AppendChild(nodeRoot)
    //            End Try
    //        End If

    //        Return m_SettingsXML
    //    End Get
    //End Property

    //Private Function GetValue(ByVal setting As SettingsProperty) As String
    //    Dim ret As String = ""

    //    Try
    //        If IsRoaming(setting) Then
    //            ret = SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & setting.Name).InnerText
    //        Else
    //            ret = SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & My.Computer.Name & "/" & setting.Name).InnerText
    //        End If

    //    Catch ex As Exception
    //        If Not setting.DefaultValue Is Nothing Then
    //            ret = setting.DefaultValue.ToString
    //        Else
    //            ret = ""
    //        End If
    //    End Try

    //    Return ret
    //End Function

    //Private Sub SetValue(ByVal propVal As SettingsPropertyValue)

    //    Dim MachineNode As Xml.XmlElement
    //    Dim SettingNode As Xml.XmlElement

    //    'Determine if the setting is roaming.
    //    'If roaming then the value is stored as an element under the root
    //    'Otherwise it is stored under a machine name node 
    //    Try
    //        If IsRoaming(propVal.Property) Then
    //            SettingNode = DirectCast(SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & propVal.Name), XmlElement)
    //        Else
    //            SettingNode = DirectCast(SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & My.Computer.Name & "/" & propVal.Name), XmlElement)
    //        End If
    //    Catch ex As Exception
    //        SettingNode = Nothing
    //    End Try

    //    'Check to see if the node exists, if so then set its new value
    //    If Not SettingNode Is Nothing Then
    //        SettingNode.InnerText = propVal.SerializedValue.ToString
    //    Else
    //        If IsRoaming(propVal.Property) Then
    //            'Store the value as an element of the Settings Root Node
    //            SettingNode = SettingsXML.CreateElement(propVal.Name)
    //            SettingNode.InnerText = propVal.SerializedValue.ToString
    //            SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(SettingNode)
    //        Else
    //            'Its machine specific, store as an element of the machine name node,
    //            'creating a new machine name node if one doesnt exist.
    //            Try
    //                MachineNode = DirectCast(SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & My.Computer.Name), XmlElement)
    //            Catch ex As Exception
    //                MachineNode = SettingsXML.CreateElement(My.Computer.Name)
    //                SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode)
    //            End Try

    //            If MachineNode Is Nothing Then
    //                MachineNode = SettingsXML.CreateElement(My.Computer.Name)
    //                SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode)
    //            End If

    //            SettingNode = SettingsXML.CreateElement(propVal.Name)
    //            SettingNode.InnerText = propVal.SerializedValue.ToString
    //            MachineNode.AppendChild(SettingNode)
    //        End If
    //    End If
    //End Sub

    //Private Function IsRoaming(ByVal prop As SettingsProperty) As Boolean
    //    'Determine if the setting is marked as Roaming
    //    For Each d As DictionaryEntry In prop.Attributes
    //        Dim a As Attribute = DirectCast(d.Value, Attribute)
    //        If TypeOf a Is System.Configuration.SettingsManageabilityAttribute Then
    //            Return True
    //        End If
    //    Next
    //    Return False
    //End Function

        
    }

}
