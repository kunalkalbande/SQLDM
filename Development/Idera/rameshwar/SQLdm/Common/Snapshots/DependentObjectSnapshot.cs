//------------------------------------------------------------------------------
// <copyright file="DependentObjectSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
// Author : Srishti Purohit
// Date : 21Aug2015
// Release : SQLdm 10.0
//------------------------------------------------------------------------------

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Data;

    [Serializable]
    public sealed class DependentObjectSnapshot : Snapshot
    {
        DataTable dependentObject;

        public DataTable DependentObject
        {
            get { return dependentObject; }
            set { dependentObject = value; }
        }

        public DependentObjectSnapshot()
        {
            dependentObject = new DataTable();
        }
    }

}
