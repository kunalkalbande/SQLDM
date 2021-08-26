using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Helpers;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class FormattedBytes: IComparable
    {
        private UInt64 _bytes = 0;
        public FormattedBytes(UInt64 bytes)
        {
            _bytes = bytes;
        }

        public override string ToString()
        {
            return (FormatHelper.FormatBytes(_bytes));
        }

        public int CompareTo(object obj)
        {
            FormattedBytes fb = obj as FormattedBytes;
            if (null == fb) return (0);
            return (_bytes.CompareTo(fb._bytes));
        }
    }
}
