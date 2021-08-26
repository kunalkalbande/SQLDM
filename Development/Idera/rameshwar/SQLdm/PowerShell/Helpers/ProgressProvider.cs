//------------------------------------------------------------------------------
// <copyright file="ProgressProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Helpers
{
    using System.Management.Automation;

    public class ProgressProvider : IDisposable
    {
        private readonly SQLdmProvider provider;
        private readonly ProgressRecord record;

        private int step = 1;
        private int totalSteps = -1;

        public ProgressProvider(SQLdmProvider provider, string activity)
        {
            this.provider = provider;
            record = new ProgressRecord(1, activity, "Working...");
        }

        public ProgressProvider(SQLdmProvider provider, string activity, string status)
        {
            this.provider = provider;
            record = new ProgressRecord(1, activity, status);
        }

        public int PercentComplete
        {
            get { return GetPercentComplete(); }
        }

        public int Step
        {
            get { return step; }
            set { step = value; }
        }

        public int TotalSteps
        {
            get { return totalSteps;  }
            set { totalSteps = value; }
        }

        private int GetPercentComplete()
        {
            if (totalSteps > 0)
                return step % 100;
            else
                return step >= totalSteps ? 100 : step * 100 / totalSteps;
        }

        internal string Activity
        {
            get { return record.Activity;  }
            set { record.Activity = value; }
        }

        internal string StatusDescription
        {
            get { return record.StatusDescription; }
            set { record.StatusDescription = value; }
        }

        public void ReportProgress(int currentStep, string activity, string statusDescription)
        {
            Step = currentStep;
            if (activity != null)
                Activity = activity;
            if (statusDescription != null)
                StatusDescription = statusDescription;
            record.RecordType = ProgressRecordType.Processing;
            record.PercentComplete = GetPercentComplete();
            provider.WriteProgress(record);
        }

        public void ReportProgress(string activity, string statusDescription)
        {
            ReportProgress(Step + 1, activity, statusDescription);
        }

        public void ReportProgress()
        {
            ReportProgress(Step + 1, null, null);
        }

        public void Dispose()
        {
            record.RecordType = ProgressRecordType.Completed;
            record.PercentComplete = 100;
            provider.WriteProgress(record);
        }
    }
}
