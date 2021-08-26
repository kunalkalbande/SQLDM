//------------------------------------------------------------------------------
// <copyright file="DiskCollectionSettings.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Configuration
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    public class DiskCollectionSettings
    {
        private bool autoDiscover;
        private List<string> drives;
        private int? cookedDiskDriveWaitTimeInSeconds = null;

        public DiskCollectionSettings()
        {
            autoDiscover = true;
        }

        public DiskCollectionSettings(DiskCollectionSettings copy)
        {
            AutoDiscover = copy.AutoDiscover;
            Drives = copy.Drives;
            CookedDiskDriveWaitTimeInSeconds = copy.CookedDiskDriveWaitTimeInSeconds;
        }

        [Auditable("Enabled auto discovery of disk drives")]
        public bool AutoDiscover
        {
            get { return autoDiscover;  }
            set { autoDiscover = value; }
        }

        public int? CookedDiskDriveWaitTimeInSeconds
        {
            get { return cookedDiskDriveWaitTimeInSeconds; }
            set { cookedDiskDriveWaitTimeInSeconds = value; }
        }

        [XmlArrayItem("Drives")]
        [Auditable(false)]
        public string[] Drives
        {
            get
            {
                if (drives == null || drives.Count == 0)
                    return new string[0];

                return drives.ToArray();
            }
            set
            {
                if (value == null || value.Length == 0)
                    drives = null;
                else
                {
                    if (drives == null)
                        drives = new List<string>();
                    else
                        drives.Clear();
                    drives.AddRange(value);
                    drives.Sort();
                }
            }
        }
    }
}
