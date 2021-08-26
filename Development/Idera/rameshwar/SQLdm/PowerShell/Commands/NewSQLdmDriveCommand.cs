//------------------------------------------------------------------------------
// <copyright file="NewSQLdmDrive.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;
    using Objects;

    [Cmdlet(VerbsCommon.New, "SQLdmDrive", SupportsShouldProcess = false)]
    public class NewSQLdmDriveCommand : PSCmdlet
    {        
        [Parameter(Mandatory=true, Position=0)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string name;

        [Parameter(Mandatory = true, Position = 1)]
        public string RepositoryInstance
        {
            get { return instanceName; }
            set { instanceName = value; }
        }
        private string instanceName;

        [Parameter(Mandatory = true, Position=2)]
        public string RepositoryName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }
        private string databaseName;

        [Credential]
        [Parameter]
        public PSCredential Credential  
        {
            get { return credential; }
            set { credential = value; }
        }
        private PSCredential credential;

        protected override void BeginProcessing()
        {
            // build the root path
//            string[] parts = instanceName.Split('\\');
//            StringBuilder builder = new StringBuilder();
//            builder.Append(parts[0]).Append('\\');
//            if (parts.Length > 1)
//                builder.Append(parts[1]);
//            else
//                builder.Append("DEFAULT");
//            builder.Append('\\');
//            builder.Append(databaseName);

            Collection<ProviderInfo> providerInfoList = this.SessionState.Provider.Get("SQLdm");
            if (providerInfoList.Count < 1)
            {
                WriteError(new ErrorRecord(
                               new PSInvalidOperationException("Unable to locate drive provider"),
                               "BadProvider",
                               ErrorCategory.InvalidArgument,
                               "SQLdm")
                    );
            }

            string root = Name + SQLdmProvider.driveSeparator + SQLdmProvider.pathSeparator;

            // fill out a drive info object
            SQLdmDriveInfo newDriveInfo = new SQLdmDriveInfo(
                name, providerInfoList[0], root, "SQL Diagnostic Manager drive", credential, instanceName, databaseName);

            // create the drive and write it to the pipeline
            object result = SessionState.Drive.New(newDriveInfo, "global");
            if (result != null)
                WriteObject(result);
        }
    }
}
