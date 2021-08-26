using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    [ToolboxBitmap(typeof (TabControl))]
    [Designer("Idera.SQLdm.DesktopClient.Controls.Designers.PropertiesControlDesigner, Idera.SQLdm.DesktopClient.Designers")]
    public partial class PropertiesControl : UserControl
    {
        private readonly PropertyPageCollection propertyPages = new PropertyPageCollection();
        private bool designModeMoving = false;

        public event EventHandler PropertyPageChanged;

        public PropertiesControl(bool isDarkThemeSelected = false)
        {
            InitializeComponent(isDarkThemeSelected);
            AllowListResize = false;

            propertyPages.PageAdded += new EventHandler(propertyPages_PageAdded);
            propertyPages.PageRemoved += new EventHandler(propertyPages_PageRemoved);
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool AllowListResize
        {
            get { return !splitContainer.IsSplitterFixed; }
            set { splitContainer.IsSplitterFixed = !value; }
        }

        [Category("Behavior")]
        [DefaultValue(150)]
        public int MenuListBoxWidth
        {
            get { return this.propertyPageListBox.Width; }
            set { this.propertyPageListBox.Width = value; }
        }

        [Category("Behavior")]
        [DefaultValue(150)]
        public int CustomSplitterDistance
        {
            get { return this.splitContainer.SplitterDistance; }
            set { this.splitContainer.SplitterDistance = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PropertyPageCollection PropertyPages
        {
            get { return propertyPages; }
        }

        [Browsable(false)]
        public int SelectedPropertyPageIndex
        {
            get { return propertyPageListBox.SelectedIndex; }
            set { propertyPageListBox.SelectedIndex = value; }
        }

        private void propertyPages_PageAdded(object sender, EventArgs e)
        {
            PropertyPageCollectionChangedEventArgs eventArgs = e as PropertyPageCollectionChangedEventArgs;

            if (eventArgs != null)
            {
                int index = propertyPageListBox.Items.Add(eventArgs.Page);
                eventArgs.Page.TextChanged += new EventHandler(PropertyPage_TextChanged);
                eventArgs.Page.TypeChanged += new EventHandler(PropertyPage_TypeChanged);
                
                if (DesignMode)
                {
                    propertyPageListBox.SelectedIndex = index;
                    splitContainer.Panel2.Controls.Clear();
                    splitContainer.Panel2.Controls.Add(eventArgs.Page);
                }
            }
        }

        private void propertyPages_PageRemoved(object sender, EventArgs e)
        {
            PropertyPageCollectionChangedEventArgs eventArgs = e as PropertyPageCollectionChangedEventArgs;

            if (eventArgs != null && !designModeMoving)
            {
                eventArgs.Page.TextChanged -= new EventHandler(PropertyPage_TextChanged);
                int selectedIndex = propertyPageListBox.Items.IndexOf(eventArgs.Page);

                if (selectedIndex >= 0)
                {
                    propertyPageListBox.Items.RemoveAt(selectedIndex);

                    if (propertyPageListBox.Items.Count > 0)
                    {
                        if (selectedIndex == propertyPageListBox.Items.Count)
                        {
                            propertyPageListBox.SelectedIndex = propertyPageListBox.Items.Count - 1;
                        }
                        else
                        {
                            propertyPageListBox.SelectedIndex = selectedIndex;
                        }
                    }
                }
            }
        }

        internal void DeleteSelectedPage()
        {
            propertyPages.RemoveAt(propertyPageListBox.SelectedIndex);
        }

        internal void MoveSelectedPageUp()
        {
            if (propertyPageListBox.SelectedIndex > 0 && propertyPageListBox.Items.Count > 1)
            {
                designModeMoving = true;

                int index = propertyPageListBox.SelectedIndex;
                PropertyPage page = propertyPages[index];

                propertyPages.RemoveAt(index);
                propertyPageListBox.Items.RemoveAt(index);

                propertyPages.Insert(index - 1, page);
                propertyPageListBox.Items.Insert(index - 1, page);

                designModeMoving = false;
            }
        }

        internal void MoveSelectedPageDown()
        {
            if (propertyPageListBox.SelectedIndex >= 0 &&
                propertyPageListBox.SelectedIndex != propertyPageListBox.Items.Count - 1 &&
                propertyPageListBox.Items.Count > 1)
            {
                designModeMoving = true;

                int index = propertyPageListBox.SelectedIndex;
                PropertyPage page = propertyPages[index];

                propertyPages.RemoveAt(index);
                propertyPageListBox.Items.RemoveAt(index);

                propertyPages.Insert(index + 1, page);
                propertyPageListBox.Items.Insert(index + 1, page);

                designModeMoving = false;
            }
        }

        private void PropertyPage_TextChanged(object sender, EventArgs e)
        {
            PropertyPage page = sender as PropertyPage;

            if (page != null)
            {
                propertyPageListBox.Invalidate();
            }
        }

        private void PropertyPage_TypeChanged(object sender, EventArgs e)
        {
            PropertyPage page = sender as PropertyPage;

            if (page != null)
            {
                int index = propertyPageListBox.Items.IndexOf(page);
                propertyPageListBox.Items.Remove(page);
                propertyPageListBox.Items.Insert(index, page);
            }
        }

        internal void DesignModeOnSelectionChanged()
        {
            ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));

            if (selectionService.PrimarySelection is PropertyPage)
            {
                PropertyPage page = selectionService.PrimarySelection as PropertyPage;

                if (propertyPages.Contains(page))
                {
                    int index = propertyPageListBox.Items.IndexOf(page);

                    if (index != -1 && index != propertyPageListBox.SelectedIndex)
                    {
                        propertyPageListBox.SelectedIndex = index;
                        splitContainer.Panel2.Controls.Clear();
                        splitContainer.Panel2.Controls.Add(page);
                    }
                }
            }
        }

        internal bool DesignModeHitTest(int x, int y)
        {
            Point clientPoint = propertyPageListBox.PointToClient(new Point(x, y));
            int index = propertyPageListBox.HitTest(clientPoint.X, clientPoint.Y);

            if (index == -1)
            {
                propertyPageListBox.DesignModeInvalidateHoverItem();
            }

            return index != -1;
        }

        private void propertyPageListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (DesignMode)
            {
                int index = propertyPageListBox.HitTest(e.X, e.Y);
                ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
                ArrayList selectedList = new ArrayList();

                if (index != -1)
                {
                    selectedList.Add(propertyPages[index]);
                    selectionService.SetSelectedComponents(selectedList);
                }
                else
                {
                    selectedList.Add(this);
                    selectionService.SetSelectedComponents(selectedList);
                }
            }
        }

        private void propertyPageListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = propertyPageListBox.SelectedIndex;

            if (index >= 0)
            {
                splitContainer.Panel2.Controls.Clear();
                splitContainer.Panel2.Controls.Add(propertyPages[index]);

                if (PropertyPageChanged != null)
                {
                    PropertyPageChanged(this, EventArgs.Empty);
                }
            }
        }

        public void applyDarkThemeColors()
        {
            if (this.propertyPageListBox != null)
            {
                this.propertyPageListBox.isDarkThemeSelected = true;
                this.propertyPageListBox.BackColor = ColorTranslator.FromHtml("#012A4F");
                this.propertyPageListBox.Invalidate();
            }
        }

        public void applyLightThemeColors()
        {
            if (this.propertyPageListBox != null)
            {
                this.propertyPageListBox.isDarkThemeSelected = false;
                this.propertyPageListBox.BackColor = Color.White;
                this.propertyPageListBox.Invalidate();
            }
        }
    }

    /// <summary>
    /// Represents a property page within the PropertiesControl.
    /// </summary>
    [ToolboxItem(false)]
    public class PropertyPage : Panel
    {
        private const string DefaultText = "New Page";
        private const string DividerName = "<DIVIDER>";

        private PropertyPageType type = PropertyPageType.Page;

        public new EventHandler TextChanged;
        public EventHandler TypeChanged;

        public PropertyPage()
        {
            Text = DefaultText;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (type != PropertyPageType.Divider)
                {
                    base.Text = value;
                    OnTextChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(PropertyPageType.Page)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public PropertyPageType Type
        {
            get { return type; }
            set
            {
                type = value;

                if (type == PropertyPageType.Divider)
                {
                    base.Text = DividerName;
                }
                else
                {
                    base.Text = "New Page";
                }

                OnTypeChanged(EventArgs.Empty);
            }
        }

        private new void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
        }

        private void OnTypeChanged(EventArgs e)
        {
            if (TypeChanged != null)
            {
                TypeChanged(this, e);
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>
    ///  Contains a collection of PropertyPages.
    /// </summary>
    public sealed class PropertyPageCollection : CollectionBase
    {
        internal PropertyPageCollection()
        {
        }

        public EventHandler PageAdded;
        public EventHandler PageRemoved;

        public PropertyPage this[int index]
        {
            get
            {
                return List[index] as PropertyPage;
            }
        }

        private void OnPageAdded(PropertyPageCollectionChangedEventArgs e)
        {
            if (PageAdded != null)
            {
                PageAdded(this, e);
            }
        }

        private void OnPageRemoved(PropertyPageCollectionChangedEventArgs e)
        {
            if (PageRemoved != null)
            {
                PageRemoved(this, e);
            }
        }

        public bool Contains(PropertyPage page)
        {
            return List.Contains(page);
        }

        public int Add(PropertyPage page)
        {
            page.Dock = DockStyle.Fill;
            int index = List.Add(page);
            OnPageAdded(new PropertyPageCollectionChangedEventArgs(page));
            return index;
        }

        public void Insert(int index, PropertyPage page)
        {
            List.Insert(index, page);
        }
        
        public void Remove(PropertyPage page)
        {
            if (List.Contains(page))
            {
                List.Remove(page);
                OnPageRemoved(new PropertyPageCollectionChangedEventArgs(page));
            }
        }

        public new void RemoveAt(int index)
        {
            if (index < Count)
            {
                PropertyPage page = this[index];
                List.RemoveAt(index);
                OnPageRemoved(new PropertyPageCollectionChangedEventArgs(page));
            }
        }
    }

    public class PropertyPageCollectionChangedEventArgs : EventArgs
    {
        private PropertyPage page;

        internal PropertyPageCollectionChangedEventArgs(PropertyPage page)
        {
            this.page = page;
        }

        public PropertyPage Page
        {
            get { return page; }
        }
    }

    /*

    /// <summary>
    /// Implements the control designer for editing the PropertiesControl at design time.
    /// </summary>
    internal class PropertiesControlDesigner : ControlDesigner
    {
        private PropertiesControl propertiesControl;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            propertiesControl = component as PropertiesControl;

            // Hook up events
            ISelectionService selectionService = (ISelectionService) GetService(typeof (ISelectionService));
            IComponentChangeService changeService = (IComponentChangeService) GetService(typeof (IComponentChangeService));
            selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
            changeService.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
        }

        public override ICollection AssociatedComponents
        {
            get { return propertiesControl.Controls; }
        }

        public override DesignerVerbCollection Verbs
        {
            get
            {
                DesignerVerbCollection verbs = new DesignerVerbCollection();
                verbs.Add(new DesignerVerb("Add Page", new EventHandler(OnAddPage)));
                verbs.Add(new DesignerVerb("Delete Page", new EventHandler(OnDeleteSelectedPage)));
                verbs.Add(new DesignerVerb("Move Up", new EventHandler(OnMoveUp)));
                verbs.Add(new DesignerVerb("Move Down", new EventHandler(OnMoveDown)));
                return verbs;
            }
        }

        private void OnAddPage(object sender, EventArgs e)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            DesignerTransaction designerTransaction;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Add a property page to the collection
            designerTransaction = designerHost.CreateTransaction("Add Page");
            PropertyPage newPage = (PropertyPage)designerHost.CreateComponent(typeof(PropertyPage));
            changeService.OnComponentChanging(propertiesControl, null);
            propertiesControl.PropertyPages.Add(newPage);
            changeService.OnComponentChanged(propertiesControl, null, null, null);
            designerTransaction.Commit();
        }

        private void OnDeleteSelectedPage(object sender, EventArgs e)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            DesignerTransaction designerTransaction;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Delete the selected page
            designerTransaction = designerHost.CreateTransaction("Delete Selected Page");
            changeService.OnComponentChanging(propertiesControl, null);
            propertiesControl.DeleteSelectedPage();
            changeService.OnComponentChanged(propertiesControl, null, null, null);
            designerTransaction.Commit();
        }

        private void OnMoveUp(object sender, EventArgs e)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            DesignerTransaction designerTransaction;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Delete the selected page
            designerTransaction = designerHost.CreateTransaction("Move Up");
            changeService.OnComponentChanging(propertiesControl, null);
            propertiesControl.MoveSelectedPageUp();
            changeService.OnComponentChanged(propertiesControl, null, null, null);
            designerTransaction.Commit();
        }

        private void OnMoveDown(object sender, EventArgs e)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            DesignerTransaction designerTransaction;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Delete the selected page
            designerTransaction = designerHost.CreateTransaction("Move Down");
            changeService.OnComponentChanging(propertiesControl, null);
            propertiesControl.MoveSelectedPageDown();
            changeService.OnComponentChanged(propertiesControl, null, null, null);
            designerTransaction.Commit();
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            propertiesControl.DesignModeOnSelectionChanged();
        }

        private void OnComponentRemoving(object sender, ComponentEventArgs e)
        {
            PropertyPage page;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));

            // If the user is removing a page
            if (e.Component is PropertyPage)
            {
                page = e.Component as PropertyPage;

                if (propertiesControl.PropertyPages.Contains(page))
                {
                    changeService.OnComponentChanging(propertiesControl, null);
                    propertiesControl.PropertyPages.Remove(page);
                    changeService.OnComponentChanged(propertiesControl, null, null, null);
                    return;
                }
            }

            // If the user is removing the control itself
            if (e.Component == propertiesControl)
            {
                for (int i = propertiesControl.PropertyPages.Count - 1; i >= 0; i--)
                {
                    page = propertiesControl.PropertyPages[i];
                    changeService.OnComponentChanging(propertiesControl, null);
                    propertiesControl.PropertyPages.Remove(page);
                    designerHost.DestroyComponent(page);
                    changeService.OnComponentChanged(propertiesControl, null, null, null);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Unhook events
            selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
            changeService.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);

            base.Dispose(disposing);
        }

        protected override bool GetHitTest(Point point)
        {
            if (propertiesControl.DesignModeHitTest(point.X, point.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    */

}