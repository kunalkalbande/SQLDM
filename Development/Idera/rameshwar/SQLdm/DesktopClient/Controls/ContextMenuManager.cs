using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using BBS.TracerX;
using Infragistics.Win.UltraWinToolbars;
using Control = System.Windows.Forms.Control;
using Point = System.Drawing.Point;

namespace Idera.SQLdm.DesktopClient.Controls
{
    /// <summary>
    /// This turd is necessary to work around problems with context menus on winforms controls sticking around.
    /// No matter how hard you try to polish this thing, you must remember it is still a turd.  The more you try,
    /// the more shit you'll get under your finger nails.
    /// </summary>
    
    public partial class ContextMenuManager : UltraToolbarsManager
    {
        private static Logger LOG = Logger.GetLogger("ContextMenuManager");
        public ContextMenuManager(IContainer container) : base(container)
        {
        }

        private Hashtable contextMenuUltras = null;
        private Hashtable ContextMenuUltras
        {
            get
            {
                if (this.contextMenuUltras == null)
                    this.contextMenuUltras = new Hashtable();

                return this.contextMenuUltras;
            }
        }

        public new string GetContextMenuUltra(Component component)
        {
            if (!this.ContextMenuUltras.Contains(component))
                return null;

            return ((ContextMenuUltraData)(this.ContextMenuUltras[component])).Key;
        }

        private ContextMenuUltraData GetContextMenuUltraData(Component component)
        {
            if (!this.ContextMenuUltras.Contains(component))
                return null;

            return (ContextMenuUltraData)this.ContextMenuUltras[component];
        }

        public new void SetContextMenuUltra(Component component, string menuKey)
        {
            if (component == null)
                throw new ArgumentNullException();

            if (!((IExtenderProvider)this).CanExtend(component))
                throw new ArgumentException("component");

            bool shouldRemove = string.IsNullOrEmpty(menuKey);

            ContextMenuUltraData data = null;

            if (this.ContextMenuUltras.Contains(component))
                data = (ContextMenuUltraData)this.ContextMenuUltras[component];

            if (data != null)
            {
                if (shouldRemove)
                {
                    // the collection contains the specified component and the text should
                    // be removed
                    this.ContextMenuUltras.Remove(component);

                    if (data.Menu != null)
                    {
                        // MRS 8/25/2009 - TFS19973
                        //data.Menu.Popup -= new EventHandler( this.OnContextMenuPopup );
                        data.Menu.Opening -= new CancelEventHandler(this.OnContextMenuPopup);

                        data.Menu.Dispose();
                    }
                }
                else
                {
                    // the component is in our collection so we need to update the
                    // key property
                    data.Key = menuKey;
                }
            }
            else if (!shouldRemove)
            {
                // MRS 8/25/2009 - TFS19973
                //ContextMenu contextMenuSurrogate = null;
                ContextMenuStrip contextMenuSurrogate = null;

                if (!this.DesignMode)
                {
                    // MRS 8/25/2009 - TFS19973
                    //contextMenuSurrogate = new ContextMenu();
                    //contextMenuSurrogate.Popup += new EventHandler(this.OnContextMenuPopup);
                    contextMenuSurrogate = new ContextMenuStrip();
                    contextMenuSurrogate.Opening += new CancelEventHandler(this.OnContextMenuPopup);
                }

                // the text is a valid string and we have not yet added the component.
                this.ContextMenuUltras.Add(component, new ContextMenuUltraData(menuKey, contextMenuSurrogate));

                try
                {
                    var control = component as Control;

                    // ChartFx gets pissy when you try to assign it a context menu strip
                    // We handle Charts a different way so just return to prevent the exception
                    if (control is ChartFX.WinForms.Chart) return;
                    
                    // Set the ContextMenu property of the control to our surrogate menu
                    // to make sure both properties are not set.
                    // note: at design time this will be null.
                    if (control != null)
                        // MRS 8/25/2009 - TFS19973
                        //control.ContextMenu = contextMenuSurrogate;
                        control.ContextMenuStrip = contextMenuSurrogate;
                    else
                    {
                        // JJD 5/09/03
                        // Added support for NotifyIcon components
                        NotifyIcon notifyIcon = component as NotifyIcon;

                        if (notifyIcon != null)
                            // MRS 8/25/2009 - TFS19973
                            //notifyIcon.ContextMenu = contextMenuSurrogate;
                            notifyIcon.ContextMenuStrip = contextMenuSurrogate;
                    }
                }
                catch { }
            }
        }

        private void OnContextMenuPopup(object sender, CancelEventArgs e)
        {
            var contextMenu = sender as ContextMenuStrip;
            if (contextMenu == null)
                return;

            NotifyIcon notifyIcon = null;
            string key = null;

            var control = contextMenu.SourceControl;
            if (control != null)
            {
                key = this.GetContextMenuUltra(control);

                //	BF 4.16.03	UWG2025
                //
                //	If the SourceControl property of the context menu is
                //	an edit control that is a part of an embeddable editor,
                //	the GetContextMenuUltra method will not return a useable
                //	key, and the ContextMenu will not be displayed. If the
                //	attempt to get the key from the SourceControl fails,
                //	walk up the parent chain and check the key of each
                //	ancestor along the way.
                //
                if (key == null)
                {
                    var parentControl = control.Parent;
                    while (parentControl != null)
                    {
                        key = this.GetContextMenuUltra(parentControl);
                        if (key != null)
                            break;

                        parentControl = parentControl.Parent;
                    }
                }
            }
            else
            {
                if (this.contextMenuUltras != null)
                {
                    foreach (DictionaryEntry entry in this.contextMenuUltras)
                    {
                        var data = entry.Value as ContextMenuUltraData;
                        if (data != null)
                        {
                            if (data.Menu == contextMenu)
                            {
                                key = data.Key;
                                control = entry.Key as Control;
                                if (control == null)
                                    notifyIcon = entry.Key as NotifyIcon;
                                break;
                            }
                        }
                    }
                }
            }

            if (key == null)
                return;

            try
            {
                if (notifyIcon != null)
                {
                    this.ShowPopupForNotifyIcon(key, Control.MousePosition);
                    return;
                }

                var screenPoint = Control.MousePosition;
                this.ShowPopup(key, screenPoint, control);
            }
            catch { }
        }

        public new void ShowPopup(string popupToolKey, Point screenPoint)
        {
            this.ShowPopup(popupToolKey, screenPoint, null);
        }

        public new void ShowPopup(string popupToolKey, Point screenPoint, Control sourceControl)
        {
            ShowPopup(popupToolKey, screenPoint, sourceControl, false);
        }

        private static IderaToolStripRenderer MenuRenderer = new IderaToolStripRenderer(new CustomMenuColorTable());
        public new void ShowPopup(string popupToolKey, Point screenPoint, Control sourceControl, bool showMiniToolbar)
        {
            var owner = sourceControl;
            if (owner == null)
            {
                owner = this.DockWithinContainer;
                if (owner == null)
                {
                    owner = this.Container as Control;
                    if (owner == null) return;
                }
            }

            var menu = BuildContextMenu(popupToolKey, sourceControl);
            if (menu == null) return;

            menu.Show(owner, owner.PointToClient(screenPoint));
        }

        private ContextMenuStrip BuildContextMenu(string key, Control sourceControl)
        {
            var popupMenuTool = this.Tools[key] as PopupMenuTool;
            if (popupMenuTool == null) return null;

            // fire before dropdown event to allow updates to the menu status
            bool cancel = false;
            BeforeDropDown(popupMenuTool, sourceControl, ref cancel);
            if (cancel) return null;

            var result = new ContextMenuStrip();
            result.Renderer = MenuRenderer;

            result.Tag = new ToolDropdownEventArgs(popupMenuTool, sourceControl);
            result.Closed += OnContextMenuClosed;
            result.ItemClicked += OnToolClick;

            BuildContextMenu(result.Items, popupMenuTool, false);

            return result.Items.Count == 0 ? null : result;
        }

        void OnContextMenuClosed(Object sender, ToolStripDropDownClosedEventArgs e)
        {
            var menu = sender as ContextMenuStrip;
            if (menu == null) return;
            var args = menu.Tag as ToolDropdownEventArgs;
            if (args != null)
                OnAfterToolCloseup(args);
        }

        private void BuildContextMenu(ToolStripItemCollection items, PopupMenuTool root, bool isSubMenu)
        {
            object lastItem = null;

            foreach (var tool in root.Tools)
            {
                if (tool.IsFirstInGroupResolved)
                {
                    if (!(lastItem is ToolStripSeparator) && items.Count > 0)
                    {
                        lastItem = new ToolStripSeparator();
                        items.Add((ToolStripSeparator)lastItem);
                    }
                }

                // skip invisible tools
                if (!tool.VisibleResolved) continue;

                var item = new ToolStripMenuItem();
                item.Name = tool.Key;
                item.Enabled = tool.EnabledResolved;
                //item.ToolTip = tool.ToolTipTextResolved;
                item.Text = tool.CaptionResolved;
                //item.Click += OnToolClick;
                item.Click += new EventHandler(item_Click);
                item.Tag = isSubMenu;

                if (tool.InstanceDisplaysImage)
                {
                    var image = tool.SharedPropsInternal.AppearancesSmall.Appearance.Image as System.Drawing.Image;
                    if (image != null)
                        item.Image = image;
                }

                if (tool is PopupMenuTool)
                {
                    BuildContextMenu(item.DropDown.Items, (PopupMenuTool)tool, true);
                    if (item.DropDown.Items.Count == 0)
                        item.Enabled = false;
                }

                items.Add(item);
                lastItem = item;
            }

            // last item shound not be a separator
            if (lastItem is ToolStripSeparator)
                items.Remove((ToolStripSeparator)lastItem);
        }

        void item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            bool isSubMenu = (bool)menuItem.Tag;

            if (!isSubMenu)
                return;

            ToolStripItemClickedEventArgs args = new ToolStripItemClickedEventArgs(menuItem);
            OnToolClick(sender, args);
        }        

        private void BeforeDropDown(PopupMenuTool popupTool, Control sourceControl, ref bool cancel)
        {
            var args = new BeforeToolDropdownEventArgs(popupTool, sourceControl);

            OnBeforeToolDropdown(args);

            cancel = args.Cancel;
        }

        void OnToolClick(object sender, ToolStripItemClickedEventArgs e)
        {   
            var menuItem = e.ClickedItem;
            if (menuItem == null) return;

            ToolBase tool = null;
            try
            {
                tool = Tools[menuItem.Name];
                
                var stateTool = tool as StateButtonTool;
                if (stateTool != null)
                    stateTool.InitializeChecked(!stateTool.Checked);
            } catch (Exception x)
            {
                Control parent = null;
                var owner = e.ClickedItem.GetCurrentParent();
                if (owner != null)
                    parent = owner.Parent as Control;
                if (parent == null)
                    LOG.ErrorFormat("Requested menu not found: {0} - {1}", menuItem.Name, e);
                else
                    LOG.ErrorFormat("Requested menu not found: {0} for control {1} - {2}", menuItem.Name, parent, e);

                return;
            }

            var tcea = new ToolClickEventArgs(tool, null);
            this.OnToolClick(tcea);
        }

        private class ContextMenuUltraData
        {
            private string key;

            internal ContextMenuUltraData(string key, ContextMenuStrip menu)
            {
                this.key = key;
                this.Menu = menu;
            }

            internal string Key
            {
                get { return this.key; }
                set { this.key = value; }
            }

            internal ContextMenuStrip Menu { get; private set; }
        }
    }

    #region menu implementation

    class CustomMenuColorTable : ProfessionalColorTable
    {
        public override Color ButtonSelectedHighlight
        {
            get { return ButtonSelectedGradientMiddle; }
        }
        public override Color ButtonSelectedHighlightBorder
        {
            get { return ButtonSelectedBorder; }
        }
        public override Color ButtonPressedHighlight
        {
            get { return ButtonPressedGradientMiddle; }
        }
        public override Color ButtonPressedHighlightBorder
        {
            get { return ButtonPressedBorder; }
        }
        public override Color ButtonCheckedHighlight
        {
            get { return ButtonCheckedGradientMiddle; }
        }
        public override Color ButtonCheckedHighlightBorder
        {
            get { return ButtonSelectedBorder; }
        }
        public override Color ButtonPressedBorder
        {
            get { return ButtonSelectedBorder; }
        }
        public override Color ButtonSelectedBorder
        {
            get { return Color.FromArgb(255, 83, 83, 83); }
        }
        public override Color ButtonCheckedGradientBegin
        {
            get { return Color.FromArgb(255, 255, 232, 166); }
        }
        public override Color ButtonCheckedGradientMiddle
        {
            get { return Color.FromArgb(255, 255, 232, 166); }
        }
        public override Color ButtonCheckedGradientEnd
        {
            get { return Color.FromArgb(255, 255, 252, 242); }
        }
        public override Color ButtonSelectedGradientBegin
        {
            get { return Color.FromArgb(255, 128, 128, 128); }
        }
        public override Color ButtonSelectedGradientMiddle
        {
            get { return Color.FromArgb(255, 128, 128, 128); }
        }
        public override Color ButtonSelectedGradientEnd
        {
            get { return Color.FromArgb(255, 128, 128, 128); }
        }
        public override Color ButtonPressedGradientBegin
        {
            get { return Color.FromArgb(255, 255, 232, 166); }
        }
        public override Color ButtonPressedGradientMiddle
        {
            get { return Color.FromArgb(255, 255, 232, 166); }
        }
        public override Color ButtonPressedGradientEnd
        {
            get { return Color.FromArgb(255, 255, 232, 166); }
        }
        public override Color CheckBackground
        {
            get { return Color.FromArgb(255, 255, 230, 162); }
        }
        public override Color CheckSelectedBackground
        {
            get { return Color.FromArgb(255, 255, 230, 162); }
        }
        public override Color CheckPressedBackground
        {
            get { return Color.FromArgb(255, 253, 170, 96); }
        }
        public override Color GripDark
        {
            get { return Color.FromArgb(255, 126, 138, 154); }
        }
        public override Color GripLight
        {
            get { return Color.FromArgb(255, 227, 230, 232); }
        }
        public override Color ImageMarginGradientBegin
        {
            get { return Color.FromArgb(255, 242, 244, 245); }
        }
        public override Color ImageMarginGradientMiddle
        {
            get { return Color.FromArgb(255, 242, 244, 245); }
        }
        public override Color ImageMarginGradientEnd
        {
            get { return Color.FromArgb(255, 242, 244, 245); }
        }
        public override Color ImageMarginRevealedGradientBegin
        {
            get { return Color.FromArgb(255, 233, 238, 238); }
        }
        public override Color ImageMarginRevealedGradientMiddle
        {
            get { return Color.FromArgb(255, 233, 238, 238); }
        }
        public override Color ImageMarginRevealedGradientEnd
        {
            get { return Color.FromArgb(255, 233, 238, 238); }
        }
        public override Color MenuStripGradientBegin
        {
            get { return Color.FromArgb(255, 162, 176, 202); }
        }
        public override Color MenuStripGradientEnd
        {
            get { return Color.FromArgb(255, 168, 182, 206); }
        }
        public override Color MenuItemSelected
        {
            get { return Color.FromArgb(255, 255, 236, 181); }
        }
        public override Color MenuItemBorder
        {
            get { return Color.FromArgb(255, 229, 195, 101); }
        }
        public override Color MenuBorder
        {
            get { return Color.FromArgb(255, 105, 119, 135); }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb(255, 252, 242, 255); }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.FromArgb(255, 254, 227, 148); }
        }
        public override Color MenuItemPressedGradientBegin
        {
            get { return Color.FromArgb(255, 207, 211, 218); }
        }
        public override Color MenuItemPressedGradientMiddle
        {
            get { return Color.FromArgb(255, 218, 223, 231); }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return Color.FromArgb(255, 218, 223, 231); }
        }
        public override Color RaftingContainerGradientBegin
        {
            get { return Color.FromArgb(255, 186, 192, 201); }
        }
        public override Color RaftingContainerGradientEnd
        {
            get { return Color.FromArgb(255, 186, 192, 201); }
        }
        public override Color SeparatorDark
        {
            get { return Color.FromArgb(255, 171, 180, 190); }
        }
        public override Color SeparatorLight
        {
            get { return Color.FromArgb(255, 117, 128, 145); }
        }
        public override Color StatusStripGradientBegin
        {
            get { return Color.FromArgb(255, 128, 140, 162); }
        }
        public override Color StatusStripGradientEnd
        {
            get { return Color.FromArgb(255, 111, 125, 149); }
        }
        public override Color ToolStripBorder
        {
            get { return Color.FromArgb(255, 99, 109, 126); }
        }
        public override Color ToolStripDropDownBackground
        {
            get { return Color.FromArgb(255, 255, 255, 255); }
        }
        public override Color ToolStripGradientBegin
        {
            get { return Color.FromArgb(255, 221, 226, 236); }
        }
        public override Color ToolStripGradientMiddle
        {
            get { return Color.FromArgb(255, 203, 210, 226); }
        }
        public override Color ToolStripGradientEnd
        {
            get { return Color.FromArgb(255, 203, 210, 226); }
        }
        public override Color ToolStripContentPanelGradientBegin
        {
            get { return Color.FromArgb(255, 137, 148, 163); }
        }
        public override Color ToolStripContentPanelGradientEnd
        {
            get { return Color.FromArgb(255, 137, 148, 163); }
        }
        public override Color ToolStripPanelGradientBegin
        {
            get { return Color.FromArgb(255, 156, 170, 193); }
        }
        public override Color ToolStripPanelGradientEnd
        {
            get { return Color.FromArgb(255, 156, 170, 193); }
        }
        public override Color OverflowButtonGradientBegin
        {
            get { return Color.FromArgb(255, 233, 235, 237); }
        }
        public override Color OverflowButtonGradientMiddle
        {
            get { return Color.FromArgb(255, 233, 235, 237); }
        }
        public override Color OverflowButtonGradientEnd
        {
            get { return Color.FromArgb(255, 208, 213, 217); }
        }
    }

    public class IderaToolStripRenderer : ToolStripProfessionalRenderer
    {
        private static ColorBlend MenuItemBackColorSelected = new ColorBlend(6);

        private Padding dropDownMenuItemPaintPadding = new Padding(2, 0, 1, 0);

        static IderaToolStripRenderer()
        {
            MenuItemBackColorSelected.Colors[0] = Color.FromArgb(0xff, 0xfe, 0xed);
            MenuItemBackColorSelected.Colors[1] = Color.FromArgb(0xff, 0xfb, 0xcc);
            MenuItemBackColorSelected.Colors[2] = Color.FromArgb(0xff, 0xf2, 0xa9);
            MenuItemBackColorSelected.Colors[3] = Color.FromArgb(0xff, 0xd5, 0x6a);
            MenuItemBackColorSelected.Colors[4] = Color.FromArgb(0xff, 0xe1, 0x93);
            MenuItemBackColorSelected.Colors[5] = Color.FromArgb(0xff, 0xe1, 0x93);
            MenuItemBackColorSelected.Positions[0] = 0.0f;
            MenuItemBackColorSelected.Positions[1] = 0.1623f;
            MenuItemBackColorSelected.Positions[2] = 0.499f;
            MenuItemBackColorSelected.Positions[3] = 0.5f;
            MenuItemBackColorSelected.Positions[4] = 0.85f;
            MenuItemBackColorSelected.Positions[5] = 1.0f;
        }
        public IderaToolStripRenderer(ProfessionalColorTable colorTable) : base(colorTable) { }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripItem item = e.Item;
            Graphics graphics = e.Graphics;
            Rectangle rectangle = new Rectangle(Point.Empty, item.Size);
            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                return;
            }

            if (!item.IsOnDropDown)
            {
                if (!item.Pressed)
                {
                    if (!item.Selected)
                    {
                        Rectangle rectangle1 = rectangle;
                        if (item.Owner != null && item.BackColor != item.Owner.BackColor)
                        {
                            using (Brush solidBrush = new SolidBrush(item.BackColor))
                            {
                                graphics.FillRectangle(solidBrush, rectangle1);
                            }
                        }
                    }
                    else
                    {
                        Color menuItemBorder = this.ColorTable.MenuItemBorder;
                        if (item.Enabled)
                        {
                            using (Brush linearGradientBrush = new LinearGradientBrush(rectangle,
                                                                                        this.ColorTable.MenuItemSelectedGradientBegin,
                                                                                        this.ColorTable.MenuItemSelectedGradientEnd,
                                                                                        LinearGradientMode.Vertical))
                            {
                                graphics.FillRectangle(linearGradientBrush, rectangle);
                            }
                        }
                        using (Pen pen = new Pen(menuItemBorder))
                        {
                            graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1,
                                                   rectangle.Height - 1);
                        }
                    }
                }
                else
                {
                    this.RenderPressedGradient(graphics, rectangle);
                    return;
                }
            }
            else
            {
                rectangle = DeflateRect(rectangle, this.dropDownMenuItemPaintPadding);
                if (!item.Selected)
                {
                    Rectangle rectangle2 = rectangle;
                    if (item.Owner != null && item.BackColor != item.Owner.BackColor)
                    {
                        using (Brush brush = new SolidBrush(item.BackColor))
                        {
                            graphics.FillRectangle(brush, rectangle2);
                        }
                    }
                }
                else
                {
                    Color highlight = this.ColorTable.MenuItemBorder;
                    if (item.Enabled)
                    {
                        using (var linearGradientBrush = new LinearGradientBrush(rectangle,
                                                             this.ColorTable.MenuItemSelectedGradientBegin,
                                                             this.ColorTable.MenuItemSelectedGradientEnd,
                                                             LinearGradientMode.Vertical))
                        {
                            linearGradientBrush.InterpolationColors = MenuItemBackColorSelected;
                            graphics.FillRectangle(linearGradientBrush, rectangle);
                        }
                    }
                    using (var pen1 = new Pen(highlight))
                    {
                        graphics.DrawRectangle(pen1, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
                    }
                }
            }
            return;
        }

        private void RenderPressedGradient(Graphics g, Rectangle bounds)
        {
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                return;
            }
            else
            {
                using (Brush linearGradientBrush = new LinearGradientBrush(bounds, this.ColorTable.MenuItemPressedGradientBegin, this.ColorTable.MenuItemPressedGradientEnd, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(linearGradientBrush, bounds);
                }
                using (Pen pen = new Pen(this.ColorTable.MenuBorder))
                {
                    g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
                return;
            }
        }

        private static Rectangle DeflateRect(Rectangle rect, Padding padding)
        {
            int x = rect.X + padding.Left;
            int y = rect.Y + padding.Top;
            int w = rect.Width - padding.Horizontal;
            int h = rect.Height - padding.Vertical;
            return new Rectangle(x, y, w, h);
        }

    }        

    #endregion

    public static class WpfInteropHelpers
    {
        private static readonly Type WinFormsAdapter;
        private static readonly FieldInfo WinFormsHostVariable;
        static WpfInteropHelpers()
        {
            var wfhType = typeof(WindowsFormsHost);
            var assembly = wfhType.Assembly;
            WinFormsAdapter = assembly.GetType("System.Windows.Forms.Integration.WinFormsAdapter");

            WinFormsHostVariable = WinFormsAdapter.GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        public static WindowsFormsHost FindWinFormsHost(this Control winformControl)
        {
            for (var c = winformControl; c != null; c = c.Parent)
            {
                if (c.GetType() == WinFormsAdapter)
                {
                    return WinFormsHostVariable.GetValue(c) as WindowsFormsHost;
                }
            }
            return null;
        }
    }
}
