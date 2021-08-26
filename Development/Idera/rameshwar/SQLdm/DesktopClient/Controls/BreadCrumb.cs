using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.UI.Dialogs;

namespace Idera.SQLdm.DesktopClient.Controls
{
    [ToolboxBitmap(typeof (Panel))]
    internal partial class BreadCrumb : UserControl
    {
        public event EventHandler<BreadCrumbTrailChangedEventArgs> OnTrailChanged; // fired when the trail changes (user selects a part of the trail, etc)

        private List<string> trail;
        private List<Crumb>  crumbs;
        private const int maxcrumbs = 20;
        private Color coldForeColor;
        private Color hotForeColor;
        private Color hoverForeColor;
        private Font  coldFont;
        private Font  hotFont;
        private Font  dividerFont;
        private Label currentLabel;

        public IList<string> Trail
        {
            get
            {
                List<string> t = new List<string>();
                t.AddRange( trail );
                return t;
            }
        }

        public IList<Crumb> Crumbs
        {
            get
            {
                List<Crumb> c = new List<Crumb>();
                c.AddRange( crumbs );
                return c;
            }
        }

        public BreadCrumb()
        {
            InitializeComponent();

            trail  = new List<string>();
            crumbs = new List<Crumb>();

            coldForeColor  = Color.Black;
            hotForeColor   = Color.Blue;
            hoverForeColor = Color.Red;
            coldFont       = new Font(this.Font, FontStyle.Regular);
            hotFont        = new Font(this.Font, FontStyle.Underline);
            dividerFont    = new Font(coldFont, FontStyle.Bold);

            for (int i = 0; i < maxcrumbs; i++)
            {
                Label label = new Label();
                Label star  = new Label();
                Label arrow = new Label();

                label.Margin    = new Padding(0);
                label.Padding   = new Padding(1, 3, 0, 3);
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Visible   = false;
                label.AutoSize  = true;
                label.Text      = "";                
                label.Font      = hotFont;
                label.Tag       = i;
                label.MouseClick += new MouseEventHandler( label_MouseClick );
                label.MouseEnter += new EventHandler( label_MouseEnter );
                label.MouseLeave += new EventHandler( label_MouseLeave );

                star.Margin    = new Padding(0);
                star.Padding   = new Padding(0, 3, 1, 0);
                star.TextAlign = ContentAlignment.TopLeft;
                star.Visible   = false;
                star.AutoSize  = true;
                star.Text      = i == 0 ? "" : "x";
                star.Font      = new Font( this.Font.FontFamily, 6.0f, FontStyle.Regular );
                star.ForeColor = Color.Red;
                star.Tag       = -i;
                star.MouseClick += new MouseEventHandler( star_MouseClick );
                star.MouseEnter += new EventHandler( label_MouseEnter );
                star.MouseLeave += new EventHandler( label_MouseLeave );

                arrow.Margin    = new Padding(0);
                arrow.Padding   = new Padding(0, 3, 0, 3);
                arrow.TextAlign = ContentAlignment.MiddleCenter;
                arrow.Visible   = false;
                arrow.AutoSize  = true;
                arrow.Text      = ">";
                arrow.Font      = dividerFont;

                this.flowLayoutPanel1.Controls.Add(label);
                this.flowLayoutPanel1.Controls.Add(star);
                this.flowLayoutPanel1.Controls.Add(arrow);
            }
        }

        private void star_MouseClick( object sender, MouseEventArgs e )
        {
            if( e.Button == MouseButtons.Left )
                RemoveCrumb( sender, e );                
            else
                ShowCrumbDialog( sender, e );
        }

        private void label_MouseClick( object sender, MouseEventArgs e )
        {
            if( e.Button == MouseButtons.Left )
                SelectCrumb( sender, e );
            else
                ShowCrumbDialog( sender, e );
        }

        private void BreadCrumb_Load( object sender, EventArgs e )
        {
            UpdateTrail();
        }        

        private void SelectCrumb(object sender, EventArgs args)
        {
            if (trail.Count == 0)
                return;

            Label label = sender as Label;

            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                if (flowLayoutPanel1.Controls[i] == label)
                {
                    int idx = (int)flowLayoutPanel1.Controls[i].Tag;

                    if( idx < trail.Count - 1 )
                    {
                        trail.RemoveRange( idx + 1, trail.Count - idx - 1 );
                        crumbs.RemoveRange( idx + 1, crumbs.Count - idx - 1 );
                    }

                    break;
                }
            }

            UpdateTrail();
            NotifyTrailChanged();
        }

        private void RemoveCrumb( object sender, EventArgs args )
        {
            if (trail.Count == 0)
                return;

            Label label = sender as Label;
            int idx     = -( int )label.Tag;

            if( idx >= trail.Count )
                idx = trail.Count - 1;

            trail.RemoveAt( idx );
            crumbs.RemoveAt( idx );

            UpdateTrail();
            NotifyTrailChanged();
        }

        private void ShowCrumbDialog( object sender, MouseEventArgs e )
        {
            currentLabel = sender as Label;

            contextMenuStrip1.Show(currentLabel, e.Location );
        }

        private void UpdateTrail()
        {            
            int idx = 0;

            for (int i = 0; i < trail.Count; i++)
            {
                // text
                flowLayoutPanel1.Controls[idx].Visible   = true;
                flowLayoutPanel1.Controls[idx].Font      = hotFont;
                flowLayoutPanel1.Controls[idx].ForeColor = hotForeColor;

                if (flowLayoutPanel1.Controls[idx].Text != trail[i])
                    flowLayoutPanel1.Controls[idx].Text = trail[i];

                // star
                flowLayoutPanel1.Controls[idx + 1].Visible   = true;
                flowLayoutPanel1.Controls[idx + 1].ForeColor = Color.Red;

                // arrow
                flowLayoutPanel1.Controls[idx + 2].Visible   = true;
                flowLayoutPanel1.Controls[idx + 2].Font      = dividerFont;
                flowLayoutPanel1.Controls[idx + 2].ForeColor = coldForeColor;

                idx += 3;
            }

            idx = trail.Count * 3;

            for (int i = idx; i < flowLayoutPanel1.Controls.Count-1; i += 3)
            {
                flowLayoutPanel1.Controls[i].Visible     = false;
                flowLayoutPanel1.Controls[i + 1].Visible = false;
                flowLayoutPanel1.Controls[i + 2].Visible = false;
            }

            if (trail.Count > 0)
            {   
                flowLayoutPanel1.Controls[idx-3].Font      = coldFont;
                flowLayoutPanel1.Controls[idx-3].ForeColor = coldForeColor;
                //flowLayoutPanel1.Controls[idx-2].Font      = new Font( this.Font.FontFamily, 6.0f, FontStyle.Regular );
                flowLayoutPanel1.Controls[idx-2].ForeColor = Color.Red;
                flowLayoutPanel1.Controls[idx-2].Visible   = true;
                flowLayoutPanel1.Controls[idx-1].Visible   = false;
            }
        }    

        private void NotifyTrailChanged()
        {
            if( OnTrailChanged != null )
                OnTrailChanged( this, new BreadCrumbTrailChangedEventArgs( Crumbs ) );
        }

        private void label_MouseLeave(object sender, EventArgs e)
        {
            Label label = sender as Label;
            int idx = (int)label.Tag;

            if (idx == trail.Count - 1)
                return;

            label.ForeColor = idx < 0 ? Color.Red : hotForeColor;
        }

        private void label_MouseEnter(object sender, EventArgs e)
        {
            Label label = sender as Label;
            int idx = (int)label.Tag;

            if( idx == trail.Count - 1 )
                return;

            label.ForeColor = hoverForeColor;
        }

        public void AddCrumb(string text)
        {
            AddCrumb( text, string.Empty, string.Empty );
        }

        public void AddCrumb( string text, string tag, string fulltext )
        {
            if (trail.Count == maxcrumbs)
                return;

            trail.Add( text );
            crumbs.Add( new Crumb( text, tag, fulltext ) );

            UpdateTrail();
        }

        public void RemoveLastCrumb()
        {
            if (trail.Count == 0)
                return;

            trail.RemoveAt( trail.Count - 1 );
            crumbs.RemoveAt( crumbs.Count - 1 );

            UpdateTrail();
        }

        private void toolStripMenuItem1_Click( object sender, EventArgs e )
        {
            if( currentLabel == null )
                return;

            int idx = (int)currentLabel.Tag;

            if( idx < 0 )
                idx = -idx;

            if( idx >= crumbs.Count )
                return;            

            ApplicationMessageBox.ShowMessage( crumbs[idx].CrumbNameFull );
        }
    }

    internal class BreadCrumbTrailChangedEventArgs : EventArgs
    {
        private List<Crumb> trail;

        public IList<Crumb> Crumbs
        {
            get
            {
                return trail.AsReadOnly();
            }
        }

        public BreadCrumbTrailChangedEventArgs(IList<Crumb> crumbs)
        {
            this.trail = new List<Crumb>();
            this.trail.AddRange( crumbs );
        }
    }

    internal class Crumb
    {
        public string CrumbName;
        public string CrumbNameFull;
        public string Tag;

        public Crumb(string crumb, string tag)
        {
            this.CrumbName     = crumb;
            this.CrumbNameFull = crumb;
            this.Tag           = tag;            
        }

        public Crumb(string crumb, string tag, string fullcrumb)
        {
            this.CrumbName     = crumb;
            this.CrumbNameFull = string.IsNullOrEmpty(fullcrumb) ? crumb : fullcrumb;
            this.Tag           = tag;  
        }
    }    
}
