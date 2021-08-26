//------------------------------------------------------------------------------
// <copyright file="DatabaseFile.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Globalization;
using System.Text;

namespace Idera.SQLdm.Common.Snapshots
{
	using System;

	/// <summary>
	/// Represents a database file, which could be a data file or a log file.
	/// </summary>
	[Serializable]
	public class DatabaseFile : DatabaseFileBase
	{
		#region fields

	    
        private string logicalFilename = null;
        private int? configuredGrowth = null;
        private FileSize freeSpaceOnDisk = new FileSize();
        private int? configuredMaxSize = null;
        private string filePath = null;
        private int statusBits = 0;
        private bool isDataFile = true;
	    private string driveName = null;

        
		#endregion

		#region constructors
		
		#endregion

		#region properties


        /// <summary>
        /// Logical file name
        /// </summary>
	    public string LogicalFilename
	    {
	        get { return logicalFilename; }
	        internal set { logicalFilename = value; }
	    }

        /// <summary>
        /// Growth value as configured on server.  Not for display.
        /// </summary>
	    internal int? ConfiguredGrowth
	    {
	        get { return configuredGrowth; }
	        set { configuredGrowth = value; }
	    }

        /// <summary>
        /// Total free space on disk.
        /// </summary>
	    public FileSize FreeSpaceOnDisk
	    {
	        get { return freeSpaceOnDisk; }
	        internal set { freeSpaceOnDisk = value; }
	    }

        /// <summary>
        /// Configured max size.  Not for display.
        /// </summary>
	    internal int? ConfiguredMaxSize
	    {
	        get { return configuredMaxSize; }
	        set { configuredMaxSize = value; }
	    }

        /// <summary>
        /// Path to file on disk
        /// </summary>
	    public string FilePath
	    {
	        get { return filePath; }
	        internal set { filePath = value; }
	    }

        /// <summary>
        /// Status bits.  Not for display.
        /// </summary>
	    internal int StatusBits
	    {
	        get { return statusBits; }
	        set { statusBits = value; }
	    }

        /// <summary>
        /// The file is a data file
        /// </summary>
	    public bool IsDataFile
	    {
	        get { return isDataFile; }
	        internal set { isDataFile = value; }
        }

	    public string DriveName
	    {
	        get { return driveName; }
	        set { driveName = value; }
	    }

        // For Tempdb files only
        public TempdbFileActivity TempdbFileActivity { get; internal set; }

	    #region calculated properties

        /// <summary>
        /// Returns whether growth is in percent.  If false, growth is in pages.
        /// </summary>
	    public bool GrowthIsInPercent
	    {
	        get
	        {
                return (statusBits & 0x100000) == 0x100000;
	        }
	    }

        /// <summary>
        /// Returns whether growth is in pages.  If false, growth is in percent.
        /// </summary>
	    public bool GrowthIsInPages
	    {
	        get
	        {
                return !GrowthIsInPercent;
	        }
	    }

        /// <summary>
        /// Identifier for internal use only
        /// </summary>
	    public string InternalIdentifier
	    {
            get { return DatabaseName + LogicalFilename; }
	    }

        

        #endregion

        #endregion

        #region methods

        public string GrowthLabel(CultureInfo culture)
	    {
            if (configuredGrowth.HasValue && configuredGrowth.Value > 0 && configuredMaxSize.HasValue && configuredMaxSize != 0)
            {
                StringBuilder returnString = new StringBuilder("Grows by ");
                if (GrowthIsInPercent)
                {
                    returnString.Append(configuredGrowth.Value.ToString("N2", culture.NumberFormat));
                    returnString.Append("%");
                }
                else
                {
                    FileSize growthFileSize = new FileSize();
                    growthFileSize.Pages = configuredGrowth;
                    returnString.Append(growthFileSize.AsString(culture));
                }
                returnString.Append(", ");

                if (configuredMaxSize < 0)
                {
                    returnString.Append("Unrestricted growth");
                }
                else
                {
                    if (configuredMaxSize == 268435456)
                    {
                        returnString.Append("Limited to 2 TB");
                    }
                    else
                    {
                        FileSize growthFileMax = new FileSize();
                        growthFileMax.Pages = configuredMaxSize;
                        returnString.Append("Limited to ");
                        returnString.Append(growthFileMax.AsString(culture));
                    }
                }
                return returnString.ToString();
            }
            else
            {
                return "No growth.";
            }
	    }

        #endregion
    }
}
