using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.SQLsafe
{
    [Serializable]
    public class SqlSafeVersion
    {
        #region Properties

        public string Version { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revision { get; set; }
        public bool IsSupported { get { return Major >= 8; } }

        public string DisplayVersion
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                buffer.AppendFormat("{0:D2}.{1:D2}.{2:D4}", Major, Minor, Build);
                if (Revision > 0)
                    buffer.AppendFormat(".{0:2}", Revision);

                return buffer.ToString();
            }
        }

        #endregion

        #region Constructors

        public SqlSafeVersion() : this("0.0.0.0")
        {
        }

        public SqlSafeVersion(string version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            Version = version;

            string[] splitVersion = version.Split('.');

            if (splitVersion.Length < 3)
                throw new ArgumentException("version string in unrecognized format");

            try
            {
                Major = Convert.ToInt32(splitVersion[0]);
                Minor = Convert.ToInt32(splitVersion[1]);
                Build = Convert.ToInt32(splitVersion[2]);

                if (splitVersion.Length == 4)
                    Revision = Convert.ToInt32(splitVersion[3]);

                if (Major < 0)
                    throw new ArgumentOutOfRangeException("major");
                if (Minor < 0)
                    throw new ArgumentOutOfRangeException("minor");
                if (Build < 0)
                    throw new ArgumentOutOfRangeException("build");
                if (Revision < 0)
                    throw new ArgumentOutOfRangeException("revision");
            }
            catch (InvalidCastException exception)
            {
                throw new FormatException("Error parsing SQLsafe version string", exception);
            }


        }

        #endregion

 
    }
}
