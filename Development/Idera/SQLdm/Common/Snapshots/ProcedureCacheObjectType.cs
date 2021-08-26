//------------------------------------------------------------------------------
// <copyright file="ProcedureCacheObjectType.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Statistics for a specific procedure cache object type
    /// </summary>
    [Serializable]
    public sealed class ProcedureCacheObjectType
    {
        #region fields

        private Double? hitRatio = null;
        private string objectTypeName = null;
        private FileSize size = null;
        private Int64? objectCount = null;
        private Int64? useCount = null;

        #endregion

        #region constructors

        internal ProcedureCacheObjectType(string objectTypeName)
        {
            this.objectTypeName = objectTypeName;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets hitRatio for the procedure cache summary object
        /// </summary>
        public double? HitRatio
        {
            get { return hitRatio; }
            set { hitRatio = value; }
        }

        /// <summary>
        /// Gets name for the procedure cache summary object
        /// </summary>
        public string LongName
        {
            get
            {
                return GetLongName(ObjectTypeName);
            }
        }

        /// <summary>
        /// Object type name
        /// </summary>
        public string ObjectTypeName
        {
            get { return objectTypeName; }
            set { objectTypeName = value; }
        }

        /// <summary>
        /// Gets size for the procedure cache summary object
        /// </summary>
        public FileSize Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Count of objects of this type
        /// Only available if procedure cache list has been gathered
        /// </summary>
        public long? ObjectCount
        {
            get { return objectCount; }
            set { objectCount = value; }
        }

        /// <summary>
        /// Use count of objects of this type
        /// Only available if procedure cache list has been gathered
        /// </summary>
        public long? UseCount
        {
            get { return useCount; }
            set { useCount = value; }
        }

        #endregion

        #region methods

        public static string GetLongName(string shortname)
        {
            switch (shortname.ToLower().Trim())
            {
                case "adhoc":
                    return "Ad hoc query";
                case "check":
                    return "Check";
                case "cursors":
                    return "Cursor";
                case "default":
                    return "Default";
                case "extended procedure":
                    return "Extended procedure";
                case "prepared":
                    return "Prepared statement";
                case "proc":
                    return "Stored procedure";
                case "replproc":
                    return "Replication procedure";
                case "rule":
                    return "Rule";
                case "trigger":
                    return "Trigger";
                case "view":
                    return "View";
                case "usertab":
                case "usrtab":
                    return "User table";
                case "systab":
                    return "System table";
                default:
                    return shortname;
            }
        }

        #endregion

    }
}
