namespace Idera.SQLdm.ManagementService
{
    using System.Diagnostics;
    using System.Management.Instrumentation;

    partial class ManagementServiceInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.perfCounterInstaller = new System.Diagnostics.PerformanceCounterInstaller();
            // 
            // perfCounterInstaller
            // 
            this.perfCounterInstaller.CategoryName = "SQLdm";
            this.perfCounterInstaller.CategoryType = System.Diagnostics.PerformanceCounterCategoryType.SingleInstance;
            this.perfCounterInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
            new System.Diagnostics.CounterCreationData("ActiveWorkers", "Number of threads processing queued work", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("WaitingWorkers", "Number of threads waiting for work to be queued", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Task Queue Length", "Number of tasks waiting for execution", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
            new System.Diagnostics.CounterCreationData("Tasks Queued/sec", "Average number of tasks queued per second", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
            new System.Diagnostics.CounterCreationData("Avg. Task Time", "Average time to process a queued task", System.Diagnostics.PerformanceCounterType.AverageTimer32),
            new System.Diagnostics.CounterCreationData("Avg. Task Time Base", "Base counter for Avg. Task Time", System.Diagnostics.PerformanceCounterType.AverageBase)});
            this.perfCounterInstaller.BeforeInstall += new System.Configuration.Install.InstallEventHandler(this.perfCounterInstaller_BeforeInstall);
            this.perfCounterInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.perfCounterInstaller_AfterInstall);
            // 
            // ManagementServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.perfCounterInstaller});

        }

        #endregion

        private System.Diagnostics.PerformanceCounterInstaller perfCounterInstaller;
    }
}