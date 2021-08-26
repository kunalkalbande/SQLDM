//------------------------------------------------------------------------------
// <copyright file="LockStatistics.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a group of lock counter statistics
    /// </summary>
    [Serializable]
    public sealed class LockStatistics : ISerializable
    {
        #region fields

        private LockCounter databaseCounters;
        private LockCounter extentCounters;
        private LockCounter keyCounters;
        private LockCounter latchCounters;
        private LockCounter pageCounters;
        private LockCounter ridCounters;
        private LockCounter tableCounters;
        private LockCounter totalCounters;
        
        // SQL 2005 Counters
        private LockCounter allocUnitCounters;
        private LockCounter applicationCounters;
        private LockCounter fileCounters;
        private LockCounter heapCounters;
        private LockCounter metadataCounters;
        private LockCounter objectCounters;


        #endregion

        #region constructors

        internal LockStatistics()
        {
            databaseCounters = new LockCounter();
            extentCounters = new LockCounter();
            keyCounters = new LockCounter();
            latchCounters = new LockCounter();
            pageCounters = new LockCounter();
            ridCounters = new LockCounter();
            tableCounters = new LockCounter();
            totalCounters = new LockCounter();

            // SQL 2005 Counters
            allocUnitCounters = new LockCounter();
            applicationCounters = new LockCounter();
            fileCounters = new LockCounter();
            heapCounters = new LockCounter();
            metadataCounters = new LockCounter();
            objectCounters = new LockCounter();
        }

        internal LockStatistics(LockCounter databaseCounters, LockCounter extentCounters, LockCounter keyCounters, LockCounter latchCounters, LockCounter pageCounters, LockCounter ridCounters, LockCounter tableCounters, LockCounter totalCounters, LockCounter allocUnitCounters, LockCounter applicationCounters, LockCounter fileCounters, LockCounter heapCounters, LockCounter metadataCounters, LockCounter objectCounters)
        {
            this.databaseCounters = databaseCounters;
            this.extentCounters = extentCounters;
            this.keyCounters = keyCounters;
            this.latchCounters = latchCounters;
            this.pageCounters = pageCounters;
            this.ridCounters = ridCounters;
            this.tableCounters = tableCounters;
            this.totalCounters = totalCounters;
            this.allocUnitCounters = allocUnitCounters;
            this.applicationCounters = applicationCounters;
            this.fileCounters = fileCounters;
            this.heapCounters = heapCounters;
            this.metadataCounters = metadataCounters;
            this.objectCounters = objectCounters;
        }

        public LockStatistics(SerializationInfo info, StreamingContext context)
        {
            databaseCounters = (LockCounter)info.GetValue("databaseCounters", typeof(LockCounter));
            extentCounters = (LockCounter)info.GetValue("extentCounters", typeof(LockCounter));
            keyCounters = (LockCounter)info.GetValue("keyCounters", typeof(LockCounter));
            latchCounters = (LockCounter)info.GetValue("latchCounters", typeof(LockCounter));
            pageCounters = (LockCounter)info.GetValue("pageCounters", typeof(LockCounter));
            ridCounters = (LockCounter)info.GetValue("ridCounters", typeof(LockCounter));
            tableCounters = (LockCounter)info.GetValue("tableCounters", typeof(LockCounter));
            totalCounters = (LockCounter)info.GetValue("totalCounters", typeof(LockCounter));
            allocUnitCounters = (LockCounter)info.GetValue("allocUnitCounters", typeof(LockCounter));
            applicationCounters = (LockCounter)info.GetValue("applicationCounters", typeof(LockCounter));
            fileCounters = (LockCounter)info.GetValue("fileCounters", typeof(LockCounter));
            heapCounters = (LockCounter)info.GetValue("heapCounters", typeof(LockCounter));
            metadataCounters = (LockCounter)info.GetValue("metadataCounters", typeof(LockCounter));
            objectCounters = (LockCounter)info.GetValue("objectCounters", typeof(LockCounter));
        }

        #endregion


        #region properties

        /// <summary>
		/// Gets databaseCounters for the lock
		/// </summary>
		public LockCounter DatabaseCounters
		{
			get { return databaseCounters; }
            internal set { databaseCounters = value; }
		}

		/// <summary>
		/// Gets extentCounters for the lock
		/// </summary>
		public LockCounter ExtentCounters
		{
			get { return extentCounters; }
		    internal set { extentCounters = value; } 

		}

		/// <summary>
		/// Gets keyCounters for the lock
		/// </summary>
		public LockCounter KeyCounters
		{
			get { return keyCounters; }
		    internal set { keyCounters = value; }
		}

		/// <summary>
		/// Gets latchCounters for the lock
		/// </summary>
		public LockCounter LatchCounters
		{
			get { return latchCounters; }
		    internal set { latchCounters = value; }
		}

		/// <summary>
		/// Gets pageCounters for the lock
		/// </summary>
		public LockCounter PageCounters
		{
			get { return pageCounters; }
		    internal set { pageCounters = value; }
    	}

		/// <summary>
		/// Gets ridCounters for the lock
		/// </summary>
		public LockCounter RidCounters
		{
			get { return ridCounters; }
		    internal set { ridCounters = value; }
		}

		/// <summary>
		/// Gets tableCounters for the lock
		/// </summary>
		public LockCounter TableCounters
		{
			get { return tableCounters; }
		    internal set { tableCounters = value; }
		}

		/// <summary>
		/// Gets totalCounters for the lock
		/// </summary>
		public LockCounter TotalCounters
		{
			get { return totalCounters; }
		    internal set { totalCounters = value; }
		}

		/// <summary>
		/// Gets AllocUnitCounters for the lock
		/// </summary>
		public LockCounter AllocUnitCounters
		{
			get { return allocUnitCounters; }
		    internal set { allocUnitCounters = value; }
		}

		/// <summary>
		/// Gets ApplicationCounters for the lock
		/// </summary>
		public LockCounter ApplicationCounters
		{
			get { return applicationCounters; }
		    internal set { applicationCounters = value; }
		}
		/// <summary>
		/// Gets FileCounters for the lock
		/// </summary>
		public LockCounter FileCounters
		{
			get { return fileCounters; }
		    internal set { fileCounters = value; }
		}
		/// <summary>
		/// Gets HeapCounters for the lock
		/// </summary>
		public LockCounter HeapCounters
		{
			get { return heapCounters; }
		    internal set { heapCounters = value; }
		}
		/// <summary>
		/// Gets MetadataCounters for the lock
		/// </summary>
		public LockCounter MetadataCounters
		{
			get { return metadataCounters; }
		    internal set { metadataCounters = value; }
		}
		/// <summary>
		/// Gets ObjectCounters for the lock
		/// </summary>
		public LockCounter ObjectCounters
		{
			get { return objectCounters; }
		    internal set { objectCounters = value; }
		}


		#endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations
       
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("databaseCounters", databaseCounters);
            info.AddValue("extentCounters", extentCounters);
            info.AddValue("keyCounters", keyCounters);
            info.AddValue("latchCounters", latchCounters);
            info.AddValue("pageCounters", pageCounters);
            info.AddValue("ridCounters", ridCounters);
            info.AddValue("tableCounters", tableCounters);
            info.AddValue("totalCounters", totalCounters);
            info.AddValue("allocUnitCounters", allocUnitCounters);
            info.AddValue("applicationCounters", applicationCounters);
            info.AddValue("fileCounters", fileCounters);
            info.AddValue("heapCounters", heapCounters);
            info.AddValue("metadataCounters", metadataCounters);
            info.AddValue("objectCounters", objectCounters);
        }

        #endregion

        #region nested types

        #endregion

    }
}
