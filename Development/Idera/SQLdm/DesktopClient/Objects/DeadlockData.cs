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
    [XmlRoot("deadlock-list", IsNullable = false)]
    public class DeadlockData
    {
        private Deadlock[] items;

        [XmlElement("deadlock")]
        public Deadlock[] Items
        {
            get { return items; }
            set { items = value; }
        }

        public static DeadlockData FromXDL(string xdl)
        {
            Type[] extraTypes = new Type[]
                {
                    typeof (Deadlock),
                    typeof (DeadlockExecutionStackFrame),
                    typeof (DeadlockProcess),
                    typeof (DeadlockResourceList),
                    typeof (DeadlockResourceOwner),
                    typeof (DeadlockResourceWaiter)
                };

            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof (DeadlockData), extraTypes);

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.CheckCharacters = false;

            XmlDeserializationEvents events = new XmlDeserializationEvents();
            events.OnUnknownAttribute = serializer_UnknownAttribute;
            events.OnUnknownElement = serializer_UnknownElement;
            events.OnUnknownNode = serializer_UnknownNode;
            events.OnUnreferencedObject = serializer_UnreferencedObject;

            //START SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - appending the root list acco to the deadlock xml by the trace 
			string modXdl = xdl;
            if (!modXdl.Contains("<deadlock-list>"))
            {
                modXdl = "<deadlock-list>" + modXdl;
                modXdl += "</deadlock-list>";
            }
			//END SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - appending the root list acco to the deadlock xml by the trace

            DeadlockData result = null;
            using (XmlReader reader = XmlReader.Create(new StringReader(modXdl), readerSettings))
            {
                result = serializer.Deserialize(reader, events) as DeadlockData;
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
            DeadlockResourceList resourceList = e.ObjectBeingDeserialized as DeadlockResourceList;
            if (resourceList != null)
            {
                DeadlockResource resource = DeserializeDeadlockResource(e.Element.Name, e.Element.OuterXml);
                if (resource != null)
                {
                    resourceList.ResourceList.Add(resource);
                }
                return;
            }
            Debug.Print("Unknown Element {0} {1}", e.Element.OuterXml, e.ObjectBeingDeserialized);
        }

        internal static DeadlockResource DeserializeDeadlockResource(string resourceType, string xdlchunk)
        {
            // override the root node to be of resource type
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            XmlAttributes atts = new XmlAttributes();
            XmlRootAttribute root = new XmlRootAttribute(resourceType);
            atts.XmlRoot = root;
            overrides.Add(typeof(DeadlockResource), atts);

            MemoryStream stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(xdlchunk));

            Type[] extraTypes = new Type[]
                {   typeof(Deadlock),
                    typeof(DeadlockExecutionStackFrame),
                    typeof(DeadlockProcess),
                    typeof(DeadlockResourceList),
                    typeof(DeadlockResourceOwner),
                    typeof(DeadlockResourceWaiter)
                 };

            XmlDeserializationEvents events = new XmlDeserializationEvents();
            events.OnUnknownAttribute = serializer_UnknownAttribute;
            events.OnUnknownElement = serializer_UnknownElement;
            events.OnUnknownNode = serializer_UnknownNode;
            events.OnUnreferencedObject = serializer_UnreferencedObject;

            XmlSerializer serializer = new XmlSerializer(typeof(DeadlockResource), overrides, extraTypes, root, String.Empty);

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.CheckCharacters = false;

            DeadlockResource resource = null;
            
            using (XmlReader reader = XmlReader.Create(stream, readerSettings))
            {
                resource = serializer.Deserialize(reader, events) as DeadlockResource;
            }
            if (resource != null)
            {
                resource.ResourceType = resourceType;
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
    }

    public class Deadlock
    {
        private DeadlockProcess[] processList;
        private DeadlockResourceList resourceListField;
        private string deadlockVictim;
		//START SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - added new property for the victim list
        private DeadlockVictim[] deadlockVictimList;

        [XmlArray("victim-list")]
        [XmlArrayItem("victimProcess", typeof(DeadlockVictim), IsNullable = false)]
        public DeadlockVictim[] DeadlockVictimList
        {
            get { return this.deadlockVictimList; }
            set { this.deadlockVictimList = value; }
        }
		//END SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - added new property for the victim list

        [XmlArray("process-list")]
        [XmlArrayItem("process", typeof(DeadlockProcess), IsNullable = false)]
        public DeadlockProcess[] ProcessList
        {
            get { return processList; }
            set { processList = value; }
        }

        [XmlElement("resource-list")]
        public DeadlockResourceList resourceList
        {
            get { return this.resourceListField; }
            set { this.resourceListField = value; }
        }

        [XmlAttribute("victim")]
        public string DeadlockVictim
        {
            get { return deadlockVictim; }
            set { deadlockVictim = value; }
        }
    }

	//START SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - added new class for the victim
    public class DeadlockVictim
    {
        string id;
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
    }
	//END SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - added new class for the victim

    public partial class DeadlockProcess
    {
        private bool victim;
        private string inputbufField;
        private DeadlockExecutionStackFrame[] executionStackField;
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
        private string transcountField;
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
        [XmlArrayItem("frame", typeof(DeadlockExecutionStackFrame))]
        public DeadlockExecutionStackFrame[] executionStack
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

        [XmlAttribute("transcount")]
        public string transcount
        {
            get { return this.transcountField; }
            set { this.transcountField = value; }
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

    public class DeadlockExecutionStackFrame
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

    public class DeadlockResourceList
    {
        private List<DeadlockResource> resourceList;

        [XmlIgnore]
        public List<DeadlockResource> ResourceList
        {
            get
            {
                if (resourceList == null)
                    resourceList = new List<DeadlockResource>();
                return resourceList;
            }
            set { resourceList = value; }
        }
    }

    public interface IDynamicPropertyProvider
    {
        NameValueCollection DynamicProperties { get; }
        void AddDynamicProperty(string name, string value);
    }

    public class DeadlockResource : IDynamicPropertyProvider
    {
        private string resourceType;
        private DeadlockResourceOwner[] ownerlistField;
        private DeadlockResourceWaiter[] waiterlistField;
//        private string fileidField;
//        private string pageidField;
//        private string dbidField;
//        private string objectnameField;
        private string idField;
//        private string modeField;
        private string associatedObjectIdField;
        private NameValueCollection dynProperties;

        public string ResourceType
        {
            get { return resourceType; }
            set { resourceType = value; }
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

        [XmlArray("owner-list", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("owner", typeof(DeadlockResourceOwner), IsNullable = false)]
        public DeadlockResourceOwner[] ownerlist
        {
            get { return this.ownerlistField; }
            set { this.ownerlistField = value; }
        }

        [XmlArray("waiter-list", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("waiter", typeof(DeadlockResourceWaiter), IsNullable = false)]
        public DeadlockResourceWaiter[] waiterlist
        {
            get { return this.waiterlistField; }
            set { this.waiterlistField = value; }
        }

        //[XmlAttribute("fileid")]
        //public string fileid
        //{
        //    get { return this.fileidField; }
        //    set { this.fileidField = value; }
        //}

        //[XmlAttribute("pageid")]
        //public string pageid
        //{
        //    get { return this.pageidField; }
        //    set { this.pageidField = value; }
        //}

        //[XmlAttribute("dbid")]
        //public string dbid
        //{
        //    get { return this.dbidField; }
        //    set { this.dbidField = value; }
        //}

        //[XmlAttribute("objectname")]
        //public string objectname
        //{
        //    get { return this.objectnameField; }
        //    set { this.objectnameField = value; }
        //}

        [XmlAttribute("id")]
        public string id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        //[XmlAttribute("mode")]
        //public string mode
        //{
        //    get { return this.modeField; }
        //    set { this.modeField = value; }
        //}

        [XmlAttribute("associatedObjectId")]
        public string associatedObjectId
        {
            get { return this.associatedObjectIdField; }
            set { this.associatedObjectIdField = value; }
        }
    }

    public class DeadlockResourceOwner : IDynamicPropertyProvider
    {
        private string idField;
        private string modeField;
        private NameValueCollection dynProperties;

        [XmlAttribute("id")]
        public string id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        [XmlAttribute("mode")]
        public string mode
        {
            get { return this.modeField; }
            set { this.modeField = value; }
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

    public class DeadlockResourceWaiter : IDynamicPropertyProvider
    {
        private string idField;
        private string modeField;
        private string requestTypeField;
        private NameValueCollection dynProperties;

        [XmlAttribute("id")]
        public string id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        [XmlAttribute("mode")]
        public string mode
        {
            get { return this.modeField; }
            set { this.modeField = value; }
        }

        [XmlAttribute("requestType")]
        public string requestType
        {
            get { return this.requestTypeField; }
            set { this.requestTypeField = value; }
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

    //class ProgramX
    //{
        //static void Main(string[] args)
        //{
        //    String filename = "C:\\pagelock_groom1.xdl"; //"c:\\keylock_groom2.xdl";
        //    String xdl = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        //    xdl += File.ReadAllText(filename);

        //    //         xdl = xdl.Replace("deadlock-list", "Deadlocks");

        //    MemoryStream stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(xdl));

        //    Type[] extraTypes = new Type[]
        //        {
        //            typeof(Deadlock),
        //            typeof(DeadlockExecutionStackFrame),
        //            typeof(DeadlockProcess),
        //            typeof(DeadlockResourceList),
        //            typeof(DeadlockResourceOwner),
        //            typeof(DeadlockResourceWaiter)
        //        };

        //    XmlSerializer serializer = new XmlSerializer(typeof(DeadlockData), extraTypes);
        //    serializer.UnknownAttribute += new XmlAttributeEventHandler(DeadlockData.serializer_UnknownAttribute);
        //    serializer.UnknownElement += new XmlElementEventHandler(DeadlockData.serializer_UnknownElement);
        //    serializer.UnknownNode += new XmlNodeEventHandler(DeadlockData.serializer_UnknownNode);
        //    serializer.UnreferencedObject += new UnreferencedObjectEventHandler(DeadlockData.serializer_UnreferencedObject);

        //    DeadlockData data = serializer.Deserialize(stream) as DeadlockData;

        //    DataTable flat = null;
        //    if (data != null)
        //        flat = FlattenDeadlockData(data);

        //    Debug.Print(".");
        //    Console.ReadLine();
        //}



        //static void serializer_UnreferencedObjectx(object sender, UnreferencedObjectEventArgs e)
        //{
        //    Debug.Print("Unreferenced object: {0} {1}", e.UnreferencedId, e.UnreferencedObject);
        //}

        //static void serializer_UnknownNodex(object sender, XmlNodeEventArgs e)
        //{
        //}

        //static void serializer_UnknownElementx(object sender, XmlElementEventArgs e)
        //{
        //    DeadlockResourceList resourceList = e.ObjectBeingDeserialized as DeadlockResourceList;
        //    if (resourceList != null)
        //    {
        //        DeadlockResource resource = DeserializeDeadlockResource(e.Element.Name, e.Element.OuterXml);
        //        if (resource != null)
        //        {
        //            resourceList.ResourceList.Add(resource);
        //        }
        //        return;
        //    }
        //    Debug.Print("Unknown Element {0} {1}", e.Element.OuterXml, e.ObjectBeingDeserialized);
        //}

        //static DeadlockResource DeserializeDeadlockResource(string resourceType, string xdlchunk)
        //{
        //    // override the root node to be of resource type
        //    XmlAttributeOverrides overrides = new XmlAttributeOverrides();
        //    XmlAttributes atts = new XmlAttributes();
        //    XmlRootAttribute root = new XmlRootAttribute(resourceType);
        //    atts.XmlRoot = root;
        //    overrides.Add(typeof(DeadlockResource), atts);

        //    MemoryStream stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(xdlchunk));

        //    Type[] extraTypes = new Type[]
        //        {   typeof(Deadlock),
        //            typeof(DeadlockExecutionStackFrame),
        //            typeof(DeadlockProcess),
        //            typeof(DeadlockResourceList),
        //            typeof(DeadlockResourceOwner),
        //            typeof(DeadlockResourceWaiter)
        //         };

        //    XmlSerializer serializer = new XmlSerializer(typeof(DeadlockResource), overrides, extraTypes, root, String.Empty);
        //    serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
        //    serializer.UnknownElement += new XmlElementEventHandler(serializer_UnknownElement);
        //    serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
        //    serializer.UnreferencedObject += new UnreferencedObjectEventHandler(serializer_UnreferencedObject);

        //    DeadlockResource resource = serializer.Deserialize(stream) as DeadlockResource;
        //    if (resource != null)
        //    {
        //        resource.ResourceType = resourceType;
        //    }
        //    return resource;
        //}

        //static void serializer_UnknownAttributex(object sender, XmlAttributeEventArgs args)
        //{
        //    IDynamicPropertyProvider objWithProperties = args.ObjectBeingDeserialized as IDynamicPropertyProvider;
        //    if (objWithProperties != null)
        //    {
        //        try
        //        {
        //            objWithProperties.AddDynamicProperty(args.Attr.Name, args.Attr.Value);
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Print("Unknown attribute handler: " + e.ToString());
        //        }
        //        return;
        //    }

        //    Debug.Print("Unknown Attribute {0} {1}", args.Attr.Name, args.ObjectBeingDeserialized);
        //}
    //}
}
