namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class FileActivityPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                controlBackColorBrush.Dispose();
                diskBackColorBrush.Dispose();
                fileBackColorBrush.Dispose();
                fileBackColorMaxBrush.Dispose();
                fileOthersBackColorBrush.Dispose();
                fileIconBackColorBrush.Dispose();
                fileDarkTextBrush.Dispose();
                fileEmptyActivityBrush.Dispose();
                fileReadActivityBrush.Dispose();
                fileWriteActivityBrush.Dispose();
                fileChartBackColorBrush.Dispose();
                outlinePen.Dispose();
                histogramDividerPen.Dispose();
                readHistogramPen.Dispose();
                writeHistorgramPen.Dispose();
                readLinePen.Dispose();
                writeLinePen.Dispose();
                histogramBotBackBrush.Dispose();

                disksScrollBar.Dispose();
                menu.Dispose();
                tooltip.Dispose();

                if (flashThread.IsAlive)
                    flashThread.Abort();

                for (int i = 0; i < readActivityBrushes.Length; i++)
                {
                    readActivityBrushes[i].Dispose();
                    writeActivityBrushes[i].Dispose();
                }

                if(components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
