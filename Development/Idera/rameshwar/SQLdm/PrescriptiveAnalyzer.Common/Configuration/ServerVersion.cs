    //------------------------------------------------------------------------------
    // <copyright file="ServerVersion.cs" company="Idera, Inc.">
    //     Copyright (c) Idera, Inc. All rights reserved.
    // </copyright>
    //------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration
{
    /// <summary>
    /// Represents the version number of a SQL Server.
    /// </summary>
    [Serializable]
    public class ServerVersion
    {
        #region fields

        private int build;
        private int major;
        private int minor;
        private int revision;
        private string servicePack;
        private string version;
        private int masterDatabaseCompatibility;

        #endregion

        #region constructors

        public ServerVersion(string version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            this.version = version;

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
                throw new FormatException("Error parsing server version string", exception);
            }

            ServicePack = GetServicePack(major, minor, build, revision);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the SQL Server build number.
        /// </summary>
        public int Build
        {
            get { return build; }
            private set { build = value; }
        }

        /// <summary>
        /// Gets the SQL Server major version number.
        /// </summary>
        public int Major
        {
            get { return major; }
            private set { major = value; }
        }

        /// <summary>
        /// Gets the SQL Server minor version number.
        /// </summary>
        public int Minor
        {
            get { return minor; }
            private set { minor = value; }
        }

        /// <summary>
        /// Gets the SQL Server revision number.
        /// </summary>
        public int Revision
        {
            get { return revision; }
            private set { revision = value; }
        }

        /// <summary>
        /// Gets the descriptive SQL Server service pack name.
        /// </summary>
        public string ServicePack
        {
            get { return servicePack; }
            private set { servicePack = value; }
        }

        /// <summary>
        /// Gets the string representation of the version number.
        /// </summary>
        public string Version
        {
            get { return version; }
            private set { version = value; }
        }

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


        public int MasterDatabaseCompatibility
        {
            get { return masterDatabaseCompatibility; }
            internal set { masterDatabaseCompatibility = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Returns a string representation of the server version.
        /// </summary>
        /// <returns>The server version string.</returns>
        public override string ToString()
        {
            string ret;

            switch (Major)
            {
                case 7:
                    {
                        ret = "SQL Server 7";
                        break;
                    }
                case 8:
                    {
                        ret = "SQL Server 2000";
                        break;
                    }
                case 9:
                    {
                        ret = "SQL Server 2005";
                        break;
                    }
                // Add SQL Server 2008
                case 10:
                    {
                        ret = "SQL Server 2008";
                        if (Minor == 50)
                            ret = ret + " R2";
                        break;
                    }
                case 11:
                    {
                        ret = "SQL Server 2012";
                        break;
                    }
                case 12:
                    {
                        ret = "SQL Server 2014";
                        break;
                    }
                default:
                    {
                        return "SQL Server " + Major + "." + Minor;
                    }
            }

            if (ServicePack != String.Empty)
                ret = ret + " " + ServicePack;

            return ret;
        }

        /// <summary>
        /// Returns whether the SQL Server version is supported by SQLdoctor.
        /// </summary>
        /// <returns>Whether the version is supported.</returns>
        public bool IsSupported
        {
            get
            {

                // Support SQL Server 2005
                if (Major == 9) return true;

                // Support SQL Server 2008
                if (Major == 10) return true;

                // Support SQL Server 2012? (Denali)
                if (Major == 11) return true;

                // Support SQL Server 2014
                if (Major == 12) return true;

                return false;
            }
        }

        /// <summary>
        /// Gets the service pack.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <param name="build">The build.</param>
        /// <param name="revision">The revision.</param>
        /// <returns></returns>
        private static string GetServicePack(int major, int minor, int build, int revision)
        {
            Dictionary<int, string> releases = new Dictionary<int, string>();
            switch (major)
            {
                case 7:// SQL Server 7.0
                    {
                        if (minor == 0)
                        {
                            releases[623] = "";
                            releases[699] = "SP1";
                            releases[842] = "SP2";
                            releases[961] = "SP3";
                            releases[1063] = "SP4";
                        }
                        break;
                    }
                case 8:// SQL Server 2000
                    {
                        releases[194] = "";
                        releases[384] = "SP1";
                        releases[532] = "SP2";
                        releases[760] = "SP3";
                        releases[2039] = "SP4";

                        break;
                    }
                case 9:// SQL Server 2005
                    {
                        releases[1399] = "";
                        releases[2047] = "SP1";
                        releases[3042] = "SP2";
                        releases[4035] = "SP3";
                        releases[5000] = "SP4";
                        
                        break;
                    }
                case 10:// SQL Server 2008
                    {
                        if (minor == 0) // SQL Server 2008
                        {
                            releases[1600] = "";
                            releases[2531] = "SP1";
                            releases[4000] = "SP2";
                            releases[5500] = "SP3";
                        }
                        if (minor == 50) // SQL Server 2008 R2
                        {
                            releases[1600] = "";
                            releases[2500] = "SP1";
                            releases[4000] = "SP2";
                        }
                        break;
                    }
                case 11:// SQL Server 2012
                    {
                        releases[2100] = "";
                        releases[3000] = "SP1";
                        break;
                    }
                case 12:// SQL Server 2014
                    {
                        releases[1524] = "CTP2";
                        break;
                    }
                default:// SQL Server Unknown
                    {
                        break;
                    }
            }

            return GetSpString(build, releases);
            // If we've made it here, return the build.revision number
            // return revision == 0 ? String.Empty : revision.ToString();
        }

        private static string GetSpString(int target, Dictionary<int, string> releases)
        {
            if (releases.Count == 0)
                return "";

            List<int> builds = new List<int>(releases.Keys);
            builds.Sort();

            string ret = null;

            for (int i = 0; i < builds.Count; i++)
            {
                int build = builds[i];

                if (target < build)
                {
                    if (i > 0)
                    {
                        //If we've got a previous known release
                        ret = releases[builds[i - 1]] + "+";
                        break;
                    }
                    else
                    {
                        //Target build number is lower than anything we know about
                        ret = "";
                        break;
                    }
                }

                if (target == build)
                {
                    //An exact match for the target.  easy
                    ret = releases[build];
                    break;
                }
            }

            if (target > builds[builds.Count - 1])
            {
                //Is the target greater than the last build we know about?
                ret = releases[builds[builds.Count - 1]];
            }

            if (ret == "+")
            {
                //Special RTM empty string hot-fix case";
                ret = "RTM+";
            }

            return ret;
        }

        #endregion
    }
}
