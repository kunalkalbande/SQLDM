namespace Idera.SQLdm.Common
{
    public static class ServerVersionExtensions
    {
        /// <summary>
        /// Inputs Version and returns true if 2008 Sp1 R2 version
        /// </summary>
        /// <param name="ver">Input Sql version</param>
        /// <returns>True for SQL 2008 Sp1 R2</returns>
        /// <remarks>
        /// See version information from <seealso cref="https://social.technet.microsoft.com/wiki/contents/articles/783.sql-server-versions.aspx"/>
        /// </remarks>
        public static bool IsGreaterThanSql2008Sp1R2(this ServerVersion ver)
        {
            return ver.Major >= 10 && ver.Minor >= 50 && ver.Build >= 2500;

            if (ver.Major > 10)
                return true;
            else if (ver.Major == 10 && ver.Minor > 50)
                return true;
            else if (ver.Major == 10 && ver.Minor == 50 && ver.Build >= 2500)
                return true;
            else
                return false;
        }

        public static bool IsGreaterThanSql2014sp2(this ServerVersion ver)
        {
            if (ver.Major > 12)
                return true;
            else if (ver.Major == 12 && ver.Revision >= 5000)
                return true;
            else
                return false;
        }
    }
}
