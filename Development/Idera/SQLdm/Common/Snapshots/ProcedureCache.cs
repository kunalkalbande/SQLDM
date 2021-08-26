//------------------------------------------------------------------------------
// <copyright file="ProcedureCache.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
	/// High level procedure cache diagnostic data for a server
	/// </summary>
    [Serializable]
    public sealed class ProcedureCache : Snapshot
    {

        #region fields

        private Double? hitRatio = null;
		private Double? hitRatioBase = null;
        private Dictionary<string,ProcedureCacheObjectType> objectTypes = new Dictionary<string,ProcedureCacheObjectType>();
        private DataTable objectList;
	
		#endregion

		#region constructors


        internal ProcedureCache(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            ObjectTypes.Add("adhoc", new ProcedureCacheObjectType("adhoc"));
            ObjectTypes.Add("check", new ProcedureCacheObjectType("check"));
            ObjectTypes.Add("default", new ProcedureCacheObjectType("default"));
            ObjectTypes.Add("extended procedure", new ProcedureCacheObjectType("extended procedure"));
            ObjectTypes.Add("prepared", new ProcedureCacheObjectType("prepared"));
            ObjectTypes.Add("proc", new ProcedureCacheObjectType("proc"));
            ObjectTypes.Add("replproc", new ProcedureCacheObjectType("replproc"));
            ObjectTypes.Add("rule", new ProcedureCacheObjectType("rule"));
            ObjectTypes.Add("trigger", new ProcedureCacheObjectType("trigger"));
            ObjectTypes.Add("view", new ProcedureCacheObjectType("view"));

            objectList = new DataTable("Procedure Cache");
            ObjectList.RemotingFormat = SerializationFormat.Binary;
            objectList.Columns.Add("Command", typeof(string));
            objectList.Columns.Add("Object Type", typeof(string));
            objectList.Columns.Add("Size", typeof(decimal));
            objectList.Columns.Add("Reference Count", typeof(Int64));
            objectList.Columns.Add("Use Count", typeof(Int64));
            objectList.Columns.Add("User/Schema Name", typeof(string));
           
        }
	
		#endregion

		#region properties

		/// <summary>
		/// Gets the total procedure cache hit ratio
		/// </summary>
		public Double? HitRatio
		{
			get { return hitRatio; }
            internal set { hitRatio = value;}
		}

		/// <summary>
		/// Gets the hit ratio base
		/// </summary>
		public Double? HitRatioBase
		{
			get { return hitRatioBase; }
            internal set { hitRatioBase = value; }
		}

        /// <summary>
        /// Gets the list of object types in procedure cache
        /// </summary>
        public Dictionary<string, ProcedureCacheObjectType> ObjectTypes
        {
            get { return objectTypes; }
            internal set { objectTypes = value; }
        }

        /// <summary>
        /// Returns the procedure cache object list if applicable
        /// </summary>
        public DataTable ObjectList
        {
            get { return objectList; }
            internal set { objectList = value; }
        }

        #endregion

		#region methods

        #endregion

    }
}
