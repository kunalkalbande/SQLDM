using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Objects
{
    [Serializable]
    public class CategoryJobStep
    {
        public string Category;
        public string JobName;
        public string StepName;

        public CategoryJobStep()
        {
            this.Category = "";
            this.JobName = "";
            this.StepName = "";
        }
        public CategoryJobStep(string category, string jobName, string stepName)
        {
            this.Category = category;
            this.JobName = jobName;
            this.StepName = stepName;

        }
    }
}
