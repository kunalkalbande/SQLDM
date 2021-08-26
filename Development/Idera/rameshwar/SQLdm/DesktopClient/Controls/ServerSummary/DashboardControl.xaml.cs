using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Infragistics.Windows.Tiles;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
    /// <summary>
    /// Interaction logic for DashboardControl.xaml
    /// </summary>
    public partial class DashboardControl : UserControl
    {
        private DashboardLayoutModel _model;
        private bool _updatePreferredSizes;
        private double _lastListBoxWidth = 125d;

        public DashboardControl()
        {
            InitializeComponent();
            ConfigureEditMode(false);
        }

        public DashboardLayoutModel Model
        {
            get { return _model; }
            set 
            {
                if (_model != null)
                    _model.PropertyChanged -= ModelChanged;
                
                _model = value;
                root.DataContext = _model;
                _model.PropertyChanged += ModelChanged;
                Init();
            }
        }

        public void Init()
        {
            if (_model != null)
            {
                tileControl.NormalModeSettings.MaxColumns =
                tileControl.NormalModeSettings.MinColumns =
                    _model.NumberColumns;
                tileControl.NormalModeSettings.MaxRows =
                tileControl.NormalModeSettings.MinRows =
                    _model.NumberRows;

                ConfigureEditMode(_model.IsInEditMode);
            }

            ConfigureLayoutControls();
        }

        public bool IsInEditMode
        {
            get { return _model != null ? _model.IsInEditMode : false; }
        }

        public void SetEditMode(bool edit)
        {
            if (_model != null)
            {
                _model.IsInEditMode = edit;
                if (edit)
                    ConfigureLayoutControls();
            }
        }

        private void ConfigureLayoutControls()
        {
            _updatePreferredSizes = true;

            var nColumns = tileControl.NormalModeSettings.MinColumns;
            var nRows = tileControl.NormalModeSettings.MinRows;

            var have = layoutGrid.ColumnDefinitions.Count - 3;
            if (have > nColumns)
            {
                var last = layoutGrid.ColumnDefinitions.Count - 2;

                have -= nColumns;
                for (var i = 0; i < have; i++, last--)
                {
                    var deleteme = layoutGrid.Children.Cast<UIElement>()
                        .Where(element => element is TileLayoutGridHeader && Grid.GetColumn(element) == last).ToList();
                    foreach (var element in deleteme)
                        layoutGrid.Children.Remove(element);

                    layoutGrid.ColumnDefinitions.RemoveAt(last);
                }
            }
            else
            {
                have = nColumns - have;
                for (int i = 0; i < have; i++)
                {
                    var newdef = new ColumnDefinition { Width = new GridLength(1.0d, GridUnitType.Star) };
                    layoutGrid.ColumnDefinitions.Insert(2, newdef);

                    var header = new TileLayoutGridHeader();
                    BindVisibilityToModel(header, "IsInEditMode");

                    Grid.SetColumnSpan(header, 1);
                    Grid.SetColumn(header, layoutGrid.ColumnDefinitions.Count - 2);
                    layoutGrid.Children.Add(header);
                }
            }

            have = layoutGrid.RowDefinitions.Count - 3;
            if (have > nRows)
            {
                var last = layoutGrid.RowDefinitions.Count - 2;

                have -= nRows;
                for (var i = 0; i < have; i++, last--)
                {
                    var deleteme = layoutGrid.Children.Cast<UIElement>()
                        .Where(element => element is TileLayoutGridRowHeader && Grid.GetRow(element) == last).ToList();
                    foreach (var element in deleteme) 
                        layoutGrid.Children.Remove(element);

                    layoutGrid.RowDefinitions.RemoveAt(last);
                }
            }
            else
            {
                have = nRows - have;
                for (int i = 0; i < have; i++)
                {
                    var newdef = new RowDefinition { Height = new GridLength(1.0d, GridUnitType.Star) };
                    layoutGrid.RowDefinitions.Insert(2, newdef);

                    var header = new TileLayoutGridRowHeader();
                    BindVisibilityToModel(header, "IsInEditMode");

                    Grid.SetRowSpan(header, 1);
                    Grid.SetRow(header, layoutGrid.RowDefinitions.Count - 2);
                    layoutGrid.Children.Add(header);
                }
            }

            Grid.SetColumnSpan(tileControl, nColumns + 2);
            Grid.SetRowSpan(tileControl, nRows + 2);

            foreach (UIElement item in layoutGrid.Children)
            {
                if (item is TileLayoutGridHeader)
                {
                    ((TileLayoutGridHeader)item).deleteColumnImage.Visibility =
                        (nColumns <= 1) ? Visibility.Hidden : Visibility.Visible;
                }
                if (item is TileLayoutGridRowHeader)
                {
                    ((TileLayoutGridRowHeader)item).deleteRowImage.Visibility =
                        (nRows <= 1) ? Visibility.Hidden : Visibility.Visible;
                }
            }
        }

        void BindVisibilityToModel(FrameworkElement element, string property)
        {
            var binding = new Binding {Source = Model, Path = new PropertyPath("IsInEditMode")};
            binding.Converter = Resources["boolToVisibilityConverter"] as BooleanToVisibilityConverter;
            element.SetBinding(UIElement.VisibilityProperty, binding); 
        }

        protected void ModelChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "IsInEditMode":
                    ConfigureEditMode(_model.IsInEditMode);
                    break;
                case "NumberRows":
                    tileControl.NormalModeSettings.MaxRows = _model.NumberRows;
                    tileControl.NormalModeSettings.MinRows = _model.NumberRows;
                    if (_model.IsInEditMode)
                        ConfigureLayoutControls();
                    break;
                case "NumberColumns":
                    tileControl.NormalModeSettings.MaxColumns = _model.NumberColumns;
                    tileControl.NormalModeSettings.MinColumns = _model.NumberColumns;
                    if (_model.IsInEditMode)
                        ConfigureLayoutControls();
                    break;
            }            
        }

        private void ConfigureEditMode(bool isInEditMode)
        {
            if (isInEditMode)
            {
                componentListBox.Visibility = Visibility.Visible;
                root.ColumnDefinitions[0].Width = new GridLength(_lastListBoxWidth);
            }
            else
            {
                _lastListBoxWidth = root.ColumnDefinitions[0].ActualWidth;
                if (_lastListBoxWidth < 48) _lastListBoxWidth = 150;
                componentListBox.Visibility = Visibility.Collapsed;
                root.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
            }
            layoutSplitter.Visibility = componentListBox.Visibility;
        }

        private void layoutGrid_LayoutUpdated(object sender, EventArgs e)
        {
            if (!_updatePreferredSizes) return;

            var width = layoutGrid.ColumnDefinitions[2].ActualWidth;
            var height = layoutGrid.RowDefinitions[2].ActualHeight;

            var constraints = tileControl.NormalModeSettings.TileConstraints ?? new TileConstraints();
            constraints.PreferredHeight = height;
            constraints.PreferredWidth = width;
            tileControl.NormalModeSettings.TileConstraints = constraints;
            _updatePreferredSizes = false;
        }

        internal void addColumnAfter(int column)
        {
            var columns = tileControl.NormalModeSettings.MinColumns;
            var n = tileControl.Items.Count;
            var m = columns * tileControl.NormalModeSettings.MinRows;
            var position = column - 1;
            for (var i = n; i < m; i++)
            {
                var component = new DashboardComponent();
                component.Title = "Added" + position;
                component.LogicalIndex = position;
                component.Model = _model.DataModel;
                _model.Components.Insert(position, component);
                position += columns;
            }

            _updatePreferredSizes = true;
        }

        internal void addRowAfter(int row)
        {

            var rows = tileControl.NormalModeSettings.MinRows;
            var n = tileControl.Items.Count;
            var m = tileControl.NormalModeSettings.MinColumns * rows;
            var position = (row - 1) * tileControl.NormalModeSettings.MinColumns;
            for (var i = n; i < m; i++)
            {
                var component = new DashboardComponent();
                component.Title = "Added " + position;
                component.LogicalIndex = position;
                component.Model = _model.DataModel;
                _model.Components.Insert(position++, component);
            }

            _updatePreferredSizes = true;
        }

        internal void deleteColumn(int column)
        {
            var ixToDelete = column - 2;
            var columns = tileControl.NormalModeSettings.MaxColumns;
            var itemsToDelete = new List<DashboardComponent>();

            foreach (var component in _model.Components)
            {
                var info = tileControl.GetItemInfo(component);
                if (info.LogicalIndex % columns == ixToDelete)
                    itemsToDelete.Add(component);
            }

            foreach (var component in itemsToDelete)
                _model.Components.Remove(component);

            _updatePreferredSizes = true;
        }

        internal void deleteRow(int row)
        {
            var ixToDelete = row - 2;
            var rows = tileControl.NormalModeSettings.MaxColumns;
            var itemsToDelete = new List<DashboardComponent>();

            foreach (var component in _model.Components)
            {
                var info = tileControl.GetItemInfo(component);
                if (info.LogicalIndex / rows == ixToDelete)
                    itemsToDelete.Add(component);
            }

            foreach (var component in itemsToDelete)
                _model.Components.Remove(component);

            _updatePreferredSizes = true; 
        }

        private void componentListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var box = sender as ListBox;
            if (box == null) return;
            var metadata = box.SelectedItem as DashboardComponentMetaData;
            if (metadata == null) return;
            // find a free tile and put it there
            var empty = FindFirstFreeComponent();
            if (empty == null)
            {
                MessageBox.Show("No empty tiles exist.  You either need to add a new row or column to your dashboard.");
                return;
            }

            empty.Title = metadata.Name;
            empty.Maximized = false;
            empty.ComponentKey = metadata.Key;
            empty.Model = _model.DataModel;

            var tile = tileControl.TileFromItem(empty);
            var content = tile.Content;
            tile.Content = null;
            tile.Content = content;
        }

        private DashboardComponent FindFirstFreeComponent()
        {
            return _model.Components.FirstOrDefault(component => String.IsNullOrEmpty(component.ComponentKey));
        }

        private void tileControl_TileStateChanged(object sender, Infragistics.Windows.Tiles.Events.TileStateChangedEventArgs e)
        {

        }

        private void tileControl_TileStateChanging(object sender, Infragistics.Windows.Tiles.Events.TileStateChangingEventArgs e)
        {
            // prevent changing tile states
            e.Cancel = e.NewState != TileState.Normal;
        }
    }
}
