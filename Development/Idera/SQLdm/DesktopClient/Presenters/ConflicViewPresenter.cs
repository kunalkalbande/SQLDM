using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLdm.DesktopClient.Presenters
{
    class ConflicViewPresenter
    {
        internal class DmGridColumn
        {
            public string Text { get; set; }
            public string PropertyName { get; set; }
        }
        private readonly UltraGrid _view;
        private readonly List<DmGridColumn> _columns;
        private Infragistics.Win.UltraWinGrid.UltraGridBand _ultraGridBand1;

        public ConflicViewPresenter(UltraGrid view, List<DmGridColumn> columns)
        {
            _view = view;
            _columns = columns;
            InitializeGrid();
            InitializeColums();            
        }

        private void InitializeGrid()
        {
            _ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("AlertBaselineEntry", -1);
            
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            // 
            // _view
            // 
            this._view.Dock = DockStyle.Fill;            
            appearance35.BackColor = System.Drawing.SystemColors.Window;
            appearance35.BorderColor = System.Drawing.Color.Silver;
            this._view.DisplayLayout.Appearance = appearance35;
            this._view.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            appearance1.TextHAlignAsString = "Center";
            this._view.DisplayLayout.BandsSerializer.Add(_ultraGridBand1);
            this._view.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this._view.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance36.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance36.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance36.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this._view.DisplayLayout.GroupByBox.Appearance = appearance36;
            appearance49.ForeColor = System.Drawing.SystemColors.GrayText;
            this._view.DisplayLayout.GroupByBox.BandLabelAppearance = appearance49;
            this._view.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this._view.DisplayLayout.GroupByBox.Hidden = true;
            appearance50.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance50.BackColor2 = System.Drawing.SystemColors.Control;
            appearance50.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance50.ForeColor = System.Drawing.SystemColors.GrayText;
            this._view.DisplayLayout.GroupByBox.PromptAppearance = appearance50;
            this._view.DisplayLayout.MaxColScrollRegions = 1;
            this._view.DisplayLayout.MaxRowScrollRegions = 1;
            this._view.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this._view.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this._view.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this._view.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this._view.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance51.BackColor = System.Drawing.SystemColors.Window;
            this._view.DisplayLayout.Override.CardAreaAppearance = appearance51;
            appearance52.BorderColor = System.Drawing.Color.Silver;
            appearance52.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this._view.DisplayLayout.Override.CellAppearance = appearance52;
            this._view.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this._view.DisplayLayout.Override.CellPadding = 0;
            this._view.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this._view.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance53.BackColor = System.Drawing.SystemColors.Control;
            appearance53.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance53.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance53.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance53.BorderColor = System.Drawing.SystemColors.Window;
            this._view.DisplayLayout.Override.GroupByRowAppearance = appearance53;
            this._view.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            this._view.DisplayLayout.Override.GroupBySummaryDisplayStyle = Infragistics.Win.UltraWinGrid.GroupBySummaryDisplayStyle.SummaryCells;
            appearance54.TextHAlignAsString = "Left";
            this._view.DisplayLayout.Override.HeaderAppearance = appearance54;
            this._view.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance55.BackColor = System.Drawing.SystemColors.Window;
            appearance55.BorderColor = System.Drawing.Color.Silver;
            this._view.DisplayLayout.Override.RowAppearance = appearance55;
            this._view.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this._view.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._view.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._view.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._view.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this._view.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.InGroupByRows;
            appearance56.BackColor = System.Drawing.SystemColors.ControlLight;
            this._view.DisplayLayout.Override.TemplateAddRowAppearance = appearance56;
            this._view.DisplayLayout.Override.TipStyleCell = Infragistics.Win.UltraWinGrid.TipStyle.Hide;
            this._view.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this._view.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this._view.DisplayLayout.UseFixedHeaders = true;            
            this._view.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this._view.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this._view.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._view.Location = new System.Drawing.Point(10, 96);
            this._view.Name = "_view";
            this._view.Size = new System.Drawing.Size(504, 116);
            this._view.TabIndex = 7;            
        }

        private void InitializeColums()
        {
            foreach (var column in _columns)
            {
                UltraGridColumn dgColumn = new UltraGridColumn(column.PropertyName);
                dgColumn.Header.Caption = column.Text;
                dgColumn.CellMultiLine = DefaultableBoolean.True;         
                _ultraGridBand1.Columns.Add(dgColumn);
            }
        }

        public void UpdateData<T>(List<T> datas )
        {
            _view.DataSource = datas;
            _view.Refresh();
            _view.ResetBindings();
        }
    }
}
