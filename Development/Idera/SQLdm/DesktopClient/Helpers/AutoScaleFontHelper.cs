// -----------------------------------------------------------------------
// <copyright file="AutoScaleFontHelper.cs" company="Idera">
// 
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    using Idera.SQLdm.DesktopClient.Properties;

    /// <summary>
    /// Class that provides methods and nested classes which will allow to autoscale font size over pre-defined controls.
    /// </summary>
    public class AutoScaleFontHelper
    {
        #region Fields & Properties

        private static AutoScaleFontHelper _default = null;
        private static object _locker = new object();
        private static short _osCurrentDpi = 0;

        protected const short NormalDPI = 96;
        protected const short MediumDPI = 120;
        protected const short LargeDPI = 144;
        protected const short VeryLargeDPI = 192;

        protected const float DefaultNormalFontSize = 8.25f;
        protected const float DefaultMediumFontSize = 7.0f;
        protected const float DefaultLargeFontSize = 6.75f;
        protected const float DefaultVeryLargeFontSize = 5.5f;

        /// <summary>
        /// Gets the current OS Dpi setting.
        /// </summary>
        public static short OSCurrentDPI
        {
            get
            {
                if (_osCurrentDpi == 0)
                {
                    _osCurrentDpi = Settings.Default.OSCurrentDPI;
                }

                return _osCurrentDpi;
            }
        }

        /// <summary>
        /// Singleton property
        /// </summary>
        public static AutoScaleFontHelper Default
        {
            get
            {
                if (_default == null)
                {
                    lock (_locker)
                    {
                        _default = new AutoScaleFontHelper();
                    }
                }

                return _default;
            }
        }

        /// <summary>
        /// Returns a default Font size for the current OSDpi
        /// </summary>
        public static float FontSize
        {
            get { return AbstractControlHelper.GetDefaultFontSize(); }
        }

        /// <summary>
        /// Gets the font size proportion for custom checkedComboBox.
        /// </summary>
        public static short CheckedComboBoxFontSizeProportion
        {
            get
            {
                short fontSizeProportion = 0;
                if (OSCurrentDPI != NormalDPI)
                {
                    fontSizeProportion = System.Convert.ToInt16(OSCurrentDPI/NormalDPI*6);
                }
                return fontSizeProportion;
            }
        }

        /// <summary>
        /// Indicates if the current OS DPI resolution is in the Normal range.
        /// </summary>
        public bool IsNormalDPI
        {
            get
            {
                return NormalDPI >= _osCurrentDpi;
            }
        }

        /// <summary>
        /// Indicates if the current OS DPI resolution is in the Medium range.
        /// </summary>
        public bool IsMediumDPI
        {
            get
            {
                return MediumDPI >= _osCurrentDpi && NormalDPI < _osCurrentDpi;
            }
        }

        /// <summary>
        /// Indicates if the current OS DPI resolution is in the Large range.
        /// </summary>
        public bool IsLargeDPI
        {
            get
            {
                return LargeDPI >= _osCurrentDpi && MediumDPI < _osCurrentDpi;
            }
        }

        /// <summary>
        /// Indicates if the current OS DPI resolution is in the Very Large range.
        /// </summary>
        public bool IsVeryLargeDPI
        {
            get
            {
                return LargeDPI < _osCurrentDpi;
            }
        }

        #endregion Fields & Properties

        #region Enums

        /// <summary>
        /// Pre-defined controls that are supported by this class.
        /// </summary>
        public enum ControlType
        {
            ChartFx,
            Form,
            Control,
            Container,
            ToolStrip,
            SqlServerInstanceThumbnail
        }

        #endregion Enums

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private AutoScaleFontHelper()
        {
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Primary method of the AutoScaleFontHelper class which will be in charge of doing the font autoscalation.
        /// </summary>
        /// <param name="control">The control which Font will be autoscaled</param>
        /// <param name="controlType">Type of the target control</param>
        public void AutoScaleControl(Control control, ControlType controlType)
        {
            if (OSCurrentDPI == NormalDPI) return;
            
            ControlHelperFactory(controlType).AutoScale(control);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Control Helper factory
        /// </summary>
        /// <param name="controlType">Type of helper we want</param>
        /// <returns>Returns an especific Helper</returns>
        private static AbstractControlHelper ControlHelperFactory(ControlType controlType)
        {
            switch(controlType)
            {
                case ControlType.Form:
                    return new FormHelper();
                case ControlType.Control:
                    return new GenericControlHelper();
                case ControlType.Container:
                    return new ContainerFontSizeControlHelper();
                case ControlType.ToolStrip:
                    return new ToolStripFontSizeControlHelper();
                case ControlType.ChartFx:
                    return new ChartFXFontSizeHelper();
                case ControlType.SqlServerInstanceThumbnail:
                    return new SqlServerInstanceThumbnailFontSizeHelper();
                default:
                    return new GenericControlHelper();
            }
        }

        #endregion Private Methods

        #region Nested Classes

        /// <summary>
        /// Abstract class which represents a base for Control Helpers
        /// </summary>
        private abstract class AbstractControlHelper
        {
            protected float normalFontSize = DefaultNormalFontSize;
            protected float mediumFontSize = DefaultMediumFontSize;
            protected float largeFontSize = DefaultLargeFontSize;
            protected float veryLargeFontSize = DefaultVeryLargeFontSize;

            /// <summary>
            /// Primary method of any ABstracControlHelper derived class
            /// </summary>
            /// <param name="control"></param>
            public abstract void AutoScale(Control control);

            /// <summary>
            ///  Returns the default FontSize for the current OSDpi
            /// </summary>
            /// <returns></returns>
            public static float GetDefaultFontSize()
            {
                return GetFontSize(new GenericControlHelper());
            }

            /// <summary>
            /// Returns the especific FontSize of the given helper
            /// </summary>
            /// <param name="controlHelper"></param>
            /// <returns></returns>
            protected static float GetFontSize(AbstractControlHelper controlHelper)
            {
                switch (OSCurrentDPI)
                {
                    case NormalDPI:
                        return controlHelper.normalFontSize;
                    case MediumDPI:
                        return controlHelper.mediumFontSize;
                    case LargeDPI:
                        return controlHelper.largeFontSize;
                    case VeryLargeDPI:
                        return controlHelper.veryLargeFontSize;
                    default:
                        return controlHelper.normalFontSize;
                }
            }
        }

        /// <summary>
        /// FormHelper is an especific Helper which is in charge of autoscaling the font of Form types
        /// </summary>
        private class FormHelper : AbstractControlHelper
        {
            public override void AutoScale(Control control)
            {
                if (control is Form)
                {
                    ((Form) control).AutoScaleDimensions = new SizeF(96F, 96F);
                    ((Form) control).AutoScaleMode = AutoScaleMode.Font;

                    control.Font = new Font(control.Font.FontFamily, GetFontSize(this), control.Font.Style);
                }
            }
        }

        /// <summary>
        /// GenericControlHelper is generic Helper which is in charge of autoscaling the font of a Control inherited type
        /// </summary>
        private class GenericControlHelper : AbstractControlHelper
        {
            public override void AutoScale(Control control)
            {
                control.Font = new Font(control.Font.FontFamily, GetFontSize(this), control.Font.Style);
            }
        }

        /// <summary>
        /// GenericControlHelper is generic Helper which is in charge of autoscaling the font of a Control inherited type
        /// </summary>
        private class ContainerFontSizeControlHelper : AbstractControlHelper
        {
            public override void AutoScale(Control control)
            {
                AbstractControlHelper controlHelper;
                control.Font = new Font(control.Font.FontFamily, GetFontSize(this), control.Font.Style);

                foreach (Control subControl in control.Controls)
                {

                    if (subControl is ChartFX.WinForms.Chart)
                    {
                        controlHelper = new ChartFXFontSizeHelper();
                    }
                    else if (subControl is ToolStrip)
                    {
                        controlHelper = new ToolStripFontSizeControlHelper();
                    }
                    else
                    {
                        controlHelper = new GenericControlHelper();
                    }

                    controlHelper.AutoScale(subControl);

                    if (subControl.HasChildren && controlHelper is GenericControlHelper)
                    {
                        this.AutoScale(subControl);
                    }
                }
            }
        }

        /// <summary>
        /// ToolStripFontSizeControlHelper is a Helper which is in charge of autoscaling the font of a ToolStrip inherited type
        /// </summary>
        private class ToolStripFontSizeControlHelper : AbstractControlHelper
        {
            public override void AutoScale(Control control)
            {
                if (control is ToolStrip)
                {
                    control.Font = new Font(control.Font.FontFamily, GetFontSize(this), control.Font.Style);

                    foreach (ToolStripItem item in ((ToolStrip) control).Items)
                    {
                        item.Font = new Font(item.Font.FontFamily, GetFontSize(this), item.Font.Style);
                    }

                    control.AutoSize = true;
                }
            }
        }

        /// <summary>
        /// Helper responsible to apply the font sizes to controls that are instance of 'Chart'.
        /// </summary>
        private class ChartFXFontSizeHelper : AbstractControlHelper
        {
            /// <summary>
            /// Indicates the font size to apply for medium scale.
            /// </summary>
            private const float MediumFontSize = 6.25f;

            /// <summary>
            /// Indicates the font size to apply for large scale.
            /// </summary>
            private const float LargeFontSize = 5.75f;

            /// <summary>
            /// Indicates the font size to apply for very large scale.
            /// </summary>
            private const float VeryLargeFontSize = 5.75f;

            /// <summary>
            /// ChartFXFontSizeHelper constructor. Initialize the medium and large font sizes.
            /// </summary>
            public ChartFXFontSizeHelper()
            {
                mediumFontSize = MediumFontSize;
                largeFontSize = LargeFontSize;
                veryLargeFontSize = VeryLargeFontSize;
            }

            /// <summary>
            /// Primary method of any ABstracControlHelper derived class
            /// </summary>
            /// <param name="container">The controls container.</param>
            public override void AutoScale(Control container)
            {
                if (container is ChartFX.WinForms.Chart)
                {
                    container.Font = new Font(container.Font.FontFamily, GetFontSize(this), container.Font.Style);
                }

                foreach (Control subControl in container.Controls)
                {
                    if (subControl.HasChildren)
                    {
                        AutoScale(subControl);
                    }

                    if (subControl is ChartFX.WinForms.Chart)
                    {
                        // If the controls is Chart kind, rescale the font size.
                        subControl.Font = new Font(subControl.Font.FontFamily, GetFontSize(this), subControl.Font.Style);
                    }
                }
            }
        }

        /// <summary>
        /// Helper responsible to apply the font sizes to controls that are instance of
        /// 'SqlServerInstanceThumbnailFontSizeHelper'.
        /// </summary>
        private class SqlServerInstanceThumbnailFontSizeHelper : AbstractControlHelper
        {
            /// <summary>
            /// Indicates the font size to apply on the header for the normal scale.
            /// </summary>
            private const float InstanceNameNormalFontSize = 10F;

            /// <summary>
            /// Indicates the font size to apply on the header for the medium scale.
            /// </summary>
            private const float InstanceNameMediumFontSize = 8.0F;

            /// <summary>
            /// Indicates the font size to apply on the header for the large scale.
            /// </summary>
            private const float InstanceNameLargeFontSize = 6.4F;

            /// <summary>
            /// Indicates the font size to apply on the header for the very large scale.
            /// </summary>
            private const float InstanceNameVeryLargeFontSize = 5.12F;

            /// <summary>
            /// Indicates the font size to apply on the details for the normal scale.
            /// </summary>
            private const float MetadataNormalFontSize = 8.0F;

            /// <summary>
            /// Indicates the font size to apply on the details for the normal scale.
            /// </summary>
            private const float MetadataMediumFontSize = 7.0F;

            /// <summary>
            /// Indicates the font size to apply on the details for the large scale.
            /// </summary>
            private const float MetadataLargeFontSize = 6.12F;

            /// <summary>
            /// Indicates the font size to apply on the details for the very large scale.
            /// </summary>
            private const float MetadataVeryLargeFontSize = 5.35F;

            /// <summary>
            /// The font size to apply on the header of the control.
            /// </summary>
            private float instanceNameFontSize;

            /// <summary>
            /// The font size to apply on the detaild of the control.
            /// </summary>
            private float metadataFontSize;

            /// <summary>
            /// SqlServerInstanceThumbnailFontSizeHelper Constructor. Initialize the font sizes to
            /// apply at the header and details of the control.
            /// </summary>
            public SqlServerInstanceThumbnailFontSizeHelper()
            {
                normalFontSize = MetadataNormalFontSize;
                mediumFontSize = MetadataMediumFontSize;
                largeFontSize = MetadataLargeFontSize;
                veryLargeFontSize = MetadataVeryLargeFontSize;

                instanceNameFontSize = GetInstanceNameFontSize();
                // 'GetFontSize' used to get the font size for details of the control.
                metadataFontSize = GetFontSize(this);
            }

            /// <summary>
            /// Calculates the header font size, according the current DPI resolution of the system.
            /// </summary>
            /// <returns>The header font size, according the current DPI resolution of the system.</returns>
            private float GetInstanceNameFontSize()
            {
                switch (OSCurrentDPI)
                {
                    case NormalDPI:
                        return InstanceNameNormalFontSize;
                    case MediumDPI:
                        return InstanceNameMediumFontSize;
                    case LargeDPI:
                        return InstanceNameLargeFontSize;
                    case VeryLargeDPI:
                        return InstanceNameVeryLargeFontSize;
                    default:
                        return InstanceNameNormalFontSize;
                }
            }

            /// <summary>
            /// Primary method of any ABstracControlHelper derived class
            /// </summary>
            /// <param name="control"></param>
            public override void AutoScale(Control control)
            {
                if (control is SqlServerInstanceThumbnail)
                {
                    var thumbnailControl = control as SqlServerInstanceThumbnail;
                    thumbnailControl.UpdateFontSizes(instanceNameFontSize, metadataFontSize);
                }
            }
        }

        #endregion Nested Classes
    }
}
