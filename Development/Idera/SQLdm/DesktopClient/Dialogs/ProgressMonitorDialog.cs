namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using Timer=System.Threading.Timer;
    using Idera.SQLdm.DesktopClient.Helpers;

    public partial class ProgressMonitorDialog : BaseDialog
    {
        private int delay = Timeout.Infinite;
        private System.Threading.Timer delayTimer;

        private BackgroundWorkerWithProgressDialog backgroundWorker;

        internal ProgressMonitorDialog(BackgroundWorkerWithProgressDialog backgroundWorker)
        {
            this.DialogHeader = "Please wait...";
            this.backgroundWorker = backgroundWorker;

            InitializeComponent();

            cancelButton.Visible = backgroundWorker.WorkerSupportsCancellation;
            this.Load += ProgressMonitorDialog_Load;

            taskLabel.Text = String.Empty;

            this.AdaptFontSize();
        }

        private void ProgressMonitorDialog_Load(object sender, EventArgs e)
        {
//            this.progressBar.Active = true;
        }

        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        public void StartDelayTimer(Form ownerForm)
        {
            if (delay != Timeout.Infinite && delayTimer == null)
            {
                delayTimer = new Timer(delayTimer_Tick, ownerForm, delay, Timeout.Infinite);
            }
        }

        internal void BeginInvokeShowDialog(Form ownerForm)
        {
            if (ownerForm != null && !this.IsDisposed && !this.Disposing)
            {
                ownerForm.BeginInvoke((MethodInvoker) delegate()
                    {
                        try
                        {
                            if (!this.IsDisposed && !this.Disposing)
                                this.ShowDialog(ownerForm);
                        } catch (Exception)
                        {
                            /* */
                        }
                    });
            }
        }

        public void ShowDialogDelayed(Form ownerForm, int delay)
        {
            if (delay == Timeout.Infinite)
                ShowDialog(ownerForm);
            else
            {
                Delay = delay;
                StartDelayTimer(ownerForm);
            }
        }

        private void delayTimer_Tick(object args)
        {
            Form ownerForm = args as Form;
            BeginInvokeShowDialog(ownerForm);
            KillDelayTimer();
        }

        internal void KillDelayTimer()
        {
            if (delayTimer != null)
            {
                Timer timer = delayTimer;
                delayTimer = null;
                try
                {
                    using (timer)
                    {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                }
                catch
                {
                }
            }
        }

        private void ProgressMonitorDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            KillDelayTimer();

            progressBar.Visible = false;
            progressBar.Style = ProgressBarStyle.Blocks;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            cancelButton.Enabled = false;
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();

            Close();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        internal void SetTaskName(string name)
        {
            if (taskLabel.InvokeRequired)
                taskLabel.Invoke((MethodInvoker) delegate() { taskLabel.Text = name; });
            else
                taskLabel.Text = name;
        }
    }

    public class BackgroundWorkerWithProgressDialog : BackgroundWorker
    {
        public readonly Form ownerForm;
        private ProgressMonitorDialog dialog;

        private int delay = Timeout.Infinite;

        private Label externalTaskLabel;

        private bool completionCallbackDone = false;

        public BackgroundWorkerWithProgressDialog(Form owner)
            : base()
        {
            ownerForm = owner;
        }

        public new void CancelAsync()
        {
            base.CancelAsync();

            RunWorkerCompletedEventArgs args = new RunWorkerCompletedEventArgs(null, null, true);
            OnRunWorkerCompleted(args);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            if (!ownerForm.Disposing && !ownerForm.IsDisposed)
            {
                ownerForm.Invoke((MethodInvoker) delegate()
                    {
                        if (!ownerForm.Disposing && !ownerForm.IsDisposed)
                        {
                            dialog = new ProgressMonitorDialog(this);
                            dialog.ShowDialogDelayed(ownerForm, Delay);
                        }
                    });
            }
            base.OnDoWork(e);

            // keep dialog from showing since background work is complete
            dialog.KillDelayTimer();

            if (CancellationPending)
                e.Cancel = true;

            Debug.Print("OnDoWork Cancel={0}", e.Cancel);
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            if (dialog != null)
            {
                using (dialog)
                {
                    dialog.Close();
                }
                dialog = null;
            }
            if (!completionCallbackDone)
            {
                completionCallbackDone = true;
                base.OnRunWorkerCompleted(e);
            }
        }

        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        public Label ExternalTaskLabel
        {
            get { return externalTaskLabel; }
            set { externalTaskLabel = value; }
        }

        private void SetExternalLabelText(Label label, string text)
        {
            if (label != null)
            {
                if (label.InvokeRequired)
                    label.Invoke((MethodInvoker) delegate() { label.Text = text; });
                else
                    label.Text = text;
            }
        }

        internal void SetStatusText(string text)
        {
            if (dialog != null && !dialog.IsDisposed) 
                dialog.SetTaskName(text);

            SetExternalLabelText(externalTaskLabel, text);
        }
    }
}