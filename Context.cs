using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Menu {
    class Context : ApplicationContext {
        public static ContextMenuStrip ListL, ListR;
        public static Screen scr;
        public static int closeFrom;
        public static string userName;
        string path, jsonFile;
        NotifyIcon quickBarItem;
        FormIcon fi;
        ToolStripMenuItem _reload, _edit, _findIcon, _exit;
        List<ToolStripItem> items;
        List<Element> elements;
        MethodInfo mi;

        public Context() {
            //var s = @"D:\\Documenti\\file inesistente.txt";
            //s = Path.GetExtension(s);
            quickBarItem = new NotifyIcon();
            ListL = new ContextMenuStrip();
            ListR = new ContextMenuStrip();
            scr = Screen.FromControl(ListL);
            closeFrom = 0;
            userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            path = Path.Combine(Application.StartupPath, "menu.json");
            jsonFile = Properties.Resources.defaultJson;
            elements = new List<Element>();
            mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);

            setLists();
            setIcon();

            _reload = new ToolStripMenuItem(
                "Reload menu",
                Properties.Resources.reload.ToBitmap(),
                (a, b) => load());

            _edit = new ToolStripMenuItem(
                "Edit menu",
                Properties.Resources.wrench.ToBitmap(),
                edit);

            _findIcon = new ToolStripMenuItem(
                "Find icon",
                Properties.Resources.binocular.ToBitmap(),
                findIcon);

            _exit = new ToolStripMenuItem(
                "Exit",
                Properties.Resources.cross.ToBitmap(),
                close);

            ListR.Items.AddRange(new ToolStripItem[] {
                _reload,
                _edit,
                _findIcon,
                new ToolStripSeparator(),
                _exit });

            quickBarItem.MouseUp += click;
            ListL.Closing += listClosing;

            load();
        }
        public void setLists() {
            ListL.Name = "MenuL";
            ListL.Size = new System.Drawing.Size(61, 4);
            ListR.Name = "MenuR";
            ListR.Size = new System.Drawing.Size(61, 4);
        }
        public void setIcon() {
            quickBarItem.Icon = Properties.Resources.json;
            quickBarItem.Text = "Menu";
            quickBarItem.Visible = true;
        }

        public void load() {
            quickBarItem.ShowBalloonTip(250, "", "Loading...", ToolTipIcon.None);

            string balloon = "Loaded";
            ListL.Items.Clear();

            try {
                items = new List<ToolStripItem>();
                if (!File.Exists(path)) {
                    File.Create(path).Close();
                    File.WriteAllText(path, jsonFile);
                    balloon = "Default menu loaded";
                } else
                    jsonFile = File.ReadAllText(path);
                elements = JsonConvert.DeserializeObject<List<Element>>(jsonFile);

                foreach (Element i in elements)
                    items.Add(i.toMenuItem());
                
                ListL.Items.AddRange(items.ToArray());
            } catch (Exception ex) {
                balloon = "Error";
                if (ex.ToString().Contains("Json"))
                    MessageBox.Show("Error while parsing menu: \n\n" + ex.Message, "Error", 0, (MessageBoxIcon)48);
                else
                    MessageBox.Show("Program error: \n\n" + ex.ToString(), "Critical", 0, (MessageBoxIcon)16);
            }
            quickBarItem.ShowBalloonTip(250, "", balloon, ToolTipIcon.None);
        }
        public void click(object sender, MouseEventArgs e) {
            quickBarItem.ContextMenuStrip = null;

            if (e.Button == MouseButtons.Left && ListL.Items.Count > 0)
                quickBarItem.ContextMenuStrip = ListL;
            else if (e.Button == MouseButtons.Right)
                quickBarItem.ContextMenuStrip = ListR;

            if (quickBarItem.ContextMenuStrip != null)
                mi.Invoke(quickBarItem, null);
        }
        public static void itemClick(object sender, MouseEventArgs e) {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;

            string[] info = (string[])s.Tag;
            if (e.Button != MouseButtons.Middle) {
                closeFrom = 1;
                try {
                    if (info.Length >= 4) {
                        ProcessStartInfo proc = new ProcessStartInfo(info[1], info[2]);
                        if (info[3] != "")
                            proc.WorkingDirectory = info[3];
                        if (e.Button == MouseButtons.Right)
                            proc.Verb = "runas";
                        Process.Start(proc);
                    }
                } catch (Exception ex) {
                    MessageBox.Show(
                        "Failed to start " + Path.GetFileNameWithoutExtension(info[0]) +
                        ":\n" + ex.Message +
                        "\n\n" + "Path:" +
                        "\n" + info[1],
                        "Fail", 0, MessageBoxIcon.Information);
                }
            } else {
                Process.Start("explorer.exe", @"/select," + info[1]);
            }
        }
        public static void listClosing(object sender, ToolStripDropDownClosingEventArgs e) {
            if (closeFrom == 2)
                e.Cancel = true;
            closeFrom = 0;
        }

        public void findIcon(object sender, EventArgs e) {
            fi = new FormIcon();
            fi.Show();
        }
        public void edit(object sender, EventArgs e) {
            if (!File.Exists(path)) {
                File.Create(path).Close();
                File.WriteAllText(path, jsonFile);
            }
            Process.Start(path);
        }
        public void close(object sender, EventArgs e) {
            quickBarItem.Visible = false;
            Application.Exit();
        }
    }

    public class ToolStripWatermarkTextbox : ToolStripTextBox {
        public string Watermark { get; set; }
        bool watermarked = true;
        string _text = "";
        public override string Text {
            get { return _text; }
            set {
                _text = value;
                base.Text = value;
            }
        }
        public new event EventHandler TextChanged;

        public ToolStripWatermarkTextbox(string watermark) : base() {
            Watermark = watermark;
            base.TextChanged += txtChg;
            toWatermark();
            GotFocus += (a, b) => toNormal();
            LostFocus += (a, b) => toWatermark();
        }

        void txtChg(object sender, EventArgs e) {
            if (!toNormal())
                toWatermark();
            if (!watermarked && TextChanged != null)
                TextChanged(sender, e);
        }
        bool toNormal() {
            if (_text != "" || Focused) {
                Console.WriteLine(watermarked);
                if (watermarked) {
                    base.Text = "";
                    base.ForeColor = System.Drawing.Color.Black;
                    base.Font = new System.Drawing.Font(base.Font, System.Drawing.FontStyle.Regular);
                    watermarked = false;
                }
                return true;
            }
            return false;
        }
        bool toWatermark() {
            if (_text == "" && !Focused) {
                if (!watermarked) {
                    watermarked = true;
                    base.Text = Watermark;
                    base.ForeColor = System.Drawing.Color.Gray;
                    base.Font = new System.Drawing.Font(base.Font, System.Drawing.FontStyle.Italic);
                }
                return true;
            }
            return false;
        }
    }
}