using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.Integration;
using System.Windows.Media.Effects;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    public class CollapsedLeftNavFlyoutHelper
    {
        public bool ShowFlyout = false;
        Window previousFlyoutWindow = null;
        bool previousTreeViewVisibilityState = false;
        Panel flyoutParent = null;
        System.Windows.Forms.Control originalTreeViewParent = null;
        System.Windows.Forms.TreeView treeView = null;
        System.Windows.Forms.ToolStrip header = null;
        System.Windows.Forms.ToolStripItem headerToggleCaret = null;
        Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  latestPanel = null;
        System.Windows.Controls.Panel latestSearchPanel = null;
        System.Windows.Forms.Label emptyTreeViewIndicator = null;
        System.Windows.Controls.Grid searchGrid = null;
        System.Windows.Controls.Control originalSearchGridParent = null;
        DockPanel dockPanel = null;
        TextBox searchTextBox = null;
        Popup searchSuggestionPopup = null;
        public CollapsedLeftNavFlyoutHelper(Panel flyoutParent, System.Windows.Forms.TreeView treeView, System.Windows.Forms.ToolStrip header, System.Windows.Forms.ToolStripItem headerToggleCaret = null, System.Windows.Forms.Label emptyTreeViewIndicator = null)
        {
            this.emptyTreeViewIndicator = emptyTreeViewIndicator;
            this.flyoutParent = flyoutParent;
            this.treeView = treeView;
            this.header = header;
            this.headerToggleCaret = headerToggleCaret;
            this.originalTreeViewParent = treeView.Parent;
        }
        public CollapsedLeftNavFlyoutHelper(Panel flyoutParent, System.Windows.Controls.Grid searchGrid, TextBox searchTextBox, DockPanel dockPanel, Popup searchSuggestionPopup)
        {
            this.searchGrid = searchGrid;
            this.flyoutParent = flyoutParent;
            this.searchTextBox = searchTextBox;
            this.dockPanel = dockPanel;
            this.searchSuggestionPopup = searchSuggestionPopup;
        }

        ~CollapsedLeftNavFlyoutHelper()
        {
            if (previousFlyoutWindow != null)
                previousFlyoutWindow.LocationChanged -= FlyoutWindow_LocationChanged;
        }

        public void OnPaneDragEnded(bool leftNavExpanded)
        {
            if (ShowFlyout && !leftNavExpanded && flyoutParent.Children.Count > 2 && flyoutParent.Children[2].Visibility != Visibility.Visible)
            {
                flyoutParent.Children.RemoveAt(1);
                ShowOrHideFlyout();
            }
            else
            {
                subscribeFlyoutToWindowMovement();
            }
        }

        public void SetBackgroundColor(Color color)
        {
            if(latestPanel != null)
            {
                latestPanel.BackColor = color;
            }
            if(latestSearchPanel != null)
            {
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (System.Windows.Media.Brush)converter.ConvertFrom(color);
                latestSearchPanel.Background = brush;
            }
        }

        public void ShowOrHideFlyout()
        {
            if (ShowFlyout && flyoutParent.Children.Count <= 2)
            {
                int headerHeight = header.Height;
                int nodeHeight = treeView.ItemHeight;
                if (emptyTreeViewIndicator != null && treeView.Nodes.Count == 0)
                    nodeHeight = emptyTreeViewIndicator.Height;
                int nodeCount = treeView.Nodes.Count;
                for(int i = 0; i < treeView.Nodes.Count; i++)
                {
                    var node = treeView.Nodes[i];
                    nodeCount += node.Nodes.Count;      
                }
                int flyoutHeight = ((nodeCount + 1) * nodeHeight) + headerHeight;
                var popup = new Popup();
                popup.PlacementTarget = flyoutParent;
                popup.Width = 292;
                popup.Height = flyoutHeight + 2;
                popup.IsOpen = true;
                popup.StaysOpen = true;
                popup.Placement = PlacementMode.Right;
                // offsets come from the Padding property of the WPF Style "LeftNavCollapsedBtn"
                popup.VerticalOffset = -4;
                popup.HorizontalOffset = 4;
                popup.AllowsTransparency = true;
                var newPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
                newPanel.Width = originalTreeViewParent.Width;
                newPanel.Height = flyoutHeight;
                newPanel.BackColor = originalTreeViewParent.BackColor;
                if (emptyTreeViewIndicator != null)
                {
                    emptyTreeViewIndicator.Parent.Controls.Remove(emptyTreeViewIndicator);
                    newPanel.Controls.Add(emptyTreeViewIndicator);
                }
                treeView.Parent.Controls.Remove(treeView);
                newPanel.Controls.Add(treeView);
                header.Parent.Controls.Remove(header);
                newPanel.Controls.Add(header);
                // also need a reference to the toggle button, to hide it
                var windowsFormsHost = new WindowsFormsHost();
                windowsFormsHost.Child = newPanel;

                var popupBorder = new Border();
                popupBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0,96,127));
                popupBorder.Padding = new Thickness(1);
                popupBorder.Child = windowsFormsHost;
                popup.Child = popupBorder;

                flyoutParent.Children.Add(popup);

                subscribeFlyoutToWindowMovement();

                if(headerToggleCaret != null)
                    headerToggleCaret.Visible = false;
                previousTreeViewVisibilityState = treeView.Visible;
                treeView.Visible = true;
                latestPanel = newPanel;
            }
            else if (flyoutParent.Children.Count > 2)
            {
                if (emptyTreeViewIndicator != null)
                {
                    emptyTreeViewIndicator.Parent.Controls.Remove(emptyTreeViewIndicator);
                    emptyTreeViewIndicator.Location = new System.Drawing.Point(0, 33);
                    originalTreeViewParent.Controls.Add(emptyTreeViewIndicator);
                }
                treeView.Parent.Controls.Remove(treeView);
                originalTreeViewParent.Controls.Add(treeView);
                treeView.Location = new System.Drawing.Point(0, 33);
                header.Parent.Controls.Remove(header);
                originalTreeViewParent.Controls.Add(header);
                header.Location = new System.Drawing.Point(0, 0);
                flyoutParent.Children.RemoveAt(2);
                treeView.Visible = previousTreeViewVisibilityState;
                if(headerToggleCaret != null)
                    headerToggleCaret.Visible = true;
                latestPanel = null;
            }
        }
        private Grid newSearchGrid = null;
        public void ShowOrHideSearchFlyout()
        {
            if (ShowFlyout && flyoutParent.Children.Count <= 2)
            {
                int flyoutHeight=28;
                var popup = new Popup();
                popup.PlacementTarget = flyoutParent;
                popup.Width = 248;
                popup.Height = flyoutHeight + 2;
                popup.IsOpen = true;
                popup.StaysOpen = true;
                popup.Placement = PlacementMode.Right;
                // offsets come from the Padding property of the WPF Style "LeftNavCollapsedBtn"
                popup.VerticalOffset = -4;
                popup.HorizontalOffset = 4;
                popup.AllowsTransparency = true;
                newSearchGrid = new System.Windows.Controls.Grid();
                newSearchGrid.Width = searchGrid.Width;
                newSearchGrid.Height = flyoutHeight;
                newSearchGrid.Background = System.Windows.Media.Brushes.White;
                searchGrid.Children.Remove(searchTextBox);
                searchGrid.Children.Remove(dockPanel);
                searchGrid.Children.Remove(searchSuggestionPopup);
                newSearchGrid.Children.Add(searchTextBox);
                newSearchGrid.Children.Add(dockPanel);
                newSearchGrid.Children.Add(searchSuggestionPopup);
                // also need a reference to the toggle button, to hide it
               
                var popupBorder = new Border();
                popupBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 96, 127));
                popupBorder.Padding = new Thickness(1);
                popupBorder.Child = newSearchGrid;
                popup.Child = popupBorder;

                flyoutParent.Children.Add(popup);

                subscribeFlyoutToWindowMovement();
                
                latestSearchPanel = newSearchGrid;
            }
            else if (flyoutParent.Children.Count > 2)
            {
                if (newSearchGrid != null)
                {
                    newSearchGrid.Children.Remove(searchTextBox);
                    newSearchGrid.Children.Remove(dockPanel);
                    newSearchGrid.Children.Remove(searchSuggestionPopup);
                }
                searchGrid.Children.Add(searchTextBox);
                searchGrid.Children.Add(dockPanel);
                searchGrid.Children.Add(searchSuggestionPopup);
                flyoutParent.Children.RemoveAt(2);
                latestSearchPanel = null;
            }
        }
        void subscribeFlyoutToWindowMovement()
        {
            var window = Window.GetWindow(flyoutParent);
            if (previousFlyoutWindow != null)
                previousFlyoutWindow.LocationChanged -= FlyoutWindow_LocationChanged;
            window.LocationChanged += FlyoutWindow_LocationChanged;
            previousFlyoutWindow = window;
        }

        private void FlyoutWindow_LocationChanged(object sender, EventArgs e)
        {
            var gridChildren = flyoutParent.Children;
            if (gridChildren.Count > 2)
            {
                var popup = (Popup)flyoutParent.Children[2];
                var originalOffset = popup.HorizontalOffset;
                popup.HorizontalOffset = originalOffset + 1;
                popup.HorizontalOffset = originalOffset;
            }
        }
    }

}
