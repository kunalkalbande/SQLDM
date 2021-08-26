using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration
{
    public class OptimizerHint
    {
        public OptimizerHint(int databaseId, int tableId, int indexId)
        {
            this.databaseId = databaseId;
            this.tableId = tableId;
            this.indexId = indexId;
        }        
        
        private int databaseId;
        public int DatabaseId
        {
            get { return databaseId; }
            set { databaseId = value; }
        }

        private int tableId;
        public int TableId
        {
            get { return tableId; }
            set { tableId = value; }
        }

        private int indexId;
        public int IndexId
        {
            get { return indexId; }
            set { indexId = value; }
        }

    }
}
