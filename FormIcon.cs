using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Menu.Functions;

namespace Menu {
    public partial class FormIcon : Form {
        public FormIcon() {
            InitializeComponent();
        }
        private void FormIcon_Load(object sender, EventArgs e) {
            refresh(null, null);
        }

        public void refresh(object sender, EventArgs e) {
            if (File.Exists(txtPath.Text)) {
                try {
                    img16.Image = ExtractIcon(txtPath.Text, (int)numIcon.Value);
                    img32.Image = ExtractIcon(txtPath.Text, (int)numIcon.Value, true);
                    txtResult.Text = txtPath.Text + "," + numIcon.Value;
                }
                catch (Exception) {
                    img16.Image = null;
                    img32.Image = null;
                    txtResult.Text = "Icon " + numIcon.Value + @" doesn't exist";
                }
            } else {
                img16.Image = null;
                img32.Image = null;
            }
        }
    }
}
