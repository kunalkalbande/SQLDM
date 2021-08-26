// ===============================
// AUTHOR       : CWF Team - Gowrish 
// PURPOSE      : Backend Isolation
// TICKET       : SQLDM-29086
// ===============================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Helpers.CWF
{
    internal static class StringExtensions
    {
        public static MemoryStream ToStream(this string s)
        {
            MemoryStream stream = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            stream.Write(bytes, 0, bytes.Length);
            if (stream.Position > 0) stream.Position = 0;
            return (stream);
        }
    }
}
