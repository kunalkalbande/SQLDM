//------------------------------------------------------------------------------
// <copyright file="FileSize.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;
    using Idera.SQLdm.Common.Attributes;

    /// <summary>
    /// Representation of a file size, intended simplify converting between file size denominations such as KB and MB.
    /// <preliminary/>
    /// </summary>
    [Serializable]
    public sealed class FileSize
    {
        #region fields

        private decimal? _bytes = null;

        #endregion

        #region constructors

        /// <summary>
        /// Create new filesize object
        /// </summary>
        public FileSize()
        {
            
        }

        public FileSize Copy()
        {
            var filesize = new FileSize();
            if (_bytes.HasValue)
                filesize._bytes = _bytes;
            return filesize;
        }

        /// <summary>
        /// Create new filesize object with kilobytes
        /// </summary>
        /// <param name="kilobytes">File size in kilobytes</param>
        public FileSize(decimal? kilobytes)
        {
            _bytes = ToSmallerDenomination(kilobytes);
        }

        #endregion

        #region properties

        /// <summary>
        /// Get or set file size in bytes
        /// </summary>
        [Auditable(false)]
        public decimal? Bytes
        {
            get
            {
                return _bytes;
            }
            set
            {
                //Do not allow fractional bytes due to conversion
                if (value.HasValue)
                {
                    _bytes = Math.Floor(value.Value);
                }
            }
        }

        /// <summary>
        /// Get or set file size in kilobytes
        /// </summary>
        [XmlIgnore]
        [Auditable("Table Size changed to")]
        public decimal? Kilobytes
        {
            get
            {
                return ToLargerDenomination(Bytes);
            }
            set
            {
                Bytes = ToSmallerDenomination(value);
            }
        }

        /// <summary>
        /// Get or set file size in SQL Server pages
        /// </summary>
        [XmlIgnore]
        [Auditable(false)]
        public decimal? Pages
        {
            get
            {
                return Kilobytes / 8;
            }
            set
            {
                Kilobytes = value*8;
            }
        }

        /// <summary>
        /// Get or set file size in megabytes
        /// </summary>
        [XmlIgnore]
        [Auditable(false)]
        public decimal? Megabytes
        {
            get
            {
                return ToLargerDenomination(Kilobytes);
            }
            set
            {
                Kilobytes = ToSmallerDenomination(value);
            }
        }

        /// <summary>
        /// Get or set file size in gigabytes
        /// </summary>
        [XmlIgnore]
        [Auditable(false)]
        public decimal? Gigabytes
        {
            get
            {
                return ToLargerDenomination(Megabytes);
            }
            set
            {
                Megabytes = ToSmallerDenomination(value);
            }
        }

        /// <summary>
        /// Get or set file size in terabytes
        /// </summary>
        [XmlIgnore]
        [Auditable(false)]
        public decimal? Terabytes
        {
            get
            {
                return ToLargerDenomination(Gigabytes);
            }
            set
            {
                Gigabytes = ToSmallerDenomination(value);
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Convert input file size to the next larger denomination by dividing by 1024
        /// </summary>
        /// <param name="input">File size to be up-converted</param>
        /// <returns>Up-converted file size</returns>
        public static decimal? ToLargerDenomination(decimal? input)
        {
            //if (input <= 0)
            //{
            //    return 0;
            //}
            //else
            //{
            return input / 1024;
            //}
        }

        /// <summary>
        /// Convert input file size to the next smaller denomination by multiplying by 1024
        /// </summary>
        /// <param name="input">File size to be down-converted</param>
        /// <returns>Down-converted file size</returns>
        public static decimal? ToSmallerDenomination(decimal? input)
        {
            return input * 1024;
        }

        /// <summary>
        /// Returns a string representation of the file size, converted to the largest whole-number file size denomination
        /// </summary>
        /// <preliminary/>
        /// <param name="culture">CultureInfo to determine proper number formatting.</param>
        /// <returns>File size with abbreviation (i.e., "100.00 KB" or "50.00 MB").  Defaults to 2 decimal places.</returns>
        public string AsString(CultureInfo culture)
        {
            return AsString(culture, "N2");
        }

        /// <summary>
        /// Returns a string representation of the file size, converted to the largest whole-number file size denomination
        /// </summary>
        /// <preliminary/>
        /// <param name="culture">CultureInfo to determine proper number formatting.</param>
        /// <param name="numberFormat">Number format string (i.e., "N3") to format the number portion of the string</param>
        /// <returns>File size with abbreviation (i.e., "100.00 KB" or "50.00 MB").  Returns null if value not set.</returns>
        public string AsString(CultureInfo culture, string numberFormat)
        {
            string denomination;
            decimal? fileSizeValue = BestDenomination(out denomination);
            if (fileSizeValue.HasValue)
            {
                return fileSizeValue.Value.ToString(numberFormat, culture.NumberFormat) + " " + denomination;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the numeric file size and associated denomination
        /// </summary>
        /// <preliminary/>
        /// <param name="denomination"></param>
        /// <returns></returns>
        public decimal? BestDenomination(out string denomination)
        {
            decimal? tempBytes = Bytes;
            string[] indicator = { "B", "KB", "MB", "GB", "TB" };
            int placeholder = 0;
            while (ToLargerDenomination(tempBytes) >= 1)
            {
                tempBytes = ToLargerDenomination(tempBytes);
                placeholder++;
                if (placeholder == 4) { break; }
            }
            denomination = indicator[placeholder];
            return tempBytes;
        }

        /// <summary>
        /// NOT FOR DISPLAY USE
        /// This ToString() does not handle culture info and is primarily for simplifying the watch window
        /// </summary>
        /// <returns></returns>
        new public string ToString()
        {
            return AsString(new CultureInfo(1033));
        }


        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            FileSize fileSize = obj as FileSize;
            if (fileSize == null) return false;
            return Equals(_bytes, fileSize._bytes);
        }

        public override int GetHashCode()
        {
            return _bytes.GetHashCode();
        }

        #endregion



    }
}
