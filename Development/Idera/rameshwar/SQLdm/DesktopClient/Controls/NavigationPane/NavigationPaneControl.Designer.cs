using Infragistics.Win;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    partial class NavigationPaneControl
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
                ApplicationController.Default.ActiveViewChanged -= ActiveViewChanged;

                if (components != null)
                {
                    components.Dispose();
                }
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
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavigationPaneControl));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup6 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            this.ultraExplorerBarContainerControl1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl();
            this.serversNavigationPane = new Idera.SQLdm.DesktopClient.Controls.NavigationPane.ServersNavigationPane();
            this.ultraExplorerBarContainerControl2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl();
            this.alertsNavigationPane = new Idera.SQLdm.DesktopClient.Controls.NavigationPane.AlertsNavigationPane();
            this.ultraExplorerBarContainerControl6 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl();
            this.pulseNavigationPane = new Idera.SQLdm.DesktopClient.Controls.NavigationPane.PulseNavigationPane();
            this.ultraExplorerBarContainerControl3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl();
            this.tasksNavigationPane = new Idera.SQLdm.DesktopClient.Controls.NavigationPane.TasksNavigationPane();
            this.ultraExplorerBarContainerControl4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl();
            this.reportsNavigationPane = new Idera.SQLdm.DesktopClient.Controls.NavigationPane.ReportsNavigationPane();
            this.ultraExplorerBarContainerControl5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl();
            this.administrationNavigationPane = new Idera.SQLdm.DesktopClient.Controls.NavigationPane.AdministrationNavigationPane();
            this.explorerBar = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.ultraExplorerBarContainerControl1.SuspendLayout();
            this.ultraExplorerBarContainerControl2.SuspendLayout();
            this.ultraExplorerBarContainerControl6.SuspendLayout();
            this.ultraExplorerBarContainerControl3.SuspendLayout();
            this.ultraExplorerBarContainerControl4.SuspendLayout();
            this.ultraExplorerBarContainerControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.explorerBar)).BeginInit();
            this.explorerBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraExplorerBarContainerControl1
            // 
            this.ultraExplorerBarContainerControl1.Controls.Add(this.serversNavigationPane);
            this.ultraExplorerBarContainerControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraExplorerBarContainerControl1.Margin = new System.Windows.Forms.Padding(0);
            this.ultraExplorerBarContainerControl1.Name = "ultraExplorerBarContainerControl1";
            this.ultraExplorerBarContainerControl1.Size = new System.Drawing.Size(256, 272);
            this.ultraExplorerBarContainerControl1.TabIndex = 4;
            this.ultraExplorerBarContainerControl1.Visible = false;
            // 
            // serversNavigationPane
            // 
            this.serversNavigationPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serversNavigationPane.Location = new System.Drawing.Point(0, 0);
            this.serversNavigationPane.Margin = new System.Windows.Forms.Padding(0);
            this.serversNavigationPane.Name = "serversNavigationPane";
            this.serversNavigationPane.Size = new System.Drawing.Size(256, 272);
            this.serversNavigationPane.TabIndex = 0;
            // 
            // ultraExplorerBarContainerControl2
            // 
            this.ultraExplorerBarContainerControl2.Controls.Add(this.alertsNavigationPane);
            this.ultraExplorerBarContainerControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraExplorerBarContainerControl2.Name = "ultraExplorerBarContainerControl2";
            this.ultraExplorerBarContainerControl2.Size = new System.Drawing.Size(256, 272);
            this.ultraExplorerBarContainerControl2.TabIndex = 11;
            this.ultraExplorerBarContainerControl2.Visible = false;
            // 
            // alertsNavigationPane
            // 
            this.alertsNavigationPane.BackColor = System.Drawing.Color.White;
            this.alertsNavigationPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsNavigationPane.Location = new System.Drawing.Point(0, 0);
            this.alertsNavigationPane.Name = "alertsNavigationPane";
            this.alertsNavigationPane.Size = new System.Drawing.Size(256, 272);
            this.alertsNavigationPane.TabIndex = 0;
            // 
            // ultraExplorerBarContainerControl6
            // 
            this.ultraExplorerBarContainerControl6.Controls.Add(this.pulseNavigationPane);
            this.ultraExplorerBarContainerControl6.Location = new System.Drawing.Point(1, 26);
            this.ultraExplorerBarContainerControl6.Margin = new System.Windows.Forms.Padding(2);
            this.ultraExplorerBarContainerControl6.Name = "ultraExplorerBarContainerControl6";
            this.ultraExplorerBarContainerControl6.Size = new System.Drawing.Size(256, 272);
            this.ultraExplorerBarContainerControl6.TabIndex = 13;
            // 
            // pulseNavigationPane
            // 
            this.pulseNavigationPane.BackColor = System.Drawing.Color.White;
            this.pulseNavigationPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pulseNavigationPane.Location = new System.Drawing.Point(0, 0);
            this.pulseNavigationPane.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pulseNavigationPane.Name = "pulseNavigationPane";
            this.pulseNavigationPane.Size = new System.Drawing.Size(256, 272);
            this.pulseNavigationPane.TabIndex = 0;
            // 
            // ultraExplorerBarContainerControl3
            // 
            this.ultraExplorerBarContainerControl3.Controls.Add(this.tasksNavigationPane);
            this.ultraExplorerBarContainerControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraExplorerBarContainerControl3.Name = "ultraExplorerBarContainerControl3";
            this.ultraExplorerBarContainerControl3.Size = new System.Drawing.Size(256, 272);
            this.ultraExplorerBarContainerControl3.TabIndex = 5;
            this.ultraExplorerBarContainerControl3.Visible = false;
            // 
            // tasksNavigationPane
            // 
            this.tasksNavigationPane.BackColor = System.Drawing.Color.White;
            this.tasksNavigationPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tasksNavigationPane.Location = new System.Drawing.Point(0, 0);
            this.tasksNavigationPane.Margin = new System.Windows.Forms.Padding(4);
            this.tasksNavigationPane.Name = "tasksNavigationPane";
            this.tasksNavigationPane.Size = new System.Drawing.Size(256, 272);
            this.tasksNavigationPane.TabIndex = 0;
            this.tasksNavigationPane.Visible = false;
            // 
            // ultraExplorerBarContainerControl4
            // 
            this.ultraExplorerBarContainerControl4.Controls.Add(this.reportsNavigationPane);
            this.ultraExplorerBarContainerControl4.Location = new System.Drawing.Point(-7500, -8125);
            this.ultraExplorerBarContainerControl4.Name = "ultraExplorerBarContainerControl4";
            this.ultraExplorerBarContainerControl4.Size = new System.Drawing.Size(256, 316);
            this.ultraExplorerBarContainerControl4.TabIndex = 6;
            this.ultraExplorerBarContainerControl4.Visible = false;
            // 
            // reportsNavigationPane
            // 
            this.reportsNavigationPane.AutoScroll = true;
            this.reportsNavigationPane.AutoScrollMargin = new System.Drawing.Size(0, 300);
            this.reportsNavigationPane.AutoSize = true;
            this.reportsNavigationPane.BackColor = System.Drawing.Color.White;
            this.reportsNavigationPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportsNavigationPane.Location = new System.Drawing.Point(0, 0);
            this.reportsNavigationPane.Margin = new System.Windows.Forms.Padding(4);
            this.reportsNavigationPane.Name = "reportsNavigationPane";
            this.reportsNavigationPane.SelectedReport = Idera.SQLdm.DesktopClient.Views.Reports.ReportTypes.GettingStarted;
            this.reportsNavigationPane.Size = new System.Drawing.Size(256, 316);
            this.reportsNavigationPane.TabIndex = 0;
            // 
            // ultraExplorerBarContainerControl5
            // 
            this.ultraExplorerBarContainerControl5.Controls.Add(this.administrationNavigationPane);
            this.ultraExplorerBarContainerControl5.Location = new System.Drawing.Point(-7500, -8125);
            this.ultraExplorerBarContainerControl5.Name = "ultraExplorerBarContainerControl5";
            this.ultraExplorerBarContainerControl5.Size = new System.Drawing.Size(256, 316);
            this.ultraExplorerBarContainerControl5.TabIndex = 12;
            this.ultraExplorerBarContainerControl5.Visible = false;
            // 
            // administrationNavigationPane
            // 
            this.administrationNavigationPane.BackColor = System.Drawing.Color.White;
            this.administrationNavigationPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.administrationNavigationPane.Location = new System.Drawing.Point(0, 0);
            this.administrationNavigationPane.Margin = new System.Windows.Forms.Padding(4);
            this.administrationNavigationPane.Name = "administrationNavigationPane";
            this.administrationNavigationPane.Size = new System.Drawing.Size(256, 316);
            this.administrationNavigationPane.TabIndex = 0;
            // 
            // explorerBar
            // 
            this.explorerBar.Controls.Add(this.ultraExplorerBarContainerControl1);
            this.explorerBar.Controls.Add(this.ultraExplorerBarContainerControl3);
            this.explorerBar.Controls.Add(this.ultraExplorerBarContainerControl4);
            this.explorerBar.Controls.Add(this.ultraExplorerBarContainerControl2);
            this.explorerBar.Controls.Add(this.ultraExplorerBarContainerControl5);
            this.explorerBar.Controls.Add(this.ultraExplorerBarContainerControl6);
            this.explorerBar.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExplorerBarGroup1.Container = this.ultraExplorerBarContainerControl1;
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            ultraExplorerBarGroup1.Settings.AppearancesLarge.HeaderAppearance = appearance1;
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            ultraExplorerBarGroup1.Settings.AppearancesSmall.HeaderAppearance = appearance2;
            ultraExplorerBarGroup1.Text = "Servers";
            ultraExplorerBarGroup2.Container = this.ultraExplorerBarContainerControl2;
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            ultraExplorerBarGroup2.Settings.AppearancesLarge.HeaderAppearance = appearance3;
            appearance4.Image = ((object)(resources.GetObject("appearance4.Image")));
            ultraExplorerBarGroup2.Settings.AppearancesSmall.HeaderAppearance = appearance4;
            ultraExplorerBarGroup2.Text = "Alerts";
            ultraExplorerBarGroup3.Container = this.ultraExplorerBarContainerControl6;
            appearance5.Image = ((object)(resources.GetObject("appearance5.Image")));
            ultraExplorerBarGroup3.Settings.AppearancesLarge.HeaderAppearance = appearance5;
            appearance6.Image = ((object)(resources.GetObject("appearance6.Image")));
            ultraExplorerBarGroup3.Settings.AppearancesSmall.HeaderAppearance = appearance6;
            ultraExplorerBarGroup3.Text = "Newsfeed";
            ultraExplorerBarGroup4.Container = this.ultraExplorerBarContainerControl3;
            appearance7.Image = ((object)(resources.GetObject("appearance7.Image")));
            ultraExplorerBarGroup4.Settings.AppearancesLarge.HeaderAppearance = appearance7;
            appearance8.Image = ((object)(resources.GetObject("appearance8.Image")));
            ultraExplorerBarGroup4.Settings.AppearancesSmall.HeaderAppearance = appearance8;
            ultraExplorerBarGroup4.Text = "To Do";
            ultraExplorerBarGroup4.Visible = false;
            ultraExplorerBarGroup5.Container = this.ultraExplorerBarContainerControl4;
            appearance9.Image = ((object)(resources.GetObject("appearance9.Image")));
            ultraExplorerBarGroup5.Settings.AppearancesLarge.HeaderAppearance = appearance9;
            appearance10.Image = ((object)(resources.GetObject("appearance10.Image")));
            ultraExplorerBarGroup5.Settings.AppearancesSmall.HeaderAppearance = appearance10;
            ultraExplorerBarGroup5.Text = "Reports";
            ultraExplorerBarGroup6.Container = this.ultraExplorerBarContainerControl5;
            appearance11.Image = ((object)(resources.GetObject("appearance11.Image")));
            ultraExplorerBarGroup6.Settings.AppearancesLarge.HeaderAppearance = appearance11;
            appearance23.Image = ((object)(resources.GetObject("appearance23.Image")));
            ultraExplorerBarGroup6.Settings.AppearancesSmall.HeaderAppearance = appearance23;
            ultraExplorerBarGroup6.Text = "Administration";
            this.explorerBar.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1,
            ultraExplorerBarGroup2,
            ultraExplorerBarGroup3,
            ultraExplorerBarGroup4,
            ultraExplorerBarGroup5,
            ultraExplorerBarGroup6});
            this.explorerBar.BorderStyle = UIElementBorderStyle.None;
            this.explorerBar.GroupSettings.HeaderButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.explorerBar.GroupSettings.NavigationAllowHide = Infragistics.Win.DefaultableBoolean.False;
            this.explorerBar.GroupSettings.Style = Infragistics.Win.UltraWinExplorerBar.GroupStyle.ControlContainer;
            this.explorerBar.ImageSizeLarge = new System.Drawing.Size(24, 24);
            this.explorerBar.Location = new System.Drawing.Point(0, 0);
            this.explorerBar.Name = "explorerBar";
            this.explorerBar.NavigationAllowGroupReorder = false;
            this.explorerBar.SaveSettings = true;
            scrollBarLook1.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Office2007;
            this.explorerBar.ScrollBarLook = scrollBarLook1;
            this.explorerBar.SettingsKey = "NavigationPaneControl.explorerBar";
            this.explorerBar.ShowDefaultContextMenu = false;
            this.explorerBar.Size = new System.Drawing.Size(258, 529);
            this.explorerBar.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.OutlookNavigationPane;
            this.explorerBar.TabIndex = 0;
            this.explorerBar.UseAppStyling = true;
            this.explorerBar.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.explorerBar.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2007;
            this.explorerBar.SelectedGroupChanged += new Infragistics.Win.UltraWinExplorerBar.SelectedGroupChangedEventHandler(this.explorerBar_SelectedGroupChanged);
            // 
            // NavigationPaneControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.explorerBar);
            this.Name = "NavigationPaneControl";
            this.Size = new System.Drawing.Size(258, 529);
            this.Load += new System.EventHandler(this.NavigationPaneControl_Load);
            this.ultraExplorerBarContainerControl1.ResumeLayout(false);
            this.ultraExplorerBarContainerControl2.ResumeLayout(false);
            this.ultraExplorerBarContainerControl6.ResumeLayout(false);
            this.ultraExplorerBarContainerControl3.ResumeLayout(false);
            this.ultraExplorerBarContainerControl4.ResumeLayout(false);
            this.ultraExplorerBarContainerControl4.PerformLayout();
            this.ultraExplorerBarContainerControl5.ResumeLayout(false);
            ((System.Configuration.IPersistComponentSettings)(this.explorerBar)).LoadComponentSettings();
            ((System.ComponentModel.ISupportInitialize)(this.explorerBar)).EndInit();
            this.explorerBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar explorerBar;
        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl ultraExplorerBarContainerControl1;
        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl ultraExplorerBarContainerControl3;
        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl ultraExplorerBarContainerControl4;
        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl ultraExplorerBarContainerControl2;
        private ServersNavigationPane serversNavigationPane;
        private ReportsNavigationPane reportsNavigationPane;
        private TasksNavigationPane tasksNavigationPane;
        private AlertsNavigationPane alertsNavigationPane;
        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl ultraExplorerBarContainerControl5;
        private AdministrationNavigationPane administrationNavigationPane;
        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl ultraExplorerBarContainerControl6;
        private PulseNavigationPane pulseNavigationPane;
    }
}
