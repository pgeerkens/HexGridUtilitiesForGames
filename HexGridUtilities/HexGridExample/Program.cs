using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using PG_Napoleonics.Utilities;

namespace PG_Napoleonics.HexGridExample {
  static class Program {
    /// <summary>The main entry point for the application.</summary>
    [STAThread]
    static void Main()      {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.ThreadException += 
        new ThreadExceptionEventHandler(
          (new ThreadExceptionHandler()).Application_ThreadException);

      Application.Run(new HexGridExampleForm());
    }

    public static void InquireOnThisException(Exception ex) {
      string message = ex.Message + (ex.InnerException == null 
                     ? "" 
                     : Environment.NewLine + ex.InnerException.Message);
      MessageBox.Show(message,"Open Map-File Error",MessageBoxButtons.OK);
    }
  }
}
