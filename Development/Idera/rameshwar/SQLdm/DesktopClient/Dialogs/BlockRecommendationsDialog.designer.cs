
using Idera.SQLdm.DesktopClient.Controls;
namespace Idera.SQLdm.StandardClient.Dialogs
{
    partial class BlockRecommendationsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
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
            this.components = new System.ComponentModel.Container();
            this.listViewImages = new System.Windows.Forms.ImageList(this.components);
            this.btnBlockSelected = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gradientPanel1 = new GradientPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lvRecommendationTypes = new System.Windows.Forms.ListView();
            this.colRecommendation = new System.Windows.Forms.ColumnHeader();
            this.lvDatabases = new System.Windows.Forms.ListView();
            this.colDatabase = new System.Windows.Forms.ColumnHeader();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.gradientPanel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewImages
            // 
            this.listViewImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.listViewImages.ImageSize = new System.Drawing.Size(1, 20);
            this.listViewImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnBlockSelected
            // 
            this.btnBlockSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBlockSelected.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnBlockSelected.Location = new System.Drawing.Point(295, 317);
            this.btnBlockSelected.Name = "btnBlockSelected";
            this.btnBlockSelected.Size = new System.Drawing.Size(87, 23);
            this.btnBlockSelected.TabIndex = 0;
            this.btnBlockSelected.Text = "&Block Selected";
            this.btnBlockSelected.Click += new System.EventHandler(this.btnBlockSelected_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(388, 317);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.gradientPanel1.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(203)))), ((int)(((byte)(204)))));
            this.gradientPanel1.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.gradientPanel1.Controls.Add(this.splitContainer1);
            this.gradientPanel1.Controls.Add(this.copyrightLabel);
            this.gradientPanel1.Controls.Add(this.btnBlockSelected);
            this.gradientPanel1.Controls.Add(this.btnCancel);
            this.gradientPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradientPanel1.FillStyle = GradientPanelFillStyle.Solid;
            this.gradientPanel1.Location = new System.Drawing.Point(0, 0);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.gradientPanel1.ShowBorder = false;
            this.gradientPanel1.Size = new System.Drawing.Size(487, 353);
            this.gradientPanel1.TabIndex = 27;
            // 
            // splitContainer1
            // 
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(12, 33);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvRecommendationTypes);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvDatabases);
            this.splitContainer1.Size = new System.Drawing.Size(463, 278);
            this.splitContainer1.SplitterDistance = 127;
            this.splitContainer1.TabIndex = 31;
            // 
            // lvRecommendationTypes
            // 
            this.lvRecommendationTypes.CheckBoxes = true;
            this.lvRecommendationTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colRecommendation});
            this.lvRecommendationTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRecommendationTypes.Location = new System.Drawing.Point(0, 0);
            this.lvRecommendationTypes.Name = "lvRecommendationTypes";
            this.lvRecommendationTypes.Size = new System.Drawing.Size(463, 127);
            this.lvRecommendationTypes.TabIndex = 0;
            this.lvRecommendationTypes.UseCompatibleStateImageBehavior = false;
            this.lvRecommendationTypes.View = System.Windows.Forms.View.Details;
            this.lvRecommendationTypes.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvRecommendationTypes_ItemChecked);
            // 
            // colRecommendation
            // 
            this.colRecommendation.Text = "Recommendation Type";
            this.colRecommendation.Width = 430;
            // 
            // lvDatabases
            // 
            this.lvDatabases.CheckBoxes = true;
            this.lvDatabases.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDatabase});
            this.lvDatabases.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDatabases.Location = new System.Drawing.Point(0, 0);
            this.lvDatabases.Name = "lvDatabases";
            this.lvDatabases.Size = new System.Drawing.Size(463, 147);
            this.lvDatabases.TabIndex = 1;
            this.lvDatabases.UseCompatibleStateImageBehavior = false;
            this.lvDatabases.View = System.Windows.Forms.View.Details;
            this.lvDatabases.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvDatabases_ItemChecked);
            // 
            // colDatabase
            // 
            this.colDatabase.Text = "Database";
            this.colDatabase.Width = 429;
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copyrightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyrightLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.copyrightLabel.Location = new System.Drawing.Point(9, 9);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(370, 21);
            this.copyrightLabel.TabIndex = 30;
            this.copyrightLabel.Text = "Please select the objects that should be blocked:";
            // 
            // BlockDialog
            // 
            this.AcceptButton = this.btnBlockSelected;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(487, 353);
            this.Controls.Add(this.gradientPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(493, 381);
            this.Name = "BlockDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Block Objects";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.BlockDialog_HelpRequested);
            this.gradientPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBlockSelected;
        private System.Windows.Forms.Button btnCancel;
        private GradientPanel gradientPanel1;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.ImageList listViewImages;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView lvRecommendationTypes;
        private System.Windows.Forms.ColumnHeader colRecommendation;
        private System.Windows.Forms.ListView lvDatabases;
        private System.Windows.Forms.ColumnHeader colDatabase;
    }
}