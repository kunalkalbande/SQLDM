using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Windows.Themes;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public class BaseDialog : Form
    {
        public Color backcolor;
        private Rectangle secondHalf;
        public string DialogHeader="";
        public BaseDialog()
        {
            Text = "Title";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            //FormBorderStyle = FormBorderStyle.FixedDialog;
            ShowInTaskbar = false;
            HelpButton = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            //  this.backcolor = Color.Red;
            if (Settings.Default.ColorScheme == "Dark")
            {
                backcolor = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
                BackColor = backcolor;
                ForeColor = Color.White;
                // this.Paint += new PaintEventHandler(paintPanel);
                //this.Font= new System.Drawing.Font("SourceSansPro", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            }

            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        public virtual void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Theme Chane Event got hit in Base dialogue class");
            this.backcolor = Color.Yellow;
            Console.WriteLine("Current color");
            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                SetWindowTheme(this.Handle, "", "");
                base.OnHandleCreated(e);
            }
        }

        //const int WM_NCPAINT = 0x85;

        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern IntPtr GetWindowDC(IntPtr hwnd);

        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern void DisableProcessWindowsGhosting();

        [DllImport("UxTheme.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);


        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);

        //    switch (m.Msg)
        //    {
        //        case WM_NCPAINT:
        //            {
        //                IntPtr hdc = GetWindowDC(m.HWnd);
        //                using (Graphics g = Graphics.FromHdc(hdc))
        //                {
        //                    Brush b = new SolidBrush(ColorTranslator.FromHtml(DarkThemeColorConstants.HeaderStripBackColor));
        //                    g.FillRectangle(b, new Rectangle(0, 0, 0, 0));

        //                    Rectangle firstHalf = new Rectangle(0, 0, this.Width, 25);
        //                    Console.WriteLine("Dialogue widht" + this.Width);
        //                    secondHalf = new Rectangle(this.Width-50, 0, this.Width, 25);

        //                    //Draw border over panel
        //                    Rectangle fullwindow = new Rectangle(0, 0, this.Width, this.Height);

        //                    LinearGradientBrush thalf = new LinearGradientBrush(fullwindow,
        //                       ColorTranslator.FromHtml(DarkThemeColorConstants.HeaderStripBackColor), ColorTranslator.FromHtml(DarkThemeColorConstants.HeaderStripBackColor), 45f);

        //                    float penWidth = 1F;
        //                    Pen myPen = new Pen(Brushes.Gray, (int)penWidth);
        //                    g.DrawRectangle(myPen, penWidth / 2F, penWidth / 2F,
        //                                             (float)this.Width - 2F * penWidth,
        //                                             (float)this.Height + 1 - 2F * penWidth);

        //                    myPen.Dispose();


        //                    // firstHalf.
        //                    LinearGradientBrush bfhalf = new LinearGradientBrush(firstHalf,
        //                        ColorTranslator.FromHtml(DarkThemeColorConstants.HeaderStripBackColor), ColorTranslator.FromHtml(DarkThemeColorConstants.HeaderStripBackColor), 45f);
        //                    g.FillRectangle(bfhalf, firstHalf);
        //                    // Second Half.
        //                    LinearGradientBrush bshalf = new LinearGradientBrush(secondHalf,
        //                   ColorTranslator.FromHtml(DarkThemeColorConstants.HeaderStripBackColor), ColorTranslator.FromHtml(DarkThemeColorConstants.HeaderStripBackColor), 45f);

        //                    g.DrawString(DialogHeader, new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.White,
        //                        new PointF(firstHalf.X + 6, firstHalf.Y + 6)); //top layer
        //                    g.FillRectangle(bshalf, secondHalf);
        //                    g.DrawString("?", new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.Black,
        //                    new PointF(secondHalf.X, secondHalf.Y + 5));
        //                    g.DrawString("X", new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.Black,
        //                       new PointF(secondHalf.X+25, secondHalf.Y + 5)); //top layer

        //                    bfhalf.Dispose();
        //                    bshalf.Dispose();


        //                }
        //                int r = ReleaseDC(m.HWnd, hdc);
        //            }
        //            break;
        //    }
        //}


    }
}
