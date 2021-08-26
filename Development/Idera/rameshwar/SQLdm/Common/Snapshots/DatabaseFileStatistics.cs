using System;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --adding the class for the database files
    /// </summary>
    [Serializable]
    public class DatabaseFileStatistics
    {
        #region fields

        private int? fileID = null;
        private double maxSize;
        private double initialSize;
        private double usedSpace;
        private double availableSpace;
        private double freeDiskSpace;
        private string driveName;
        private string databaseName;
        private string fileGroupName;
        private string fileName;
        private string filePath;
        private double totalPossibleFileSize;
        private DateTime? collectionDateTime = null;
        private bool isDataFile;

        #endregion

        #region properties

        public DateTime? UTCCollectionDateTime
        {
            get { return collectionDateTime; }
            set { collectionDateTime = value; }
        }

        public int? FileID
        {
            get { return fileID; }
            set { fileID = value; }
        }

        public double MaximumSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        public double InitialSize
        {
            get { return initialSize; }
            set { initialSize = value; }
        }

        public double UsedSpace
        {
            get { return usedSpace; }
            set { usedSpace = value; }
        }

        public double AvailableSpace
        {
            get { return availableSpace; }
            set { availableSpace = value; }
        }

        public double FreeDiskSpace
        {
            get { return freeDiskSpace; }
            set { freeDiskSpace = value; }
        }

        public string DriveName
        {
            get { return driveName; }
            set { driveName = value; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public bool IsDataFile
        {
            get { return isDataFile; }
            set { isDataFile = value; }
        }

        public string FileGroupName
        {
            get { return fileGroupName; }
            set { fileGroupName = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public double TotalPossibleFileSize
        {
            get { return totalPossibleFileSize; }
            set { totalPossibleFileSize = value; }
        }

        #endregion

    }

}
