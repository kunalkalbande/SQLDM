//------------------------------------------------------------------------------
// <copyright file="DatabaseFileGroup.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Globalization;
using System.Text;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a database filegroup
    /// </summary>
    [Serializable]
    public sealed class DatabaseFileGroup : DatabaseFileBase
    {
        #region fields

        private int fileCount = 0;
        private AutogrowStatus autogrow = AutogrowStatus.Unknown;
        private FileSize dataSize = new FileSize();
        private FileSize indexSize = new FileSize();
        private FileSize textSize = new FileSize();


        #endregion

        #region constructors

        #endregion

        #region properties

        /// <summary>
        /// Represents the rollup of autogrow settings for the filegroup
        /// </summary>
        public AutogrowStatus Autogrow
        {
            get { return autogrow; }
            // Use SetAutogrow unless you know for sure what this value is
            internal set { autogrow = value; }
        }

        /// <summary>
        /// Gets the current data size used of the allocated data file size. This does NOT include text and index.
        /// </summary>
        public FileSize DataSize
        {
            get { return dataSize; }
            internal set { dataSize = value; }
        }

        /// <summary>
        /// Count of files
        /// </summary>
        public int FileCount
        {
            get { return fileCount; }
            internal set { fileCount = value; }
        }

        /// <summary>
        /// Gets the current index size used.
        /// </summary>
        public FileSize IndexSize
        {
            get { return indexSize; }
            internal set { indexSize = value; }
        }

        /// <summary>
        /// Gets the current text size used.
        /// </summary>
        public FileSize TextSize
        {
            get { return textSize; }
            internal set { textSize = value; }
        }

        #region calculated properties

        /// <summary>
        /// Identifier for internal use only
        /// </summary>
        public string InternalIdentifier
        {
            get { return DatabaseName + FilegroupName; }
        }
	    

        #endregion

        #endregion

        #region methods

        /// <summary>
        /// Data size as a percent of the current size of the filegroup
        /// </summary>
        public float? PercentDataSizeCurrent
        {
            get
            {
                if (DataSize.Bytes.HasValue && CurrentSize.Bytes.HasValue)
                {
                    return (float)DataSize.Bytes.Value / (float)CurrentSize.Bytes.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Text size as a percent of the current size of the filegroup
        /// </summary>
        public float? PercentTextSizeCurrent
        {
            get
            {
                if (TextSize.Bytes.HasValue && CurrentSize.Bytes.HasValue)
                {
                    return (float)TextSize.Bytes.Value / (float)CurrentSize.Bytes.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Index size as a percent of the current size of the filegroup
        /// </summary>
        public float? PercentIndexSizeCurrent
        {
            get
            {
                if (IndexSize.Bytes.HasValue && CurrentSize.Bytes.HasValue)
                {
                    return (float)IndexSize.Bytes.Value / (float)CurrentSize.Bytes.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        // Autogrow Flags
        // 1 = data autogrow on
        // 4 = data autogrow off
        // 8 = log autogrow on
        // 16 = log autogrow off
        internal void SetAutogrow(int autogrowFlag)
        {
            if (((autogrowFlag & 1) == 1) && ((autogrowFlag & 4) == 4))
            {
                autogrow = AutogrowStatus.Mixed;
            }
            else
            {
                if (((autogrowFlag & 1) == 1))
                {
                    autogrow = AutogrowStatus.AutogrowOn;
                }
                else
                {
                    if (((autogrowFlag & 4) == 4))
                        autogrow = AutogrowStatus.AutogrowOff;
                }
            }

            if (((autogrowFlag & 8) == 8) && ((autogrowFlag & 16) == 16))
            {
                autogrow = AutogrowStatus.Mixed;
            }
            else
            {
                if (((autogrowFlag & 8) == 8))
                {
                    autogrow = AutogrowStatus.AutogrowOn;
                }
                else
                {
                    if (((autogrowFlag & 16) == 16))
                        autogrow = AutogrowStatus.AutogrowOff;
                }
            }
        }

        #endregion
    }
}
