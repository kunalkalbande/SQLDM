using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{

    [Serializable]
    public class BufferLocation
    {
        private int offset;
        private int line;
        private int column;

        public BufferLocation()
        {
        }   
       public BufferLocation(int offset, int line, int column)
        {
            this.offset = offset;
            this.line = line;
            this.column = column;
        }        
        
        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public int Line
        {
            get { return line; }
            set { line = value; }
        }

        public int Column
        {
            get { return column; }
            set { column = value; }
        }
    }
}
