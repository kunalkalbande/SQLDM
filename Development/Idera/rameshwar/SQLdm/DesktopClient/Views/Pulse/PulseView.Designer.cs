using System.Windows.Forms;
using Idera.Newsfeed.Plugins.UI;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Views.Pulse
{
    partial class PulseView
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
            if (disposing)
            {
                PulseController.Default.MoreInfoRequested += PulseController_MoreInfoRequested;
                PulseController.Default.ApplicationActionRequested += PulseController_ApplicationActionRequested;
                Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PulseView
            // 
            this.Name = "PulseView";
            this.Size = new System.Drawing.Size(766, 650);
            osUnsupportedLabel = new Label();
            osUnsupportedPanel = new Panel();
            _newsfeedLinkLabelRequirements = new LinkLabel();
            this.Resize += new System.EventHandler(PulseView_Resize);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
