//------------------------------------------------------------------------------
// <copyright file="DatabaseFileBase.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Base class for files and filegroups
    /// </summary>
    [Serializable]
    public abstract class DatabaseFileBase
    {
        #region fields

        private string databaseName = null;
        private string filegroupName = null;
        private FileSize currentSize = new FileSize();
        private FileSize currentUsedSize = new FileSize();
        private FileSize expansionSpace = new FileSize();

        #endregion

        #region constructors

        #endregion

        #region properties

        /// <summary>
        /// Name of database
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            internal set { databaseName = value; }
        }

        /// <summary>
        /// Filegroup name
        /// </summary>
        public string FilegroupName
        {
            get { return filegroupName; }
            internal set { filegroupName = value; }
        }

        /// <summary>
        /// Current total file size
        /// </summary>
        public FileSize CurrentSize
        {
            get { return currentSize; }
            internal set { currentSize = value; }
        }

        /// <summary>
        /// Current used file size
        /// </summary>
        public FileSize CurrentUsedSize
        {
            get { return currentUsedSize; }
            internal set { currentUsedSize = value; }
        }

        /// <summary>
        /// Current free size within currently allocated file
        /// </summary>
        public FileSize CurrentFreeSize
        {
            get
            {
                return new FileSize(CurrentSize.Kilobytes - CurrentUsedSize.Kilobytes);
            }
        }

        /// <summary>
        /// Current potential free size within maximum potential
        /// </summary>
        public FileSize CurrentPotentialFreeSize
        {
            get
            {
                return new FileSize(CurrentFreeSize.Kilobytes + ExpansionSpace.Kilobytes);
            }
        }

        /// <summary>
        /// Space available to expand to.  Will be 0 if growth is not possible.
        /// </summary>
        public FileSize ExpansionSpace
        {
            get { return expansionSpace; }
            internal set { expansionSpace = value; }
        }

        /// <summary>
        /// Percent of file currently used
        /// </summary>
        public float? PercentUsed
        {
            get
            {
                if (CurrentSize.Kilobytes.HasValue && CurrentSize.Kilobytes > 0)
                {
                    return (float?)(CurrentUsedSize.Kilobytes / CurrentSize.Kilobytes);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Percent of maximum expandable file size used
        /// </summary>
        public float? PercentFull
        {
            get
            {
                if (CurrentSize.Kilobytes.HasValue && ExpansionSpace.Kilobytes.HasValue
                    && CurrentSize.Kilobytes + ExpansionSpace.Kilobytes > 0)
                {
                    return (float?)(CurrentUsedSize.Kilobytes / (CurrentSize.Kilobytes + ExpansionSpace.Kilobytes));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Percent of maximum potential file size not being used whether allocated or not (1 - PercentFull)
        /// </summary>
        public float? PercentFreePotential
        {
            get
            {
                return 1 - PercentFull;
            }
        }

        /// <summary>
        /// Free size as a percent of the current size of the filegroup (1 - PercentUsed)
        /// </summary>
        public float? PercentFreeCurrent
        {
            get
            {
                return 1 - PercentUsed;
            }
        }

        /// <summary>
        /// Maximum potential file size
        /// </summary>
        public FileSize MaximumPotentialFileSize
        {
            get
            {
                return new FileSize(CurrentSize.Kilobytes + ExpansionSpace.Kilobytes);
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
