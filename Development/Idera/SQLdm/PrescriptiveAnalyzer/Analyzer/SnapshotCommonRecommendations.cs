using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    public class SnapshotCommonRecommendations
    {
        public bool EncryptedConnections { get; set; }
        public bool MultipleInstances { get; set; }
        public bool ThemesService { get; set; }
        public bool PageCompression { get; set; }
    }
}
