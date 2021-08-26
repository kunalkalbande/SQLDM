using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public class CustomToolbarsManager : UltraToolbarsManager
    {
        public CustomToolbarsManager(IContainer container) : base(container)
        {
        }

        // activate the top level window handle containing the control hosting the toolbars manager
        protected override bool ActivateForm()
        {
            var succeeded = base.ActivateForm();

            if (succeeded)
                return true;

            var dockWithinContainer = this.DockWithinContainer;

            if (dockWithinContainer == null || dockWithinContainer.IsHandleCreated == false)
                return false;

            var parentHwnd = NativeWindowMethods.FindTopLevelWindow(dockWithinContainer.Handle);

            if (parentHwnd == IntPtr.Zero)
                return false;

            NativeWindowMethods.SetActiveWindow(parentHwnd);
            return true;
        }

        protected override Control ActiveControlOfActiveForm
        {
            get
            {
                var control = base.ActiveControlOfActiveForm;
                if (control != null)
                    return control;

                return Control.FromChildHandle(NativeWindowMethods.GetFocus());
            }
        }

        protected override FormWindowState FormWindowState
        {
            get
            {
                var windowState = base.FormWindowState;
                if (windowState != FormWindowState.Normal)
                    return windowState;

                var dockWithinContainer = this.DockWithinContainer;
                if (dockWithinContainer == null || dockWithinContainer.IsHandleCreated == false)
                    return windowState;

                var window = NativeWindowMethods.FindTopLevelWindow(dockWithinContainer.Handle);
                if (window == IntPtr.Zero)
                    return windowState;

                return NativeWindowMethods.GetPlacement(window);
            }
        }

        protected override bool IsFormActive
        {
            get
            {
                if (base.IsFormActive)
                    return true;

                var dockWithinContainer = this.DockWithinContainer;
                if (dockWithinContainer == null || dockWithinContainer.IsHandleCreated == false)
                    return false;

                var activeForm = NativeWindowMethods.GetForegroundWindow();
                var form = NativeWindowMethods.FindTopLevelWindow(dockWithinContainer.Handle);

                while (activeForm != IntPtr.Zero)
                {
                    if (activeForm == form)
                        return true;

                    activeForm = NativeWindowMethods.GetWindowLong(activeForm, NativeWindowMethods.GWL_HWNDPARENT);
                }

                return false;
            }
        }

        protected override bool IsControlOnActiveForm(Control control)
        {
            if (base.IsControlOnActiveForm(control))
                return true;

            var activeForm = NativeWindowMethods.GetForegroundWindow();
            var controlHandle = control.Handle;
            while (controlHandle != IntPtr.Zero)
            {
                if (controlHandle == activeForm)
                    return true;

                controlHandle = NativeWindowMethods.GetParent(controlHandle);
            }

            return false;
        }

        protected override void OnFloatingToolbarWindowShown(FloatingToolbarWindowBase floatingToolbarWindow)
        {
            base.OnFloatingToolbarWindowShown(floatingToolbarWindow);

            if (floatingToolbarWindow.Owner != null)
                return;

            var dockWithinContainer = this.DockWithinContainer;
            if (dockWithinContainer != null && dockWithinContainer.IsHandleCreated)
            {
                try
                {
                    IntPtr ownerHandle = NativeWindowMethods.FindTopLevelWindow(dockWithinContainer.Handle);
                    if (ownerHandle != IntPtr.Zero)
                    {
                        NativeWindowMethods.SetWindowLong(floatingToolbarWindow.Handle, NativeWindowMethods.GWL_HWNDPARENT, ownerHandle);
                        // TODO: If the owner is TopMost, make the floating toolbar window TopMost
                    }
                }
                catch (SecurityException) { }
            }
        }

    }

    [SuppressUnmanagedCodeSecurity]
    internal class NativeWindowMethods
    {
        internal const int GWL_HWNDPARENT = -8;
        const Int32 SW_HIDE = 0;
        const Int32 SW_SHOWNORMAL = 1;
        const Int32 SW_NORMAL = 1;
        const Int32 SW_SHOWMINIMIZED = 2;
        const Int32 SW_SHOWMAXIMIZED = 3;
        const Int32 SW_MAXIMIZE = 3;
        const Int32 SW_SHOWNOACTIVATE = 4;
        const Int32 SW_SHOW = 5;
        const Int32 SW_MINIMIZE = 6;
        const Int32 SW_SHOWMINNOACTIVE = 7;
        const Int32 SW_SHOWNA = 8;
        const Int32 SW_RESTORE = 9;

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        [DllImport("user32")]
        internal static extern IntPtr GetFocus();

        [DllImport("user32")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32")]
        internal static extern IntPtr GetParent(IntPtr childHwnd);

        [DllImport("user32")]
        internal static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32")]
        internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr newLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        internal static IntPtr FindTopLevelWindow(IntPtr childWindow)
        {
            IntPtr control = childWindow;

            while (true)
            {
                IntPtr nextControl = GetParent(control);

                if (nextControl == IntPtr.Zero)
                    break;

                control = nextControl;
            }

            return control;
        }


        internal static FormWindowState GetPlacement(IntPtr handle)
        {
            var placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(handle, ref placement);
            switch (placement.showCmd)
            {
                case SW_SHOWNORMAL: return FormWindowState.Normal;
                case SW_SHOWMAXIMIZED: return FormWindowState.Maximized;
                case SW_SHOWMINIMIZED: return FormWindowState.Minimized;
            }
            return FormWindowState.Normal;
        }
    }
}
