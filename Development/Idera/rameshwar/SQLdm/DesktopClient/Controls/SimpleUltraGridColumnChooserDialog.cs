using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Idera.SQLdm.Common.UI.Dialogs;

namespace Idera.SQLdm.DesktopClient.Controls
{
    using System.Diagnostics;
    using System.Drawing;
    using Infragistics.Win;
    using Wintellect.PowerCollections;

    internal partial class SimpleUltraGridColumnChooserDialog : Form
    {
        private UltraGridBase grid;

        private int selectedColumns
        {
            get
            {
                int count = 0;

                if (grid.DisplayLayout.Bands.Count > 0)
                {
                    if (!grid.DisplayLayout.Bands[0].Hidden)
                    {
                        foreach (UltraGridColumn col in grid.DisplayLayout.Bands[0].Columns)
                        {
                            if (!col.Hidden)
                            {
                                count++;
                            }
                        }
                    }
                }

                return count;
            }
        }

        private bool columnVisible
        {
            get
            {
                return selectedColumns > 0;
            }
        }

        public SimpleUltraGridColumnChooserDialog(UltraGrid sourceGrid)
        {
            InitializeComponent();
            grid = columnChooser.SourceGrid = sourceGrid;
            if (columnChooser.SourceGrid.DisplayLayout.Bands.Count > 0)
            {
                columnChooser.MultipleBandSupport = MultipleBandSupport.DisplayColumnsFromAllBands;
            }
            else
            {
                columnChooser.MultipleBandSupport = MultipleBandSupport.SingleBandOnly;
            }

            // the chooser control will use the column chooser caption property if set otherwise it
            // defaults to use the column caption property.  If column caption is not set then no text will
            // get displayed.  There are some columns that do not have a caption set.  We need to find them
            // and default them to something useful.
            if (grid.DisplayLayout.Bands.Count > 0)
            {
                foreach (UltraGridBand band in grid.DisplayLayout.Bands)
                {
                    if (band.Columns.Count > 0)
                    {
                        foreach (UltraGridColumn column in band.Columns)
                        {
                            if (!column.Hidden)
                            {
                                string chooserText = column.ColumnChooserCaption;
                                if (string.IsNullOrEmpty(chooserText))
                                {
                                    chooserText = column.Header.Caption;
                                    if (string.IsNullOrEmpty(chooserText))
                                        chooserText = column.Key;

                                    column.ColumnChooserCaption = chooserText;
                                }
                            }
                        }
                    }
                }
            }
            // find the grid and replace its draw and creation filters
            foreach (Control control in columnChooser.Controls)
            {
                if (control is UltraGrid)
                {
                    ((UltraGrid)control).CreationFilter = new NoImagesCreationFilter(((UltraGrid)control).CreationFilter);
                    ((UltraGrid)control).DrawFilter = new HideFocusRectangleDrawFilter2();
                    break;
                }
            }

        }

        private void SimpleUltraGridColumnChooserDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!columnVisible)
            {
                ApplicationMessageBox.ShowError(this, "At least one column in the main band of the grid must be visible. Please select at least one column before closing the Column Chooser.");
                e.Cancel = true;
            }
        }

        private void close_Button_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        internal sealed class NoImagesCreationFilter : IUIElementCreationFilter
        {
            private readonly IUIElementCreationFilter previous;

            public NoImagesCreationFilter(IUIElementCreationFilter previousFilter)
            {
                previous = previousFilter;
            }
            
            public void AfterCreateChildElements(UIElement parent)
            {
                if (parent is HeaderUIElement)
                {
                    foreach (UIElement element in parent.ChildElements)
                    {
                        if (element is ImageUIElement)
                            element.Parent.ChildElements.Remove(element);
                    }
                }
                if (previous != null)
                {
                    previous.AfterCreateChildElements(parent);
                }
            }

            public bool BeforeCreateChildElements(UIElement parent)
            {
                if (previous != null)
                    return previous.BeforeCreateChildElements(parent);

                return false;
            }
        }

        internal sealed class HideFocusRectangleDrawFilter2 : IUIElementDrawFilter
        {
            private readonly IUIElementDrawFilter previousFilter;

            internal HideFocusRectangleDrawFilter2()
            {
                this.previousFilter = null;
            }

            internal HideFocusRectangleDrawFilter2(IUIElementDrawFilter previousFilter)
            {
                this.previousFilter = previousFilter;
            }

            public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
            {
                if (drawPhase == DrawPhase.BeforeDrawFocus)
                {
                    return true;
                }

                if (drawPhase == DrawPhase.BeforeDrawForeground && drawParams.Element is TextUIElement)
                {
                    // all text in the chooser should be black and normal regardless
                    // of how the text is styled in the source grid column header
                    drawParams.AppearanceData.ForeColor = Color.Black;
                    drawParams.AppearanceData.FontData.Bold = DefaultableBoolean.False;
                }

                if (previousFilter != null)
                    return previousFilter.DrawElement(drawPhase, ref drawParams);

                return false;
            }

            public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
            {
                DrawPhase result = DrawPhase.BeforeDrawFocus | DrawPhase.BeforeDrawForeground;
                if (previousFilter != null)
                    result |= previousFilter.GetPhasesToFilter(ref drawParams);

                return result;
            }
        }

    }
}