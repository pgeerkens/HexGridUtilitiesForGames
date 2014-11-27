#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexgridPanel.Properties;

namespace  PGNapoleonics.WinForms {
  /// <summary>A Last-chance Thread Exception handler.</summary>
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
  public class ThreadExceptionHandler {
    static ResourceManager StringManager =
            new ResourceManager("en-US", Assembly.GetExecutingAssembly());
    static CultureInfo Culture = CultureInfo.CurrentCulture;

    static readonly string NewLine     = Environment.NewLine;
    static readonly string NewLineX2   = Environment.NewLine + Environment.NewLine;
    readonly string ErrorTitle  = Application.ProductName + Resources.SevereApplicationError;
    readonly string FatalTitle  = Application.ProductName + Resources.FatalApplicationError;
    readonly string NullEventArgs = Resources.NullThreadExceptionArgs;
    readonly string FatalMessage  = Resources.FatalThreadingError;

    /// <summary>Handles the thread exception.</summary> 
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public void ApplicationThreadException(object sender, ThreadExceptionEventArgs e) {
      try {
        var senderType = sender==null ? StringManager.GetString("NullSender",Culture) : sender.ToString();
        if (e == null) {  // This should never happen
            MessageBox.Show(
              NullEventArgs + senderType, 
              ErrorTitle, 
              MessageBoxButtons.OK, 
              MessageBoxIcon.Stop,
              MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            Application.Exit();
        } else {

          if( e.Exception is NotImplementedException
          ||  e.Exception is NotSupportedException
          ||  e.Exception is InvalidOperationException
          ) {
             MessageBox.Show(
                e.Exception.Message + StringManager.GetString("From",Culture) + senderType, 
                ErrorTitle, 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Stop,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
          } else {
             if (ShowThreadExceptionDialog(e.Exception) == DialogResult.Abort) Application.Exit();
          }
          Application.ExitThread();
        }
      } catch(Exception) {

        try {
          MessageBox.Show(FatalMessage, 
            FatalTitle, 
            MessageBoxButtons.OK, 
            MessageBoxIcon.Stop,
            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        } finally {
          Application.Exit();
        }

      }
    }

    /// <summary>Creates and displays the error message.</summary>
    private static DialogResult ShowThreadExceptionDialog(Exception ex) {
      var result = DialogResult.Abort;

      var errorMessage = 
        "Unhandled Exception:" + NewLineX2 +
        ex.Message             + NewLineX2 +
        ex.GetType()           + NewLineX2 +
        "Stack Trace:"         + NewLine +
        ex.StackTrace;

      using (var dialog = new ExceptionDialog(errorMessage)) {
        dialog.ShowDialog();
        result = dialog.DialogResult;
      }

      return result;
    }
  }
}
