using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class SyncTextBox : TextBox
    {
        public SyncTextBox()
        {
            Multiline = true;
            this.ScrollBars = ScrollBars.None;

        }

	protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyValue == 38) //up
            {
                SendMessage(Buddy.Handle, (int)WM_VSCROLL, new IntPtr(0), new IntPtr(0)); //WParam: 1- scroll down, 0- scroll up
            }
            else if (e.KeyValue == 40) //down
            {
                SendMessage(Buddy.Handle, (int)WM_VSCROLL, new IntPtr(1), new IntPtr(0)); //WParam: 1- scroll down, 0- scroll up
            }

            base.OnKeyDown(e);
        }

        public Control Buddy { get; set; }

        private static bool scrolling;   // In case buddy tries to scroll us
        int WM_MOUSEWHEEL = 0x20a; // or 522
        int WM_VSCROLL = 0x115; // or 277
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (Buddy != this && !scrolling && Buddy != null && Buddy.IsHandleCreated)
            {
                if (m.Msg == WM_MOUSEWHEEL)
                {
                    int updown = unchecked((short)((long)m.WParam >> 16));

                    scrolling = true;

                    if (updown < 0)  //mouse wheel scrolls down
                    {
                        int reps = (updown / -40);

                        for (int i = 0; i < reps; i++)
                            SendMessage(Buddy.Handle, (int)WM_VSCROLL, new IntPtr(1), new IntPtr(0)); //WParam: 1- scroll down, 0- scroll up
                    }
                    else if ((long)m.WParam > 0)
                    {
                        int reps = (updown / 40);
                        for (int i = 0; i < reps; i++)
                            SendMessage(Buddy.Handle, (int)WM_VSCROLL, new IntPtr(0), new IntPtr(0));
                    }
                    scrolling = false;

                }
                else if (m.Msg == WM_VSCROLL)
                {
                    scrolling = true;
                    SendMessage(Buddy.Handle, m.Msg, m.WParam, m.LParam);
                    scrolling = false;
                }
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }
    public class SyncRichTextBox : RichTextBox
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("User32.dll")]
        public extern static int GetScrollPos(IntPtr hWnd, int nBar);

        public RichTextBox Buddy { get; set; }

        public enum ScrollBarType : uint
        {
            SbHorz = 0,
            SbVert = 1,
            SbCtl = 2,
            SbBoth = 3
        }

        public enum Message : uint
        {
            WM_VSCROLL = 0x0115
        }

        public enum ScrollBarCommands : uint
        {
            SB_THUMBPOSITION = 4
        }

        public SyncRichTextBox()
        {
            Multiline = true;
            SelectionAlignment = HorizontalAlignment.Right;
        }

        protected override void OnVScroll(EventArgs e)
        {
            if (this == Buddy)
                return;

            if (Buddy == null)
                throw new NullReferenceException("must set the Buddy first");

            int nPos = GetScrollPos(this.Handle, (int)ScrollBarType.SbVert);
            nPos <<= 16;
            uint wParam = (uint)ScrollBarCommands.SB_THUMBPOSITION | (uint)nPos;
            SendMessage(Buddy.Handle, (int)Message.WM_VSCROLL, new IntPtr(wParam), new IntPtr(0));

            base.OnVScroll(e);
        }
    }
}
