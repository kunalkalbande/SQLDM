//------------------------------------------------------------------------------
// <copyright file="MachineNameSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

	//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- New Class MachineNameSnapshot which contains MachineName whichever connecting in the given order 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
    [Serializable]
    public sealed class MachineNameSnapshot : Snapshot
    {
        string _machineName;

        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; }
        }
    }

}
