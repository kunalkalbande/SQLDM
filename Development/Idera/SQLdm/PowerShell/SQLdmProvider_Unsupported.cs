//------------------------------------------------------------------------------
// <copyright file="SQLdmProvider_Unsupported.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell
{
    using System.Management.Automation;

    public partial class SQLdmProvider
    {
        protected override void ClearItem(string path)
        {
            WriteError(
                new ErrorRecord(new NotImplementedException("The SQLDM Provider does not support the Clear-Item command."),
                                "NotImplemented", ErrorCategory.NotImplemented, path));
        }

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            WriteError(
                new ErrorRecord(new NotImplementedException("The SQLDM Provider does not support the Copy-Item command."),
                                "NotImplemented", ErrorCategory.NotImplemented, path));
        }

        protected override void MoveItem(string path, string destination)
        {
            WriteError(new ErrorRecord(new NotImplementedException("The SQLDM Provider does not support the Move-Item command."),
                                 "NotImplemented", ErrorCategory.NotImplemented, path));
        }

        protected override void RenameItem(string path, string newName)
        {
            WriteError(
                new ErrorRecord(new NotImplementedException("The SQLDM Provider does not support the Rename-Item command."),
                                "NotImplemented", ErrorCategory.NotImplemented, path));
        }

        protected override void InvokeDefaultAction(string path)
        {
            WriteError(
                new ErrorRecord(new NotImplementedException("The SQLDM Provider does not support the Invoke-Action command."),
                                "NotImplemented", ErrorCategory.NotImplemented, path));
        }


    }
}
