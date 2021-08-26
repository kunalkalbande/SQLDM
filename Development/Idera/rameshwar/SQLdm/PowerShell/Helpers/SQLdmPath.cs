//------------------------------------------------------------------------------
// <copyright file="SQLdmPath.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Helpers
{
    public static class SQLdmPath
    {
        public const char DriveSeparatorChar = ':';
        public const string DriveSeparator = ":";
        public const char PathSeparatorChar = '\\';
        public const string PathSeparator = "\\";

        public static bool IsRooted(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                path = path.TrimStart();
                if (path[0] == PathSeparatorChar)
                    return true;
                int i = path.IndexOf(DriveSeparatorChar);
                if (i < 0)
                    return false;
                if (path.Length > i + 1)
                    return path[i + 1] == PathSeparatorChar;
            }
            return false;
        }

        public static bool ContainsDrive(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                path = path.TrimStart();
                int i = path.IndexOf(DriveSeparatorChar);
                if (i != -1)
                    return true;
            }
            return false;
        }

        public static string GetDriveName(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                path = path.TrimStart();
                int i = path.IndexOf(DriveSeparatorChar);
                if (i > 0)
                    return path.Substring(0,i);
            }
            return String.Empty;
        }

        public static string RemoveDrive(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                if (String.IsNullOrEmpty(path))
                    return String.Empty;

                int cx = path.IndexOf(DriveSeparatorChar);
                if (cx >= 0)
                {
                    int bsx = path.IndexOf(PathSeparatorChar);
                    if (bsx == cx + 1)
                        path = path.Substring(bsx);
                    else
                        path = path.Substring(cx);
                }
                if (path.StartsWith(PathSeparator) || path.StartsWith(DriveSeparator))
                {
                    if (path.Length > 1)
                        path = path.Substring(1);
                    else
                        path = String.Empty;
                }
            }

            return path;
        }

        public static string MakeAbsolutePath(string drive, string path)
        {
            StringBuilder builder = new StringBuilder();
            if (!String.IsNullOrEmpty(drive))
            {
                builder.Append(drive).Append(DriveSeparatorChar);
            }
            if (String.IsNullOrEmpty(path))             
                builder.Append(PathSeparatorChar);
            else
            {
                if (!path.StartsWith(PathSeparator))
                    builder.Append(PathSeparatorChar);
                builder.Append(path);
            }
            return builder.ToString();
        }

        public static string Concat(params string[] parts)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string part in parts)
            {
                if (builder.Length > 0 && builder[builder.Length - 1] != PathSeparatorChar)
                {
                    if (!part.StartsWith(PathSeparator))
                        builder.Append(PathSeparatorChar);
                }
                builder.Append(part);
            }
            return builder.ToString();
        }
    }
}
