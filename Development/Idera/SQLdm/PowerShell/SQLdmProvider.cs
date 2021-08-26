//------------------------------------------------------------------------------
// <copyright file="SQLdmProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Runtime.Remoting.Messaging;
using Idera.SQLdm.Common.Auditing;


namespace Idera.SQLdm.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using System.Management.Automation.Provider;
    using System.Net;
    using System.Reflection;
    using System.Security;
    using System.Text;
    using BBS.TracerX;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Idera.SQLdm.Common.Services;
    using Objects;
    using Wintellect.PowerCollections;
    using Configuration = Idera.SQLdm.Common.Objects.ApplicationSecurity.Configuration;
    using Common.Configuration.ServerActions;
    using Common.Data;
    using Common.Snapshots;
    

    /// </summary>
    [CmdletProvider("SQLdm", ProviderCapabilities.Credentials)]
    public partial class SQLdmProvider : NavigationCmdletProvider
    {
        public static readonly DateTime Jan_1_1900 = new DateTime(1900, 1, 1, 0, 0, 0);

        private static Logger LOG;

        internal const string pathSeparator = "\\";
        static SQLdmProvider()
        {
            Logger.FileLogging.Directory = "%LOCALAPPDATA%\\Idera\\SQLdm\\Logs";
            Logger.FileLogging.Name = "SQLdmProvider.tx1";
            Logger.FileLogging.MaxSizeMb = 5;
            Logger.FileLogging.Archives = 3;
            Logger.FileLogging.CircularStartSizeKb = 100;
            Logger.FileLogging.CircularStartDelaySeconds = 4000000000;

            Logger.Root.FileTraceLevel = BBS.TracerX.TraceLevel.Info;
            Logger.Root.ConsoleTraceLevel = BBS.TracerX.TraceLevel.Off;
            Logger.Root.EventLogTraceLevel = BBS.TracerX.TraceLevel.Off;
            Logger.Root.DebugTraceLevel = BBS.TracerX.TraceLevel.Off;

            Logger Log = Logger.GetLogger("StandardData");
            using (Log.InfoCall())
            {
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                {
                    entryAssembly = typeof(SQLdmProvider).Assembly;
                }

                Log.Info("EntryAssembly.Location = ", entryAssembly.Location);
                Log.Info("EntryAssembly.FullName = ", entryAssembly.FullName); // Includes assembly version.

                try
                {
                    // Try to get the file version.
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
                    Log.Info("FileVersionInfo.FileVersion = ", fvi.FileVersion);
                    Log.Info("FileVersionInfo.ProductVersion = ", fvi.ProductVersion);
                }
                catch (Exception)
                {
                }

                Log.Info("Environment.OSVersion = ", Environment.OSVersion);
                Log.Info("Environment.CurrentDirectory = ", Environment.CurrentDirectory);
                Log.Info("Environment.UserInteractive = ", Environment.UserInteractive);

                Log.Debug("Environment.CommandLine = ", Environment.CommandLine);

                Log.Verbose("Environment.MachineName = ", Environment.MachineName);
                Log.Verbose("Environment.UserDomainName = ", Environment.UserDomainName);
                Log.Verbose("Environment.UserName = ", Environment.UserName);
            }

            Logger.FileLogging.Open();

            LOG = Logger.GetLogger("SQLdmProvider");
            LOG.Info("SQLdmProvider logging initialized");
        }

        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            using (LOG.InfoCall("Start"))
            {
                return providerInfo;
            }
        }

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            using (LOG.InfoCall("NewDrive"))
            {
                // check if drive object is null
                if (drive == null)
                {
                    WriteError(new ErrorRecord(
                                   new ArgumentNullException("drive"),
                                   "NullDrive",
                                   ErrorCategory.InvalidArgument,
                                   null)
                        );
                    LOG.Error("Drive is null");
                    return null;
                }

                // Check if drive root is not null or empty.
                if (String.IsNullOrEmpty(drive.Root))
                {
                    WriteError(new ErrorRecord(
                                   new ArgumentException("drive.Root"),
                                   "NoRoot",
                                   ErrorCategory.InvalidArgument,
                                   drive)
                        );
                    LOG.Error("Drive root is null or empty");
                    return null;
                }

                try
                {
                    SQLdmDriveInfo newDrive = drive as SQLdmDriveInfo;
                    if (newDrive == null)
                    {
                        RuntimeDefinedParameterDictionary dynamicParameters =
                            DynamicParameters as RuntimeDefinedParameterDictionary;
                        if (dynamicParameters != null)
                        {
                            string instance =
                                Helper.GetDynamicParameterValue(dynamicParameters, Constants.RepositoryInstanceParameter,
                                                                String.Empty);
                            string database =
                                Helper.GetDynamicParameterValue(dynamicParameters, Constants.RepositoryNameParameter,
                                                                String.Empty);

                            LOG.InfoFormat("Creating SQLdmDrive {0} for {1}\\{2}", drive.Name, instance, database);

                            // some trickery on the root
                            string root = drive.Root;
                            if (root.Contains(driveSeparator))
                            {
                                string[] parts = root.Split(driveSeparatorArray, 2, StringSplitOptions.None);
                                root = parts.Length == 2 ? parts[1] : "";
                            }
                            if (!root.StartsWith(pathSeparator))
                                root = pathSeparator + root;

                            // F%$# it - set root to \
                            root = drive.Name + driveSeparator + pathSeparator;

                            newDrive =
                                new SQLdmDriveInfo(drive.Name, drive.Provider, root, drive.Description, drive.Credential,
                                                   instance, database);
                        }
                    }

                    if (newDrive != null)
                    {
                        newDrive.Validate();
                        return newDrive;
                    }
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(
                                   e,
                                   "NotValid",
                                   ErrorCategory.DeviceError,
                                   drive)
                        );
                    LOG.Error("New drive failed: ", e);
                }
                return null;
            }
        }

        protected override object NewDriveDynamicParameters()
        {
            using (LOG.InfoCall("NewDriveDynamicParameters"))
            {
                RuntimeParameterBuilder parms = new RuntimeParameterBuilder();

                parms.AddParameter<string>(Constants.RepositoryInstanceParameter, true, null);
                parms.AddParameter<string>(Constants.RepositoryNameParameter, true, null);

                return parms.Parameters;
            }
        }

        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            using (LOG.InfoCall("RemoveDrive"))
            {
                // Check if drive object is null
                if (drive == null)
                {
                    WriteError(new ErrorRecord(
                                   new ArgumentNullException("drive"),
                                   "NullDrive",
                                   ErrorCategory.InvalidArgument,
                                   drive)
                        );

                    return null;
                }

                // Close the ODBC connection to the drive.
                SQLdmDriveInfo dmDriveInfo = drive as SQLdmDriveInfo;
                if (dmDriveInfo == null)
                {
                    return null;
                }

                dmDriveInfo.Close();

                return dmDriveInfo;
            }
        }

        /// <remarks>
        /// This test should not verify the existance of the item at the path. 
        /// It should only perform syntactic and semantic validation of the 
        /// path. For instance, for the file system provider, that path should
        /// be canonicalized, syntactically verified, and ensure that the path
        /// does not refer to a device.
        /// </remarks>
        protected override bool IsValidPath(string path)
        {
            using (LOG.InfoCall("IsValidPath"))
            {
                // empty path is considered to be the root
                if (String.IsNullOrEmpty(path))
                {
                    LOG.Verbose("path is empty - returning true");
                    return true;
                }

                // Convert all separators in the path to a uniform one.
                path = NormalizePath(path);

                if (String.IsNullOrEmpty(path))
                {
                    LOG.Verbose("path is empty - returning true");
                    return true;
                }

                // lop off drive from path
                path = RemoveDriveFromPath(path);

                // Split the path into individual chunks.
                string[] pathChunks = NormalizeAndChunkPath(path);
                // figure out what type of path it be
                PathType pathType = GetPathType(pathChunks);

                LOG.VerboseFormat("path={0} is a {0}", path, pathType);

                return pathType != PathType.Invalid;
            }
        }

        /// <remarks>
        /// Providers can override this method to give the user the ability to 
        /// check for the existence of provider objects using the test-path 
        /// cmdlet. The default implementation returns false.
        /// 
        /// Providers that declare 
        /// <see cref="System.Management.Automation.Provider.ProviderCapabilities"/>
        /// of ExpandWildcards, Filter, Include, or Exclude should ensure that 
        /// the path passed meets those requirements by accessing the 
        /// appropriate property from the base class.
        /// 
        /// The implemenation of this method should take into account any form 
        /// of access to the object that may make it visible to the user.  For 
        /// instance, if a user has write access to a file in the file system 
        /// provider but not read access, the file still exists and the method
        /// should return true.  Sometimes this may require checking the parent
        /// to see if the child can be enumerated.
        /// </remarks>
        protected override bool ItemExists(string path)
        {
            using (LOG.InfoCall("ItemExists"))
            {
                // check if the path represented is a drive
                if (PathIsDrive(path))
                {
                    return true;
                }

                SQLdmDriveInfo driveInfo = GetSQLdmDriveInfo(path);
                UserToken userToken = driveInfo.UserToken;

                // split the path into chunks
                string[] pathChunks = NormalizeAndChunkPath(path);
                PathType pathType = GetPathType(pathChunks);
                switch (pathType)
                {
                    case PathType.Invalid:
                        return false;
                    case PathType.Root:
                        // The root always exists 
                        return true;
                    case PathType.Instances:
                        // Instances is a static directory so it always exists
                        return true;
                    case PathType.Instance:
                        // check the repository and see if the instance named in the path exists
                        if (pathChunks.Length < 2)
                            return false;

                        string instanceName = UnescapeString(pathChunks[1]);
                        if (GetInstancePermission(userToken, instanceName) == PermissionType.None)
                        {
                            return false;
                        }
                        using (ProgressProvider progress = new ProgressProvider(this, String.Format("Checking '{0}' exists", instanceName)))
                        {
                            return Helper.IsInstanceValid(driveInfo, UnescapeString(pathChunks[1]), progress);
                        }
                    case PathType.Alerts:
                        // Alerts is a static directory.  It exists if its parent exists.
                        if (pathChunks.Length < 2)
                            return false;

                        instanceName = UnescapeString(pathChunks[1]);
                        if (GetInstancePermission(userToken, instanceName) == PermissionType.None)
                        {
                            return false;
                        }
                        using (ProgressProvider progress = new ProgressProvider(this, String.Format("Checking '{0}' exists", instanceName)))
                        {
                            return Helper.IsInstanceValid(driveInfo, UnescapeString(pathChunks[1]), progress);
                        }
                    case PathType.Alert:
                        return false;
                    case PathType.CustomCounters:
                        // Instances is a static directory so it always exists
                        if (pathChunks.Length < 2)
                            return false;

                        instanceName = UnescapeString(pathChunks[1]);
                        if (GetInstancePermission(userToken, instanceName) == PermissionType.None)
                        {
                            return false;
                        }
                        using (ProgressProvider progress = new ProgressProvider(this, String.Format("Checking '{0}' exists", instanceName)))
                        {
                            return Helper.IsInstanceValid(driveInfo, UnescapeString(pathChunks[1]), progress);
                        }
                    case PathType.CustomCounter:
                        return false;
                    case PathType.AppSecurity:
                        // Instances is a static directory so it always exists
                        return true;
                    case PathType.AppUser:
                        {
                            if (pathChunks.Length < 2)
                                return false;

                            if (userToken.IsSecurityEnabled)
                            {
                                if (!(userToken.IsSysadmin || userToken.IsSQLdmAdministrator))
                                {
                                    return false;
                                }
                            }

                            string user = UnescapeString(pathChunks[1]);
                            return Helper.GetSQLdmUserExists(driveInfo, user);
                        }
                    case PathType.AppUserPermission:
                        {
                            if (pathChunks.Length < 3)
                                return false;

                            if (userToken.IsSecurityEnabled)
                            {
                                if (!(userToken.IsSysadmin || userToken.IsSQLdmAdministrator))
                                {
                                    return false;
                                }
                            }

                            if (pathChunks[2].StartsWith("#--"))
                                return true;

                            string user = UnescapeString(pathChunks[1]);
                            string server = UnescapeString(pathChunks[2]);
                            return Helper.GetSQLdmUserHasServerPermission(driveInfo, user, server);
                        }
                }
                WriteError(new ErrorRecord
                               (new ArgumentException("The argument specified is not valid"),
                                "InvalidArgument", ErrorCategory.InvalidArgument, pathChunks));
                return false;
            }
        }

        private PermissionType GetInstancePermission(UserToken token, string instanceName)
        {
            if (!token.IsSecurityEnabled)
                return PermissionType.Administrator;

            return token.GetServerPermission(instanceName);
        }


        /// <summary>
        /// Gets the item at the specified path. 
        /// </summary>
        /// 
        /// <param name="path">
        /// The path to the item to retrieve.
        /// </param>
        /// 
        /// <returns>
        /// Nothing is returned, but all objects will be written to the 
        /// WriteItemObject method.
        /// </returns>
        /// 
        /// <remarks>
        /// Providers can override this method to give the user access to the 
        /// provider objects using the get-item and get-childitem cmdlets. All
        /// objects should be written to the 
        /// <see cref="System.Management.Automation.Provider.CmdletProvider.WriteItemObject"/>
        /// method.
        /// 
        /// Providers that declare 
        /// <see cref="System.Management.Automation.Provider.ProviderCapabilities"/>
        /// of ExpandWildcards, Filter, Include, or Exclude should ensure that 
        /// the path passed meets those requirements by accessing the 
        /// appropriate property from the base class.
        /// 
        /// By default overrides of this method should not write objects that 
        /// are generally hidden from the user unless the Force property is set
        /// to true. For instance, the FileSystem provider should not call 
        /// WriteItemObject for hidden or system files unless the Force 
        /// property is set to true. 
        /// 
        /// The default implementation of this method does nothing
        /// </remarks>    
        protected override void GetItem(string path)
        {
            using (LOG.InfoCall("GetItem"))
            {
                LOG.VerboseFormat("path is [{0}]", path);
                SQLdmDriveInfo driveInfo = GetSQLdmDriveInfo(path);

                // check if the path represented is a drive
                if (PathIsDrive(path))
                {
                    WriteItemObject(driveInfo, driveInfo.Root, true);
                }

                UserToken userToken = driveInfo.UserToken;
                MonitoredSqlServerInfo instance;

                // split the path into chunks
                string[] pathChunks = NormalizeAndChunkPath(path);
                int nPathChunks = pathChunks.Length;
                PathType pathType = GetPathType(pathChunks);
                switch (pathType)
                {
                    case PathType.Invalid:
                        return;
                    case PathType.Root:
                        return;
                    case PathType.AppSecurity:
                        WriteToPipeline(new AppSecurityInfo(driveInfo), path, true);
                        return;
                    case PathType.Instances:
                        WriteToPipeline(new InstancesInfo(driveInfo), path, true);
                        return;
                    case PathType.Instance:
                        {
                            string instanceName = UnescapeString(pathChunks[1]);
                            using (ProgressProvider progress = new ProgressProvider(this, String.Format("Getting '{0}'", instanceName)))
                            {
                                MonitoredSqlServerInfo msi = Helper.GetInstance(driveInfo, instanceName, progress);
                                if (msi != null)
                                    WriteToPipeline(msi, path, true);
                            }
                        }
                        return;
                    case PathType.Alerts:
                        WriteToPipeline(new AlertsInfo(driveInfo, UnescapeString(pathChunks[1])), path, true);
                        return;
                    case PathType.Alert:
                        return;
                    case PathType.AppUser:
                        {
                            if (userToken.IsSecurityEnabled)
                            {
                                if (!(userToken.IsSysadmin || userToken.IsSQLdmAdministrator))
                                {
                                    WriteError(new ErrorRecord
                                                   (new SecurityException(
                                                        "You are not authorized to view application security"),
                                                    "SecurityException", ErrorCategory.SecurityError, pathChunks)
                                        );
                                    return;
                                }
                            }

                            string user = UnescapeString(pathChunks[1]);
                            SQLdmUserInfo userInfo = Helper.GetSQLdmUserInfo(driveInfo, user);
                            if (userInfo != null)
                                WriteToPipeline(userInfo,
                                                GetParentPath(path, String.Empty) + pathSeparator +
                                                EscapeString(userInfo.Login), false);
                            return;
                        }
                    case PathType.AppUserPermission:
                        {
                            if (userToken.IsSecurityEnabled)
                            {
                                if (!(userToken.IsSysadmin || userToken.IsSQLdmAdministrator))
                                {
                                    WriteError(new ErrorRecord
                                                   (new SecurityException(
                                                        "You are not authorized to view application security"),
                                                    "SecurityException", ErrorCategory.SecurityError, pathChunks)
                                        );
                                    return;
                                }
                            }

                            string user = UnescapeString(pathChunks[1]);
                            string instanceName = UnescapeString(pathChunks[2]);
                            SQLdmUserInfo userInfo = Helper.GetSQLdmUserInfo(driveInfo, user);
                            if (userInfo != null)
                            {
                                if (!userInfo.IsAdministrator)
                                {
                                    foreach (SQLdmPermissionInfo permission in userInfo.Permissions)
                                    {
                                        if (
                                            permission.InstanceName.Equals(instanceName,
                                                                           StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            WriteToPipeline(permission,
                                                            GetParentPath(path, String.Empty) + pathSeparator +
                                                            EscapeString(permission.InstanceName), false);
                                            break;
                                        }
                                    }
                                }
                            }
                            return;
                        }
                }

                WriteError(new ErrorRecord
                               (new ArgumentException("The argument specified is not valid"),
                                "InvalidArgument", ErrorCategory.InvalidArgument, pathChunks)
                    );
            }
        }

        /// <summary>
        /// Sets the item specified by the path. 
        /// </summary>
        /// 
        /// <param name="path">
        /// The path to the item to set.
        /// </param>
        /// 
        /// <param name="value">
        /// The value of the item specified by the path.
        /// </param>
        /// 
        /// <remarks>
        /// The item that is set should be written to the 
        /// <see cref="System.Management.Automation.Providers.CmdletProvider.WriteItemObject"/> 
        /// method.
        /// 
        /// Providers can override this method to give the user the ability to 
        /// modify provider objects using the set-item cmdlet.
        /// 
        /// Providers that declare 
        /// <see cref="System.Management.Automation.Provider.ProviderCapabilities"/>
        /// of ExpandWildcards, Filter, Include, or Exclude should ensure that
        /// the path passed meets those requirements by accessing the 
        /// appropriate property from the base class.
        /// 
        /// By default, overrides of this method should not set or write 
        /// objects that are generally hidden from the user unless the Force 
        /// property is set to true. An error should be sent to the 
        /// <see cref="System.Management.Automation.Provider.CmdletProvider.WriteError(ErrorRecord)"/>
        /// method if the path represents an item that is hidden from the user 
        /// and Force is set to false.
        /// 
        /// The default implementation of this method throws an 
        /// <see cref="System.Management.Automation.PSNotSupportedException"/>.
        /// This method should call ShouldProcess and check its return value 
        /// before making any changes to the store this provider is working upon.
        /// </remarks>     
        protected override void SetItem(string path, object value)
        {
            using (LOG.InfoCall("SetItem"))
            {
                if (DynamicParameters == null && value != null)
                {
                    value =
                        BindCommandToDynamicParameters(path, (PSCmdlet)value,
                                                       SetItemDynamicParameters(path, null) as
                                                       RuntimeDefinedParameterDictionary);
                }

                string[] pathChunks = NormalizeAndChunkPath(path);

                try
                {
                    ValidateDynamicParameters();
                }
                catch (ParameterBindingException pie)
                {
                    WriteError(pie.ErrorRecord);
                    return;
                }

                switch (GetPathType(pathChunks))
                {
                    case PathType.Instance:
                        SetItemInstance(path, pathChunks, value);
                        break;
                    case PathType.CustomCounter:
                        SetItemCustomCounter(path, pathChunks, value);
                        break;
                    case PathType.AppSecurity:
                        SetItemAppSecurity(path, pathChunks, value);
                        break;
                    case PathType.AppUser:
                        SetItemAppUser(path, pathChunks, value);
                        break;
                    case PathType.AppUserPermission:
                        NewAppUserPermission(path, pathChunks[1], pathChunks[2], value);
                        break;
                }


                // Example
                // if (ShouldProcess(path, "ClearItem"))
                // {
                //      // Set the item and then call WriteItemObject
                //      WriteItemObject(item, path, isContainer);
                // }
            }
        }

        private object BindCommandToDynamicParameters(string path, PSCmdlet cmdlet, RuntimeDefinedParameterDictionary dynamicParameters)
        {
            using (LOG.InfoCall("BindCommandToDynamicParameters"))
            {
                if (dynamicParameters == null)
                    return null;

                object cmdletProviderContext = GetCmdletProviderContext();

                SetDynamicParametersProperty(cmdletProviderContext, dynamicParameters);

                object result = null;

                foreach (PropertyInfo pi in cmdlet.GetType().GetProperties())
                {
                    foreach (
                        ParameterAttribute patt in Attribute.GetCustomAttributes(pi, typeof(ParameterAttribute), true))
                    {
                        RuntimeDefinedParameter rdp;
                        if (dynamicParameters.TryGetValue(pi.Name, out rdp))
                        {
                            if (rdp.ParameterType == pi.PropertyType)
                            {
                                rdp.Value = pi.GetValue(cmdlet, null);
                                break;
                            }
                        }
                        else if (pi.Name.Equals("Value"))
                        {
                            result = pi.GetValue(cmdlet, null);
                        }
                    }
                }

                return result;
            }
        }

        private void SetItemAppUser(string path, string[] pathChunks, object value)
        {
            using (LOG.InfoCall("SetItemAppUser"))
            {
                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);

                string userName = pathChunks[1];

                // see if user authz to change instance
                UserToken token = drive.UserToken;
                PermissionType permissions = (!token.IsSecurityEnabled ||
                                              (token.IsSysadmin || token.IsSQLdmAdministrator))
                                                 ? PermissionType.Administrator
                                                 : PermissionType.None;
                if (permissions != PermissionType.Administrator)
                {
                    WriteError(new ErrorRecord
                                   (new PSSecurityException("You are not authorized to change user permissions"),
                                    "SecurityException", ErrorCategory.SecurityError, userName)
                        );
                    return;
                }


                SQLdmUserInfo user = Helper.GetSQLdmUserInfo(drive, userName);
                // can't fu** with users that don't exist
                if (user == null)
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException("User does not exist"),
                                    "ObjectNotFound", ErrorCategory.ObjectNotFound, userName)
                        );
                    return;
                }
                // can't fu** with system users
                if (user.IsSystem)
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException("You are not allowed to modify system admin users"),
                                    "InvalidOperation", ErrorCategory.InvalidOperation, userName)
                        );
                    return;
                }

                RuntimeDefinedParameterDictionary dynamicParms = (RuntimeDefinedParameterDictionary)DynamicParameters;
                RuntimeDefinedParameter adminParameter = dynamicParms[Constants.AdmininstratorParameter];
                SQLdmUserInfo newInstance = (SQLdmUserInfo)user.Clone();

                // update with dynamic parameters
                newInstance = UpdateSQLdmUserWithDynamicParameters(drive, newInstance);

                bool updateComments = false;
                bool updateEnabled = false;
                bool updateDisabled = false;
                foreach (RuntimeDefinedParameter rdp in dynamicParms.Values)
                {
                    if (rdp.IsSet)
                    {
                        switch (rdp.Name)
                        {
                            case Constants.EnabledParameter:
                                updateEnabled = true;
                                break;
                            case Constants.DisabledParameter:
                                updateDisabled = true;
                                break;
                            case Constants.CommentParameter:
                                updateComments = true;
                                break;
                        }
                    }
                }

                List<PermissionDefinition> adminPermissions = new List<PermissionDefinition>();
                foreach (PermissionDefinition pd in Helper.GetSQLdmUserPermission(drive, userName))
                {
                    if (pd.PermissionType == PermissionType.Administrator)
                    {
                        adminPermissions.Add(pd);
                    }
                }

                if (adminParameter.IsSet)
                {
                    bool adminParameterValue = GetBooleanParameterValue(adminParameter);
                    if (adminParameterValue)
                    {
                        if (adminPermissions.Count == 0)
                        {
                            // add an admin permission
                            drive.SetContextDataChangeLog(drive);
                            DataHelper.ManagementService.AddPermission(user.Login,
                                                                  user.LoginType == LoginType.SQLLogin,
                                                                  String.Empty,
                                                                  PermissionType.Administrator,
                                                                  new int[0],
                                                                  new int[0],
                                                                  newInstance.Comments,
                                                                  user.LoginType != LoginType.SQLLogin ? user.WebAppPermission : false
                                );
                        }
                        else
                        {
                            while (adminPermissions.Count > 1)
                            {
                                PermissionDefinition pd = adminPermissions[adminPermissions.Count - 1];
                                drive.SetContextDataChangeLog(drive);
                                DataHelper.ManagementService.DeletePermission(pd.PermissionID);
                            }

                            drive.SetContextDataChangeLog(drive);
                            DataHelper.ManagementService.EditPermission(adminPermissions[0].PermissionID,
                                                                   true,
                                                                   PermissionType.Administrator,
                                                                   new int[0],
                                                                   new int[0],
                                                                   newInstance.Comments,
                                                                   user.LoginType != LoginType.SQLLogin ? adminPermissions[0].WebAppPermission : false);
                        }
                    }
                    else
                    {
                        if (adminPermissions.Count > 0)
                        {
                            foreach (PermissionDefinition pd in adminPermissions)
                            {
                                drive.SetContextDataChangeLog(drive);
                                DataHelper.ManagementService.DeletePermission(pd.PermissionID);
                            }
                        }
                    }
                }

                foreach (PermissionDefinition pd in Helper.GetSQLdmUserPermission(drive, newInstance.Login))
                {
                    bool updateInstance = false;
                    if (updateComments && !pd.Comment.Equals(newInstance.Comments))
                        updateInstance = true;
                    if (updateEnabled && pd.Enabled != true)
                        updateInstance = true;
                    else if (updateDisabled && pd.Enabled != false)
                        updateInstance = true;

                    if (updateInstance)
                    {
                        List<int> servers = new List<int>();
                        foreach (Server server in pd.GetServerList())
                        {
                            servers.Add(server.SQLServerID);
                        }

                        drive.SetContextDataChangeLog(drive);
                        DataHelper.ManagementService.EditPermission(pd.PermissionID,
                                                               updateEnabled
                                                                   ? true
                                                                   : updateDisabled ? false : pd.Enabled,
                                                               pd.PermissionType,
                                                               new int[0],
                                                               servers,
                                                               updateComments ? newInstance.Comments : pd.Comment,
                                                               user.LoginType != LoginType.SQLLogin ? pd.WebAppPermission : false);
                    }
                }

                drive.SetSecurityConfigurationDirty();

                user = Helper.GetSQLdmUserInfo(drive, userName);

                WriteItemObject(user, path, !user.IsAdministrator);
            }
        }

        private void SetItemAppSecurity(string path, string[] pathChunks, object value)
        {
            using (LOG.InfoCall("SetItemAppSecurity"))
            {
                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);

                // see if user authz to change instance
                UserToken token = drive.UserToken;
                if (!token.IsSQLdmAdministrator && !token.IsSysadmin)
                {
                    WriteError(new ErrorRecord
                                   (new SecurityException("You are not authorized to change SQLDM application security"),
                                    "SecurityException", ErrorCategory.SecurityError, pathChunks)
                        );
                    return;
                }

                // get the required Enabled setting
                bool newEnabled = false;
                RuntimeDefinedParameterDictionary dynamicParameters =
                    DynamicParameters as RuntimeDefinedParameterDictionary;
                if (dynamicParameters != null)
                {
                    foreach (RuntimeDefinedParameter dynamicParameter in dynamicParameters.Values)
                    {
                        if (dynamicParameter.IsSet)
                        {
                            switch (dynamicParameter.Name)
                            {
                                case Constants.EnabledParameter:
                                    newEnabled = GetBooleanParameterValue(dynamicParameter);
                                    break;
                            }
                        }
                    }
                }

                if (newEnabled != token.IsSecurityEnabled)
                {
                    drive.SetAppSecurityEnabled(drive, newEnabled);
                }

                AppSecurityInfo asi = new AppSecurityInfo(drive);

                WriteToPipeline(asi, path, drive.UserToken.IsSecurityEnabled);
            }
        }

        private void SetItemCustomCounter(string path, string[] pathChunks, object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        private void SetItemInstance(string path, string[] pathChunks, object value)
        {
            using (LOG.InfoCall("SetItemInstance"))
            {
                MonitoredSqlServerInfo server = null;
                if (value is PSObject)
                    server = ((PSObject)value).BaseObject as MonitoredSqlServerInfo;
                else
                    server = value as MonitoredSqlServerInfo;

                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);
                string instanceName = UnescapeString(pathChunks[1]);

                using (ProgressProvider progress = new ProgressProvider(this, String.Format("Setting '{0}'", instanceName)))
                {
                    bool updateInstanceName = false;
                    if (server == null)
                    {
                        server = Helper.GetInstance(drive, instanceName, progress);
                    }
                    else
                        updateInstanceName = true;

                    if (server == null)
                    {
                        WriteError(new ErrorRecord
                                       (new ItemNotFoundException(
                                            String.Format("Monitored Instance '{0}' not found.", instanceName)),
                                        "ObjectNotFound", ErrorCategory.ObjectNotFound, instanceName));
                        return;
                    }

                    // see if user authz to change instance
                    UserToken token = drive.UserToken;
                    PermissionType permissions = token.GetServerPermission(instanceName);
                    if (permissions == PermissionType.None || permissions == PermissionType.View)
                    {
                        WriteError(new ErrorRecord
                                       (new SecurityException(
                                            String.Format("You are not authorized to change instance {0}", instanceName)),
                                        "SecurityException", ErrorCategory.SecurityError, pathChunks)
                            );
                        return;
                    }


                    // update with dynamic parameters
                    server = UpdateInstanceWithDynamicParameters(server);

                    if (updateInstanceName)
                        server.InstanceName = instanceName;

                    if (Credential != null && !String.IsNullOrEmpty(Credential.UserName))
                    {
                        NetworkCredential netcred = Credential.GetNetworkCredential();
                        if (netcred != null && !String.IsNullOrEmpty(netcred.Password))
                        {
                            if (
                                Helper.IsDynamicParameterSet(DynamicParameters as RuntimeDefinedParameterDictionary,
                                                             Constants.WindowsAuthenticationParameter))
                            {
                                WriteError(new ErrorRecord
                                               (new PSNotSupportedException(
                                                    String.Format(
                                                        "You cannot use both the Credential and the WindowsAuthentication parameters.",
                                                        instanceName)),
                                                "InvalidData", ErrorCategory.InvalidData, pathChunks)
                                    );
                                return;
                            }
                            server.Credential = Credential;
                        }
                    }

                    progress.ReportProgress(null, "Writing changes to repository");
                    MonitoredSqlServerConfiguration configuration = (MonitoredSqlServerConfiguration)server;

                    string user;

                    if (String.IsNullOrEmpty(drive.Credential.UserName))
                    {
                        user = AuditingEngine.GetWorkstationUser();
                    }
                    else
                    {
                        user = drive.Credential.UserName.StartsWith("\\") ? drive.Credential.UserName.Remove(0, 1) : drive.Credential.UserName;
                    }

                    AuditingEngine.SetContextData(user);
                    AuditingEngine.SetAuxiliarData("VmConfigurationFlag", new AuditAuxiliar<bool>(true));

                    MonitoredSqlServer mss = DataHelper.ManagementService.UpdateMonitoredSqlServer(server.Id, configuration);

                    WriteToPipeline(new MonitoredSqlServerInfo(mss, drive), path, true);
                }
            }
        }

        private MonitoredSqlServerInfo UpdateInstanceWithDynamicParameters(MonitoredSqlServerInfo instance)
        {
            using (LOG.InfoCall("UpdateInstanceWithDynamicParameters"))
            {
                string[] addTags = null;
                string[] removeTags = null;

                MonitoredSqlServerInfo result = (MonitoredSqlServerInfo)instance.Clone();


                IManagementService managementService =
                DataHelper.ManagementService;

               
                
                RuntimeDefinedParameterDictionary dynamicParameters =
                    DynamicParameters as RuntimeDefinedParameterDictionary;
                if (dynamicParameters != null)
                {
                    foreach (RuntimeDefinedParameter dynamicParameter in dynamicParameters.Values)
                    {
                        if (dynamicParameter.IsSet)
                        {
                            switch (dynamicParameter.Name)
                            {
                                case Constants.WindowsAuthenticationParameter:
                                case Constants.CredentialParameter:
                                    result.Credential = GetObjectParameterValue(dynamicParameter) as PSCredential;
                                    break;
                                case Constants.EncryptParameter:
                                    result.EncryptData = GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.TrustCertsParameter:
                                    result.TrustServerCertificate = GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.ScheduledCollectionIntervalParameter:
                                    result.ScheduledCollectionIntervalMinutes = GetIntParameterValue(dynamicParameter);
                                    break;
                                //SQLDM-30516. Add New parameter for setting "Alert if server is inaccessible" in general settings
                                case Constants.ServerInAccessibleRateParameter:
                                    result.ServerInaccessibleAlertIntervalMinutes = GetIntParameterValue(dynamicParameter);
                                    break;
                                case Constants.DatabaseStatisticsParameter:
                                    result.DatabaseStatisticsIntervalMinutes = GetIntParameterValue(dynamicParameter);
                                    break;
                                case Constants.CollectExtendedSessionDataParameter:
                                    result.ExtendedHistoryCollectionDisabled = GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.SetCollectExtendedSessionDataParameter:
                                    result.ExtendedHistoryCollectionDisabled = !GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.CollectReplicationDataParameter:
                                    result.ReplicationMonitoringDisabled = GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.SetCollectReplicationDataParameter:
                                    result.ReplicationMonitoringDisabled = !GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.DisableOleAutomationUseParameter:
                                    result.WmiConfig.OleAutomationDisabled = GetBooleanParameterValue(dynamicParameter);
                                    result.WmiConfig.DirectWmiEnabled = false;
                                    break;
                                case Constants.TagsParameter:
                                    result.Tags = GetObjectParameterValue(dynamicParameter) as string[];
                                    break;
                                case Constants.AddTagParameter:
                                    addTags = GetObjectParameterValue(dynamicParameter) as string[];
                                    break;
                                case Constants.RemoveTagParameter:
                                    removeTags = GetObjectParameterValue(dynamicParameter) as string[];
                                    break;
                                case Constants.MModeNeverParameter:
                                    result.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Never;
                                    result.MaintenanceModeEnabled = false;
                                    break;
                                case Constants.MModeAlwaysParameter:
                                    result.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Always;
                                    result.MaintenanceModeEnabled = true;
                                    break;
                                case Constants.MModeOnceParameter:
                                    result.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Once;
                                    break;
                                case Constants.MModeRecurringParameter:
                                    result.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Recurring;
                                    break;
                                case Constants.MModeDaysParameter:
                                    result.MaintenanceMode.MaintenanceModeDays =
                                        (Days)CombineDays(GetObjectParameterValue(dynamicParameter));
                                    break;
                                case Constants.MModeStartTimeParameter:
                                    {
                                        TimeSpan ts = GetTypedParameterValue<TimeSpan>(dynamicParameter);
                                        result.MaintenanceMode.MaintenanceModeRecurringStart = ts;
                                        bool mmModeForToday = false;

                                        //See if today is of of the day for recurring maintenance mode.
                                        foreach (int val in Enum.GetValues(typeof(DayOfWeek)))
                                        {
                                            if (MonitoredSqlServer.MatchDayOfWeek((DayOfWeek)val, (short)result.MaintenanceMode.MaintenanceModeDays))
                                            {
                                                if (val == (int)DateTime.Now.DayOfWeek)
                                                {
                                                    mmModeForToday = true;
                                                    break;
                                                }
                                            }
                                        }

                                        bool isInDateRange = (DateTime.Now.TimeOfDay >=
                                                              result.MaintenanceMode.MaintenanceModeRecurringStart) &&
                                                             (DateTime.Now.TimeOfDay <
                                                              (result.MaintenanceMode.MaintenanceModeRecurringStart +
                                                               result.MaintenanceMode.MaintenanceModeDuration));
                                        //do not set the maintenance mode flag if it is not time for maintenance mode yet.
                                        result.MaintenanceModeEnabled = mmModeForToday && isInDateRange;
                                    }
                                    break;
                                case Constants.MModeDurationParameter:
                                    result.MaintenanceMode.MaintenanceModeDuration =
                                        GetTypedParameterValue<TimeSpan>(dynamicParameter);
                                    break;
                                case Constants.MModeStartDateParameter:
                                    result.MaintenanceMode.MaintenanceModeStart =
                                        GetTypedParameterValue<DateTime>(dynamicParameter);

                                    //do not set the maintenance mode flag if it is not time for maintenance mode yet.	                                    
                                    result.MaintenanceModeEnabled =
                                        (DateTime.Now >= result.MaintenanceMode.MaintenanceModeStart);

                                    break;
                                case Constants.MModeEndDateParameter:
                                    result.MaintenanceMode.MaintenanceModeStop =
                                        GetTypedParameterValue<DateTime>(dynamicParameter);
                                    break;

                                #region OsMetrics

                                case Constants.OsMetricCollectByOleParameter:
                                    result.WmiConfig.OleAutomationDisabled = false;
                                    result.WmiConfig.DirectWmiEnabled = false;
                                    break;
                                case Constants.OsMetricCollectByWmiParameter:
                                    result.WmiConfig.OleAutomationDisabled = true;
                                    result.WmiConfig.DirectWmiEnabled = true;
                                    break;
                                case Constants.OsMetricConnectAsCollectionService:
                                    result.WmiConfig.DirectWmiConnectAsCollectionService = GetTypedParameterValue<bool>(dynamicParameter);
                                    break;
                                case Constants.OsMetricWmiUser:
                                    result.WmiConfig.DirectWmiUserName = GetTypedParameterValue<string>(dynamicParameter);
                                    result.WmiConfig.DirectWmiConnectAsCollectionService = false;
                                    break;
                                case Constants.OsMetricWmiPassword:
                                    result.WmiConfig.DirectWmiPassword = GetTypedParameterValue<string>(dynamicParameter); ;
                                    result.WmiConfig.DirectWmiConnectAsCollectionService = false;
                                    break;
                                case Constants.OsMetricNotCollect:
                                    result.WmiConfig.OleAutomationDisabled = true;
                                    result.WmiConfig.DirectWmiEnabled = false;
                                    break;

                                #endregion

                                case Constants.QMEnabledParameter:
                                    result.QueryMonitorConfiguration.Enabled = true;
                                    break;
                                case Constants.QMDisabledParameter:
                                    result.QueryMonitorConfiguration.Enabled = false;
                                    break;
                                case Constants.CaptureStatementsParameter:
                                    result.QueryMonitorConfiguration.SqlStatementEventsEnabled =
                                        GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.CaptureBatchesParameter:
                                    result.QueryMonitorConfiguration.SqlBatchEventsEnabled =
                                        GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.CaptureProcsParameter:
                                    result.QueryMonitorConfiguration.StoredProcedureEventsEnabled =
                                        GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.EnableNonQueryActivityParameter:
                                    result.ActivityMonitorConfiguration.Enabled = true;
                                    break;
                                case Constants.DisableNonQueryActivityParameter:
                                    result.ActivityMonitorConfiguration.Enabled = false;
                                    break;
                                //START -- Power shell snapin for remaining Activity Monitor Control for controls
                                //SQLdm10.1 (Srishti Purohit)
                                case Constants.EnableNonQueryActivitySQLTrace:
                                    result.ActivityMonitorConfiguration.TraceMonitoringEnabled = true;
                                    break;
                                case Constants.EnableNonQueryActivityExtendedEvent: //SQLDM-29005
                                    if (result.ServerVersion >= 11 || result.ServerVersion == 0 || result.ServerVersion == null )
                                    {
                                        result.ActivityMonitorConfiguration.TraceMonitoringEnabled = false;
                                    }
                                    else
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "Use extended event for Activity Monitor can be set enable for SQL Servers 2012+ only."));
                                    }
                                    break;
                                case Constants.EnableNonQueryActivityAutoGrow:
                                    result.ActivityMonitorConfiguration.AutogrowEventsEnabled = true;
                                    break;
                                case Constants.DisableNonQueryActivityAutoGrow:
                                    result.ActivityMonitorConfiguration.AutogrowEventsEnabled = false;
                                    break;
                                case Constants.EnableNonQueryActivityCaptureBlocking:
                                    if (result.ServerVersion >= 9)
                                    {
                                        result.ActivityMonitorConfiguration.BlockingEventsEnabled = true;
                                    }
                                    else
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "Capture blocking can be set enable for SQL Servers 2005+ only."));
                                    }
                                    break;
                                case Constants.DisableNonQueryActivityCaptureBlocking:
                                    result.ActivityMonitorConfiguration.BlockingEventsEnabled = false;
                                    break;
                                case Constants.NonQueryActivityCaptureBlocking:
                                    if (result.ServerVersion >= 9)
                                    {
                                        int blockingValue = GetTypedParameterValue<int>(dynamicParameter);
                                        Serialized<Snapshot> blockedProcessThresholdSnapshot = null;
                                        if (blockingValue >= 0 && blockingValue <= 86400)
                                        {
                                            IManagementService defaultManagementService =
                                            DataHelper.ManagementService;

                                            blockedProcessThresholdSnapshot = defaultManagementService.SendBlockedProcessThresholdChange(
                                                new BlockedProcessThresholdConfiguration(instance.Id, blockingValue));
                                            // We need to know if we succeded changing the BlockedProcessThreshold value on the monitored server
                                            if (blockedProcessThresholdSnapshot != null && blockedProcessThresholdSnapshot.Deserialize().Error != null)
                                            {
                                                result.ActivityMonitorConfiguration.BlockedProcessThreshold = blockingValue;
                                                //result.ActivityMonitorConfiguration.BlockingEventsEnabled = true;
                                            }
                                        }
                                        else
                                        {
                                            throw new System.Management.Automation.ParameterBindingException(
                                            String.Format(
                                                "Blocking value {0} out of range (0 to 86400).",
                                                blockingValue));
                                        }
                                    }
                                    else
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "Blocking value can be set for SQL Servers 2005+ only."));
                                    }
                                    break;
                                //END -- Power shell snapin for remaining Activity Monitor Control for controls
                                case Constants.CaptureDeadlocksParameter:
                                    result.ActivityMonitorConfiguration.DeadlockEventsEnabled =
                                        GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.QueryDurationParameter:
                                   result.QueryMonitorConfiguration.DurationFilter = TimeSpan.FromMilliseconds(GetIntParameterValue(dynamicParameter));
                                    //result.QueryMonitorConfiguration.DurationFilter =
                                    //    GetTypedParameterValue<TimeSpan>(dynamicParameter);
                                    break;
                                case Constants.QueryTopPlanCountParameter:
                                    result.QueryMonitorConfiguration.TopPlanCountFilter = GetIntParameterValue(dynamicParameter);
                                    break;
                                case Constants.QueryTopPlanCategoryParameter:
                                    result.QueryMonitorConfiguration.TopPlanCategoryFilter = GetIntParameterValue(dynamicParameter);
                                    break;
                                case Constants.CPUUsageParameter:
                                    // (start) SQLdm 10.1 (Pulkit Puri)
                                    //SQLDM-25399 (10.1) -- Giving Negative value for Existing command getting error popup when trying to open server property
                                    TimeSpan TimeDurationValue = TimeSpan.FromMilliseconds(GetIntParameterValue(dynamicParameter));
                                    if (TimeDurationValue.TotalMilliseconds >= 0)//CPUUsageParameter can be zero also
                                        result.QueryMonitorConfiguration.CpuUsageFilter = TimeDurationValue;
                                       else
                                   {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format("CPU Usage cannaot be negetive"));
                                   }

                                    // (end) SQLdm 10.1 (Pulkit Puri)

                                    //result.QueryMonitorConfiguration.CpuUsageFilter =
                                    //    GetTypedParameterValue<TimeSpan>(dynamicParameter);
                                    break;
                                case Constants.LogicalDiskReadsParameter:
                                    result.QueryMonitorConfiguration.LogicalDiskReads =
                                        GetIntParameterValue(dynamicParameter);
                                    break;
                                case Constants.PhysicalDiskWritesParameter:
                                    result.QueryMonitorConfiguration.PhysicalDiskWrites =
                                        GetIntParameterValue(dynamicParameter);
                                    break;
                                case Constants.ExcludedAppsParameter:
                                    result.QueryMonitorConfiguration.ExcludedApplications =
                                        GetTypedParameterValue<string[]>(dynamicParameter);
                                    break;
                                case Constants.ExcludedDatabasesParameter:
                                    result.QueryMonitorConfiguration.ExcludedDatabases =
                                        GetTypedParameterValue<string[]>(dynamicParameter);
                                    break;
                                case Constants.ExcludedSqlParameter:
                                    result.QueryMonitorConfiguration.ExcludedSqlText =
                                        GetTypedParameterValue<string[]>(dynamicParameter);
                                    break;
                                //START --Power shell snapin for remaining for controls
                                //SQLdm10.1 (Srishti Purohit)
                                case Constants.EnableQueryMonitorTraceMonitoring:
                                    result.QueryMonitorConfiguration.QueryMonitorTraceMonitoringEnabled = true;
                                    break;
                                // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
                                case Constants.EnableQueryMonitorQueryStoreMonitoring:
                                    if (result.ServerVersion >= 13)
                                    {
                                        // SQLDM-29633 (Varun Chopra) Query Monitor default option
                                        result.QueryMonitorConfiguration.QueryMonitorQueryStoreMonitoringEnabled = false;
                                    }
                                    else
                                    {
                                        throw new ParameterBindingException(
                                            "Collect query data using query store can be set enable for SQL Servers 2016+ only.");
                                    }
                                    break;
                                case Constants.EnableQueryMonitorExtendedEvents:
                                    if (result.ServerVersion >= 10)
                                        result.QueryMonitorConfiguration.QueryMonitorTraceMonitoringEnabled = false;
                                    else
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "Collect query data using extended events can be set enable for SQL Servers 2008+ only."));
                                    }
                                    break;
                                case Constants.EnableQueryMonitorCollectQueryPlan:
                                    if (result.ServerVersion >= 10)
                                    {
                                        result.QueryMonitorConfiguration.QueryMonitorCollectQueryPlan = true;
                                        result.QueryMonitorConfiguration.QueryMonitorCollectEstimatedQueryPlan = false;
                                        // SQLDM-25395 (10.1) --disable trace monitoring while enabling the actual query plan
                                        result.QueryMonitorConfiguration.QueryMonitorTraceMonitoringEnabled = false;// SQLdm 10.1 (Pulkit Puri) 

                                        // SQLDM-29633 (Varun Chopra) Query Monitor default option
                                        if (result.ServerVersion >= 13)
                                        {
                                            result.QueryMonitorConfiguration.QueryMonitorQueryStoreMonitoringEnabled = false;
                                        }
                                    }
                                    else
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "Collect actual query plans can be set enable for SQL Servers 2008+ only."));
                                    }

                                    break;
                                case Constants.DisableQueryMonitorCollectQueryPlan:
                                    result.QueryMonitorConfiguration.QueryMonitorCollectQueryPlan = false;
                                    break;
                                case Constants.EnableQueryMonitorCollectEstimatedQueryPlan:
                                    result.QueryMonitorConfiguration.QueryMonitorCollectEstimatedQueryPlan = true;
                                    result.QueryMonitorConfiguration.QueryMonitorCollectQueryPlan = false;
                                    // SQLDM - 25395 (10.1) --disable trace monitoring while enabling the estimated query plan
                                    result.QueryMonitorConfiguration.QueryMonitorTraceMonitoringEnabled = false;// SQLdm 10.1 (Pulkit Puri) 
                                    // SQLDM-29633 (Varun Chopra) Query Monitor default option
                                    if (result.ServerVersion >= 13)
                                    {
                                        result.QueryMonitorConfiguration.QueryMonitorQueryStoreMonitoringEnabled = false;
                                    }
                                    break;
                                case Constants.DisableQueryMonitorCollectEstimatedQueryPlan:
                                    result.QueryMonitorConfiguration.QueryMonitorCollectEstimatedQueryPlan = false;
                                    break;
                                //END --Power shell snapin for remaining for controls
                                case Constants.QTDaysParameter:
                                    result.TableStatisticsCollectionConfiguration.CollectionDays =
                                        (Days)CombineDays(GetObjectParameterValue(dynamicParameter));
                                    break;
                                case Constants.QTStartTimeParameter:
                                    result.TableStatisticsCollectionConfiguration.CollectionStartTime =
                                        GetTypedParameterValue<TimeSpan>(dynamicParameter);
                                    break;
                                case Constants.QTReorgMinTableSizeParameter:
                                    result.TableStatisticsCollectionConfiguration.ReorgMinimumTableSizeK =
                                        GetIntParameterValue(dynamicParameter);
                                    break;
                                case Constants.QTExcludedDatabasesParameter:
                                    string[] databases = GetTypedParameterValue<string[]>(dynamicParameter);
                                    result.TableStatisticsCollectionConfiguration.ExcludedDatabases =
                                        new List<string>(databases);
                                    break;

                                //START SQLdm 10.1 (Srishti Purohit): Powershell Snap-in Added Functions
                                case Constants.FriendlyServerName:
                                    //SQLDM-26052--VALIDATION ON THE LENGTH OF FRIENDLYSERVERNAME (Pulkit Puri)
                                    string tempFriendlyServerName= GetTypedParameterValue<string>(dynamicParameter);
                                    if (tempFriendlyServerName.Length <=Constants.MAX_FRIENDLYSERVERNAME_LENGTH)
                                    {
                                        result.FriendlyServerName = tempFriendlyServerName;
                                    }
                                    else
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "The length of friendly server name cannot be more than {0} characters", Constants.MAX_FRIENDLYSERVERNAME_LENGTH));
                                    }

                                    break;
                                case Constants.FriendlyServerNameBlank:
                                    result.FriendlyServerName = null;
                                    break;
                                case Constants.InputBufferLimiter:
                                    int limiterValue = GetTypedParameterValue<int>(dynamicParameter);
                                    if (limiterValue >= 1 && limiterValue <= 100000)
                                    {
                                        result.InputBufferLimiter = limiterValue;
                                        result.InputBufferLimited = true;
                                    }
                                    else
                                    {
                                        LOG.Error("InputBufferLimiter cant be less than 1 and greater than 100000.");
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "InputBufferLimiter value {0} out of range (1 - 100000).",limiterValue));
                                    }
                                    break;
                                case Constants.InputBufferLimiterEnable:
                                    result.InputBufferLimited = true;
                                    break;
                                case Constants.InputBufferLimiterDisable:
                                    result.InputBufferLimited = false;
                                    break;
                                //START -- Active Wait configuration
                                //case Constants.QWStatisticsEnable:
                                //    if (result.ServerVersion >= 9)
                                //        result.ActiveWaitsConfiguration.Enabled = true;
                                //    break;
                                case Constants.QWStatisticsDisable:
                                    result.ActiveWaitsConfiguration.Enabled = false;
                                    break;
                                case Constants.QWExtendedEnable:
                                    if (result.ServerVersion >= 11)
                                        result.ActiveWaitsConfiguration.EnabledXe = true;
                                    else
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "Use extended event (SQL 2012+) can be set enable for SQL Servers 2012+ only."));
                                    }
                                    break;
                                case Constants.QWExtendedDisable:
                                    result.ActiveWaitsConfiguration.EnabledXe = false;
                                    break;
                                case Constants.QWStatisticsCollectIndefinite:
                                    result.ActiveWaitsConfiguration.Enabled = true;
                                    result.ActiveWaitsConfiguration.RunTime = null;
                                    break;

                                case Constants.QWStatisticsDuration:
                                    result.ActiveWaitsConfiguration.Enabled = true;
                                    if (GetTypedParameterValue<TimeSpan>(dynamicParameter).TotalSeconds > 0)
                                        result.ActiveWaitsConfiguration.RunTime =
                                        GetTypedParameterValue<TimeSpan>(dynamicParameter);
                                    else
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "Query waits statistics duration should be greater than zero."));
                                    }
                                    break;
                                case Constants.QWStatisticsStartDate:
                                    //SQLDM-26044 FIX :the QWStatisticsStartDate should be more than present time
                                    result.ActiveWaitsConfiguration.Enabled = true;
                                    DateTime QueryWaitStatisticsStartDateTime = GetTypedParameterValue<DateTime>(dynamicParameter);
                                    if (QueryWaitStatisticsStartDateTime >= DateTime.Now)
                                        result.ActiveWaitsConfiguration.StartTimeRelative = QueryWaitStatisticsStartDateTime;
                                    else
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                        String.Format(
                                            "Query waits start datetime should be greater than actual time"));
                                    }
                                    break;
                                    //END -- Active Wait configuration
                                    //END SQLdm 10.1 (Srishti Purohit): Powershell Snap-in Added Functions
                            }
                        }
                    }
                    // tag parameters are applied in a fixed order: -Tags then -RemoveTag then -AddTag
                    if (removeTags != null)
                        result.RemoveTags(removeTags);
                    if (addTags != null)
                        result.AddTags(addTags);
                }

                return result;
            }
        }

        private SQLdmUserInfo UpdateSQLdmUserWithDynamicParameters(SQLdmDriveInfo drive, SQLdmUserInfo user)
        {
            using (LOG.InfoCall("UpdateSQLdmUserWithDynamicParameters"))
            {
                SQLdmUserInfo result = (SQLdmUserInfo)user.Clone();

                RuntimeDefinedParameterDictionary dynamicParameters =
                    DynamicParameters as RuntimeDefinedParameterDictionary;
                if (dynamicParameters != null)
                {
                    foreach (RuntimeDefinedParameter dynamicParameter in dynamicParameters.Values)
                    {
                        if (dynamicParameter.IsSet)
                        {
                            switch (dynamicParameter.Name)
                            {
                                case Constants.LoginTypeWindowsUserParameter:
                                    result.LoginType = LoginType.WindowsUser;
                                    break;
                                case Constants.LoginTypeWindowsGroupParameter:
                                    result.LoginType = LoginType.WindowsGroup;
                                    break;
                                case Constants.LoginTypeSqlParameter:
                                    result.LoginType = LoginType.SQLLogin;
                                    break;
                                case Constants.PasswordParameter:
                                    result.Password = GetTypedParameterValue<string>(dynamicParameter);
                                    break;
                                case Constants.AdmininstratorParameter:
                                    result.IsAdministrator = GetBooleanParameterValue(dynamicParameter);
                                    break;
                                case Constants.CommentParameter:
                                    result.Comments = GetTypedParameterValue<string>(dynamicParameter);
                                    break;
                            }
                        }
                    }
                }
                return result;
            }
        }


        private Days CombineDays(object days)
        {
            Days result = new Days();

            if (days is Days)
                return (Days)days;
            if (days is Days[])
            {
                foreach (Days day in (Days[])days)
                {
                    result |= day;
                }
            }
            return result;
        }

        private static T GetTypedParameterValue<T>(RuntimeDefinedParameter parameter)
        {
            if (parameter.ParameterType == typeof(T))
                return (T)parameter.Value;

            return (T)Convert.ChangeType(parameter.Value, typeof(T));
        }

        private static object GetObjectParameterValue(RuntimeDefinedParameter dynamicParameter)
        {
            return dynamicParameter.Value;
        }

        private static int GetIntParameterValue(RuntimeDefinedParameter parameter)
        {
            if (parameter.ParameterType == typeof(int))
                return (int)parameter.Value;

            return Convert.ToInt32(parameter.Value);
        }

        private static bool GetBooleanParameterValue(RuntimeDefinedParameter parameter)
        {
            bool result = false;

            object value = parameter.Value;
            if (value != null)
            {
                if (value is SwitchParameter)
                    result = ((SwitchParameter)value).ToBool();
                else if (value is bool)
                    result = (bool)value;
            }

            return result;
        }

        /// <summary>
        /// Parameter set names don't work very well with dynamic parameters.  The RuntimeParameterBuilder will
        /// add a custom attribute to the dynamic parameters that contains the parameter set name.  This method 
        /// will read the dynamic parameters and validate that if a parameter was specified that belongs to a 
        /// parameter set that only one parameter set is selected.
        /// </summary>
        private void ValidateDynamicParameters()
        {
            using (LOG.InfoCall("ValidateDynamicParameters"))
            {
                Dictionary<string, Set<string>> parameterSetMap = new Dictionary<string, Set<string>>();
                MultiDictionary<string, string> parameterSets = new MultiDictionary<string, string>(false);
                MultiDictionary<string, string> requiredParameterSets = new MultiDictionary<string, string>(false);


                RuntimeDefinedParameterDictionary dynamicParameters =
                    DynamicParameters as RuntimeDefinedParameterDictionary;
                if (dynamicParameters != null)
                {
                    foreach (RuntimeDefinedParameter rdp in dynamicParameters.Values)
                    {
                        List<DynamicParameterAttribute> rdpaList = new List<DynamicParameterAttribute>();

                        foreach (Attribute rdpa in rdp.Attributes)
                        {
                            if (rdpa is DynamicParameterAttribute)
                            {
                                rdpaList.Add((DynamicParameterAttribute)rdpa);
                                if (((DynamicParameterAttribute)rdpa).Required)
                                {
                                    foreach (string psn in ((DynamicParameterAttribute)rdpa).ParameterSetNames)
                                    {
                                        string[] psnparts = psn.Split('.');
                                        string qual = psnparts.Length > 1 ? psnparts[0] : String.Empty;
                                        requiredParameterSets.Add(qual, rdp.Name);
                                    }
                                }
                            }
                        }

                        if (rdp.IsSet)
                        {
                            parameterSets.Clear();
                            foreach (DynamicParameterAttribute rdpa in rdpaList)
                            {
                                foreach (string psn in ((DynamicParameterAttribute)rdpa).ParameterSetNames)
                                {
                                    string[] psnparts = psn.Split('.');
                                    string qual = psnparts.Length > 1 ? psnparts[0] : String.Empty;
                                    string setname = psnparts.Length > 1 ? psnparts[1] : psnparts[0];
                                    parameterSets.Add(qual, setname);
                                }
                            }

                            if (parameterSets.Count > 0)
                            {
                                Set<string> set = null;

                                foreach (string qual in parameterSets.Keys)
                                {
                                    Set<string> setNames = new Set<string>(parameterSets[qual]);
                                    if (parameterSetMap.TryGetValue(qual, out set))
                                    {
                                        set.IntersectionWith(setNames);
                                    }
                                    else
                                    {
                                        set = setNames;
                                        parameterSetMap.Add(qual, set);
                                    }
                                    if (set.Count == 0)
                                    {
                                        throw new System.Management.Automation.ParameterBindingException(
                                            String.Format(
                                                "Parameter '{0}' can not be used with one or more of the other parameters specified.",
                                                rdp.Name));
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (Set<string> set in parameterSetMap.Values)
                {
                    if (set.Count > 1)
                        throw new System.Management.Automation.ParameterBindingException(
                            String.Format("Parameters belonging to multiple parameter sets were selected ({0}).", set));
                }

                foreach (string qual in requiredParameterSets.Keys)
                {
                    bool qualSet = false;
                    foreach (string parameterName in requiredParameterSets[qual])
                    {
                        RuntimeDefinedParameter rdp;
                        if (dynamicParameters.TryGetValue(parameterName, out rdp))
                        {
                            if (rdp.IsSet)
                            {
                                qualSet = true;
                                break;
                            }
                        }
                    }
                    if (!qualSet)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("At least one of the following parameters must be specified (");
                        int mark = builder.Length;
                        foreach (string parameterName in requiredParameterSets[qual])
                        {
                            if (builder.Length > mark)
                                builder.Append(", ");
                            builder.Append(parameterName);
                        }
                        builder.Append(")");
                        throw new System.Management.Automation.ParameterBindingException(builder.ToString());
                    }
                }
            }
        }

        public RuntimeDefinedParameterDictionary GetDynamicParameters(string verb, string path)
        {
            using (LOG.InfoCall("GetDynamicParameters"))
            {
                switch (verb)
                {
                    case VerbsCommon.New:
                    case VerbsSecurity.Grant:
                        return NewItemDynamicParameters(path, null, null) as RuntimeDefinedParameterDictionary;
                    case VerbsCommon.Remove:
                    case VerbsSecurity.Revoke:
                        return RemoveItemDynamicParameters(path, false) as RuntimeDefinedParameterDictionary;
                    case VerbsCommon.Set:
                        return SetItemDynamicParameters(path, null) as RuntimeDefinedParameterDictionary;
                }
                return null;
            }
        }

        /// <summary>
        /// Allows the provider to attach additional parameters to the set-item
        /// cmdlet.
        /// </summary>
        /// 
        /// <param name="path">
        /// If the path was specified on the command line, this is the path to
        /// the item to get the dynamic parameters for.
        /// </param>
        /// 
        /// <param name="value">
        /// The value of the item specified by the path.
        /// </param>
        /// 
        /// <returns>
        /// Overrides of this method should return an object that has 
        /// properties and fields decorated with parsing attributes similar to
        /// a cmdlet class or a 
        /// <see cref="System.Management.Automation.PseudoParameterDictionary"/>.
        /// 
        /// The default implemenation returns null. 
        /// </returns>
        protected override object SetItemDynamicParameters(string path, object value)
        {
            using (LOG.InfoCall("SetItemDynamicParameters"))
            {
                RuntimeParameterBuilder parms = new RuntimeParameterBuilder();

                string[] pathChunks = NormalizeAndChunkPath(path);
                switch (GetPathType(pathChunks))
                {
                    case PathType.Instance:
                        // Instance
                        parms.AddParameter<int>(Constants.ScheduledCollectionIntervalParameter, false, null);
                        parms.AddParameter<int>(Constants.DatabaseStatisticsParameter, false, null);
                        //SQLDM-30516
                        parms.AddParameter<int>(Constants.ServerInAccessibleRateParameter, false, null);

                        //                 parms.AddParameter<PSCredential>(Constants.CredentialParameter, false, "Credential.SQLLogin");
                        parms.AddParameter<SwitchParameter>(Constants.WindowsAuthenticationParameter, false, null);
                        parms.AddParameter<bool>(Constants.EncryptParameter, false, null);
                        parms.AddParameter<bool>(Constants.TrustCertsParameter, false, null);
                        parms.AddParameter<bool>(Constants.SetCollectExtendedSessionDataParameter, false, null);
                        parms.AddParameter<bool>(Constants.SetCollectReplicationDataParameter, false, null);
                        parms.AddParameter<bool>(Constants.DisableOleAutomationUseParameter, false, null);
                        parms.AddParameter<string[]>(Constants.TagsParameter, false, null);
                        parms.AddParameter<string[]>(Constants.AddTagParameter, false, null);
                        parms.AddParameter<string[]>(Constants.RemoveTagParameter, false, null);

                        // Maintenance Mode
                        parms.AddParameter<SwitchParameter>(Constants.MModeNeverParameter, false, "MM.MMNever");
                        parms.AddParameter<SwitchParameter>(Constants.MModeAlwaysParameter, false, "MM.MMAlways");
                        parms.AddParameter<SwitchParameter>(Constants.MModeOnceParameter, false, "MM.MMOnce");
                        parms.AddParameter<SwitchParameter>(Constants.MModeRecurringParameter, false, "MM.MMRecurring");
                        parms.AddParameter<Days[]>(Constants.MModeDaysParameter, false, "MM.MMRecurring");
                        parms.AddParameter<TimeSpan>(Constants.MModeStartTimeParameter, false, "MM.MMRecurring");
                        parms.AddParameter<TimeSpan>(Constants.MModeDurationParameter, false, "MM.MMRecurring");
                        parms.AddParameter<DateTime>(Constants.MModeStartDateParameter, false, "MM.MMOnce");
                        parms.AddParameter<DateTime>(Constants.MModeEndDateParameter, false, "MM.MMOnce");

                        // Query Monitor
                        parms.AddParameter<SwitchParameter>(Constants.QMEnabledParameter, false, "QM.QMEnabled");
                        parms.AddParameter<SwitchParameter>(Constants.QMDisabledParameter, false, "QM.QMDisabled");
                        parms.AddParameter<bool>(Constants.CaptureStatementsParameter, false, "QM.QMEnabled");
                        parms.AddParameter<bool>(Constants.CaptureBatchesParameter, false, "QM.QMEnabled");
                        parms.AddParameter<bool>(Constants.CaptureProcsParameter, false, "QM.QMEnabled");

                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivityParameter, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableNonQueryActivityParameter, false, "QM.NQADisable");
                        parms.AddParameter<bool>(Constants.CaptureDeadlocksParameter, false, "QM.NQAEnable");

                        //Power shell snapin for remaining Activity Monitor controls
                        //SQLdm10.1 (Srishti Purohit)
                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivitySQLTrace, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivityExtendedEvent, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivityAutoGrow, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableNonQueryActivityAutoGrow, false, "QM.NQADisable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivityCaptureBlocking, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableNonQueryActivityCaptureBlocking, false, "QM.NQADisable");
                        parms.AddParameter<int>(Constants.NonQueryActivityCaptureBlocking, false, "QM.NQADisable");

                        parms.AddParameter<int>(Constants.QueryDurationParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.QueryTopPlanCountParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.QueryTopPlanCategoryParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.CPUUsageParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.LogicalDiskReadsParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.PhysicalDiskWritesParameter, false, "QM.QMEnabled");
                        parms.AddParameter<string[]>(Constants.ExcludedAppsParameter, false, "QM.QMEnabled");
                        parms.AddParameter<string[]>(Constants.ExcludedDatabasesParameter, false, "QM.QMEnabled");
                        parms.AddParameter<string[]>(Constants.ExcludedSqlParameter, false, "QM.QMEnabled");

                        parms.AddParameter<Days[]>(Constants.QTDaysParameter, false, null);
                        parms.AddParameter<TimeSpan>(Constants.QTStartTimeParameter, false, null);
                        parms.AddParameter<int>(Constants.QTReorgMinTableSizeParameter, false, null);
                        parms.AddParameter<string[]>(Constants.QTExcludedDatabasesParameter, false, null);

                        //Power shell snapin for remaining for controls
                        //SQLdm10.1 (Srishti Purohit)
                        parms.AddParameter<SwitchParameter>(Constants.EnableQueryMonitorTraceMonitoring, false, "QM.QMTMEnable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableQueryMonitorExtendedEvents, false, "QM.QMTMEnable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableQueryMonitorCollectQueryPlan, false, "QM.QMTMEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableQueryMonitorCollectQueryPlan, false, "QM.QMTMDisable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableQueryMonitorCollectEstimatedQueryPlan, false, "QM.QMTMEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableQueryMonitorCollectEstimatedQueryPlan, false, "QM.QMTMDisable");

                        //Os Metrics
                        parms.AddParameter<SwitchParameter>(Constants.OsMetricCollectByOleParameter, false, "OM.OMEnabled");
                        parms.AddParameter<SwitchParameter>(Constants.OsMetricCollectByWmiParameter, false, "OM.OMEnabled");
                        parms.AddParameter<bool>(Constants.OsMetricConnectAsCollectionService, false, "OM.OMEnabled");
                        parms.AddParameter<SwitchParameter>(Constants.OsMetricNotCollect, false, "OM.OMEnabled");
                        parms.AddParameter<string>(Constants.OsMetricWmiUser, false, "OM.OMEnabled");
                        parms.AddParameter<string>(Constants.OsMetricWmiPassword, false, "OM.OMEnabled");

                        //general settings configuration
                        parms.AddParameter<string>(Constants.FriendlyServerName, false, "FriendlyServerName");
                        parms.AddParameter<SwitchParameter>(Constants.FriendlyServerNameBlank, false, "FriendlyServerNameBlank");
                        parms.AddParameter<int>(Constants.InputBufferLimiter, false, "InputBufferLimiter");
                        parms.AddParameter<SwitchParameter>(Constants.InputBufferLimiterEnable, false, "InputBufferLimiterEnable");
                        parms.AddParameter<SwitchParameter>(Constants.InputBufferLimiterDisable, false, "InputBufferLimiterDisable");

                        //Active Wait Configuration
                        //parms.AddParameter<SwitchParameter>(Constants.QWStatisticsEnable, false, "QW.QWStatisticsEnabled");
                        parms.AddParameter<SwitchParameter>(Constants.QWStatisticsDisable, false, "QW.QWStatisticsDisabled");
                        parms.AddParameter<SwitchParameter>(Constants.QWExtendedEnable, false, "QW.QWExtendedEnable");
                        parms.AddParameter<SwitchParameter>(Constants.QWExtendedDisable, false, "QW.QWExtendedDisable");
                        parms.AddParameter<SwitchParameter>(Constants.QWStatisticsCollectIndefinite, false, "QW.QWStatisticsEnabled");
                        parms.AddParameter<DateTime>(Constants.QWStatisticsStartDate, false, "QW.QWStatisticsEnabled");
                        parms.AddParameter<TimeSpan>(Constants.QWStatisticsDuration, false, "QW.QWStatisticsEnabled");
                        break;
                    case PathType.AppSecurity:
                        parms.AddParameter<SwitchParameter>(Constants.EnabledParameter, false, true,
                                                            "AppSecurity.Enabled");
                        parms.AddParameter<SwitchParameter>(Constants.DisabledParameter, false, true,
                                                            "AppSecurity.Disabled");
                        break;
                    case PathType.AppUser:
                        parms.AddParameter<SwitchParameter>(Constants.EnabledParameter, false, "AppUser.Enabled");
                        parms.AddParameter<SwitchParameter>(Constants.DisabledParameter, false, "AppUser.Disabled");
                        parms.AddParameter<bool>(Constants.AdmininstratorParameter, false, "AppUser.Enabled");
                        parms.AddParameter<string>(Constants.CommentParameter, false, "AppUser.Enabled");
                        break;
                    case PathType.AppUserPermission:
                        RuntimeDefinedParameter p =
                            parms.AddParameter<PermissionType>(Constants.PermissionParameter, true, null);
                        p.Attributes.Add(new ValidateSetAttribute("View", "Modify"));
                        break;
                    case PathType.CustomCounter:
                        break;
                }

                return parms.Parameters;
            }
        }

        /// <summary>
        /// Gets the children of the item at the specified path.
        /// </summary>
        /// 
        /// <param name="path">
        /// The path (or name in a flat namespace) to the item from which to
        /// retrieve the children.
        /// </param>
        /// 
        /// <param name="recurse">
        /// True if all children in a subtree should be retrieved, false if
        /// only a single level of children should be retrieved.  
        /// </param>
        /// 
        /// <remarks>
        /// Providers override this method to give the user access to the 
        /// provider objects using the get-childitem cmdlets.
        /// 
        /// Providers that declare 
        /// <see cref="System.Management.Automation.Provider.ProviderCapabilities"/>
        /// of ExpandWildcards, Filter, Include, or Exclude should ensure that 
        /// the path passed meets those requirements by accessing the 
        /// appropriate property from the base class.
        /// 
        /// By default, overrides of this method should not write objects that 
        /// are generally hidden from the user unless the Force property is set
        /// to true. For instance, the FileSystem provider should
        /// not call WriteItemObject for hidden or system files unless the Force
        /// property is set to true. 
        /// 
        /// The provider implementation is responsible for preventing infinite 
        /// recursion when there are circular links and the like. An 
        /// appropriate terminating exception should be thrown if this
        /// situation occurs.
        /// </remarks>
        protected override void GetChildItems(string path, bool recurse)
        {
            using (LOG.InfoCall("GetChildItems"))
            {
                SQLdmDriveInfo driveInfo = GetSQLdmDriveInfo(path);
                UserToken userToken = driveInfo.UserToken;

                string[] pathChunks = NormalizeAndChunkPath(path);
                PathType pathType = GetPathType(pathChunks);
                switch (pathType)
                {
                    case PathType.Root:
                        StaticContainerInfo node = new InstancesInfo(driveInfo);
                        WriteToPipeline(node, path + pathSeparator + node.Name, true);
                        if (recurse)
                            GetChildItems(path + pathSeparator + node.Name, recurse);
                        node = new AppSecurityInfo(driveInfo);
                        WriteToPipeline(node, path + pathSeparator + node.Name, true);
                        if (recurse)
                            GetChildItems(path + pathSeparator + node.Name, recurse);
                        /*
                                            node = new CustomCountersInfo(driveInfo);
                                            WriteItemObject(node, path + pathSeparator + node.Name, true);
                                            if (recurse)
                                                GetChildItems(path + pathSeparator + node.Name, recurse);
                        */
                        break;
                    case PathType.Instances:
                        IList<MonitoredSqlServerInfo> servers = null;
                        using (ProgressProvider progress = new ProgressProvider(this, "Get Monitored Instances"))
                        {
                            servers = new InstancesInfo(driveInfo).GetInstances(progress);
                        }
                        foreach (MonitoredSqlServerInfo instance in servers)
                        {
                            string instanceName = EscapeString(instance.InstanceName);
                            WriteToPipeline(instance, path + pathSeparator + instanceName, true);
                        }
                        break;
                    case PathType.Instance:
                        node = new AlertsInfo(driveInfo, UnescapeString(pathChunks[1]));
                        WriteToPipeline(node, path + pathSeparator + EscapeString(node.Name), true);
                        break;
                    case PathType.Alerts:
                        string name = UnescapeString(pathChunks[1]);
                        if (GetInstancePermission(userToken, name) == PermissionType.None)
                        {
                            WriteError(new ErrorRecord(new SecurityException(
                                                           String.Format(
                                                               "You do not have permissions to the instance {0}",
                                                               name)),
                                                       "PermissionDenied", ErrorCategory.PermissionDenied, name));
                            return;
                        }
                        foreach (AlertInfo alert in Helper.GetActiveAlerts(driveInfo, name))
                        {
                            WriteToPipeline(alert, path + pathSeparator + alert.AlertID.ToString(), false);
                        }
                        break;
                    case PathType.AppSecurity:
                        {
                            if (userToken.IsSecurityEnabled)
                            {
                                if (!(userToken.IsSysadmin || userToken.IsSQLdmAdministrator))
                                {
                                    WriteError(
                                        new ErrorRecord(
                                            new SecurityException("You do not have permissions to view AppSecurity."),
                                            "PermissionDenied", ErrorCategory.PermissionDenied, "AppSecurity"));
                                    return;
                                }
                            }

                            SortedDictionary<string, SQLdmUserInfo> users =
                                new SortedDictionary<string, SQLdmUserInfo>();
                            foreach (PermissionDefinition permission in
                                driveInfo.GetAppSecurityConfiguration().Permissions.Values)
                            {
                                SQLdmUserInfo user;
                                if (users.TryGetValue(permission.Login, out user))
                                {
                                    user.AddPermissions(permission);
                                }
                                else
                                {
                                    users.Add(permission.Login, new SQLdmUserInfo(permission));
                                }
                            }
                            foreach (SQLdmUserInfo user in users.Values)
                            {
                                string userName = EscapeString(user.Login);
                                WriteToPipeline(user, path + pathSeparator + userName, !user.IsAdministrator);
                            }
                        }
                        break;
                    case PathType.AppUser:
                        {
                            if (userToken.IsSecurityEnabled)
                            {
                                if (!(userToken.IsSysadmin || userToken.IsSQLdmAdministrator))
                                {
                                    WriteError(
                                        new ErrorRecord(
                                            new SecurityException("You do not have permissions to view AppSecurity."),
                                            "PermissionDenied", ErrorCategory.PermissionDenied, "AppSecurity"));
                                    return;
                                }
                            }

                            string userName = EscapeString(pathChunks[1]);
                            SQLdmUserInfo user = Helper.GetSQLdmUserInfo(driveInfo, userName);
                            if (user == null)
                                return;

                            foreach (SQLdmPermissionInfo permission in user.Permissions)
                            {
                                string instanceName = EscapeString(permission.InstanceName);
                                WriteToPipeline(permission, path + pathSeparator + instanceName, false);
                            }
                        }
                        break;

                    case PathType.CustomCounters:
                        foreach (CustomCounterInfo cc in Helper.GetCustomCounters(driveInfo, null))
                        {
                            string instaceName = EscapeString(cc.Name);
                            WriteToPipeline(cc, path + pathSeparator + instaceName, false);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Gets names of the children of the specified path.
        /// </summary>
        /// 
        /// <param name="path">
        /// The path to the item from which to retrieve the child names.
        /// </param>
        /// 
        /// <param name="returnAllContainers">
        /// If true, the provider should return all containers, even if they
        /// don't match the filter.  If false, the provider should only return
        /// containers that do match the filter.
        /// </param>
        /// 
        /// <remarks>
        /// Providers override this method to give the user access to the 
        /// provider objects using the get-childitem -name cmdlet.
        /// 
        /// Providers that declare 
        /// <see cref="System.Management.Automation.Provider.ProviderCapabilities"/>
        /// of ExpandWildcards, Filter, Include, or Exclude should ensure that
        /// the path passed meets those requirements by accessing the 
        /// appropriate property from the base class. The exception to this is
        /// if <paramref name="returnAllContainers"/> is true, then any child 
        /// name for a container should be returned even if it doesn't match 
        /// the Filter, Include, or Exclude.
        /// 
        /// By default overrides of this method should not write the names of
        /// objects that are generally hidden from the user unless the Force 
        /// property is set to true. For instance, the FileSystem provider 
        /// should not call WriteItemObject for hidden or system files unless 
        /// the Force property is set to true. 
        /// 
        /// The provider implementation is responsible for preventing infinite 
        /// recursion when there are circular links and the like. An 
        /// appropriate terminating exception should be thrown if this
        /// situation occurs.
        /// 
        /// The child names are the leaf portion of the path. Example, for the
        /// file system the name for the path c:\windows\system32\foo.dll would
        /// be foo.dll or for the directory c:\windows\system32 would be
        /// system32.  For Active Directory the child names would be RDN values
        /// of the child objects of the container.
        /// 
        /// All names should be written to the WriteItemObject method.
        /// </remarks>
        protected override void GetChildNames(string path, ReturnContainers returnAllContainers)
        {
            using (LOG.InfoCall("GetChildNames"))
            {
                SQLdmDriveInfo driveInfo = GetSQLdmDriveInfo(path);
                UserToken userToken = driveInfo.UserToken;

                if (String.IsNullOrEmpty(path))
                    path = driveInfo.Root;

                string[] pathChunks = NormalizeAndChunkPath(RemoveDriveFromPath(path));
                PathType pathType = GetPathType(pathChunks);
                switch (pathType)
                {
                    case PathType.Root:
                        WriteToPipeline(InstancesInfo.ContainerName, path + pathSeparator + InstancesInfo.ContainerName,
                                        true);
                        //                   WriteItemObject("CustomCounters", path, true);
                        WriteToPipeline(AppSecurityInfo.ContainerName,
                                        path + pathSeparator + AppSecurityInfo.ContainerName, true);
                        break;
                    case PathType.Instances:
                        foreach (MonitoredSqlServerInfo instance in new InstancesInfo(driveInfo).Instances)
                        {
                            WriteToPipeline(instance.InstanceName,
                                            path + pathSeparator + EscapeString(instance.InstanceName), true);
                        }
                        break;
                    case PathType.Instance:
                        WriteToPipeline(AlertsInfo.ContainerName, path + pathSeparator + AlertsInfo.ContainerName, true);
                        break;
                    case PathType.Alerts:
                        foreach (AlertInfo alert in Helper.GetActiveAlerts(driveInfo, pathChunks[1]))
                        {
                            WriteToPipeline(alert.AlertID, path + pathSeparator + alert.AlertID, true);
                        }
                        break;
                    case PathType.AppSecurity:
                        if (userToken.IsSecurityEnabled)
                        {
                            if (!(userToken.IsSysadmin || userToken.IsSQLdmAdministrator))
                            {
                                WriteError(
                                    new ErrorRecord(
                                        new SecurityException("You do not have permissions to view AppSecurity."),
                                        "PermissionDenied", ErrorCategory.PermissionDenied, "AppSecurity"));
                                return;
                            }
                        }

                        foreach (
                            PermissionDefinition permission in
                                driveInfo.GetAppSecurityConfiguration().Permissions.Values)
                        {
                            string instanceName = EscapeString(permission.Login);
                            WriteToPipeline(instanceName, path + pathSeparator + instanceName, false);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Creates a new item at the specified path.
        /// </summary>
        /// 
        /// <param name="path">
        /// The path to the item to create.
        /// </param>
        /// 
        /// <param name="type">
        /// The provider defined type for the object to create.
        /// </param>
        /// 
        /// <param name="newItemValue">
        /// This is a provider specific type that the provider can use to
        /// create a new instance of an item at the specified path.
        /// </param>
        /// 
        /// <remarks>
        /// Providers override this method to give the user the ability to 
        /// create new provider objects using the new-item cmdlet.
        /// 
        /// The <paramref name="type"/> parameter is a provider specific 
        /// string that the user specifies to tell the provider what type of 
        /// object to create.  For instance, in the FileSystem provider the 
        /// <paramref name="type"/> parameter can take a value of "file" or 
        /// "directory". The comparison of this string should be 
        /// case-insensitive and you should also allow for least ambiguous 
        /// matches. So if the provider allows for the types "file" and 
        /// "directory", only the first letter is required to disambiguate.
        /// If <paramref name="type"/> refers to a type the provider cannot 
        /// create, the provider should produce an 
        /// <see cref="ArgumentException"/> with a message indicating the 
        /// types the provider can create.
        /// 
        /// The <paramref name="newItemValue"/> parameter can be any type of 
        /// object that the provider can use to create the item. It is 
        /// recommended that the provider accept at a minimum strings, and an 
        /// instance of the type of object that would be returned from 
        /// GetItem() for this path. 
        /// <see cref="LanguagePrimitives.ConvertTo(System.Object, System.Type)"/>
        /// can be used to convert some types to the desired type.
        /// 
        /// This method should call 
        /// <see cref="CmdletProvider.ShouldProcess"/>
        /// and check its return value before making any changes to the store 
        /// this provider is working upon.
        /// </remarks>
        protected override void NewItem(string path, string type, object newItemValue)
        {
            using (LOG.InfoCall("NewItem"))
            {
                string[] chunks = NormalizeAndChunkPath(path);
                PathType pathType = GetPathType(chunks);

                try
                {
                    ValidateDynamicParameters();
                }
                catch (ParameterBindingException pie)
                {
                    WriteError(pie.ErrorRecord);
                    return;
                }

                switch (pathType)
                {
                    case PathType.Root:
                    case PathType.Instances:
                    case PathType.CustomCounters:
                    case PathType.AppSecurity:
                    case PathType.Alerts:
                        WriteError(
                            new ErrorRecord(
                                new NotImplementedException(
                                    String.Format("Creating of static container objects ({0}) is not allowed.", pathType)),
                                "NotImplemented", ErrorCategory.NotImplemented, path));
                        break;
                    case PathType.Instance:
                        NewInstanceItem(path, UnescapeString(chunks[1]), newItemValue);
                        break;
                    case PathType.CustomCounter:
                        NewCustomCounterItem(path, UnescapeString(chunks[1]), newItemValue);
                        break;
                    case PathType.AppUser:
                        NewAppUserItem(path, UnescapeString(chunks[1]), newItemValue);
                        break;
                    case PathType.AppUserPermission:
                        if (type == "PermissionViaTags")
                            NewAppUserPermissionViaTag(path, UnescapeString(chunks[1]), UnescapeString(chunks[2]), newItemValue);
                        else
                            NewAppUserPermission(path, UnescapeString(chunks[1]), UnescapeString(chunks[2]), newItemValue);
                        break;
                    default:
                        WriteError(new ErrorRecord
                                    (new InvalidOperationException(
                                        String.Format("Path '{0}' does not appear to be valid.  (Remember to escape special characters used in path names)", path)),
                                        "ObjectNotFound", ErrorCategory.ObjectNotFound, path)
                                  );
                        return;
                }
            }
        }

        private void NewAppUserPermissionViaTag(string path, string userName, string tagName, object newItemValue)
        {
            using (LOG.InfoCall("NewAppUserPermissionViaTag"))
            {
                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);

                // see if user authz to change instance
                UserToken token = drive.UserToken;
                PermissionType permissions = (!token.IsSecurityEnabled || token.IsSQLdmAdministrator)
                                                 ? PermissionType.Administrator
                                                 : PermissionType.None;
                if (permissions != PermissionType.Administrator)
                {
                    WriteError(new ErrorRecord
                                   (new PSSecurityException("You are not authorized to add new instances"),
                                    "SecurityException", ErrorCategory.SecurityError, userName)
                        );
                    return;
                }

                SQLdmUserInfo user = Helper.GetSQLdmUserInfo(drive, userName);
                if (user == null)
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException("User does not exist"),
                                    "ObjectNotFound", ErrorCategory.ObjectNotFound, userName)
                        );
                    return;
                }
                if (user.IsSystem)
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException("You are not allowed to modify system admin users"),
                                    "InvalidOperation", ErrorCategory.InvalidOperation, userName)
                        );
                    return;
                }
                if (user.IsAdministrator)
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException(
                                        "You are not allowed to grant permissions to administrators"),
                                    "InvalidOperation", ErrorCategory.InvalidOperation, userName)
                        );
                    return;
                }

                Tag tag;
                if (!drive.TagsByName.TryGetValue(tagName.ToLower(), out tag))
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException("Tag does not exist"),
                                    "ObjectNotFound", ErrorCategory.ObjectNotFound, tagName)
                        );
                    return;
                }

                RuntimeDefinedParameterDictionary dynamicParms = DynamicParameters as RuntimeDefinedParameterDictionary;
                // get the permission type to set
                PermissionType permissionType =
                    Helper.GetDynamicParameterValue(dynamicParms, Constants.PermissionParameter, PermissionType.View);

                // find a permission definition that has this permission
                PermissionDefinition grantpd = null;
                PermissionDefinition[] revokepd = null;

                bool grantNeeded =
                    Helper.GetGrantPermissionModifications(drive, userName, tag, permissionType, out grantpd,
                                                           out revokepd);

                if (grantpd != null)
                {
                    Triple<bool, IList<int>, IList<int>> pts = Helper.GetPermissionTagsAndServers(drive, grantpd.PermissionID);

                    // get current set of tags
                    Set<int> tags = Helper.GetTagsForPermissionId(drive, grantpd.PermissionID);
                    // add new tag to current set
                    tags.Add(tag.Id);

                    // update permission
                    drive.SetContextDataChangeLog(drive);
                    DataHelper.ManagementService.EditPermission(
                        grantpd.PermissionID,
                        grantpd.Enabled,
                        grantpd.PermissionType,
                        tags.ToArray(),
                        Algorithms.ToArray(pts.Third),
                        grantpd.Comment,
                        grantpd.LoginType != LoginType.SQLLogin ? grantpd.WebAppPermission : false);
                }
                else if (grantNeeded)
                {
                    // adding a new permission
                    drive.SetContextDataChangeLog(drive);
                    DataHelper.ManagementService.AddPermission(
                        user.Login,
                        user.LoginType == LoginType.SQLLogin,
                        user.Password,
                        permissionType,
                        new int[] { tag.Id },
                        new int[0],
                        String.Empty,
                        user.LoginType != LoginType.SQLLogin ? user.WebAppPermission : false);
                }
                if (revokepd != null)
                {
                    foreach (PermissionDefinition pd in revokepd)
                    {
                        Triple<bool, IList<int>, IList<int>> pts = Helper.GetPermissionTagsAndServers(drive, grantpd.PermissionID);

                        // get current set of tags
                        Set<int> tags = Helper.GetTagsForPermissionId(drive, pd.PermissionID);
                        // remove tag from current set
                        tags.Remove(tag.Id);

                        // update permission
                        drive.SetContextDataChangeLog(drive);//set context data for changeLog
                        DataHelper.ManagementService.EditPermission(
                            pd.PermissionID,
                            pd.Enabled,
                            pd.PermissionType,
                            tags.ToArray(),
                            Algorithms.ToArray(pts.Third),
                            pd.Comment,
                            pd.LoginType != LoginType.SQLLogin ? pd.WebAppPermission : false);
                    }
                }
                // retrieve the tag again
                drive.SetTagsDirty();
                drive.TagsByName.TryGetValue(tagName.ToLower(), out tag);

                drive.SetSecurityConfigurationDirty();
                //                Configuration config = drive.GetAppSecurityConfiguration();
                //
                //                user = Helper.GetSQLdmUserInfo(drive, userName);
                //                Set<string> affectedServers = new Set<string>();
                //                foreach (PermissionDefinition pd in config.Permissions.Values)
                //                {
                //                    if (pd.Enabled && tag.Permissions.Contains(pd.PermissionID)) 
                //                    {
                //                        foreach (Server server in pd.GetServerList())
                //                            affectedServers.Add(server.InstanceName);
                //                    }
                //                }
                //
                //                foreach (SQLdmPermissionInfo pi in user.Permissions)
                //                {
                //                    if (pi.Permission == permissionType && affectedServers.Contains(pi.InstanceName))
                //                        WriteToPipeline(pi,
                //                                        pathSeparator +
                //                                        AppSecurityInfo.ContainerName +
                //                                        pathSeparator +
                //                                        EscapeString(userName) +
                //                                        pathSeparator +
                //                                        EscapeString(pi.InstanceName),
                //                                        false);
                //                }
            }
        }

        private void NewAppUserPermission(string path, string userName, string instanceName, object newItemValue)
        {
            using (LOG.InfoCall("NewAppUserPermission"))
            {
                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);

                // see if user authz to change instance
                UserToken token = drive.UserToken;
                PermissionType permissions = (!token.IsSecurityEnabled || token.IsSQLdmAdministrator)
                                                 ? PermissionType.Administrator
                                                 : PermissionType.None;
                if (permissions != PermissionType.Administrator)
                {
                    WriteError(new ErrorRecord
                                   (new PSSecurityException("You are not authorized to add new instances"),
                                    "SecurityException", ErrorCategory.SecurityError, userName)
                        );
                    return;
                }

                SQLdmUserInfo user = Helper.GetSQLdmUserInfo(drive, userName);
                if (user == null)
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException("User does not exist"),
                                    "ObjectNotFound", ErrorCategory.ObjectNotFound, userName)
                        );
                    return;
                }
                if (user.IsSystem)
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException("You are not allowed to modify system admin users"),
                                    "InvalidOperation", ErrorCategory.InvalidOperation, userName)
                        );
                    return;
                }
                if (user.IsAdministrator)
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException(
                                        "You are not allowed to grant permissions to administrators"),
                                    "InvalidOperation", ErrorCategory.InvalidOperation, userName)
                        );
                    return;
                }

                MonitoredSqlServerInfo server = Helper.GetInstance(drive, instanceName, null);
                if (server == null)
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException(
                                        String.Format("Instance name '{0}' not found", instanceName)),
                                    "InvalidOperation", ErrorCategory.InvalidOperation, instanceName)
                        );
                    return;
                }

                RuntimeDefinedParameterDictionary dynamicParms = DynamicParameters as RuntimeDefinedParameterDictionary;
                // get the permission type to set
                PermissionType permissionType =
                    Helper.GetDynamicParameterValue(dynamicParms, Constants.PermissionParameter, PermissionType.View);

                // see if the permission is already set
                foreach (SQLdmPermissionInfo p in user.Permissions)
                {
                    if (p.InstanceName.Equals(instanceName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (p.Permission == permissionType)
                        {
                            WriteToPipeline(p, path, false);
                            return;
                        }
                        break;
                    }
                }

                // find a permission definition that has this permission
                PermissionDefinition grantpd = null;
                PermissionDefinition[] revokepd = null;

                bool grantNeeded =
                    Helper.GetGrantPermissionModifications(drive, userName, instanceName, permissionType, out grantpd,
                                                           out revokepd);
                if (grantpd != null)
                {
                    using (ProgressProvider progress = new ProgressProvider(this, "Granting permissions"))
                    {
                        drive.SetContextDataChangeLog(drive);
                        DataHelper.ManagementService.EditPermission(
                            grantpd.PermissionID,
                            grantpd.Enabled,
                            grantpd.PermissionType,
                            new int[0],
                            Helper.GetUpdatedServerList(drive, instanceName, null, grantpd, progress),
                            grantpd.Comment,
                            grantpd.LoginType != LoginType.SQLLogin ? grantpd.WebAppPermission : false);
                    }
                }
                else if (grantNeeded)
                {
                    drive.SetContextDataChangeLog(drive);
                    DataHelper.ManagementService.AddPermission(
                        user.Login,
                        user.LoginType == LoginType.SQLLogin,
                        user.Password,
                        permissionType,
                        new int[0],
                        Helper.GetInstanceIds(drive, new string[] { instanceName }),
                        String.Empty,
                        user.LoginType != LoginType.SQLLogin ? user.WebAppPermission : false);
                }
                if (revokepd != null)
                {
                    foreach (PermissionDefinition pd in revokepd)
                    {
                        using (ProgressProvider progress = new ProgressProvider(this, "Revoking permissions"))
                        {
                            drive.SetContextDataChangeLog(drive);
                            DataHelper.ManagementService.EditPermission(
                                pd.PermissionID,
                                pd.Enabled,
                                pd.PermissionType,
                                new int[0],
                                Helper.GetUpdatedServerList(drive, null, instanceName, pd, progress),
                                pd.Comment,
                                pd.LoginType != LoginType.SQLLogin ? pd.WebAppPermission : false);
                        }
                    }
                }

                drive.SetSecurityConfigurationDirty();

                user = Helper.GetSQLdmUserInfo(drive, userName);
                foreach (SQLdmPermissionInfo pi in user.Permissions)
                {
                    if (pi.InstanceName.Equals(instanceName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        WriteToPipeline(pi,
                                        pathSeparator +
                                        AppSecurityInfo.ContainerName +
                                        pathSeparator +
                                        EscapeString(userName) +
                                        pathSeparator +
                                        EscapeString(pi.InstanceName),
                                        false);
                        break;
                    }
                }
            }
        }

        private void NewAppUserItem(string path, string userName, object newItemValue)
        {
            using (LOG.InfoCall("NewAppUserItem"))
            {
                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);

                // see if user authz to change instance
                UserToken token = drive.UserToken;
                PermissionType permissions = (!token.IsSecurityEnabled ||
                                              (token.IsSysadmin || token.IsSQLdmAdministrator))
                                                 ? PermissionType.Administrator
                                                 : PermissionType.None;
                if (permissions != PermissionType.Administrator)
                {
                    WriteError(new ErrorRecord
                                   (new PSSecurityException("You are not authorized to add new instances"),
                                    "SecurityException", ErrorCategory.SecurityError, userName)
                        );
                    return;
                }

                if (Helper.GetSQLdmUserExists(drive, userName))
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException("User already exists"),
                                    "Duplicate", ErrorCategory.ResourceExists, userName)
                        );
                    return;
                }

                SQLdmUserInfo template = newItemValue as SQLdmUserInfo;
                SQLdmUserInfo newInstance = template != null
                                                ? (SQLdmUserInfo)template.Clone()
                                                : new SQLdmUserInfo();

                // make sure the login gets set in case we are copying a user
                newInstance.Login = userName;

                // update with dynamic parameters
                RuntimeDefinedParameterDictionary dynamicParms = DynamicParameters as RuntimeDefinedParameterDictionary;
                newInstance = UpdateSQLdmUserWithDynamicParameters(drive, newInstance);

                bool passwordRequired = newInstance.LoginType == LoginType.SQLLogin;
                if (passwordRequired && String.IsNullOrEmpty(newInstance.Password))
                {
                    if (this.Host != null)
                    {
                        PSHostUserInterface ui = this.Host.UI;
                        PSCredential creds =
                            ui.PromptForCredential("SQLDM Password Request",
                                                   "Please enter the password for the new SQLDM user.",
                                                   newInstance.Login, "SQLdm", PSCredentialTypes.Generic,
                                                   PSCredentialUIOptions.ReadOnlyUserName);
                        if (creds != null)
                        {
                            newInstance.Password = creds.GetNetworkCredential().Password;
                        }
                    }
                }


                bool isAdmin = GetBooleanParameterValue(dynamicParms[Constants.AdmininstratorParameter]);

                drive.SetContextDataChangeLog(drive);
                DataHelper.ManagementService.AddPermission(
                    newInstance.Login,
                    passwordRequired,
                    newInstance.Password,
                    isAdmin ? PermissionType.Administrator : PermissionType.View,
                    emptyIntArray,
                    emptyIntArray,
                    newInstance.Comments,
                    !passwordRequired
                    );

                drive.SetSecurityConfigurationDirty();

                WriteItemObject(newInstance, path, false);
            }
        }

        private void NewCustomCounterItem(string path, string counterName, object newItemValue)
        {
            WriteError(
                new ErrorRecord(
                    new PSNotImplementedException(
                        "The method or operation for creating custom counters has not been implemented."),
                    "NotImplemented", ErrorCategory.NotImplemented, counterName));
        }

        private void NewInstanceItem(string path, string instanceName, object newItemValue)
        {
            using (LOG.InfoCall("NewInstanceItem"))
            {
                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);

                // see if user authz to change instance
                UserToken token = drive.UserToken;
                PermissionType permissions = (!token.IsSecurityEnabled ||
                                              (token.IsSysadmin || token.IsSQLdmAdministrator))
                                                 ? PermissionType.Administrator
                                                 : PermissionType.None;
                if (permissions != PermissionType.Administrator)
                {
                    WriteError(new ErrorRecord
                                   (new PSSecurityException("You are not authorized to add new instances"),
                                    "SecurityException", ErrorCategory.SecurityError, instanceName)
                        );
                    return;
                }

                MonitoredSqlServerInfo template = newItemValue as MonitoredSqlServerInfo;
                MonitoredSqlServerInfo newInstance = template != null
                                                         ? (MonitoredSqlServerInfo)template.Clone()
                                                         : new MonitoredSqlServerInfo(drive);

                newInstance.InstanceName = instanceName;

                RuntimeDefinedParameterDictionary dynamicParms = DynamicParameters as RuntimeDefinedParameterDictionary;

                if (Credential != null && !String.IsNullOrEmpty(Credential.UserName))
                {
                    NetworkCredential netcred = Credential.GetNetworkCredential();
                    if (netcred != null && !String.IsNullOrEmpty(netcred.Password))
                    {
                        if (
                            Helper.IsDynamicParameterSet(DynamicParameters as RuntimeDefinedParameterDictionary,
                                                         Constants.WindowsAuthenticationParameter))
                        {
                            WriteError(new ErrorRecord
                                           (new PSNotSupportedException(
                                                String.Format(
                                                    "You cannot use both the Credential and the WindowsAuthentication parameters.",
                                                    instanceName)),
                                            "InvalidData", ErrorCategory.InvalidData, instanceName)
                                );
                            return;
                        }
                        newInstance.Credential = Credential;
                    }
                }

                // update with dynamic parameters
                newInstance = UpdateInstanceWithDynamicParameters(newInstance);

                try
                {
                    string user;

                    if (String.IsNullOrEmpty(drive.Credential.UserName))
                    {
                        user = AuditingEngine.GetWorkstationUser();
                    }
                    else
                    {
                        user = drive.Credential.UserName.StartsWith("\\") ? drive.Credential.UserName.Remove(0, 1) : drive.Credential.UserName;
                    }
                    AuditingEngine.SetContextData(user);

                    MonitoredSqlServerConfiguration configuration = (MonitoredSqlServerConfiguration)newInstance;
                    //SQLdm 10.1 (Barkha Khatri) getting sys admin flag 
                    MonitoredSqlServerMixin versionAndPermission = Helper.GetServerVersionAndPermission(configuration.ConnectionInfo);
                    if (versionAndPermission == null)
                    {
                        LOG.Error("after calling RepositoryHelper.GetServerVersionAndPermission, versionAndPermission object found null. Initializing object with default value.");
                        versionAndPermission = new MonitoredSqlServerMixin();
                    }
                    configuration.IsUserSysAdmin = versionAndPermission.IsUserSysAdmin;
                    MonitoredSqlServer mss = DataHelper.ManagementService.AddMonitoredSqlServer(configuration);
                    WriteToPipeline(new MonitoredSqlServerInfo(mss, drive), path, true);
                }
                catch (Exception e)
                {
                    while (e.InnerException is ManagementServiceException)
                        e = e.InnerException;

                    WriteError(new ErrorRecord(e, "ServiceException", ErrorCategory.NotSpecified, instanceName));
                }
            }
        }

        /// <summary>
        /// Allows the provider to attach additional parameters to the
        /// new-item cmdlet.
        /// </summary>
        /// 
        /// <param name="path">
        /// If the path was specified on the command line, this is the path
        /// to the item to get the dynamic parameters for.
        /// </param>
        /// 
        /// <param name="type">
        /// The provider defined type of the item to create.
        /// </param>
        /// 
        /// <param name="newItemValue">
        /// This is a provider specific type that the provider can use to 
        /// create a new instance of an item at the specified path.
        /// </param>
        /// 
        /// <returns>
        /// Overrides of this method should return an object that has 
        /// properties and fields decorated with parsing attributes similar to 
        /// a cmdlet class or a 
        /// <see cref="System.Management.Automation.PseudoParameterDictionary"/>.
        /// 
        /// The default implemenation returns null. 
        /// </returns>
        protected override object NewItemDynamicParameters(string path, string type, object newItemValue)
        {
            using (LOG.InfoCall("NewItemDynamicParameters"))
            {
                RuntimeParameterBuilder parms = new RuntimeParameterBuilder();

                if (String.IsNullOrEmpty(type))
                {
                    string[] chunks = NormalizeAndChunkPath(path);
                    PathType pathType = GetPathType(chunks);

                    switch (pathType)
                    {
                        case PathType.Instance:
                            type = "Instance";
                            break;
                        case PathType.AppUser:
                            type = "User";
                            break;
                        case PathType.AppUserPermission:
                            type = "Permission";
                            break;
                    }
                }


                switch (type)
                {
                    case "Instance":
                        // Instance
                        parms.AddParameter<int>(Constants.ScheduledCollectionIntervalParameter, false, null);
                        parms.AddParameter<int>(Constants.DatabaseStatisticsParameter, false, null);
                        //SQLDM-30516
                        parms.AddParameter<int>(Constants.ServerInAccessibleRateParameter, false, null);
                        //           parms.AddParameter<PSCredential>(Constants.CredentialParameter, false, "Credential.SQLLogin");
                        parms.AddParameter<SwitchParameter>(Constants.WindowsAuthenticationParameter, false, null);
                        parms.AddParameter<SwitchParameter>(Constants.EncryptParameter, false, null);
                        parms.AddParameter<SwitchParameter>(Constants.TrustCertsParameter, false, null);
                        parms.AddParameter<SwitchParameter>(Constants.CollectExtendedSessionDataParameter, false, null);
                        parms.AddParameter<SwitchParameter>(Constants.CollectReplicationDataParameter, false, null);
                        parms.AddParameter<SwitchParameter>(Constants.DisableOleAutomationUseParameter, false, null);
                        parms.AddParameter<string[]>(Constants.TagsParameter, false, null);
                        // Maintenance Mode
                        parms.AddParameter<SwitchParameter>(Constants.MModeNeverParameter, false, "MM.MMNever");
                        parms.AddParameter<SwitchParameter>(Constants.MModeAlwaysParameter, false, "MM.MMAlways");
                        parms.AddParameter<SwitchParameter>(Constants.MModeOnceParameter, false, "MM.MMOnce");
                        parms.AddParameter<SwitchParameter>(Constants.MModeRecurringParameter, false, "MM.MMRecurring");
                        parms.AddParameter<Days[]>(Constants.MModeDaysParameter, false, "MM.MMRecurring");
                        parms.AddParameter<TimeSpan>(Constants.MModeStartTimeParameter, false, "MM.MMRecurring");
                        parms.AddParameter<TimeSpan>(Constants.MModeDurationParameter, false, "MM.MMRecurring");
                        parms.AddParameter<DateTime>(Constants.MModeStartDateParameter, false, "MM.MMOnce");
                        parms.AddParameter<DateTime>(Constants.MModeEndDateParameter, false, "MM.MMOnce");
                        // Query Monitor
                        parms.AddParameter<SwitchParameter>(Constants.QMEnabledParameter, false, "QM.QMEnabled");
                        parms.AddParameter<SwitchParameter>(Constants.QMDisabledParameter, false, "QM.QMDisabled");
                        parms.AddParameter<bool>(Constants.CaptureStatementsParameter, false, "QM.QMEnabled");

                        //Power shell snapin for remaining Activity Monitor controls
                        //SQLdm10.1 (Srishti Purohit)
                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivitySQLTrace, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivityExtendedEvent, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivityAutoGrow, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableNonQueryActivityAutoGrow, false, "QM.NQADisable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivityCaptureBlocking, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableNonQueryActivityCaptureBlocking, false, "QM.NQADisable");
                        parms.AddParameter<int>(Constants.NonQueryActivityCaptureBlocking, false, "QM.NQADisable");

                        parms.AddParameter<bool>(Constants.CaptureBatchesParameter, false, "QM.QMEnabled");
                        parms.AddParameter<bool>(Constants.CaptureProcsParameter, false, "QM.QMEnabled");

                        parms.AddParameter<SwitchParameter>(Constants.EnableNonQueryActivityParameter, false, "QM.NQAEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableNonQueryActivityParameter, false, "QM.NQADisable");
                        parms.AddParameter<bool>(Constants.CaptureDeadlocksParameter, false, "QM.NQAEnable");

                        parms.AddParameter<int>(Constants.QueryDurationParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.QueryTopPlanCountParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.QueryTopPlanCategoryParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.CPUUsageParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.LogicalDiskReadsParameter, false, "QM.QMEnabled");
                        parms.AddParameter<int>(Constants.PhysicalDiskWritesParameter, false, "QM.QMEnabled");
                        parms.AddParameter<string[]>(Constants.ExcludedAppsParameter, false, "QM.QMEnabled");
                        parms.AddParameter<string[]>(Constants.ExcludedDatabasesParameter, false, "QM.QMEnabled");
                        parms.AddParameter<string[]>(Constants.ExcludedSqlParameter, false, "QM.QMEnabled");

                        //Power shell snapin for remaining for controls
                        //SQLdm10.1 (Srishti Purohit)
                        parms.AddParameter<SwitchParameter>(Constants.EnableQueryMonitorTraceMonitoring, false, "QM.QMTMEnable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableQueryMonitorExtendedEvents, false, "QM.QMTMEnable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableQueryMonitorCollectQueryPlan, false, "QM.QMTMEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableQueryMonitorCollectQueryPlan, false, "QM.QMTMDisable");
                        parms.AddParameter<SwitchParameter>(Constants.EnableQueryMonitorCollectEstimatedQueryPlan, false, "QM.QMTMEnable");
                        parms.AddParameter<SwitchParameter>(Constants.DisableQueryMonitorCollectEstimatedQueryPlan, false, "QM.QMTMDisable");
                        // Quiet Time
                        parms.AddParameter<Days[]>(Constants.QTDaysParameter, false, null);
                        parms.AddParameter<TimeSpan>(Constants.QTStartTimeParameter, false, null);
                        parms.AddParameter<int>(Constants.QTReorgMinTableSizeParameter, false, null);
                        parms.AddParameter<string[]>(Constants.QTExcludedDatabasesParameter, false, null);
                        //Os Metrics
                        parms.AddParameter<SwitchParameter>(Constants.OsMetricCollectByOleParameter, false, "OM.OMEnabled");
                        parms.AddParameter<SwitchParameter>(Constants.OsMetricCollectByWmiParameter, false, "OM.OMEnabled");
                        parms.AddParameter<bool>(Constants.OsMetricConnectAsCollectionService, false, "OM.OMEnabled");
                        parms.AddParameter<SwitchParameter>(Constants.OsMetricNotCollect, false, "OM.OMEnabled");
                        parms.AddParameter<string>(Constants.OsMetricWmiUser, false, "OM.OMEnabled");
                        parms.AddParameter<string>(Constants.OsMetricWmiPassword, false, "OM.OMEnabled");

                        //general settings configuration
                        parms.AddParameter<string>(Constants.FriendlyServerName, false, "FriendlyServerName");
                        parms.AddParameter<SwitchParameter>(Constants.FriendlyServerNameBlank, false, "FriendlyServerNameBlank");
                        parms.AddParameter<int>(Constants.InputBufferLimiter, false, "InputBufferLimiter");
                        parms.AddParameter<SwitchParameter>(Constants.InputBufferLimiterEnable, false, "InputBufferLimiterEnable");
                        parms.AddParameter<SwitchParameter>(Constants.InputBufferLimiterDisable, false, "InputBufferLimiterDisable");

                        //Active Wait Configuration
                        //parms.AddParameter<SwitchParameter>(Constants.QWStatisticsEnable, false, "OM.QWStatisticsEnabled");
                        parms.AddParameter<SwitchParameter>(Constants.QWStatisticsDisable, false, "OM.QWStatisticsDisabled");
                        parms.AddParameter<SwitchParameter>(Constants.QWExtendedEnable, false, "QW.QWExtendedEnable");
                        parms.AddParameter<SwitchParameter>(Constants.QWExtendedDisable, false, "QW.QWExtendedDisable");
                        parms.AddParameter<SwitchParameter>(Constants.QWStatisticsCollectIndefinite, false, "QW.QWStatisticsEnabled");
                        parms.AddParameter<DateTime>(Constants.QWStatisticsStartDate, false, "QW.QWStatisticsEnabled");
                        parms.AddParameter<TimeSpan>(Constants.QWStatisticsDuration, false, "QW.QWStatisticsEnabled");
                        break;
                    case "AppSecurity":
                        parms.AddParameter<bool>(Constants.EnabledParameter, true, null, null);
                        break;
                    case "User":
                        parms.AddParameter<SwitchParameter>(Constants.LoginTypeSqlParameter, false, true, "AppUser.SQL");
                        parms.AddParameter<SwitchParameter>(Constants.LoginTypeWindowsGroupParameter, false, true,
                                                            "AppUser.Group");
                        parms.AddParameter<SwitchParameter>(Constants.LoginTypeWindowsUserParameter, false, true,
                                                            "AppUser.User");
                        RuntimeDefinedParameter rdp =
                            parms.AddParameter<string>(Constants.PasswordParameter, false, "AppUser.SQL");
                        rdp.Attributes.Add(new PasswordAttribute());
                        rdp.Attributes.Add(new AllowEmptyStringAttribute());
                        parms.AddParameter<SwitchParameter>(Constants.AdmininstratorParameter, false, null);
                        parms.AddParameter<string>(Constants.CommentParameter, false, null);
                        break;
                    case "Permission":
                        RuntimeDefinedParameter p =
                            parms.AddParameter<PermissionType>(Constants.PermissionParameter, true, null);
                        p.Attributes.Add(new ValidateSetAttribute("View", "Modify"));
                        break;
                    case "CustomCounter":
                        break;
                }
                return parms.Parameters;
            }
        }

        /// <summary>
        /// Removes (deletes) the item at the specified path 
        /// </summary>
        /// 
        /// <param name="path">
        /// The path to the item to remove.
        /// </param>
        /// 
        /// <param name="recurse">
        /// True if all children in a subtree should be removed, false if only
        /// the item at the specified path should be removed. 
        /// </param>
        /// 
        /// <remarks>
        /// Providers override this method to allow the user the ability to 
        /// remove provider objects using the remove-item cmdlet.
        /// 
        /// Providers that declare 
        /// <see cref="System.Management.Automation.Provider.ProviderCapabilities"/>
        /// of ExpandWildcards, Filter, Include, or Exclude should ensure that 
        /// the path passed meets those requirements by accessing the 
        /// appropriate property from the base class.
        /// 
        /// By default, overrides of this method should not remove objects that
        /// are generally hidden from the user unless the Force property is set 
        /// to true. For instance, the FileSystem provider should not remove a 
        /// hidden or system file unless the Force property is set to true. 
        /// 
        /// The provider implementation is responsible for preventing infinite 
        /// recursion when there are circular links and the like. An 
        /// appropriate terminating exception should be thrown if this 
        /// situation occurs.
        /// 
        /// This method should call 
        /// <see cref="System.Management.Automation.CmdletProvider.ShouldProcess"/> 
        /// and check its return value before making any changes to the store 
        /// this provider is working upon.
        /// </remarks>
        protected override void RemoveItem(string path, bool recurse)
        {
            using (LOG.InfoCall("RemoveItem"))
            {
                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);

                string[] chunks = NormalizeAndChunkPath(path);
                PathType pathType = GetPathType(chunks);

                switch (pathType)
                {
                    case PathType.Instance:
                        RemoveInstanceItem(drive, UnescapeString(chunks[1]), recurse);
                        break;
                    case PathType.AppUser:
                        RemoveAppUserItem(drive, UnescapeString(chunks[1]), recurse);
                        break;
                    case PathType.AppUserPermission:
                        if (recurse)
                            RemoveUserPermissionViaTag(drive, UnescapeString(chunks[1]), UnescapeString(chunks[2]));
                        else
                            RemoveUserPermission(drive, UnescapeString(chunks[1]), UnescapeString(chunks[2]));
                        break;
                    //              case PathType.CustomCounter:
                    //                  RemoveCustomCounterItem(UnescapeString(chunks[1]), recurse);
                    //                  break;
                    default:
                        WriteError(
                            new ErrorRecord(
                                new NotImplementedException(
                                    String.Format("Removing {0} objects is not supported.", pathType)),
                                "NotImplemented", ErrorCategory.NotImplemented, path));
                        break;
                }
                // if (ShouldProcess(path, "delete"))
                // {
                //      // delete the item
                // }
            }
        }

        private void RemoveUserPermissionViaTag(SQLdmDriveInfo drive, string userName, string tagName)
        {
            using (LOG.InfoCall("RemoveUserPermissionViaTag"))
            {
                // see if user authz to change instance
                UserToken token = drive.UserToken;
                PermissionType permissions = (!token.IsSecurityEnabled ||
                                              (token.IsSysadmin || token.IsSQLdmAdministrator))
                                                 ? PermissionType.Administrator
                                                 : PermissionType.None;
                if (permissions != PermissionType.Administrator)
                {
                    WriteError(new ErrorRecord
                                   (new PSSecurityException("You are not authorized to add new instances"),
                                    "SecurityException", ErrorCategory.SecurityError, userName)
                        );
                    return;
                }

                if (tagName.StartsWith("#--"))
                    tagName = tagName.Substring(3);

                Tag tag = null;

                drive.SetTagsDirty();
                if (!drive.TagsByName.TryGetValue(tagName.ToLower(), out tag))
                {
                    WriteError(new ErrorRecord
                                   (new InvalidOperationException("Tag does not exist"),
                                    "ObjectNotFound", ErrorCategory.ObjectNotFound, tagName)
                        );
                    return;
                }

                PermissionDefinition pd;
                Configuration securityConfig = drive.GetAppSecurityConfiguration();
                foreach (int permissionId in tag.Permissions)
                {
                    if (securityConfig.Permissions.TryGetValue(permissionId, out pd))
                    {
                        Triple<bool, IList<int>, IList<int>> pts =
                            Helper.GetPermissionTagsAndServers(drive, pd.PermissionID);
                        // get current set of tags
                        Set<int> tags = Helper.GetTagsForPermissionId(drive, pd.PermissionID);
                        // remove tag from current set
                        tags.Remove(tag.Id);

                        // update permission
                        drive.SetContextDataChangeLog(drive);//set conetxt for ChangeLog
                        DataHelper.ManagementService.EditPermission(pd.PermissionID,
                                                               pd.Enabled,
                                                               pd.PermissionType,
                                                               tags.ToArray(),
                                                               Algorithms.ToArray(pts.Third),
                                                               pd.Comment,
                                                               pd.LoginType != LoginType.SQLLogin ? pd.WebAppPermission : false);
                    }
                }
            }
        }

        private void RemoveUserPermission(SQLdmDriveInfo drive, string userName, string instanceName)
        {
            using (LOG.InfoCall("RemoveUserPermission"))
            {
                // see if user authz to change instance
                UserToken token = drive.UserToken;
                PermissionType permissions = (!token.IsSecurityEnabled ||
                                              (token.IsSysadmin || token.IsSQLdmAdministrator))
                                                 ? PermissionType.Administrator
                                                 : PermissionType.None;
                if (permissions != PermissionType.Administrator)
                {
                    WriteError(new ErrorRecord
                                   (new PSSecurityException("You are not authorized to add new instances"),
                                    "SecurityException", ErrorCategory.SecurityError, userName)
                        );
                    return;
                }

                foreach (PermissionDefinition pd in Helper.GetSQLdmUserPermission(drive, userName))
                {
                    if (!pd.Enabled)
                        continue;

                    if (Helper.IsServerInPermissionDefinition(pd, instanceName))
                    {

                        using (ProgressProvider progress = new ProgressProvider(this, "Revoking permissions"))
                        {
                            drive.SetContextDataChangeLog(drive);
                            DataHelper.ManagementService.EditPermission(pd.PermissionID,
                                                               pd.Enabled,
                                                               pd.PermissionType,
                                                               new int[0],
                                                               Helper.GetUpdatedServerList(drive, null, instanceName, pd, progress),
                                                               pd.Comment,
                                                               pd.LoginType != LoginType.SQLLogin ? pd.WebAppPermission : false);
                        }
                    }
                }
            }
        }

        private void RemoveAppUserItem(SQLdmDriveInfo drive, string userName, bool recurse)
        {
            using (LOG.InfoCall("RemoveAppUserItem"))
            {
                // see if user authz to change instance
                UserToken token = drive.UserToken;
                PermissionType permissions = (!token.IsSecurityEnabled ||
                                              (token.IsSysadmin || token.IsSQLdmAdministrator))
                                                 ? PermissionType.Administrator
                                                 : PermissionType.None;
                if (permissions != PermissionType.Administrator)
                {
                    WriteError(new ErrorRecord
                                   (new PSSecurityException("You are not authorized to add new instances"),
                                    "SecurityException", ErrorCategory.SecurityError, userName)
                        );
                    return;
                }

                // Get the ID for the userName
                int id = -1;
                Configuration secConfig = drive.GetAppSecurityConfiguration();
                List<PermissionDefinition> pds = Helper.GetSQLdmUserPermission(drive, userName);
                if (pds.Count == 0)
                {
                    WriteError(new ErrorRecord
                                   (new ApplicationException("User not found"),
                                    "NotFoundException", ErrorCategory.SecurityError, userName)
                        );
                    return;
                }
                foreach (PermissionDefinition pd in pds)
                {
                    drive.SetContextDataChangeLog(drive);
                    DataHelper.ManagementService.DeletePermission(pd.PermissionID);
                }
            }
        }

        private void RemoveInstanceItem(SQLdmDriveInfo drive, string instanceName, bool recurse)
        {
            using (LOG.InfoCall("RemoveInstanceItem"))
            {
                MonitoredSqlServerInfo server = null;
                using (ProgressProvider progress = new ProgressProvider(this, "Get instance"))
                {
                    server = Helper.GetInstance(drive, instanceName, progress);
                    if (server == null)
                    {
                        WriteError(
                            new ErrorRecord(
                                new ItemNotFoundException(
                                    String.Format("The monitored instance {0} was not found.", instanceName)),
                                "NotFound", ErrorCategory.ObjectNotFound, instanceName));
                        return;
                    }

                    bool retainData = false;
                    RuntimeDefinedParameterDictionary dynamicParameters =
                        DynamicParameters as RuntimeDefinedParameterDictionary;
                    if (dynamicParameters != null)
                    {
                        foreach (RuntimeDefinedParameter dynamicParameter in dynamicParameters.Values)
                        {
                            if (dynamicParameter.IsSet)
                            {
                                if (dynamicParameter.Name.Equals("RetainData"))
                                    retainData = ((SwitchParameter)dynamicParameter.Value).ToBool();
                            }
                        }
                    }

                    string user;

                    if (String.IsNullOrEmpty(drive.Credential.UserName))
                    {
                        user = AuditingEngine.GetWorkstationUser();
                    }
                    else
                    {
                        user = drive.Credential.UserName.StartsWith("\\") ? drive.Credential.UserName.Remove(0, 1) : drive.Credential.UserName;
                    }

                    if (retainData)
                    {
                        progress.StatusDescription = "Deactivating Instance";
                        progress.ReportProgress();

                        AuditingEngine.SetContextData(user);

                        DataHelper.ManagementService.DeactivateMonitoredSqlServer(server.Id);
                    }
                    else
                    {
                        progress.StatusDescription = "Deleting Instance";
                        progress.ReportProgress();

                        AuditingEngine.SetContextData(user);

                        DataHelper.ManagementService.DeleteMonitoredSqlServer(server.Id);
                    }
                }
            }
        }

        protected override object RemoveItemDynamicParameters(string path, bool recurse)
        {
            using (LOG.InfoCall("RemoveItemDynamicParameters"))
            {
                RuntimeParameterBuilder dynamicParameters = new RuntimeParameterBuilder();

                string[] chunks = NormalizeAndChunkPath(path);
                PathType pathType = GetPathType(chunks);

                switch (pathType)
                {
                    case PathType.Instance:
                        dynamicParameters.AddParameter<SwitchParameter>("RetainData", false, null, null);
                        break;
                }

                return dynamicParameters.Parameters;
            }
        }

        /// <summary>
        /// Determines if the item at the specified path has children.
        /// </summary>
        /// 
        /// <param name="path">
        /// The path to the item to see if it has children.
        /// </param>
        /// 
        /// <returns>
        /// True if the item has children, false otherwise.
        /// </returns>
        /// 
        /// <remarks>
        /// Providers override this method to give the provider infrastructure 
        /// the ability to determine if a particular provider object has 
        /// children without having to retrieve all the child items.
        /// 
        /// For providers that are derived from 
        /// <see cref="ContainerCmdletProvider"/> class, if a null or empty 
        /// path is passed, the provider should consider any items in the data 
        /// store to be children and return true.
        /// 
        /// If this provider exposes a root that contains interesting mount
        /// points (as described in InitializeDefaultDrives) it should return
        /// true when null or String.Empty is passed. 
        /// </remarks>
        protected override bool HasChildItems(string path)
        {
            using (LOG.InfoCall("HasChildItems"))
            {
                string[] chunks = NormalizeAndChunkPath(path);
                PathType pathType = GetPathType(chunks);
                switch (pathType)
                {
                    case PathType.Alert:
                    case PathType.CustomCounter:
                    case PathType.Invalid:
                    case PathType.AppUserPermission:
                        return false;
                    case PathType.AppUser:
                        return AppUserHasChildren(GetSQLdmDriveInfo(path), EscapeString(chunks[1]));
                }

                return true;
            }
        }

        /// <summary>
        /// Joins two strings with a provider specific path separator.
        /// </summary>
        /// 
        /// <param name="parent">
        /// The parent segment of a path to be joined with the child.
        /// </param>
        /// 
        /// <param name="child">
        /// The child segment of a path to be joined with the parent.
        /// </param>
        /// 
        /// <returns>
        /// A string that represents the parent and child segments of the path
        /// joined by a path separator.
        /// </returns>
        /// 
        /// <remarks>
        /// This method should use lexical joining of two path segments with a
        /// path separator character.  It should not validate the path as a
        /// legal fully qualified path in the provider namespace as each
        /// parameter could be only partial segments of a path and joined
        /// they may not generate a fully qualified path.
        /// Example: the file system provider may get "windows\system32" as
        /// the parent parameter and "foo.dll" as the child parameter. The
        /// method should join these with the "\" separator and return
        /// "windows\system32\foo.dll". Note that the returned path is not a
        /// fully qualified file system path.
        /// 
        /// Also beware that the path segments may contain characters that are
        /// illegal in the provider namespace. These characters are most
        /// likely used as wildcards and should not be removed by the
        /// implementation of this method.
        /// 
        /// The default implementation will take paths with '/' or '\' as the
        /// path separator and normalize the path separator to '\' and then
        /// join the child and parent with a '\'.
        /// </remarks>
        protected override string MakePath(string parent, string child)
        {
            using (LOG.InfoCall("MakePath"))
            {
                string result;

                string normalParent = NormalizePath(parent);
                normalParent = RemoveDriveFromPath(normalParent);
                string normalChild = NormalizePath(child);
                normalChild = RemoveDriveFromPath(normalChild);

                if (String.IsNullOrEmpty(normalParent) && String.IsNullOrEmpty(normalChild))
                {
                    result = String.Empty;
                }
                else if (String.IsNullOrEmpty(normalParent) && !String.IsNullOrEmpty(normalChild))
                {
                    result = normalChild;
                }
                else if (!String.IsNullOrEmpty(normalParent) && String.IsNullOrEmpty(normalChild))
                {
                    if (normalParent.EndsWith(pathSeparator, StringComparison.OrdinalIgnoreCase))
                    {
                        result = normalParent;
                    }
                    else
                    {
                        result = normalParent + pathSeparator;
                    }
                } // else if (!String...
                else
                {
                    if (!normalParent.Equals(String.Empty, StringComparison.OrdinalIgnoreCase) &&
                        !normalParent.EndsWith(pathSeparator, StringComparison.OrdinalIgnoreCase))
                    {
                        result = normalParent + pathSeparator;
                    }
                    else
                    {
                        result = normalParent;
                    }

                    if (normalChild.StartsWith(pathSeparator, StringComparison.OrdinalIgnoreCase))
                    {
                        result += normalChild.Substring(1);
                    }
                    else
                    {
                        result += normalChild;
                    }
                }

                LOG.VerboseFormat("MakePath parent={0} child={1} result={2}", parent, child, result);

                return RewritePath(result);
            }
        }

        private string RewritePath(string path)
        {
            string[] parts = path.Split(pathSeparator[0]);

            if (parts == null || parts.Length == 0)
                return String.Empty;

            // compare all this crap in lowercase
            switch (parts[0].ToLower())
            {
                case InstancesInfo.ContainerNameLower:
                    parts[0] = InstancesInfo.ContainerName;
                    if (parts.Length == 1)
                        return InstancesInfo.ContainerName;
                    if (parts.Length > 2)
                    {
                        if (parts[2].ToLower().Equals(AlertsInfo.ContainerNameLower))
                            parts[2] = AlertsInfo.ContainerName;
                    }
                    break;
                case AppSecurityInfo.ContainerNameLower:
                    parts[0] = AppSecurityInfo.ContainerName;
                    break;
                default:
                    return path;
            }

            StringBuilder builder = new StringBuilder();
            foreach (string part in parts)
            {
                if (builder.Length > 0)
                    builder.Append(pathSeparator);
                builder.Append(part);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Removes the child segment of a path and returns the remaining
        /// parent portion.
        /// </summary>
        /// 
        /// <param name="path">
        /// A full or partial provider specific path. The path may be to an
        /// item that may or may not exist.
        /// </param>
        /// 
        /// <param name="root">
        /// The fully qualified path to the root of a drive. This parameter
        /// may be null or empty if a mounted drive is not in use for this
        /// operation.  If this parameter is not null or empty the result
        /// of the method should not be a path to a container that is a
        /// parent or in a different tree than the root.
        /// </param>
        /// 
        /// <returns>
        /// The path of the parent of the path parameter.
        /// </returns>
        /// 
        /// <remarks>
        /// This should be a lexical splitting of the path on the path
        /// separator character for the provider namespace.  For example, the
        /// file system provider should look for the last "\" and return
        /// everything to the left of the "\".
        /// 
        /// The default implementation accepts paths that have both '/' and
        /// '\' as the path separator. It first normalizes the path to have
        /// only '\' separators and then splits the parent path off at the
        /// last '\' and returns it.
        /// </remarks>
        protected override string GetParentPath(string path, string root)
        {
            using (LOG.InfoCall("GetParentPath"))
            {
                string[] chunks = NormalizeAndChunkPath(path);
                int nchunks = chunks.Length - 1;
                if (nchunks < 1)
                {
                    LOG.VerboseFormat("GetParentPath path={0} root={1} result=[] (empty string)", path, root);
                    return String.Empty;
                }

                StringBuilder builder = new StringBuilder(root);
                if (!root.EndsWith(pathSeparator))
                    builder.Append(pathSeparator);

                for (int i = 0; i < nchunks; i++)
                {
                    if (i > 0)
                        builder.Append(pathSeparator);
                    builder.Append(chunks[i]);
                }
                LOG.VerboseFormat("GetParentPath path={0} root={1} result={2}", path, root, builder);
                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the name of the leaf element in the specified path.
        /// </summary>
        /// 
        /// <param name="path">
        /// The full or partial provider specific path.
        /// </param>
        /// 
        /// <returns>
        /// The leaf element in the path.
        /// </returns>
        /// 
        /// <remarks>
        /// This should be implemented as a split on the path separator.  The
        /// characters in the fullPath may not be legal characters in the
        /// namespace but may be used for wildcards or regular expression
        /// matching.  If the path contains no path separators the path should
        /// be returned unmodified.
        /// 
        /// The default implementation accepts paths that have both '/' and
        /// '\' as the path separator. It first normalizes the path to have
        /// only '\' separators and then splits the parent path off at the
        /// last '\' and returns it.
        /// </remarks>
        protected override string GetChildName(string path)
        {
            using (LOG.InfoCall("GetChildName"))
            {
                string[] pathChunks = NormalizeAndChunkPath(path);
                LOG.VerboseFormat("pathChunks.Length={0}", pathChunks.Length);
                for (int i = 0; i < pathChunks.Length; i++)
                {
                    LOG.VerboseFormat("pathChunks[{0}]=[{1}]", i, pathChunks[i]);
                }
                PathType type = GetPathType(pathChunks);

                LOG.VerboseFormat("path in [{0}] is a {1}", path, type);

                switch (type)
                {
                    case PathType.Root:
                        LOG.VerboseFormat("GetChildName Root path=|{0}|", path);
                        return path;
                    case PathType.Instances:
                        LOG.VerboseFormat("GetChildName Instances path=|{0}| r=|Instances|", path);
                        return "Instances";
                    case PathType.Instance:
                        LOG.VerboseFormat("GetChildName Instance path=|{0}| r=|{1}|", path, pathChunks[1]);
                        return pathChunks[1];
                    case PathType.CustomCounters:
                        LOG.VerboseFormat("GetChildName CustomCounters path=|{0}|", path);
                        return "CustomCounters";
                    case PathType.CustomCounter:
                        LOG.VerboseFormat("GetChildName CustomCounter path=|{0}|", path);
                        return pathChunks[1];
                    case PathType.AppSecurity:
                        LOG.VerboseFormat("GetChildName AppSecurity path=|{0}|", path);
                        return "AppSecurity";
                    case PathType.AppUser:
                        LOG.VerboseFormat("GetChildName AppUser path=|{0}|", path);
                        return pathChunks[1];
                    case PathType.AppUserPermission:
                        LOG.VerboseFormat("GetChildName AppUserPermission path=|{0}|", path);
                        return pathChunks[2];
                    case PathType.Alerts:
                        LOG.VerboseFormat("GetChildName Alerts path=|{0}|", path);
                        return "Alerts";
                    case PathType.Alert:
                        LOG.VerboseFormat("GetChildName Alert path=|{0}|", path);
                        return pathChunks[3];
                }
                string child = base.GetChildName(path);
                LOG.VerboseFormat("GetChildName base.GetChild path=|{0}|", child);
                return child;
            }
        }

        /// <summary>
        /// Determines if the specified object is a container
        /// </summary>
        /// 
        /// <param name="path">
        /// The path to the item to determine if it is a container.
        /// </param>
        /// 
        /// <returns>
        /// true if the item specified by path exists and is a container, 
        /// false otherwise.
        /// </returns>
        /// 
        /// <remarks>
        /// Providers override this method to give the user the ability to 
        /// check to see if a provider object is a container using the 
        /// test-path -container cmdlet.
        /// 
        /// Providers that declare 
        /// <see cref="System.Management.Automation.Provider.ProviderCapabilities"/>
        /// of ExpandWildcards, Filter, Include, or Exclude should ensure that 
        /// the path passed meets those requirements by accessing the 
        /// appropriate property from the base class.
        /// </remarks>
        protected override bool IsItemContainer(string path)
        {
            using (LOG.InfoCall("IsItemContainer"))
            {
                string[] pathChunks = NormalizeAndChunkPath(path);
                PathType type = GetPathType(pathChunks);

                switch (type)
                {
                    case PathType.Root:
                    case PathType.Instances:
                    case PathType.Instance:
                    case PathType.CustomCounters:
                    case PathType.AppSecurity:
                    case PathType.AppUser:
                    case PathType.Alerts:
                        LOG.VerboseFormat("Path [{0}] is a container: {1}", path, type);
                        return true;
                }

                LOG.VerboseFormat("Path [{0}] is a container: {1}", path, type);
                return false;
            }
        }

        public bool AppUserHasChildren(SQLdmDriveInfo drive, string userName)
        {
            SQLdmUserInfo user = Helper.GetSQLdmUserInfo(drive, userName);
            return (user == null || !user.IsAdministrator);
        }

        /// <summary>
        /// Normalizes the path that was passed in and returns the normalized
        /// path as a relative path to the basePath that was passed.
        /// </summary>
        /// 
        /// <param name="path">
        /// A fully qualified provider specific path to an item.  The item
        /// should exist or the provider should write out an error.
        /// </param>
        /// 
        /// <param name="basePath">
        /// The path that the return value should be relative to.
        /// </param>
        /// 
        /// <returns>
        /// A normalized path that is relative to the basePath that was
        /// passed.  The provider should parse the path parameter, normalize
        /// the path, and then return the normalized path relative to the
        /// basePath.
        /// </returns>
        ///
        /// <remarks>
        /// This method does not have to be purely syntactical parsing of the
        /// path.  It is encouraged that the provider actually use the path to
        /// lookup in its store and create a relative path that matches the
        /// casing, and standardized path syntax.
        /// 
        /// Note, the base class implemenation uses GetParentPath,
        /// GetChildName, and MakePath to normalize the path and then make it
        /// relative to basePath.  All string comparisons are done using
        /// StringComparison.InvariantCultureIgnoreCase.
        /// </remarks>
        protected override string NormalizeRelativePath(string path, string basePath)
        {
            using (LOG.InfoCall("NormalizeRelativePath"))
            {
                string nrp = base.NormalizeRelativePath(path, basePath);
                LOG.VerboseFormat("NormalizeRelativePath() = {0}", nrp);
                return nrp;
            }
        }

        #region Hacks

        private object GetCmdletProviderContext()
        {
            using (LOG.VerboseCall("GetCmdletProviderContext"))
            {
                PropertyInfo pi = GetType().GetProperty("Context",
                                          BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
                return pi.GetValue(this, null);
            }
        }

        private Cmdlet GetCmdlet(object cmdletProviderContext)
        {
            using (LOG.VerboseCall("GetCmdlet"))
            {
                FieldInfo fi = cmdletProviderContext.GetType().GetField("command");
                return (Cmdlet)fi.GetValue(cmdletProviderContext);
            }
        }

        private void SetDynamicParametersProperty(object cmdletProviderContext, object value)
        {
            using (LOG.VerboseCall("SetDynamicParametersProperty"))
            {
                PropertyInfo pi = cmdletProviderContext.GetType().GetProperty("DynamicParameters",
                                                                BindingFlags.Instance | BindingFlags.GetProperty |
                                                                BindingFlags.NonPublic);
                pi.SetValue(cmdletProviderContext, value, null);
            }
        }

        #endregion 

        #region Helpers

        /// <summary>
        /// Breaks up the path into individual elements.
        /// </summary>
        private static int[] emptyIntArray = new int[0];
        private static string[] emptyStringArray = new string[0];
        public const string PathEscapeChars = "\\/:";
        public const string SQLNameEscapeChars = "\\/:<>*?[]|";

        public static string EscapeString(string input)
        {
            return EscapeString(input, SQLNameEscapeChars);
        }

        public static string EscapeString(string input, string specials)
        {
            if (String.IsNullOrEmpty(input))
                return String.Empty;

            // convert escape characters to hex escape codes
            StringBuilder builder = new StringBuilder(input.Length * 2);
            foreach (char ch in input.ToCharArray())
            {
                if (Char.IsControl(ch) || specials.IndexOf(ch) != -1)
                    builder.AppendFormat("%{0:X2}", Convert.ToByte(ch));
                else
                    builder.Append(ch);
            }

            return builder.ToString();
        }

        public static string UnescapeString(string input)
        {
            using (LOG.VerboseCall("UnescapeString"))
            {
                if (String.IsNullOrEmpty(input))
                    return String.Empty;

                StringBuilder builder = new StringBuilder(input.Length);
                int n = input.Length;
                int j = 0;
                for (int i = input.IndexOf('%'); i >= 0; i = input.IndexOf('%', i))
                {
                    if (j < i)
                    {
                        builder.Append(input.Substring(j, i - j));
                    }

                    i++;
                    if (i + 1 < n)
                    {
                        int byteCode = Convert.ToInt32(input.Substring(i, 2), 16);
                        builder.Append(Convert.ToChar(byteCode));
                        i += 2;
                    }
                    else
                        builder.Append('%');

                    j = i;
                }

                if (builder.Length == 0)
                    return input;

                if (j < n)
                    builder.Append(input.Substring(j));

                return builder.ToString();
            }
        }

        private void WriteToPipeline(object value, string path, bool isContainer)
        {
            /*            PSObject psobject = value as PSObject;
                        if (psobject == null && value != null)
                        {
                            psobject = new PSObject(value);

                            PSNoteProperty note = new PSNoteProperty("PSPath", PSDriveInfo.Name + ":" + pathSeparator + path);

                            psobject.Properties.Add(note);
                        }
                        if (psobject != null)
             */

            WriteItemObject(value, "dm:\\" + path, isContainer);
        }


        public const string DOT = ".";
        public const string DOTDOT = "..";
        /// <summary>
        /// Adapts the path, making sure the correct path separator character is used.
        /// </summary>
        public static string NormalizePath(string path)
        {
            using (LOG.InfoCall("NormalizePath"))
            {
                LOG.VerboseFormat("path in: [{0}]", path);

                //       path = RemoveDrive(path);

                if (String.IsNullOrEmpty(path))
                {
                    LOG.Verbose("path out: is [] (empty string)");
                    return String.Empty;
                }
                List<string> parts = new List<string>();

                // resolve dots and convert all / to \
                foreach (string part in path.Split(new char[] { '\\', '/' }, StringSplitOptions.None))
                {
                    if (part.Length == 0)
                        continue;
                    if (String.Equals(part, DOT))
                        continue;
                    if (String.Equals(part, DOTDOT))
                    {
                        if (parts.Count > 0)
                            parts.RemoveAt(parts.Count - 1);
                        continue;
                    }
                    parts.Add(part);
                }
                // reassemble the path
                StringBuilder builder = new StringBuilder();
                foreach (string part in parts)
                {
                    if (builder.Length > 0)
                        builder.Append(pathSeparator);
                    builder.Append(part);
                }

                LOG.VerboseFormat("path out: [{0}]", builder);
                return builder.ToString();
            }
        }

        static string RemoveDrive(string path)
        {
            using (LOG.VerboseCall("RemoveDrive"))
            {
                if (String.IsNullOrEmpty(path))
                {
                    LOG.Verbose("path is empty");
                    return String.Empty;
                }

                LOG.VerboseFormat("path in is [{0}]", path);
                int cx = path.IndexOf(driveSeparator[0]);
                if (cx >= 0)
                {
                    int bsx = path.IndexOf(pathSeparator[0]);
                    if (bsx == cx + 1)
                        path = path.Substring(bsx);
                    else
                        path = path.Substring(cx);
                }
                if (path.StartsWith(pathSeparator) || path.StartsWith(driveSeparator))
                {
                    if (path.Length > 1)
                        path = path.Substring(1);
                    else
                        path = String.Empty;
                }

                LOG.VerboseFormat("path out is [{0}]", path);
                return path;
            }
        }

        private string[] NormalizeAndChunkPath(string path)
        {
            using (LOG.VerboseCall("NormalizeAndChunkPath"))
            {
                LOG.VerboseFormat("path=[{0}]", path);

                if (String.IsNullOrEmpty(path))
                    return emptyStringArray;

                path = NormalizePath(RemoveDriveFromPath(path));

                return path.Split(pathSeparator[0]);
            }
        }

        public const string driveSeparator = ":";
        public static string[] driveSeparatorArray = new string[] { driveSeparator };

        private SQLdmDriveInfo GetSQLdmDriveInfo(string path)
        {
            using (LOG.InfoCall("GetSQLdmDriveInfo"))
            {
                LOG.VerboseFormat("path=[{0}]", path);
                try
                {
                    if (path.Contains(driveSeparator))
                    {
                        string[] parts = path.Split(driveSeparatorArray, 2, StringSplitOptions.None);

                        // see if there is a drive that matches the specified name
                        PSDriveInfo drive = this.SessionState.Drive.Get(parts[0]);
                        if (drive is SQLdmDriveInfo)
                            return (SQLdmDriveInfo)drive;
                    }

                    if (PSDriveInfo == null)
                        return SessionState.Drive.Current as SQLdmDriveInfo;

                    return PSDriveInfo as SQLdmDriveInfo;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Checks if a given path is actually a drive name.
        /// </summary>
        private bool PathIsDrive(string path)
        {
            using (LOG.VerboseCall("PathIsDrive"))
            {
                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);
                if (drive != null)
                {
                    // Remove the drive name and first path separator.  If the 
                    // path is reduced to nothing, it is a drive. Also if its
                    // just a drive then there wont be any path separators
                    if (String.IsNullOrEmpty(path.Replace(drive.Root, "")) ||
                        String.IsNullOrEmpty(path.Replace(drive.Root + pathSeparator, "")))
                        return true;

                    if (path.Contains(driveSeparator))
                    {
                        string[] parts = path.Split(driveSeparatorArray, 2, StringSplitOptions.None);
                        if (parts.Length == 1)
                            return true;

                        if (String.IsNullOrEmpty(parts[1]) || parts[1] == "\\")
                            return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Ensures that the drive is removed from the specified path
        /// </summary>
        /// 
        /// <param name="path">Path from which drive needs to be removed</param>
        /// <returns>Path with drive information removed</returns>
        private string RemoveDriveFromPath(string path)
        {
            using (LOG.VerboseCall("RemoveDriveFromPath"))
            {
                string result = path;
                string root;

                SQLdmDriveInfo drive = GetSQLdmDriveInfo(path);

                if (drive == null)
                {
                    root = String.Empty;
                }
                else
                {
                    root = drive.Root;
                }

                if (result == null)
                {
                    result = String.Empty;
                }

                if (result.Contains(root))
                {
                    result = result.Substring(result.IndexOf(root, StringComparison.OrdinalIgnoreCase) + root.Length);
                }

                return result;
            }
        }

        private static PathType GetPathType(string[] parts)
        {
            using (LOG.InfoCall("GetPathType"))
            {
                if (parts == null || parts.Length == 0 || String.IsNullOrEmpty(parts[0]))
                    return PathType.Root;

                // compare all this crap in lowercase
                switch (parts[0].ToLower())
                {
                    case InstancesInfo.ContainerNameLower:
                        if (parts.Length == 1)
                            return PathType.Instances;
                        if (parts.Length == 2)
                            return PathType.Instance;
                        switch (parts[2].ToLower())
                        {
                            case AlertsInfo.ContainerNameLower:
                                if (parts.Length == 3)
                                    return PathType.Alerts;
                                if (parts.Length == 4)
                                    return PathType.Alert;
                                break;
                        }
                        ;
                        return PathType.Invalid;
                    case AppSecurityInfo.ContainerNameLower:
                        if (parts.Length == 1)
                            return PathType.AppSecurity;
                        if (parts.Length == 2)
                            return PathType.AppUser;
                        if (parts.Length == 3)
                            return PathType.AppUserPermission;
                        break;

                    /* not doing custom counters right now...
                                case "CustomCounters":
                                    if (parts.Length == 1)
                                        return PathType.CustomCounters;
                                    if (parts.Length == 2)
                                        return PathType.CustomCounter;
                                    return PathType.Invalid;
                */

                    default:
                        return PathType.Invalid;
                }
                return PathType.Invalid;
            }
        }

        #endregion
    }
}


