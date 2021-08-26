using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Objects
{
    using System.Collections.Specialized;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable()]
    [XmlRoot("blocked-process-report", IsNullable = false)]
    public class BlockData
    {
        private BlockedProcess blocked;
        private BlockingProcess blocking;

        private static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();
        private static object sync = new object();

        [XmlElement("blocked-process")]
        public BlockedProcess Blocked
        {
            get { return blocked; }
            set { blocked = value; }
        }

        [XmlElement("blocking-process")]
        public BlockingProcess Blocking
        {
            get { return blocking; }
            set { blocking = value; }
        }

        public static BlockData FromXML(string bXML)
        {
            Type[] extraTypes = new Type[]
                {
                    typeof (BlockedProcess),
                    typeof (BlockingProcess)
                };

            XmlSerializer serializer = Common.Data.XmlSerializerFactory.GetSerializer(typeof(BlockData), extraTypes);

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.CheckCharacters = false;

            XmlDeserializationEvents events = new XmlDeserializationEvents();
            events.OnUnknownAttribute = serializer_UnknownAttribute;
            events.OnUnknownElement = serializer_UnknownElement;
            events.OnUnknownNode = serializer_UnknownNode;
            events.OnUnreferencedObject = serializer_UnreferencedObject;

            BlockData result = null;
            using (XmlReader reader = XmlReader.Create(new StringReader(bXML), readerSettings))
            {
                result = serializer.Deserialize(reader, events) as BlockData;
            }
            return result;
        }

        internal static void serializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            Debug.Print("Unreferenced object: {0} {1}", e.UnreferencedId, e.UnreferencedObject);
        }

        internal static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
        }

        internal static void serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            BlockedProcess resourceList = e.ObjectBeingDeserialized as BlockedProcess;
            if (resourceList != null)
            {
                BlockProcess resource = DeserializeBlockedProcess(e.Element.Name, e.Element.OuterXml);
                if (resource != null)
                {
                    resourceList.Blocked = resource;
                }
                return;
            }
            Debug.Print("Unknown Element {0} {1}", e.Element.OuterXml, e.ObjectBeingDeserialized);
        }

        internal static BlockProcess DeserializeBlockedProcess(string resourceType, string xdlchunk)
        {
            // override the root node to be of resource type
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            XmlAttributes atts = new XmlAttributes();
            XmlRootAttribute root = new XmlRootAttribute(resourceType);
            atts.XmlRoot = root;
            overrides.Add(typeof(BlockProcess), atts);

            MemoryStream stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(xdlchunk));

            Type[] extraTypes = new Type[]
                {typeof(BlockExecutionStackFrame),
                    typeof(BlockProcess)
                 };

            XmlDeserializationEvents events = new XmlDeserializationEvents();
            events.OnUnknownAttribute = serializer_UnknownAttribute;
            events.OnUnknownElement = serializer_UnknownElement;
            events.OnUnknownNode = serializer_UnknownNode;
            events.OnUnreferencedObject = serializer_UnreferencedObject;

            XmlSerializer serializer = new XmlSerializer(typeof(BlockProcess), overrides, extraTypes, root, String.Empty);

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.CheckCharacters = false;

            BlockProcess resource = null;

            using (XmlReader reader = XmlReader.Create(stream, readerSettings))
            {
                resource = serializer.Deserialize(reader, events) as BlockProcess;
            }

            return resource;
        }

        internal static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs args)
        {
            IDynamicPropertyProvider objWithProperties = args.ObjectBeingDeserialized as IDynamicPropertyProvider;
            if (objWithProperties != null)
            {
                try
                {
                    objWithProperties.AddDynamicProperty(args.Attr.Name, args.Attr.Value);
                }
                catch (Exception e)
                {
                    Debug.Print("Unknown attribute handler: " + e.ToString());
                }
                return;
            }

            Debug.Print("Unknown Attribute {0} {1}", args.Attr.Name, args.ObjectBeingDeserialized);
        }

        public interface IDynamicPropertyProvider
        {
            NameValueCollection DynamicProperties { get; }
            void AddDynamicProperty(string name, string value);
        }
    }

    public class BlockedProcess : BlockData.IDynamicPropertyProvider
    {
        private BlockProcess _blocked;
        private NameValueCollection dynProperties;

        [XmlElement("process", typeof(BlockProcess))]
        public BlockProcess Blocked
        {
            get { return _blocked; }
            set { _blocked = value; }
        }
        [XmlIgnore]
        public NameValueCollection DynamicProperties
        {
            get { return dynProperties; }
        }

        public void AddDynamicProperty(string name, string value)
        {
            if (dynProperties == null)
                dynProperties = new NameValueCollection();
            dynProperties.Add(name, value);
        }
    }

    public class BlockingProcess : BlockData.IDynamicPropertyProvider
    {
        private BlockProcess _blocking;
        private NameValueCollection dynProperties;

        [XmlElement("process", typeof(BlockProcess))]
        public BlockProcess Blocking
        {
            get { return _blocking; }
            set { _blocking = value; }
        }
        [XmlIgnore]
        public NameValueCollection DynamicProperties
        {
            get { return dynProperties; }
        }

        public void AddDynamicProperty(string name, string value)
        {
            if (dynProperties == null)
                dynProperties = new NameValueCollection();
            dynProperties.Add(name, value);
        }
    }

    /// <summary>
    /// Block and blocked processes look the same
    /// Blocking process is the same minus the first 12 attributes
    /// </summary>
    public partial class BlockProcess
    {
        private bool victim;
        private string inputbufField;
        private BlockExecutionStackFrame[] executionStackField;
        private string id;
        private string taskPriorityField;
        private string logusedField;
        private string waitresourceField;
        private string waittimeField;
        private string ownerIdField;
        private string transactionnameField;
        private string lasttranstartedField;
        private string xDESField;
        private string lockModeField;
        private string scheduleridField;
        private string kpidField;
        private string statusField;
        private string spidField;
        private string sbidField;
        private string ecidField;
        private string priorityField;
        private string trancountField;
        private string lastbatchstartedField;
        private string lastbatchcompletedField;
        private string lastattentionField;
        private string clientappField;
        private string hostnameField;
        private string hostpidField;
        private string loginnameField;
        private string isolationlevelField;
        private string xactidField;
        private string currentdbField;
        private string lockTimeoutField;
        private string clientoption1Field;
        private string clientoption2Field;
        private string databaseName;

        [XmlIgnore]
        public bool Victim
        {
            get { return victim; }
            set { victim = value; }
        }

        [XmlAttribute("databaseName")]
        public string Database
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string inputbuf
        {
            get { return this.inputbufField; }
            set { this.inputbufField = value; }
        }

        [XmlArray(Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("frame", typeof(BlockExecutionStackFrame))]
        public BlockExecutionStackFrame[] executionStack
        {
            get { return this.executionStackField; }
            set { this.executionStackField = value; }
        }

        [XmlAttribute("id")]
        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        [XmlAttribute("taskpriority")]
        public string taskpriority
        {
            get { return this.taskPriorityField; }
            set { this.taskPriorityField = value; }
        }

        [XmlAttribute("logused")]
        public string logused
        {
            get { return this.logusedField; }
            set { this.logusedField = value; }
        }

        [XmlAttribute("waitresource")]
        public string waitresource
        {
            get { return this.waitresourceField; }
            set { this.waitresourceField = value; }
        }

        [XmlAttribute("waittime")]
        public string waittime
        {
            get { return this.waittimeField; }
            set { this.waittimeField = value; }
        }

        [XmlAttribute("ownerId")]
        public string ownerId
        {
            get { return this.ownerIdField; }
            set { this.ownerIdField = value; }
        }

        [XmlAttribute("transactionname")]
        public string transactionname
        {
            get { return this.transactionnameField; }
            set { this.transactionnameField = value; }
        }

        [XmlAttribute("lasttranstarted")]
        public string lasttranstarted
        {
            get { return this.lasttranstartedField; }
            set { this.lasttranstartedField = value; }
        }

        [XmlAttribute("XDES")]
        public string XDES
        {
            get { return this.xDESField; }
            set { this.xDESField = value; }
        }

        [XmlAttribute("lockMode")]
        public string lockMode
        {
            get { return this.lockModeField; }
            set { this.lockModeField = value; }
        }

        [XmlAttribute("schedulerid")]
        public string schedulerid
        {
            get { return this.scheduleridField; }
            set { this.scheduleridField = value; }
        }

        [XmlAttribute("kpid")]
        public string kpid
        {
            get { return this.kpidField; }
            set { this.kpidField = value; }
        }

        [XmlAttribute("status")]
        public string status
        {
            get { return this.statusField; }
            set { this.statusField = value; }
        }

        [XmlAttribute("spid")]
        public string spid
        {
            get { return this.spidField; }
            set { this.spidField = value; }
        }

        [XmlAttribute("sbid")]
        public string sbid
        {
            get { return this.sbidField; }
            set { this.sbidField = value; }
        }

        [XmlAttribute("ecid")]
        public string ecid
        {
            get { return this.ecidField; }
            set { this.ecidField = value; }
        }

        [XmlAttribute("priority")]
        public string priority
        {
            get { return this.priorityField; }
            set { this.priorityField = value; }
        }

        [XmlAttribute("trancount")]
        public string trancount
        {
            get { return this.trancountField; }
            set { this.trancountField = value; }
        }

        [XmlAttribute("lastbatchstarted")]
        public string lastbatchstarted
        {
            get { return this.lastbatchstartedField; }
            set { this.lastbatchstartedField = value; }
        }

        [XmlAttribute("lastbatchcompleted")]
        public string lastbatchcompleted
        {
            get { return this.lastbatchcompletedField; }
            set { this.lastbatchcompletedField = value; }
        }

        [XmlAttribute("lastattention")]
        public string lastattention
        {
            get { return this.lastattentionField; }
            set { this.lastattentionField = value; }
        }

        [XmlAttribute("clientapp")]
        public string clientapp
        {
            get { return this.clientappField; }
            set { this.clientappField = value; }
        }

        [XmlAttribute("hostname")]
        public string hostname
        {
            get { return this.hostnameField; }
            set { this.hostnameField = value; }
        }

        [XmlAttribute("hostpid")]
        public string hostpid
        {
            get { return this.hostpidField; }
            set { this.hostpidField = value; }
        }

        [XmlAttribute("loginname")]
        public string loginname
        {
            get { return this.loginnameField; }
            set { this.loginnameField = value; }
        }

        [XmlAttribute("isolationlevel")]
        public string isolationlevel
        {
            get { return this.isolationlevelField; }
            set { this.isolationlevelField = value; }
        }

        [XmlAttribute("xactid")]
        public string xactid
        {
            get { return this.xactidField; }
            set { this.xactidField = value; }
        }

        [XmlAttribute("currentdb")]
        public string currentdb
        {
            get { return this.currentdbField; }
            set { this.currentdbField = value; }
        }

        [XmlAttribute("lockTimeout")]
        public string lockTimeout
        {
            get { return this.lockTimeoutField; }
            set { this.lockTimeoutField = value; }
        }

        [XmlAttribute("clientoption1")]
        public string clientoption1
        {
            get { return this.clientoption1Field; }
            set { this.clientoption1Field = value; }
        }

        [XmlAttribute("clientoption2")]
        public string clientoption2
        {
            get { return this.clientoption2Field; }
            set { this.clientoption2Field = value; }
        }
    }

    /// <summary>
    /// Execution stack is the same as for Blocks
    /// </summary>
    public class BlockExecutionStackFrame
    {
        private string procnameField;
        private string lineField;
        private string stmtstartField;
        private string sqlhandleField;
        private string stmtendField;
        private string valueField;

        [XmlAttribute("procname")]
        public string procname
        {
            get { return this.procnameField; }
            set { this.procnameField = value; }
        }

        [XmlAttribute("line")]
        public string line
        {
            get { return this.lineField; }
            set { this.lineField = value; }
        }

        [XmlAttribute("stmtstart")]
        public string stmtstart
        {
            get { return this.stmtstartField; }
            set { this.stmtstartField = value; }
        }

        [XmlAttribute("sqlhandle")]
        public string sqlhandle
        {
            get { return this.sqlhandleField; }
            set { this.sqlhandleField = value; }
        }

        [XmlAttribute("stmtend")]
        public string stmtend
        {
            get { return this.stmtendField; }
            set { this.stmtendField = value; }
        }

        [XmlText()]
        public string Value
        {
            get { return this.valueField; }
            set { this.valueField = value; }
        }
    }
    //[Serializable()]
    //[XmlRoot("blocked-process-report", IsNullable = false)]
    //public class BlockData
    //{
    //    private BlockProcess blocked;
    //    private BlockProcess blocking;

    //    [XmlElement("blocked-process")]
    //    public BlockProcess Blocked
    //    {
    //        get { return blocked; }
    //        set { blocked = value; }
    //    }

    //    [XmlElement("blocking-process")]
    //    public BlockProcess Blocking
    //    {
    //        get { return blocking; }
    //        set { blocking = value; }
    //    }

    //    public static BlockData FromXDL(string xdl)
    //    {
    //        Type[] extraTypes = new Type[]
    //            {
    //                typeof (Deadlock),
    //                typeof (DeadlockExecutionStackFrame),
    //                typeof (DeadlockProcess),
    //                typeof (DeadlockResourceList),
    //                typeof (DeadlockResourceOwner),
    //                typeof (DeadlockResourceWaiter)
    //            };

    //        XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof (BlockData), extraTypes);

    //        XmlReaderSettings readerSettings = new XmlReaderSettings();
    //        readerSettings.CheckCharacters = false;

    //        XmlDeserializationEvents events = new XmlDeserializationEvents();
    //        events.OnUnknownAttribute = serializer_UnknownAttribute;
    //        events.OnUnknownElement = serializer_UnknownElement;
    //        events.OnUnknownNode = serializer_UnknownNode;
    //        events.OnUnreferencedObject = serializer_UnreferencedObject;

    //        BlockData result = null;
    //        using (XmlReader reader = XmlReader.Create(new StringReader(xdl), readerSettings))
    //        {
    //            result = serializer.Deserialize(reader, events) as BlockData;
    //        }
    //        return result;
    //    }

    //    internal static void serializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
    //    {
    //        Debug.Print("Unreferenced object: {0} {1}", e.UnreferencedId, e.UnreferencedObject);
    //    }

    //    internal static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
    //    {
    //    }

    //    internal static void serializer_UnknownElement(object sender, XmlElementEventArgs e)
    //    {
    //        DeadlockResourceList resourceList = e.ObjectBeingDeserialized as DeadlockResourceList;
    //        if (resourceList != null)
    //        {
    //            DeadlockResource resource = DeserializeDeadlockResource(e.Element.Name, e.Element.OuterXml);
    //            if (resource != null)
    //            {
    //                resourceList.ResourceList.Add(resource);
    //            }
    //            return;
    //        }
    //        Debug.Print("Unknown Element {0} {1}", e.Element.OuterXml, e.ObjectBeingDeserialized);
    //    }

    //    internal static BlockResource DeserializeBlockResource(string resourceType, string xdlchunk)
    //    {
    //        // override the root node to be of resource type
    //        XmlAttributeOverrides overrides = new XmlAttributeOverrides();
    //        XmlAttributes atts = new XmlAttributes();
    //        XmlRootAttribute root = new XmlRootAttribute(resourceType);
    //        atts.XmlRoot = root;
    //        overrides.Add(typeof(DeadlockResource), atts);

    //        MemoryStream stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(xdlchunk));

    //        Type[] extraTypes = new Type[]
    //            {   typeof(Deadlock),
    //                typeof(DeadlockExecutionStackFrame),
    //                typeof(DeadlockProcess),
    //                typeof(DeadlockResourceList),
    //                typeof(DeadlockResourceOwner),
    //                typeof(DeadlockResourceWaiter)
    //             };

    //        XmlDeserializationEvents events = new XmlDeserializationEvents();
    //        events.OnUnknownAttribute = serializer_UnknownAttribute;
    //        events.OnUnknownElement = serializer_UnknownElement;
    //        events.OnUnknownNode = serializer_UnknownNode;
    //        events.OnUnreferencedObject = serializer_UnreferencedObject;

    //        XmlSerializer serializer = new XmlSerializer(typeof(BlockResource), overrides, extraTypes, root, String.Empty);

    //        XmlReaderSettings readerSettings = new XmlReaderSettings();
    //        readerSettings.CheckCharacters = false;

    //        DeadlockResource resource = null;
            
    //        using (XmlReader reader = XmlReader.Create(stream, readerSettings))
    //        {
    //            resource = serializer.Deserialize(reader, events) as DeadlockResource;
    //        }
    //        if (resource != null)
    //        {
    //            resource.ResourceType = resourceType;
    //        }
    //        return resource;
    //    }

    //    internal static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs args)
    //    {
    //        IDynamicPropertyProvider objWithProperties = args.ObjectBeingDeserialized as IDynamicPropertyProvider;
    //        if (objWithProperties != null)
    //        {
    //            try
    //            {
    //                objWithProperties.AddDynamicProperty(args.Attr.Name, args.Attr.Value);
    //            }
    //            catch (Exception e)
    //            {
    //                Debug.Print("Unknown attribute handler: " + e.ToString());
    //            }
    //            return;
    //        }

    //        Debug.Print("Unknown Attribute {0} {1}", args.Attr.Name, args.ObjectBeingDeserialized);
    //    }
    //}



    //public class Block
    //{
    //    private DeadlockProcess[] processList;
    //    private DeadlockResourceList resourceListField;
    //    private string deadlockVictim;

    //    [XmlArray("process-list")]
    //    [XmlArrayItem("process", typeof(DeadlockProcess), IsNullable = false)]
    //    public DeadlockProcess[] ProcessList
    //    {
    //        get { return processList; }
    //        set { processList = value; }
    //    }

    //    [XmlElement("resource-list")]
    //    public DeadlockResourceList resourceList
    //    {
    //        get { return this.resourceListField; }
    //        set { this.resourceListField = value; }
    //    }

    //    [XmlAttribute("victim")]
    //    public string DeadlockVictim
    //    {
    //        get { return deadlockVictim; }
    //        set { deadlockVictim = value; }
    //    }
    //}

    //public class BlockedProcess
    //{

    //}
    ///// <summary>
    ///// Deadlock and blocked processes look the same
    ///// Blocking process is the same minus the first 12 attributes
    ///// </summary>
    //public partial class BlockProcess
    //{
    //    private bool victim;
    //    private string inputbufField;
    //    private BlockExecutionStackFrame[] executionStackField;
    //    private string id;
    //    private string taskPriorityField;
    //    private string logusedField;
    //    private string waitresourceField;
    //    private string waittimeField;
    //    private string ownerIdField;
    //    private string transactionnameField;
    //    private string lasttranstartedField;
    //    private string xDESField;
    //    private string lockModeField;
    //    private string scheduleridField;
    //    private string kpidField;
    //    private string statusField;
    //    private string spidField;
    //    private string sbidField;
    //    private string ecidField;
    //    private string priorityField;
    //    private string trancountField;
    //    private string lastbatchstartedField;
    //    private string lastbatchcompletedField;
    //    private string lastattentionField;
    //    private string clientappField;
    //    private string hostnameField;
    //    private string hostpidField;
    //    private string loginnameField;
    //    private string isolationlevelField;
    //    private string xactidField;
    //    private string currentdbField;
    //    private string lockTimeoutField;
    //    private string clientoption1Field;
    //    private string clientoption2Field;
    //    private string databaseName;

    //    [XmlIgnore]
    //    public bool Victim
    //    {
    //        get { return victim; }
    //        set { victim = value; }
    //    }

    //    [XmlAttribute("databaseName")]
    //    public string Database
    //    {
    //        get { return databaseName; }
    //        set { databaseName = value; }
    //    }

    //    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    //    public string inputbuf
    //    {
    //        get { return this.inputbufField; }
    //        set { this.inputbufField = value; }
    //    }

    //    [XmlArray(Form = XmlSchemaForm.Unqualified)]
    //    [XmlArrayItem("frame", typeof(BlockExecutionStackFrame))]
    //    public BlockExecutionStackFrame[] executionStack
    //    {
    //        get { return this.executionStackField; }
    //        set { this.executionStackField = value; }
    //    }

    //    [XmlAttribute("id")]
    //    public string Id
    //    {
    //        get { return this.id; }
    //        set { this.id = value; }
    //    }

    //    [XmlAttribute("taskpriority")]
    //    public string taskpriority
    //    {
    //        get { return this.taskPriorityField; }
    //        set { this.taskPriorityField = value; }
    //    }

    //    [XmlAttribute("logused")]
    //    public string logused
    //    {
    //        get { return this.logusedField; }
    //        set { this.logusedField = value; }
    //    }

    //    [XmlAttribute("waitresource")]
    //    public string waitresource
    //    {
    //        get { return this.waitresourceField; }
    //        set { this.waitresourceField = value; }
    //    }

    //    [XmlAttribute("waittime")]
    //    public string waittime
    //    {
    //        get { return this.waittimeField; }
    //        set { this.waittimeField = value; }
    //    }

    //    [XmlAttribute("ownerId")]
    //    public string ownerId
    //    {
    //        get { return this.ownerIdField; }
    //        set { this.ownerIdField = value; }
    //    }

    //    [XmlAttribute("transactionname")]
    //    public string transactionname
    //    {
    //        get { return this.transactionnameField; }
    //        set { this.transactionnameField = value; }
    //    }

    //    [XmlAttribute("lasttranstarted")]
    //    public string lasttranstarted
    //    {
    //        get { return this.lasttranstartedField; }
    //        set { this.lasttranstartedField = value; }
    //    }

    //    [XmlAttribute("XDES")]
    //    public string XDES
    //    {
    //        get { return this.xDESField; }
    //        set { this.xDESField = value; }
    //    }

    //    [XmlAttribute("lockMode")]
    //    public string lockMode
    //    {
    //        get { return this.lockModeField; }
    //        set { this.lockModeField = value; }
    //    }

    //    [XmlAttribute("schedulerid")]
    //    public string schedulerid
    //    {
    //        get { return this.scheduleridField; }
    //        set { this.scheduleridField = value; }
    //    }

    //    [XmlAttribute("kpid")]
    //    public string kpid
    //    {
    //        get { return this.kpidField; }
    //        set { this.kpidField = value; }
    //    }

    //    [XmlAttribute("status")]
    //    public string status
    //    {
    //        get { return this.statusField; }
    //        set { this.statusField = value; }
    //    }

    //    [XmlAttribute("spid")]
    //    public string spid
    //    {
    //        get { return this.spidField; }
    //        set { this.spidField = value; }
    //    }

    //    [XmlAttribute("sbid")]
    //    public string sbid
    //    {
    //        get { return this.sbidField; }
    //        set { this.sbidField = value; }
    //    }

    //    [XmlAttribute("ecid")]
    //    public string ecid
    //    {
    //        get { return this.ecidField; }
    //        set { this.ecidField = value; }
    //    }

    //    [XmlAttribute("priority")]
    //    public string priority
    //    {
    //        get { return this.priorityField; }
    //        set { this.priorityField = value; }
    //    }

    //    [XmlAttribute("trancount")]
    //    public string trancount
    //    {
    //        get { return this.trancountField; }
    //        set { this.trancountField = value; }
    //    }

    //    [XmlAttribute("lastbatchstarted")]
    //    public string lastbatchstarted
    //    {
    //        get { return this.lastbatchstartedField; }
    //        set { this.lastbatchstartedField = value; }
    //    }

    //    [XmlAttribute("lastbatchcompleted")]
    //    public string lastbatchcompleted
    //    {
    //        get { return this.lastbatchcompletedField; }
    //        set { this.lastbatchcompletedField = value; }
    //    }

    //    [XmlAttribute("lastattention")]
    //    public string lastattention
    //    {
    //        get { return this.lastattentionField; }
    //        set { this.lastattentionField = value; }
    //    }

    //    [XmlAttribute("clientapp")]
    //    public string clientapp
    //    {
    //        get { return this.clientappField; }
    //        set { this.clientappField = value; }
    //    }

    //    [XmlAttribute("hostname")]
    //    public string hostname
    //    {
    //        get { return this.hostnameField; }
    //        set { this.hostnameField = value; }
    //    }

    //    [XmlAttribute("hostpid")]
    //    public string hostpid
    //    {
    //        get { return this.hostpidField; }
    //        set { this.hostpidField = value; }
    //    }

    //    [XmlAttribute("loginname")]
    //    public string loginname
    //    {
    //        get { return this.loginnameField; }
    //        set { this.loginnameField = value; }
    //    }

    //    [XmlAttribute("isolationlevel")]
    //    public string isolationlevel
    //    {
    //        get { return this.isolationlevelField; }
    //        set { this.isolationlevelField = value; }
    //    }

    //    [XmlAttribute("xactid")]
    //    public string xactid
    //    {
    //        get { return this.xactidField; }
    //        set { this.xactidField = value; }
    //    }

    //    [XmlAttribute("currentdb")]
    //    public string currentdb
    //    {
    //        get { return this.currentdbField; }
    //        set { this.currentdbField = value; }
    //    }

    //    [XmlAttribute("lockTimeout")]
    //    public string lockTimeout
    //    {
    //        get { return this.lockTimeoutField; }
    //        set { this.lockTimeoutField = value; }
    //    }

    //    [XmlAttribute("clientoption1")]
    //    public string clientoption1
    //    {
    //        get { return this.clientoption1Field; }
    //        set { this.clientoption1Field = value; }
    //    }

    //    [XmlAttribute("clientoption2")]
    //    public string clientoption2
    //    {
    //        get { return this.clientoption2Field; }
    //        set { this.clientoption2Field = value; }
    //    }
    //}

    ///// <summary>
    ///// Execution stack is the same as for deadlocks
    ///// </summary>
    //public class BlockExecutionStackFrame
    //{
    //    private string procnameField;
    //    private string lineField;
    //    private string stmtstartField;
    //    private string sqlhandleField;
    //    private string stmtendField;
    //    private string valueField;

    //    [XmlAttribute("procname")]
    //    public string procname
    //    {
    //        get { return this.procnameField; }
    //        set { this.procnameField = value; }
    //    }

    //    [XmlAttribute("line")]
    //    public string line
    //    {
    //        get { return this.lineField; }
    //        set { this.lineField = value; }
    //    }

    //    [XmlAttribute("stmtstart")]
    //    public string stmtstart
    //    {
    //        get { return this.stmtstartField; }
    //        set { this.stmtstartField = value; }
    //    }

    //    [XmlAttribute("sqlhandle")]
    //    public string sqlhandle
    //    {
    //        get { return this.sqlhandleField; }
    //        set { this.sqlhandleField = value; }
    //    }

    //    [XmlAttribute("stmtend")]
    //    public string stmtend
    //    {
    //        get { return this.stmtendField; }
    //        set { this.stmtendField = value; }
    //    }

    //    [XmlText()]
    //    public string Value
    //    {
    //        get { return this.valueField; }
    //        set { this.valueField = value; }
    //    }
    //}

    //public class BlockResourceList
    //{
    //    private List<BlockResource> resourceList;

    //    [XmlIgnore]
    //    public List<BlockResource> ResourceList
    //    {
    //        get
    //        {
    //            if (resourceList == null)
    //                resourceList = new List<BlockResource>();
    //            return resourceList;
    //        }
    //        set { resourceList = value; }
    //    }
    //}

    ///// <summary>
    ///// Has no block equivalent
    ///// </summary>
    //public class BlockResource : IDynamicPropertyProvider
    //{
    //    private string resourceType;
    //    private BlockResourceOwner[] ownerlistField;
    //    private BlockResourceWaiter[] waiterlistField;
    //    private string idField;
    //    private string associatedObjectIdField;
    //    private NameValueCollection dynProperties;

    //    public string ResourceType
    //    {
    //        get { return resourceType; }
    //        set { resourceType = value; }
    //    }

    //    [XmlIgnore]
    //    public NameValueCollection DynamicProperties
    //    {
    //        get { return dynProperties; }
    //    }

    //    public void AddDynamicProperty(string name, string value)
    //    {
    //        if (dynProperties == null)
    //            dynProperties = new NameValueCollection();
    //        dynProperties.Add(name, value);
    //    }

    //    [XmlArray("owner-list", Form = XmlSchemaForm.Unqualified)]
    //    [XmlArrayItem("owner", typeof(BlockResourceOwner), IsNullable = false)]
    //    public BlockResourceOwner[] ownerlist
    //    {
    //        get { return this.ownerlistField; }
    //        set { this.ownerlistField = value; }
    //    }

    //    [XmlArray("waiter-list", Form = XmlSchemaForm.Unqualified)]
    //    [XmlArrayItem("waiter", typeof(BlockResourceWaiter), IsNullable = false)]
    //    public BlockResourceWaiter[] waiterlist
    //    {
    //        get { return this.waiterlistField; }
    //        set { this.waiterlistField = value; }
    //    }

    //    [XmlAttribute("id")]
    //    public string id
    //    {
    //        get { return this.idField; }
    //        set { this.idField = value; }
    //    }

    //    [XmlAttribute("associatedObjectId")]
    //    public string associatedObjectId
    //    {
    //        get { return this.associatedObjectIdField; }
    //        set { this.associatedObjectIdField = value; }
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public class BlockResourceOwner : IDynamicPropertyProvider
    //{
    //    private string idField;
    //    private string modeField;
    //    private NameValueCollection dynProperties;

    //    [XmlAttribute("id")]
    //    public string id
    //    {
    //        get { return this.idField; }
    //        set { this.idField = value; }
    //    }

    //    [XmlAttribute("mode")]
    //    public string mode
    //    {
    //        get { return this.modeField; }
    //        set { this.modeField = value; }
    //    }

    //    [XmlIgnore]
    //    public NameValueCollection DynamicProperties
    //    {
    //        get { return dynProperties; }
    //    }

    //    public void AddDynamicProperty(string name, string value)
    //    {
    //        if (dynProperties == null)
    //            dynProperties = new NameValueCollection();
    //        dynProperties.Add(name, value);
    //    }

    //}
    ///// <summary>
    ///// Does not exist in blocks
    ///// </summary>
    //public class BlockResourceWaiter : IDynamicPropertyProvider
    //{
    //    private string idField;
    //    private string modeField;
    //    private string requestTypeField;
    //    private NameValueCollection dynProperties;

    //    [XmlAttribute("id")]
    //    public string id
    //    {
    //        get { return this.idField; }
    //        set { this.idField = value; }
    //    }

    //    [XmlAttribute("mode")]
    //    public string mode
    //    {
    //        get { return this.modeField; }
    //        set { this.modeField = value; }
    //    }

    //    [XmlAttribute("requestType")]
    //    public string requestType
    //    {
    //        get { return this.requestTypeField; }
    //        set { this.requestTypeField = value; }
    //    }

    //    [XmlIgnore]
    //    public NameValueCollection DynamicProperties
    //    {
    //        get { return dynProperties; }
    //    }

    //    public void AddDynamicProperty(string name, string value)
    //    {
    //        if (dynProperties == null)
    //            dynProperties = new NameValueCollection();
    //        dynProperties.Add(name, value);
    //    }
    //}
}
