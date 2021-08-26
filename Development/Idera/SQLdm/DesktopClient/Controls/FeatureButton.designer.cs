namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class FeatureButton
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
            this.featureImagePictureBox = new System.Windows.Forms.PictureBox();
            this.featureDescriptionLabel = new System.Windows.Forms.Label();
            this.featureHeaderLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.featureImagePictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // featureImagePictureBox
            // 
            this.featureImagePictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.featureImagePictureBox.BackColor = System.Drawing.Color.Transparent;
            this.featureImagePictureBox.Location = new System.Drawing.Point(3, 9);
            this.featureImagePictureBox.Name = "featureImagePictureBox";
            this.tableLayoutPanel1.SetRowSpan(this.featureImagePictureBox, 2);
            this.featureImagePictureBox.Size = new System.Drawing.Size(32, 32);
            this.featureImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.featureImagePictureBox.TabIndex = 0;
            this.featureImagePictureBox.TabStop = false;
            this.featureImagePictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
            this.featureImagePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
            this.featureImagePictureBox.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
            this.featureImagePictureBox.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
            this.featureImagePictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseMove);
            this.featureImagePictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseUp);
            // 
            // featureDescriptionLabel
            // 
            this.featureDescriptionLabel.AutoEllipsis = true;
            this.featureDescriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.featureDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.featureDescriptionLabel.Location = new System.Drawing.Point(41, 18);
            this.featureDescriptionLabel.Name = "featureDescriptionLabel";
            this.featureDescriptionLabel.Size = new System.Drawing.Size(262, 32);
            this.featureDescriptionLabel.TabIndex = 2;
            this.featureDescriptionLabel.Text = "<description>";
            this.featureDescriptionLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
            this.featureDescriptionLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
            this.featureDescriptionLabel.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
            this.featureDescriptionLabel.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
            this.featureDescriptionLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseMove);
            this.featureDescriptionLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseUp);
            // 
            // featureHeaderLabel
            // 
            this.featureHeaderLabel.BackColor = System.Drawing.Color.Transparent;
            this.featureHeaderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.featureHeaderLabel.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.featureHeaderLabel.ForeColor = System.Drawing.Color.Red;
            this.featureHeaderLabel.Location = new System.Drawing.Point(41, 0);
            this.featureHeaderLabel.Name = "featureHeaderLabel";
            this.featureHeaderLabel.Size = new System.Drawing.Size(262, 18);
            this.featureHeaderLabel.TabIndex = 1;
            this.featureHeaderLabel.Text = "<header>";
            this.featureHeaderLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
            this.featureHeaderLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
            this.featureHeaderLabel.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
            this.featureHeaderLabel.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
            this.featureHeaderLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseMove);
            this.featureHeaderLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseUp);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.featureDescriptionLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.featureImagePictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.featureHeaderLabel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(306, 50);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // FeatureButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(0, 40);
            this.Name = "FeatureButton";
            this.Size = new System.Drawing.Size(306, 50);
            ((System.ComponentModel.ISupportInitialize)(this.featureImagePictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox featureImagePictureBox;
        private System.Windows.Forms.Label featureDescriptionLabel;
        private System.Windows.Forms.Label featureHeaderLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
