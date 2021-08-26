namespace Idera.SQLdm.Common.Dialogs.Config
{
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Forms;

    public partial class WaitDialog : Form
    {
        public DoWorkEventHandler OnDoWork;
        public RunWorkerCompletedEventHandler OnCompletion;

        private ManualResetEvent workerEvent;
        private object sync = new object();
        private bool showing;

        public WaitDialog()
        {
            InitializeComponent();
            sync = new ManualResetEvent(false);
        }

        public string Description
        {
            get { return description.Text; }
            set { description.Text = value; }
        }

        public void WaitForCompletion(Form parent)
        {
            workerEvent = new ManualResetEvent(false);

            backgroundWorker.RunWorkerAsync();

            workerEvent.WaitOne();

            if (Monitor.TryEnter(sync, 2000))
            {
                Monitor.Exit(sync);
                return;
            }

            showing = true;
            this.ShowDialog(parent);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (sync)
            {
                workerEvent.Set();
                if (OnDoWork != null)
                    OnDoWork.Invoke(sender, e);
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (showing)
                Close();

            if (OnCompletion != null)
                OnCompletion.Invoke(sender, e);
        }
    }
}