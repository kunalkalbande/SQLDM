using System;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    partial class AdministrationNavigationPane
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
                ApplicationController.Default.AdministrationViewChanged -= new EventHandler<AdministrationViewChangedEventArgs>(administrationViewChanged);

                if (components != null)
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Application Security", 1, 1);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Custom Counters", 2, 2);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Change Log", 3, 3);
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Import/Export", 4, 4);//SQL dm 10.0 (Swati Gogia) Import Export wizard
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Administration", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode4,
            treeNode5});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdministrationNavigationPane));
            this.adminTree = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // adminTree
            // 
            this.adminTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.adminTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adminTree.FullRowSelect = true;
            this.adminTree.HideSelection = false;
            this.adminTree.ImageIndex = 0;
            this.adminTree.ImageList = this.imageList;
            this.adminTree.Location = new System.Drawing.Point(0, 0);
            this.adminTree.Name = "adminTree";
            treeNode1.ImageIndex = 1;
            treeNode1.Name = "applicationSecurity";
            treeNode1.SelectedImageIndex = 1;
            treeNode1.Text = "Application Security";
            treeNode2.ImageIndex = 2;
            treeNode2.Name = "customCounters";
            treeNode2.SelectedImageIndex = 2;
            treeNode2.Text = "Custom Counters";
            treeNode3.ImageIndex = 0;
            treeNode3.Name = "administration";
            treeNode3.SelectedImageIndex = 0;
            treeNode3.Text = "Administration";
            treeNode4.ImageIndex = 3;
            treeNode4.Name = "auditedActions";
            treeNode4.SelectedImageIndex = 3;
            treeNode4.Text = "Change Log";
            //START SQL dm 10.0 (Swati Gogia) Import/Export Wizard
            treeNode5.ImageIndex = 4;
            treeNode5.Name = "Import/Export";
            treeNode5.SelectedImageIndex = 4;
            treeNode5.Text = "Import/Export";
            //END
            this.adminTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3});
            this.adminTree.SelectedImageIndex = 0;
            this.adminTree.ShowLines = false;
            this.adminTree.Size = new System.Drawing.Size(295, 484);
            this.adminTree.TabIndex = 1;
            this.adminTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.adminTree_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "AdministrationArea16x16.png");
            this.imageList.Images.SetKeyName(1, "AppSecurity16x16.png");
            this.imageList.Images.SetKeyName(2, "CustomCounter16x16.png");
            this.imageList.Images.Add(Idera.SQLdm.DesktopClient.Properties.Resources.ChangeLog16x16);
            this.imageList.Images.Add(Idera.SQLdm.DesktopClient.Properties.Resources.ImportExport16x16);
            //this.imageList.Images.SetKeyName(3, "ServerProfile32.png");
            // 
            // AdministrationNavigationPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.adminTree);
            this.Name = "AdministrationNavigationPane";
            this.Size = new System.Drawing.Size(295, 484);
            this.Load += new System.EventHandler(this.AdministrationNavigationPane_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView adminTree;
        private System.Windows.Forms.ImageList imageList;



    }
}
