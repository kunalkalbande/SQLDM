namespace Idera.SQLdm.Common.UI.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinEditors;
    using Infragistics.Win.UltraWinListView;
    using Infragistics.Win.UltraWinMaskedEdit;
    using Microsoft.SqlServer.Management.Smo;

    public partial class TimeComboEditor : UltraDateTimeEditor
    {
        private static TimeSpan DEFAULT_LIST_INTERVAL = TimeSpan.FromMinutes(30);
        private TimeSpan listInterval;
        private int listHeight;
        private bool isInitialized;

        private TimeDropDownList dropDownList;

        public TimeComboEditor()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            listInterval = TimeSpan.FromMinutes(30);
            listHeight = 125;
            MaskInput = "{time}";
            ButtonsRight.Clear();
            DropDownButtonDisplayStyle = ButtonDisplayStyle.Never;

            ResumeLayout(false);
        }

        #region Initialize
        private void Initialize()
        {
            if (this.isInitialized)
                return;

            // create the dropdown tree the first time it drops down
            dropDownList = new TimeDropDownList(this);
            dropDownList.Visible = false;
            dropDownList.Size = new Size(125, 125);
            dropDownList.MouseUp += new MouseEventHandler(this.OnFolderTreeMouseUp);
            dropDownList.MouseDown += new MouseEventHandler(this.OnFolderTreeMouseDown);
            dropDownList.KeyDown += new KeyEventHandler(this.OnFolderTreeKeyDown);

            this.Controls.Add(dropDownList);

            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton = null;
            int i = this.ButtonsRight.IndexOf("DropDownList");
            if (i == -1)
            {
                dropDownEditorButton = new DropDownEditorButton("DropDownList");
                this.ButtonsRight.Add(dropDownEditorButton);
            }
            else
                dropDownEditorButton = this.ButtonsRight.GetItem(i) as DropDownEditorButton;

            if (dropDownEditorButton != null)
            {
                dropDownEditorButton.Control = dropDownList;  
            }

            this.isInitialized = true;
        }
        #endregion //Initialize
        
        [Description("Interval to use when building the drop-down list.")]
        public TimeSpan ListInterval
        {
            get { return listInterval; }
            set 
            { 
                listInterval = value; 
               
            }
        }

        [DefaultValue(125)]
        [Description("Height of the drop-down list.")]
        public int ListHeight
        {
            get { return listHeight;  }
            set 
            { 
                listHeight = value;
                if (dropDownList != null)
                    dropDownList.Height = listHeight;
            }
        }

        public TimeSpan Time
        {
            get { return this.DateTime.TimeOfDay; }
            set { this.DateTime = DateTime.Now.Date + value;  }
        }

        protected override void OnBeforeEditorButtonDropDown(BeforeEditorButtonDropDownEventArgs e)
        {
            base.OnBeforeEditorButtonDropDown(e);

            if (ReadOnly)
            {   // if we are readonly cancel the dropdown
                e.Cancel = true;
            }
            else
            {
                if (e.Button.Key == "DropDownList")
                {
                    BuildSelectionList();

                    dropDownList.Width = Width;
                    if (dropDownList != null)
                    {
                        dropDownList.SelectItem(this.DateTime);
                    }
                }
            }
        }


        private void BuildSelectionList()
        {
            this.dropDownList.Items.Clear();

            TimeSpan interval = ListInterval;
            DateTime dateTime = DateTime.Now.Date;
            for (TimeSpan time = new TimeSpan(0, 0, 0); time.Days != 1; time = time.Add(interval))
            {
                this.dropDownList.AddTime(dateTime + time);
            }
        }

        #region OnEndInit
        /// <summary>
        /// Invoked during the <see cref="ISupportInitialize.EndInit"/> of the component.
        /// </summary>
        protected override void OnEndInit()
        {
            this.Initialize();

            base.OnEndInit();
        }
        #endregion //OnEndInit

        #region OnCreateControl
        protected override void OnCreateControl()
        {
            this.Initialize();

            base.OnCreateControl();
        }
        #endregion //OnCreateControl

        private bool isMouseDownInNodeArea = false;

        #region UltraExplorerTree events
        private void OnFolderTreeMouseUp(object sender, MouseEventArgs e)
        {
            if (this.isMouseDownInNodeArea)
            {
                DropDownEditorButton dd = DropDownEditorButton.FromControl(dropDownList);
                if (dd != null)
                {
                    if (dropDownList.SelectedItems.Count > 0)
                    {
                        DateTime current = this.DateTime;
                        current -= current.TimeOfDay;
                        current += (TimeSpan) dropDownList.SelectedItems[0].Tag;
                        this.DateTime = current;
                    }
                    dd.CloseUp();
                }
            }
        }

        private void OnFolderTreeMouseDown(object sender, MouseEventArgs e)
        {
            this.isMouseDownInNodeArea = false;

            if (e.Button == MouseButtons.Left)
            {
                UIElement element = dropDownList.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                if (element != null)
                {
                    this.isMouseDownInNodeArea =
                        element is Infragistics.Win.UltraWinListView.UltraListViewItemEditorAreaUIElement ||
                        element is Infragistics.Win.EditorWithTextDisplayTextUIElement;                    
                }
            }
        }

        private void OnFolderTreeKeyDown(object sender, KeyEventArgs e)
        {
            // when the enter key is pressed, close up
            if (e.KeyData == Keys.Return)
            {
                DropDownEditorButton dd = DropDownEditorButton.FromControl(dropDownList);
                if (dd != null)
                {
                    if (dropDownList.SelectedItems.Count > 0)
                    {
                        DateTime current = this.DateTime;
                        current -= current.TimeOfDay;
                        current += (TimeSpan)dropDownList.SelectedItems[0].Tag;
                        this.DateTime = current;
                    }
                    dd.CloseUp();
                    e.Handled = true;
                }
            }
        }
        #endregion //UltraExplorerTree events

        internal class TimeDropDownList : Infragistics.Win.UltraWinListView.UltraListView
        {
            private TimeComboEditor editor;
            private bool isInitialized = false;

            internal TimeDropDownList(TimeComboEditor editor)
            {
                this.editor = editor;
                InitializeComponent();
            }

            private void InitializeComponent()
            {
                ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
                this.SuspendLayout();

                // only need to work in details mode
                View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
                ViewSettingsList.ColumnWidth = -1;
                ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
                ViewSettingsList.MultiColumn = false;
                BorderStyle = UIElementBorderStyle.Rounded1;

                ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
                this.ResumeLayout(false);
            }

            #region Initialize
            private void Initialize()
            {
                if (this.isInitialized)
                    return;

                this.isInitialized = true;
            }
            #endregion //Initialize

            [EditorBrowsable(EditorBrowsableState.Advanced)]
            public bool ShouldSerializeNodes()
            {
                return false;
            }

            public void SelectItem(DateTime dateTime)
            {
                UltraListViewItem item = FindClosestItem(dateTime);
                if (item != null)
                {
                    this.SelectedItems.Clear();
                    this.SelectedItems.Add(item);
                    this.ActiveItem = item;

                    // the window isn't visible yet so we have to post this
                    Delegate post = new MethodInvoker(BringActiveItemIntoView);
                    this.BeginInvoke(post);
                }
            }

            public void BringActiveItemIntoView()
            {
                UltraListViewItem item = ActiveItem;
                if (item != null)
                    item.BringIntoView();
            }

            public void AddTime(DateTime dateTime)
            {
                string format = editor.FormatString;
                if (String.IsNullOrEmpty(format))
                {
                    format = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
                }
                string label = dateTime.ToString(format);

                TimeSpan time = dateTime.TimeOfDay;

                this.Items.Add(time.ToString(), label).Tag = time;
            }

//            public void AddTimeSpan(TimeSpan timeSpan, DateTimeFormatInfo dtfi)
//            {
//                string label = TimeSpanToString(timeSpan, dtfi);
//                this.Items.Add(timeSpan.ToString(), label).Tag = timeSpan;
//            }

            private string TimeSpanToString(TimeSpan time, DateTimeFormatInfo formatInfo)
            {
                int hour = time.Hours;
                if (hour > 12)
                    hour -= 12;
                if (hour == 0)
                    hour = 12;

                return String.Format(
                    "{0}{1}{2:00} {3}",
                    hour,
                    formatInfo.TimeSeparator,
                    time.Minutes,
                    time.Hours < 12 ? formatInfo.AMDesignator : formatInfo.PMDesignator);
            }


            #region FindClosestItem

            private UltraListViewItem FindClosestItem(DateTime dateTime)
            {
                return FindClosestItem(dateTime.TimeOfDay);
            }

            private UltraListViewItem FindClosestItem(TimeSpan timeSpan)
            {
                if (!this.isInitialized)
                    this.Initialize();

                if (this.Items.Count == 0)
                    return null;

                if (timeSpan == TimeSpan.MinValue)
                {
                    return this.Items[0];
                }

                if (timeSpan == TimeSpan.MaxValue)
                {
                    return this.Items[this.Items.Count - 1];
                }

                int i = Items.IndexOf(timeSpan.ToString());
                if (i == -1)
                    return null;

                return Items[i];
            }
            #endregion //FindClosestNode

            private bool IsOverScrollBar()
            {
                Infragistics.Win.UIElement lastElement = this.ControlUIElement.LastElementEntered;

                if (lastElement == null)
                    return false;

                if (lastElement is Infragistics.Win.UltraWinScrollBar.ScrollBarUIElement)
                    return true;

                return lastElement.GetAncestor(typeof(Infragistics.Win.UltraWinScrollBar.ScrollBarUIElement)) != null;
            }

            #region OnEndInit
            /// <summary>
            /// Invoked during the <see cref="ISupportInitialize.EndInit"/> of the component.
            /// </summary>
            protected override void OnEndInit()
            {
                this.Initialize();

                base.OnEndInit();
            }
            #endregion //OnEndInit

            #region OnCreateControl
            protected override void OnCreateControl()
            {
                this.Initialize();

                base.OnCreateControl();
            }
            #endregion //OnCreateControl

            #region OnMouseEnterElement

            protected override void OnMouseEnterElement(UIElementEventArgs e)
            {
                base.OnMouseEnterElement(e);

                if (e.Element.SelectableItem == null)
                    return;

                UltraListViewItem item = e.Element.SelectableItem as UltraListViewItem; 

                if (SelectedItems.Count > 0)
                {
                    if (item == SelectedItems[0])
                        return;

                    SelectedItems.Clear();
                }
                SelectedItems.Add(item);
                ActiveItem = item;
            }

            #endregion //OnMouseEnterElement


        }

    }
}
