#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
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
using System.Threading;
using System.Windows.Forms;

namespace  PGNapoleonics.WinForms {
  /// <summary>A Last-chance THread Exception handler.</summary>
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
  public class ThreadExceptionHandler {
    /// <summary>Handles the thread exception.</summary> 
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1031:DoNotCatchGeneralExceptionTypes"), 
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", 
      "CA1303:Do not pass literals as localized parameters", 
      MessageId = "System.Windows.Forms.MessageBox.Show(System.String,System.String,"
                + "System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxIcon)"), 
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", 
      "CA1804:RemoveUnusedLocals", MessageId = "result"), 
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", 
      "CA1300:SpecifyMessageBoxOptions")]
    public void ApplicationThreadException(object sender, ThreadExceptionEventArgs e) {
      if (sender==null) throw new ArgumentNullException("sender");
      if (e==null) throw new ArgumentNullException("e");
      if(e.Exception is NotImplementedException 
      || e.Exception is NotSupportedException
      || e.Exception is InvalidOperationException
      ) {
          MessageBox.Show(e.Exception.Message, 
            "Application Error", 
            MessageBoxButtons.OK, 
            MessageBoxIcon.Information);
      } else {
        try {
          var result = ShowThreadExceptionDialog(e.Exception);
          Application.ExitThread();
        }
        catch {  // Fatal error, terminate program
          try {
            MessageBox.Show("Fatal Error in Unhandled Exception Handler.", 
              "Fatal Application Error", 
              MessageBoxButtons.OK, 
              MessageBoxIcon.Stop);
          }
          finally { Application.Exit(); }
        }
      }
    }

    /// <summary>Creates and displays the error message.</summary>
    private static DialogResult ShowThreadExceptionDialog(Exception ex) {
      var errorMessage= 
        "Unhandled Exception:" + Environment.NewLine + Environment.NewLine +
        ex.Message + Environment.NewLine + Environment.NewLine +
        ex.GetType() + Environment.NewLine + Environment.NewLine +
        "Stack Trace:" + Environment.NewLine +
        ex.StackTrace;

      var dialog = new ExceptionDialog(errorMessage);
      try {
        dialog.Show();
      } finally { if (dialog!=null) dialog.Dispose(); }
      return DialogResult.Abort;
    }
  }
}
