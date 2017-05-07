using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static Micro.Menu.Core;

namespace Micro.Menu {
    public partial class FormIcon : Form {
        public FormIcon() {
            InitializeComponent();
            view.SmallImageList = new ImageList() { ColorDepth = ColorDepth.Depth32Bit };
            view.LargeImageList = new ImageList() { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(32, 32) };
        }

        void switchSmallLarge(object sender, EventArgs e)
            => view.View = larger.Checked ? View.LargeIcon : View.SmallIcon;
        void txtPathKey(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                refresh(sender, e);
        }
        void txtPathSelect(object sender, EventArgs e) {
            if (txtPath.SelectedIndex >= 0)
                refresh(sender, e);
        }
        void viewSelect(object sender, EventArgs e) {
            if (view.SelectedItems.Count > 0)
                txtResult.Text = $"{txtPath.Text},{view.SelectedItems[0].Text}";
        }

        void refresh(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            view.Clear();
            view.SmallImageList.Images.Clear();
            view.LargeImageList.Images.Clear();
            if (File.Exists(txtPath.Text)) {
                int max = -1;
                try {
                    for (int i = 0; true; i++) {
                        max = i;
                        var s = i + "";
                        var ic = ExtractIcons(txtPath.Text, i);
                        view.SmallImageList.Images.Add(s, ic[0]);
                        view.LargeImageList.Images.Add(s, ic[1]);
                    }
                } catch { }
                for (int i = 0; i < max; i++) {
                    var s = i + "";
                    view.Items.Add(new ListViewItem(s, s));
                }
            }
            Cursor = Cursors.Default;
        }
    }
}
