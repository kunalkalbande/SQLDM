using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wintellect.PowerCollections;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class WarningListDialog : Form
    {
        public WarningListDialog(List<Pair<IRecommendation, List<string>>> recommendationsWithMessages)
        {
            InitializeComponent();
            int counter = 0;

            foreach (Pair<IRecommendation, List<string>> recWithMessages in recommendationsWithMessages)
            {
                IRecommendation recommendation = recWithMessages.First;
                List<string> messages = recWithMessages.Second;

                _warningTreeView.Nodes.Add(counter.ToString(), recommendation.FindingText);

                foreach (string messageText in messages)
                {
                    _warningTreeView.Nodes[counter.ToString()].Nodes.Add(messageText, messageText);
                }
                counter++;
            }
            _warningTreeView.ExpandAll();
        }
    }
}
