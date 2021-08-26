//------------------------------------------------------------------------------
// <copyright file="ItemProviderCmdletBase.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Reflection;
    using Helpers;
    using Microsoft.PowerShell.Commands;
    using Objects;


    public class NewItemProviderCmdletBase : ItemProviderPathCmdletBase
    {
        private readonly string newItemType;

        public NewItemProviderCmdletBase(string contextPath, string newItemType, string samplePath) : base(contextPath, samplePath)
        {
            this.newItemType = newItemType;
        }

        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "FullPath":
                    foreach (string path in Path)
                    {
                        NewItem(path, null, NewItemType, null);
                    }
                    break;
                case "Path":
                    foreach (string path in Path)
                    {
                        foreach (string instance in Name)
                        {
                            NewItem(path, instance, NewItemType, null);
                        }
                    }
                    break;
            }
        }

        protected virtual string NewItemType
        {
            get { return newItemType; }
        }

    }

    public class SetItemProviderCmdletBase : ItemProviderPathCmdletBase
    {
        public SetItemProviderCmdletBase(string contextPath, string samplePath) : base(contextPath, samplePath)
        {
        }

        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "FullPath":
                    foreach (string path in Path)
                    {
                        SetItem(path, null);
                    }
                    break;
                case "Path":
                    foreach (string path in Path)
                    {
                        foreach (string instance in Name)
                        {
                            SetItem(String.Format("{0}\\{1}", path, instance), null);
                        }
                    }
                    break;
            }
        }
    }

    public class RemoveItemProviderCmdletBase : ItemProviderPathCmdletBase
    {
        public RemoveItemProviderCmdletBase(string contextPath, string samplePath) : base(contextPath, samplePath)
        {
        }

        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "FullPath":
                    foreach (string path in Path)
                    {
                        RemoveItem(path, false);
                    }
                    break;
                case "Path":
                    foreach (string path in Path)
                    {
                        foreach (string instance in Name)
                        {
                            RemoveItem(path + SQLdmProvider.pathSeparator + instance, false);
                        }
                    }
                    break;
            }
        }
    }

    public class ItemProviderPathCmdletBase : ItemProviderCmdletBase
    {
        protected readonly string contextPath;
        public ItemProviderPathCmdletBase(string contextPath, string samplePath) : base(samplePath)
        {
            this.contextPath = contextPath;
        }

        public virtual string[] Path
        {
            get { return paths; }
            set
            {
                paths = value;
                if (!String.IsNullOrEmpty(contextPath))
                {
                    // force relative paths to start at the root
                    for (int i = 0; i < paths.Length; i++)
                    {
                        string path = paths[i];
                        if (!SQLdmPath.IsRooted(path))
                        {
                            string drive = SQLdmPath.GetDriveName(path);
                            string location = SQLdmPath.RemoveDrive(path);

                            // does path already have the correct context
                            if (!location.StartsWith(contextPath, StringComparison.CurrentCultureIgnoreCase))
                            {
                                location = SQLdmPath.Concat(contextPath, location);
                                paths[i] = SQLdmPath.MakeAbsolutePath(drive, location);
                            }
                        }
                    }
                }
            }
        }
        private string[] paths;

        public virtual string[] Name
        {
            get { return names; }
            set { names = value; }
        }
        private string[] names;

    }

    public class ItemProviderPathAndCredentialsCmdletBase : ItemProviderWithCredentialsCmdletBase
    {
        protected readonly string contextPath;
        public ItemProviderPathAndCredentialsCmdletBase(string contextPath, string samplePath)
            : base(samplePath)
        {
            this.contextPath = contextPath;
        }

        public virtual string[] Path
        {
            get { return paths; }
            set
            {
                paths = value;
                if (!String.IsNullOrEmpty(contextPath))
                {
                    // force relative paths to start at the root
                    for (int i = 0; i < paths.Length; i++)
                    {
                        string path = paths[i];
                        if (!SQLdmPath.IsRooted(path))
                        {
                            string drive = SQLdmPath.GetDriveName(path);
                            string location = SQLdmPath.RemoveDrive(path);

                            // does path already have the correct context
                            if (!location.StartsWith(contextPath, StringComparison.CurrentCultureIgnoreCase))
                            {
                                location = SQLdmPath.Concat(contextPath, location);
                                paths[i] = SQLdmPath.MakeAbsolutePath(drive, location);
                            }
                        }
                    }
                }
            }
        }
        private string[] paths;

        public virtual string[] Name
        {
            get { return names; }
            set { names = value; }
        }
        private string[] names;

    }


    public class ItemProviderCmdletBase : CoreCommandBase, IDynamicParameters
    {
        private string verb;
        private string samplePath;

        public ItemProviderCmdletBase(string samplePath) 
        {
            this.samplePath = samplePath;
            CmdletAttribute cmdletAttribute = (CmdletAttribute)Attribute.GetCustomAttribute(GetType(),typeof(CmdletAttribute));
            if (cmdletAttribute != null)
                verb = cmdletAttribute.VerbName;
        }

        private object GetCmdletProviderContext()
        {
            if (cmdletProviderContextProp == null)
                cmdletProviderContextProp = GetType().GetProperty("CmdletProviderContext", BindingFlags.Instance | BindingFlags.NonPublic);

            return cmdletProviderContextProp.GetValue(this, null);
        }

        private ProviderInfo GetProviderInfoForPath(string path)
        {
            ProviderInfo providerInfo = null;
            try
            {
                this.SessionState.Path.GetResolvedProviderPathFromPSPath(path, out providerInfo);
            }
            catch (Exception e)
            {
            }
            return providerInfo;
        }

        protected Collection<PSObject> NewItem(string path, string name, string type, object value)
        {
            try
            {
                object cmdletProviderContext = GetCmdletProviderContext();
                ProviderInfo providerInfo = GetProviderInfoForPath(path);
                if (providerInfo == null || !providerInfo.Name.Equals("SQLdm") || providerInfo.Drives.Count == 0)
                    throw new ArgumentException("This command requires a connection to an SQLDM Repository.  Please create an SQLDM drive using the New-SQLdmDrive command and specify a path that refrences the new drive.");


                object[] args = new object[] {path, name, type, value, cmdletProviderContext};

                if (newItemMethod == null)
                {
                    newItemMethod =
                        typeof (ItemCmdletProviderIntrinsics).GetMethod("New",
                                                                        BindingFlags.Instance | BindingFlags.NonPublic);
                }

                return newItemMethod.Invoke(InvokeProvider.Item, args) as Collection<PSObject>;
            }
            catch (Exception e)
            {
                HandleException(e, path);
            }
            return null;
        }

        protected Collection<PSObject> SetItem(string path, object value)
        {
            try
            {
                object cmdletProviderContext = GetCmdletProviderContext();
                ProviderInfo providerInfo = GetProviderInfoForPath(path);
                if (providerInfo == null || !providerInfo.Name.Equals("SQLdm") || providerInfo.Drives.Count == 0)
                    throw new ArgumentException("This command requires a connection to an SQLDM Repository.  Please create an SQLDM drive using the New-SQLdmDrive command and specify a path that refrences the new drive.");


                object[] args = new object[] {path, value, cmdletProviderContext};

                if (setItemMethod == null)
                {
                    setItemMethod =
                        typeof (ItemCmdletProviderIntrinsics).GetMethod("Set",
                                                                        BindingFlags.Instance | BindingFlags.NonPublic);
                }

                return setItemMethod.Invoke(InvokeProvider.Item, args) as Collection<PSObject>;
            } catch (Exception e)
            {
                HandleException(e, path);            
            }
            return null;
        }

        protected void HandleException(Exception e, object target)
        {
            while (e is TargetInvocationException && e.InnerException != null)
                e = e.InnerException;

            ErrorRecord erec = null;
            if (e is IContainsErrorRecord)
                erec = ((IContainsErrorRecord) e).ErrorRecord;

            if (erec == null) 
                erec = new ErrorRecord(e, "", ErrorCategory.NotSpecified, target);                

            WriteError(erec);
        }

        protected void RemoveItem(string path, bool recurse)
        {
            try
            {
                object cmdletProviderContext = GetCmdletProviderContext();
                ProviderInfo providerInfo = GetProviderInfoForPath(path);
                if (providerInfo == null || !providerInfo.Name.Equals("SQLdm") || providerInfo.Drives.Count == 0)
                    throw new ArgumentException("This command requires a connection to an SQLDM Repository.  Please create an SQLDM drive using the New-SQLdmDrive command and specify a path that refrences the new drive.");


                object[] args = new object[] {path, recurse, cmdletProviderContext};

                if (removeItemMethod == null)
                {
                    removeItemMethod =
                        typeof (ItemCmdletProviderIntrinsics).GetMethod("Remove",
                                                                        BindingFlags.Instance | BindingFlags.NonPublic);
                }

                removeItemMethod.Invoke(InvokeProvider.Item, args);
            }
            catch (Exception e)
            {
                HandleException(e, path);
            }
        }

        private static PropertyInfo cmdletProviderContextProp;
        private static MethodInfo newItemMethod;
        private static MethodInfo setItemMethod;
        private static MethodInfo removeItemMethod;


        public new object GetDynamicParameters()
        {
            RuntimeDefinedParameterDictionary dynamicParameters = RetrievedDynamicParameters as RuntimeDefinedParameterDictionary;
            if (dynamicParameters == null)
            {
                SQLdmProvider provider = new SQLdmProvider();
                dynamicParameters = provider.GetDynamicParameters(verb, samplePath);

                FieldInfo field = typeof(CoreCommandBase).GetField("dynamicParameters", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(this, dynamicParameters);
            }

            return dynamicParameters;
        }
    }

    public class ItemProviderWithCredentialsCmdletBase : CoreCommandWithCredentialsBase, IDynamicParameters
    {
        private string verb;
        private string samplePath;

        public ItemProviderWithCredentialsCmdletBase(string samplePath)
        {
            this.samplePath = samplePath;
            CmdletAttribute cmdletAttribute = (CmdletAttribute)Attribute.GetCustomAttribute(GetType(), typeof(CmdletAttribute));
            if (cmdletAttribute != null)
                verb = cmdletAttribute.VerbName;
        }

        private object GetCmdletProviderContext()
        {
            if (cmdletProviderContextProp == null)
                cmdletProviderContextProp = GetType().GetProperty("CmdletProviderContext", BindingFlags.Instance | BindingFlags.NonPublic);

            return cmdletProviderContextProp.GetValue(this, null);
        }

        private ProviderInfo GetProviderInfoForPath(string path)
        {
            ProviderInfo providerInfo = null;
            try
            {
                this.SessionState.Path.GetResolvedProviderPathFromPSPath(path, out providerInfo);
            } catch (Exception e)
            {
            }
            return providerInfo;
        }

        protected Collection<PSObject> NewItem(string path, string name, string type, object value)
        {
            try
            {
                object cmdletProviderContext = GetCmdletProviderContext();

                ProviderInfo providerInfo = GetProviderInfoForPath(path);
                if (providerInfo == null || !providerInfo.Name.Equals("SQLdm") || providerInfo.Drives.Count == 0)
                    throw new ArgumentException("This command requires a connection to an SQLDM Repository.  Please create an SQLDM drive using the New-SQLdmDrive command and specify a path that refrences the new drive.");

                object[] args = new object[] { path, name, type, value, cmdletProviderContext };

                if (newItemMethod == null)
                {
                    newItemMethod =
                        typeof(ItemCmdletProviderIntrinsics).GetMethod("New",
                                                                        BindingFlags.Instance | BindingFlags.NonPublic);
                }

                return newItemMethod.Invoke(InvokeProvider.Item, args) as Collection<PSObject>;
            }
            catch (Exception e)
            {
                HandleException(e, path);
            }
            return null;
        }

        protected Collection<PSObject> SetItem(string path, object value)
        {
            try
            {
                object cmdletProviderContext = GetCmdletProviderContext();

                ProviderInfo providerInfo = GetProviderInfoForPath(path);
                if (providerInfo == null || !providerInfo.Name.Equals("SQLdm") || providerInfo.Drives.Count == 0)
                    throw new ArgumentException("This command requires a connection to an SQLDM Repository.  Please create an SQLDM drive using the New-SQLdmDrive command and specify a path that refrences the new drive.");

                object[] args = new object[] { path, value, cmdletProviderContext };

                if (setItemMethod == null)
                {
                    setItemMethod =
                        typeof(ItemCmdletProviderIntrinsics).GetMethod("Set",
                                                                        BindingFlags.Instance | BindingFlags.NonPublic);
                }

                return setItemMethod.Invoke(InvokeProvider.Item, args) as Collection<PSObject>;
            }
            catch (Exception e)
            {
                HandleException(e, path);
            }
            return null;
        }

        protected void HandleException(Exception e, object target)
        {
            while (e is TargetInvocationException && e.InnerException != null)
            {
                e = e.InnerException;
            }

            ErrorRecord erec = null;
            if (e is IContainsErrorRecord)
                erec = ((IContainsErrorRecord)e).ErrorRecord;

            if (erec == null)
                erec = new ErrorRecord(e, "", ErrorCategory.NotSpecified, target);

            WriteError(erec);
        }

        protected void RemoveItem(string path, bool recurse)
        {
            try
            {
                object cmdletProviderContext = GetCmdletProviderContext();
             
                ProviderInfo providerInfo = GetProviderInfoForPath(path);
                if (providerInfo == null || !providerInfo.Name.Equals("SQLdm") || providerInfo.Drives.Count == 0)
                    throw new ArgumentException("This command requires a connection to an SQLDM Repository.  Please create an SQLDM drive using the New-SQLdmDrive command and specify a path that refrences the new drive.");

                object[] args = new object[] { path, recurse, cmdletProviderContext };

                if (removeItemMethod == null)
                {
                    removeItemMethod =
                        typeof(ItemCmdletProviderIntrinsics).GetMethod("Remove",
                                                                        BindingFlags.Instance | BindingFlags.NonPublic);
                }

                removeItemMethod.Invoke(InvokeProvider.Item, args);
            }
            catch (Exception e)
            {
                HandleException(e, path);
            }
        }

        private static PropertyInfo cmdletProviderContextProp;
        private static MethodInfo newItemMethod;
        private static MethodInfo setItemMethod;
        private static MethodInfo removeItemMethod;


        public new object GetDynamicParameters()
        {
            RuntimeDefinedParameterDictionary dynamicParameters = RetrievedDynamicParameters as RuntimeDefinedParameterDictionary;
            if (dynamicParameters == null)
            {
                SQLdmProvider provider = new SQLdmProvider();
                dynamicParameters = provider.GetDynamicParameters(verb, samplePath);

                FieldInfo field = typeof(CoreCommandBase).GetField("dynamicParameters", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(this, dynamicParameters);
            }

            return dynamicParameters;
        }
    }
}
