using System.Windows;
using System.Windows.Media;

namespace Idera.SQLdm.DesktopClient.Controls.Presentation
{
    public class ViewTabItem : Infragistics.Windows.Ribbon.RibbonTabItem
    {
        public readonly static DependencyProperty SourceProperty;

        static ViewTabItem()
        {
            ViewTabItem.SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ViewTabItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);
        }

        public ImageSource Source
        {
            get { return (ImageSource)base.GetValue(ViewTabItem.SourceProperty); }
            set { base.SetValue(ViewTabItem.SourceProperty, value); }
        }

    }
}
