using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static Micro.Menu.Core;

namespace Micro.Menu {
    class Context : ApplicationContext {
        public static ContextMenuStrip  ListL = new ContextMenuStrip() { Name = "MenuL", Size = new Size(61, 4) },
                                        ListR = new ContextMenuStrip() { Name = "MenuR", Size = new Size(61, 4) };
        public static NotifyIcon quickBarItem = new NotifyIcon() { Text = "Menu", Visible = true };
        public static Screen              scr = Screen.FromControl(ListL);
        public static int           closeFrom = 0;

        public static string path = Path.Combine(Application.StartupPath, "menu.json"),
                             temp = Path.Combine(Environment.ExpandEnvironmentVariables("%temp%"), "default.json"),
                         jsonFile = defaultMenu;
        bool justLoaded;
        List<Element> elements = new List<Element>();
        List<ToolStripItem> items;
        ToolStripMenuItem powerCheck;
        FormIcon fi;
        Rectangle lastRect;
        MethodInfo _showMenu = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);

        public Context() {
            ListR.Items.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Show sample menu", Properties.Resources.question.ToBitmap(), showSample),
                new ToolStripMenuItem("Edit menu", Properties.Resources.pencil.ToBitmap(), edit),
                new ToolStripMenuItem("Find icon", Properties.Resources.magnifier.ToBitmap(), findIcon),
                new ToolStripMenuItem("Reload menu", Properties.Resources.reload.ToBitmap(), (a, b) => load()),
                powerCheck = new ToolStripMenuItem("Launch at startup", Properties.Resources.power.ToBitmap(), (a, b) => startupLaunch = powerCheck.Checked),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Exit", Properties.Resources.cross.ToBitmap(), close) });

            quickBarItem.MouseUp += click;
            ListL.Closing += listClosing;

            load();
        }
        
        public void load() {
            quickBarItem.Icon = Properties.Resources.clock;
            ListL.Items.Clear();
            
            items = new List<ToolStripItem>();
            if (!File.Exists(path)) {
                File.Create(path).Close();
                File.WriteAllText(path, jsonFile);
            } else
                jsonFile = File.ReadAllText(path);

            try {
                elements = JsonConvert.DeserializeObject<List<Element>>(jsonFile);
            } catch (Exception ex) {
                quickBarItem.Icon = Properties.Resources.cross;
                quickBarItem.ShowBalloonTip(8000, "Error", $"Unable to load menu:\n{ex.Message}", ToolTipIcon.Error);
                return;
            }

            foreach (Element i in elements)
                items.Add(i.toMenuItem());

            ListL.Items.AddRange(items.ToArray());
            GC.Collect();
            quickBarItem.Icon = Properties.Resources.book;
            justLoaded = true;
        }
        public void click(object sender, MouseEventArgs e) {
            quickBarItem.ContextMenuStrip = null;

            if (e.Button == MouseButtons.Left && ListL.Items.Count > 0) {
                quickBarItem.ContextMenuStrip = ListL;
                if (justLoaded || lastRect != mouseScreen.WorkingArea) {
                    justLoaded = false;
                    lastRect = mouseScreen.WorkingArea;
                    var menu = quickBarItem.ContextMenuStrip.Items;
                    for (int i = 0; i < menu.Count; i++) {
                        if (menu[i] is ToolStripMenuItem tsmi) {
                            menu.RemoveAt(i);
                            menu.Insert(i, Element.moreMenu(tsmi));
                        }
                    }
                }
                quickBarItem.Icon = Properties.Resources.bookOpen;
            } else if (e.Button == MouseButtons.Right) {
                quickBarItem.ContextMenuStrip = ListR;
                powerCheck.Checked = startupLaunch;
            }

            if (quickBarItem.ContextMenuStrip != null)
                _showMenu.Invoke(quickBarItem, null);
        }
        public static void itemClick(object sender, MouseEventArgs e) {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            quickBarItem.Icon = Properties.Resources.book;

            FileInfo info = (FileInfo)s.Tag;
            var url = info.extension == ".url";
            var final = info.finalPath;
            if (url || File.Exists(final) || Directory.Exists(final)) {
                if (e.Button != MouseButtons.Middle) {
                    closeFrom = 1;
#if !DEBUG
                    try {
#endif
                        ProcessStartInfo proc = !url && IsCUI(info.finalPath) ?
                            new ProcessStartInfo(@"cmd", $"/k title {info.name} & {(final.Contains(' ') ? $"\"{final}\"" : final)}{(string.IsNullOrEmpty(info.arguments) ? "" : $" {EscapeCMD(info.arguments)}")}") :
                            new ProcessStartInfo(final, info.arguments);
                        proc.WorkingDirectory = info.finalWorkDir;
                        if (e.Button == MouseButtons.Right)
                            proc.Verb = "runas";
                        Process.Start(proc);
#if !DEBUG
                    } catch (Exception ex) {
                        quickBarItem.ShowBalloonTip(8000, "Warning", $"Failed to start {info.name}:\n{ex.Message}\n\nPath: {final}", ToolTipIcon.Warning);
                    }
#endif
                } else
                    Process.Start("explorer.exe", $@"/select,{final}");
            } else
                quickBarItem.ShowBalloonTip(8000, "Error", $"Missing file!\n{final}", ToolTipIcon.Error);
        }
        public static void listClosing(object sender, ToolStripDropDownClosingEventArgs e) {
            quickBarItem.Icon = Properties.Resources.book;
            if (closeFrom == 2)
                e.Cancel = true;
            closeFrom = 0;
        }

        public void edit(object sender, EventArgs e) {
            if (!File.Exists(path)) {
                File.Create(path).Close();
                File.WriteAllText(path, jsonFile);
            }
            Process.Start(path);
        }
        public void showSample(object sender, EventArgs e) {
            File.WriteAllText(temp, defaultMenu);
            Process.Start(temp);
        }
        public void findIcon(object sender, EventArgs e) {
            fi = new FormIcon();
            fi.Show();
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