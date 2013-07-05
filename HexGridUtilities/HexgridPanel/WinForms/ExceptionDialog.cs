using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PGNapoleonics.WinForms {
  public partial class ExceptionDialog : Form {
    public ExceptionDialog(string messageText) {
      InitializeComponent();
      this.ErrorText.Text = messageText;
    }
  }
}
