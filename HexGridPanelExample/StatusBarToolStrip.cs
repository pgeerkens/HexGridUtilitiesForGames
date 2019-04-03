using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PGNapoleonics.HexgridPanelExample {
    public partial class StatusBarToolStrip : UserControl {
        public StatusBarToolStrip() => InitializeComponent();

        public string StatusLabelText {
            get => statusLabel.Text;
            set => statusLabel.Text = value;
        }

        public ToolStrip ToolStrip => toolStrip;
    }
}
