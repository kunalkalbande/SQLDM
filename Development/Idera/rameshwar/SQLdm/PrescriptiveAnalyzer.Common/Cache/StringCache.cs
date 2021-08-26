using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache
{
    public class StringCache : IDisposable
    {
        private CacheFile _file = null;
        private StringCache() { }
        public StringCache(string fileName, int minSizeMB, int maxSizeMB, int growSizeMB) { _file = new CacheFile(fileName, minSizeMB, maxSizeMB, growSizeMB); }

        public long AddString(string s)
        {
            return (_file.Append(s));
        }

        public string GetString(long key)
        {
            if (0 > key) return (string.Empty);
            long len = _file.ReadInt64(key);
            return (_file.ReadString(key + sizeof(Int64), len));
        }

        public void Dispose()
        {
            if (null != _file) { using (_file) { } }
            _file = null;
        }
    }
}
