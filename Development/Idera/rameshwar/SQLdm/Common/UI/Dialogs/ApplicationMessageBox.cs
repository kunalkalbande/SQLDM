using System;
using System.Windows.Forms;
using Microsoft.SqlServer.MessageBox;
using BBS.TracerX;

namespace Idera.SQLdm.Common.UI.Dialogs {
    /// <summary>
    /// This wrapper of ExceptionMessageBox always defaults the caption to Application.ProductName.
    /// It has some static methods for showing messages so you don't always have to 
    /// construct your own instance.  If they aren't flexible enough, use them as examples for
    /// writing your own code.
    /// </summary>
    public class ApplicationMessageBox : ExceptionMessageBox 
    {
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(ApplicationMessageBox));

        public ApplicationMessageBox()
        {
            Caption = Application.ProductName;
            Symbol = ExceptionMessageBoxSymbol.Information;
        }

        public ApplicationMessageBox(Exception exception) : this()
        {
            Message = exception;
            Symbol = ExceptionMessageBoxSymbol.Error;
        }

        public static DialogResult ShowMessage(string message)
        {
            return ShowMessage(null, message, null, ExceptionMessageBoxSymbol.None, ExceptionMessageBoxButtons.OK, true);
        }

        public static DialogResult ShowMessage(IWin32Window owner, string message)
        {
            return ShowMessage(owner, message, null, ExceptionMessageBoxSymbol.None, ExceptionMessageBoxButtons.OK, true);
        }

        public static DialogResult ShowMessage(IWin32Window owner, string message, Exception exception, ExceptionMessageBoxSymbol symbol, ExceptionMessageBoxButtons buttons, bool beep)
        {
            return ShowMessage(owner, message, exception, symbol, buttons, ExceptionMessageBoxDefaultButton.Button1, beep);
        }

        public static DialogResult ShowMessage(IWin32Window owner, string message, Exception exception, ExceptionMessageBoxSymbol symbol, ExceptionMessageBoxButtons buttons, ExceptionMessageBoxDefaultButton defaultButton, bool beep)
        {
            switch (symbol)
            {
                case ExceptionMessageBoxSymbol.Information:
                    Log.Info(message, exception);
                    break;
                case ExceptionMessageBoxSymbol.Warning:
                    Log.Warn(message, exception);
                    break;
                case ExceptionMessageBoxSymbol.Error:
                    Log.Error(message, exception);
                    break;
                default:
                    Log.Debug(message, exception);
                    break;
            }

            ApplicationMessageBox messageBox = new ApplicationMessageBox();

            Form ownerForm = owner as Form;
            if (ownerForm != null && ownerForm.Text != null && ownerForm.Text.Length != 0)
            {
                messageBox.Caption = ownerForm.Text;
            }

            if (message != null && message.Length > 0)
            {
                messageBox.Message = new Exception(message, exception);
            }
            else if (exception != null)
            {
                messageBox.Message = exception;
            }

            messageBox.Symbol = symbol;
            messageBox.Buttons = buttons;
            messageBox.DefaultButton = defaultButton;
            messageBox.Beep = beep;

            return messageBox.Show(owner);
        }

        public static void ShowInfo(IWin32Window owner, string message)
        {
            ShowInfo(owner, message, true);
        }

        public static void ShowInfo(IWin32Window owner, string message, bool beep)
        {
            ShowMessage(owner, message, null, ExceptionMessageBoxSymbol.Information, ExceptionMessageBoxButtons.OK, beep);
        }

        public static void ShowWarning(IWin32Window owner, string message)
        {
            ShowMessage(owner, message, null, ExceptionMessageBoxSymbol.Warning, ExceptionMessageBoxButtons.OK, true);
        }

        public static DialogResult ShowWarning(IWin32Window owner, string message, ExceptionMessageBoxButtons buttons)
        {
            return ShowMessage(owner, message, null, ExceptionMessageBoxSymbol.Warning, buttons, true);
        }

        public static void ShowWarning(IWin32Window owner, string message, Exception exception)
        {
            ShowMessage(owner, message, exception, ExceptionMessageBoxSymbol.Warning, ExceptionMessageBoxButtons.OK, true);
        }

        public static DialogResult ShowWarning(IWin32Window owner, string message, Exception exception, ExceptionMessageBoxButtons buttons)
        {
            return ShowMessage(owner, message, exception, ExceptionMessageBoxSymbol.Warning, buttons, true);
        }

        public static void ShowError(IWin32Window owner, Exception exception)
        {
            ShowMessage(owner, null, exception, ExceptionMessageBoxSymbol.Error, ExceptionMessageBoxButtons.OK, true);
        }

        public static void ShowError(IWin32Window owner, string message) {
            ShowMessage(owner, message, null, ExceptionMessageBoxSymbol.Error, ExceptionMessageBoxButtons.OK, true);
        }

        public static void ShowError(IWin32Window owner, string message, Exception exception)
        {
            ShowMessage(owner, message, exception, ExceptionMessageBoxSymbol.Error, ExceptionMessageBoxButtons.OK, true);
        }

        public static void ShowError(IWin32Window owner, string message, Exception exception, bool beep)
        {
            ShowMessage(owner, message, exception, ExceptionMessageBoxSymbol.Error, ExceptionMessageBoxButtons.OK, beep);
        }

        public static DialogResult ShowQuestion(IWin32Window owner, string message)
        {
            return ShowMessage(owner, message, null, ExceptionMessageBoxSymbol.Question, ExceptionMessageBoxButtons.YesNo,ExceptionMessageBoxDefaultButton.Button1, false);
        }

        public static DialogResult ShowQuestion(IWin32Window owner, string message, ExceptionMessageBoxButtons button)
        {
            return ShowMessage(owner, message, null, ExceptionMessageBoxSymbol.Question, button, ExceptionMessageBoxDefaultButton.Button1, false);
        }

        public static DialogResult ShowQuestion(IWin32Window owner, string message, ExceptionMessageBoxButtons button, ExceptionMessageBoxDefaultButton defaultButton)
        {
            return ShowMessage(owner, message, null, ExceptionMessageBoxSymbol.Question, button, defaultButton, false);
        }

        /// <summary>
        /// Show a text message with a checkbox and one to five arbitrary strings for the buttons.
        /// If the user checked the checkbox
        /// in the past, whatever button he pressed is returned without displaying the message.
        /// </summary>
        /// <param name="msg">Message text to display.</param>
        /// <param name="checkBoxText">Caption for the check box.  E.g. "Don't show this again."</param>
        /// <param name="savedSettingKey">The key of an integer defined in the Idera.SQLdm.DesktopClient.Properties.Settings.Default object.  This is where the checked state and button to return are saved.</param>
        /// <param name="buttons">Up to five strings that appear as buttons.</param>
        /// <returns></returns>
        //public static ExceptionMessageBoxDialogResult ShowMessageCheck(string msg, string checkBoxText, string savedSettingKey, params string[] buttons)
        //{
        //    // Try to look up the specified saved setting.  If it's not found or has an invalid value, 
        //    // an exception will be thrown, causing the message to be displayed.
        //    try
        //    {
        //        Log.Info("Showing message: " + msg);
        //        int savedVal = (int)Settings.Default[savedSettingKey];
        //        ExceptionMessageBoxDialogResult result = (ExceptionMessageBoxDialogResult)(savedVal - 100);
        //        Log.Info("Automatic result: " + result);
        //        return result;
        //    }
        //    catch
        //    {
        //        ApplicationMessageBox box = new ApplicationMessageBox();
        //        box.Text = msg;
        //        box.ShowCheckBox = true;
        //        box.CheckBoxText = checkBoxText;
        //        box.SetCustomButtons(buttons);
        //        box.Show(null);

        //        if (box.IsCheckBoxChecked &&
        //            box.CustomDialogResult != ExceptionMessageBoxDialogResult.None &&
        //            savedSettingKey != null)
        //        {
        //            try
        //            {
        //                // Save the button clicked by the user for the next time this is called.
        //                Settings.Default[savedSettingKey] = 100 + (int)box.CustomDialogResult;
        //            }
        //            catch (Exception ex)
        //            {
        //                // This probably means the savedSettingKey was not properly defined in Settings.settings.
        //                Log.Error("Could not save checkbox state with key " + savedSettingKey, ex);
        //            }
        //        }

        //        Log.Info("Manual result: " + box.CustomDialogResult);
        //        return box.CustomDialogResult;
        //    }
        //}

        public void SetCustomButtons(params string[] buttons) {
            Buttons = ExceptionMessageBoxButtons.Custom;
            switch (buttons.Length) {
                case 1:
                    SetButtonText(buttons[0]);
                    break;
                case 2:
                    SetButtonText(buttons[0], buttons[1]);
                    break;
                case 3:
                    SetButtonText(buttons[0], buttons[1], buttons[2]);
                    break;
                case 4:
                    SetButtonText(buttons[0], buttons[1], buttons[2], buttons[3]);
                    break;
                case 5:
                    SetButtonText(buttons[0], buttons[1], buttons[2], buttons[3], buttons[4]);
                    break;
                default:
                    throw new ApplicationException("ApplicationMessageBox.ShowMessage called with an invalid number of buttons.");
            }
        }
    }
}
