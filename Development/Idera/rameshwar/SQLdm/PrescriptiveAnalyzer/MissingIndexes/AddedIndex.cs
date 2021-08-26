using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes
{
    internal class AddedIndex
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Int64 MissingIndexID { get; set; }
        private AddedIndex() { }
        public AddedIndex(int id, string name, Int64 missingIndexID) { ID = id; Name = name; MissingIndexID = missingIndexID; }
    }
}
