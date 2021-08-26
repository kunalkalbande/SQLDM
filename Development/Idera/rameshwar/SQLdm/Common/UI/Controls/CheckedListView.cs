//------------------------------------------------------------------------------
// <copyright file="CheckedListView.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.UI.Controls
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Windows.Forms;

    public class CheckedListView : ListView
    {
        public const int WM_USER = 0x0400;
        public const int WM_REFLECT = WM_USER + 0x1C00;
        public const int WM_NOTIFY = 0x004E;
        public const int LVM_HITTEST = (0x1000 + 18);
        public const int NM_DBLCLK = (-3);

        public CheckedListView()
        {
            this.FullRowSelect = true;
            this.CheckBoxes = true;
            this.View = View.Details;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public int code;
        }

        private bool m_inDoubleClickCheckHack = false;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_REFLECT + WM_NOTIFY:
                    NMHDR hdr = (NMHDR)m.GetLParam(typeof(NMHDR));
                    if (hdr.code == NM_DBLCLK)
                        m_inDoubleClickCheckHack = true;
                    break;
                case LVM_HITTEST:
                    if (m_inDoubleClickCheckHack)
                    {
                        m_inDoubleClickCheckHack = false;
                        m.Result = (System.IntPtr)(-1);
                        return;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            ResizeColumns();
            base.OnLayout(levent);
        }

        private void ResizeColumns()
        {
            int nColumns = this.Columns.Count;
            if (nColumns < 1)
                return;

            int width = this.ClientSize.Width - 2;
            if (width <= 0)
                return;

            int total = 0; 
            foreach (ColumnHeader column in this.Columns)
            {
                total += column.Width;
            }
            float adj = ((float)width) / total;
            foreach (ColumnHeader column in this.Columns)
            {
                column.Width = (int)Math.Floor(column.Width * adj);
            }            
        }
    }
}
