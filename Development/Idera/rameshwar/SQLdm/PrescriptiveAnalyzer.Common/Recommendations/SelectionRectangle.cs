using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{

    [Serializable]
    public class SelectionRectangle
    {
        public BufferLocation Start;
        public int Length;

        public SelectionRectangle()
        {
        }
        public SelectionRectangle(BufferLocation start, int length)
        {
            Start = start;
            Length = length;
        }

        public SelectionRectangle(int offset, int line, int column, int length)
        {
            Start = new BufferLocation(offset, line, column);
            Length = length;
        }
    }
}
