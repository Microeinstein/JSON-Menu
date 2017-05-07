using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static Micro.Menu.Core;

namespace Micro.Menu {
    class Element {
        List<string> int_files;
        string _text, _icon, _path, _args, _workdir, _nirPack;
        int depth = 0;

        public bool separator { get; set; }
        public bool showHiddenFiles { get; set; }
        public bool showHiddenFolders { get; set; }
        public bool showOnlyFiles { get; set; }
        public bool showOnlyFolders { get; set; }
        public bool sortByName { get; set; }
        public string text {
            get => _text;
            set => _text = Environment.ExpandEnvironmentVariables(value);
        }
        public string icon {
            get => _icon;
            set => _icon = Environment.ExpandEnvironmentVariables(value);
        }
        public string path {
            get => _path;
            set => _path = Environment.ExpandEnvironmentVariables(value);
        }
        public string args {
            get => _args;
            set => _args = Environment.ExpandEnvironmentVariables(value);
        }
        public string workDir {
            get => _workdir;
            set => _workdir = Environment.ExpandEnvironmentVariables(value);
        }
        public string nirPack {
            get => _nirPack;
            set => _nirPack = Environment.ExpandEnvironmentVariables(value);
        }
        public int maxDepth = -1;
        public List<string> mask {
            get => int_files;
            set {
                int_files = new List<string>();
                foreach (var s in value)
                    int_files.Add(Environment.ExpandEnvironmentVariables(s));
            }
        }
               List<string> noMask = new List<string>(new string[] { "*" });
        public List<Element> items = new List<Element>();

        public bool isText => !string.IsNullOrEmpty(text);
        public bool isIcon => !string.IsNullOrEmpty(icon);
        public bool isPath => !string.IsNullOrEmpty(path);
        public bool isArgs => !string.IsNullOrEmpty(args);
        public bool isWorkdir => !string.IsNullOrEmpty(workDir);
        public bool isFolder => isPath && isDirectory(path);
        public bool isNirPack => !string.IsNullOrEmpty(nirPack);
        public bool isMask => mask != null && mask.Count > 0;

        public ToolStripItem toMenuItem() {
            ToolStripItem it;

            if (isNirPack)
                it = parseNirPack();
            else if (!separator)
                it = parseItem();
            else 
                it = new ToolStripSeparator();

            return it;
        }
        ToolStripMenuItem parseItem() {
            //If path is root assign name "C:\" otherwise "myFolder"
            string name = text ?? (path == Directory.GetDirectoryRoot(path) ? path : Path.GetFileName(path));
            Image image = isIcon ? getCustomImage(icon) : null;
            FileInfo fi = isPath ? new FileInfo(path, args, workDir, name, image) : null;
            var it = new ToolStripMenuItem(name, fi?.icon ?? image);
            if (!fi.nonExistant) {
                if (isPath) {
                    it.Tag = fi;
                    it.MouseUp += Context.itemClick;
                    it.ToolTipText = fi.description;
                }
                if (isFolder)
                    addTree(path, ref it);
                if (items.Count > 0) {
                    for (int e = 0; e < items.Count; e++) {
                        var sub = items[e];
                        it.DropDownItems.Insert(e, sub.toMenuItem());
                    }
                }
                if (isFolder && items.Count > 0)
                    it.DropDownItems.Insert(items.Count, new ToolStripSeparator());
            } else {
                it.Enabled = false;
                it.ToolTipText = "This path does not exist";
            }
            return it;
        }
        ToolStripMenuItem parseNirPack() {
            var np = new NirPack(nirPack);
            var it = new ToolStripMenuItem(np.Name, isIcon ? getCustomImage(icon) : Properties.Resources.toolbox.ToBitmap());
            foreach (var g in np.Groups) {
                var git = new ToolStripMenuItem(g.Name);
                foreach (var s in g) {
                    if (File.Exists(s.BestExe)) {
                        var fin = new FileInfo(s.BestExe);
                        var sit = new ToolStripMenuItem(s.Name, fin.icon);
                        sit.MouseUp += Context.itemClick;
                        sit.Tag = fin;
                        sit.ToolTipText = string.IsNullOrWhiteSpace(s.ShortDesc) ? fin.description : s.ShortDesc;
                        git.DropDownItems.Add(sit);
                    }
                }
                it.DropDownItems.Add(git);
            }
            return it;
        }

        void addTree(string folder, ref ToolStripMenuItem menu) {
            if (!showOnlyFiles)
                addFolders(folder, depth, ref menu);
            if (!showOnlyFolders)
                addFiles(folder, ref menu);
            menu = sortMenu(menu);
        }
        void addFolders(string folder, int depth, ref ToolStripMenuItem menu) {
            depth++;

            try {
                foreach (string subfolder in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly)) {
                    var folderItem = new ToolStripMenuItem();
                    if (maxDepth == -1 || depth < maxDepth) {
                        addFolders(subfolder, depth, ref folderItem);
                        if (!showOnlyFolders)
                            addFiles(subfolder, ref folderItem);
                    }
                    if (showHiddenFolders || !isHidden(subfolder)) {
                        var fi = new FileInfo(subfolder);
                        folderItem.Text = fi.name;
                        if (fi.hasIcon)
                            folderItem.Image = fi.icon;
                        folderItem.MouseUp += Context.itemClick;
                        folderItem.Name = "folder";
                        folderItem.ToolTipText = fi.description;
                        folderItem.Tag = fi;
                        menu.DropDownItems.Add(folderItem);
                    }
                }
            } catch { }

            depth--;
        }
        void addFiles(string folder, ref ToolStripMenuItem menu) {
            foreach (string type in isMask ? mask : noMask) {
                foreach (string file in Directory.GetFiles(folder, type)) {
                    if (showHiddenFiles || !isHidden(file)) {
                        var fileItem = new ToolStripMenuItem();
                        var fi = new FileInfo(file);
                        fileItem.MouseUp += Context.itemClick;
                        fileItem.Name = "file";
                        fileItem.Text = fi.name;
                        if (fi.hasIcon)
                            fileItem.Image = fi.icon;
                        fileItem.ToolTipText = fi.description;
                        fileItem.Tag = fi;
                        menu.DropDownItems.Add(fileItem);
                    }
                }
            }
        }

        ToolStripMenuItem sortMenu(ToolStripMenuItem menu) {
            if (menu.HasDropDownItems) {
                var menuCopy = new List<ToolStripItem>();
                foreach (ToolStripItem item in menu.DropDownItems)
                    menuCopy.Add(item);

                var menuList = new List<ToolStripItem>();
                menuList.AddRange(_sortMenu("folder", menuCopy));
                menuList.AddRange(_sortMenu("file", menuCopy));

                menu.DropDownItems.Clear();
                menu.DropDownItems.AddRange(menuList.ToArray());
            }

            return menu;
        }
        List<ToolStripItem> _sortMenu(string type, List<ToolStripItem> list) {
            var sList = new List<ToolStripItem>();
            foreach (ToolStripItem item in list.Where(x => x.Name == type)) {
                if (item is ToolStripMenuItem)
                    sList.Add(sortMenu((ToolStripMenuItem)item));
                else
                    sList.Add(item);
            }
            if (type == "file") {
                if (!sortByName) {
                    Dictionary<string, List<ToolStripItem>> filez = new Dictionary<string, List<ToolStripItem>>();
                    sList.Sort((x, y) => compareExt(x, y));             //Sort files by extension
                    foreach (ToolStripItem item in sList) {             //Split each extension in a different key
                        string ext = ((FileInfo)item.Tag).extension;
                        if (!filez.ContainsKey(ext))
                            filez.Add(ext, new List<ToolStripItem>());
                        filez[ext].Add(item);
                    }
                    foreach (List<ToolStripItem> nlist in filez.Values) {
                        nlist.Sort((x, y) => compareName(x, y));               //Sort files by name
                    }
                    sList.Clear();
                    foreach (var k in filez.Keys) {
                        sList.AddRange(filez[k]);
                    }
                } else
                    sList.Sort((x, y) => compareName(x, y));
            }

            return sList;
        }

        public static ToolStripMenuItem moreMenu(ToolStripMenuItem menu) {
            if (menu.HasDropDownItems) {
                var menuList = new List<ToolStripItem>();
                foreach (ToolStripItem item in menu.DropDownItems) {
                    if (item is ToolStripMenuItem)
                        menuList.Add(moreMenu((ToolStripMenuItem)item));
                    else
                        menuList.Add(item);
                }

                var listes = new List<List<ToolStripItem>>();
                var maxElem = maxElements;
                while (menuList.Count > maxElem) {
                    listes.Add(new List<ToolStripItem>(menuList.GetRange(0, maxElem)));
                    menuList.RemoveRange(0, maxElem);
                }
                listes.Add(new List<ToolStripItem>(menuList));
                menuList.Clear();

                listes.Reverse();
                foreach (var iterList in listes.Skip(1)) {
                    var next = listes.IndexOf(iterList) - 1;
                    var mo = newMore();
                    ((ToolStripMenuItem)mo[1]).DropDownItems.AddRange(listes[next].ToArray());
                    iterList.AddRange(mo);
                }
                listes.Reverse();

                menu.DropDownItems.Clear();
                menu.DropDownItems.AddRange(listes[0].ToArray());
            }

            return menu;
        }
        public static ToolStripItem[] newMore() {
            return new ToolStripItem[] {
                new ToolStripSeparator(),
                new ToolStripMenuItem("More...", null) };
        }

        public static int compareExt(ToolStripItem x, ToolStripItem y) {
            string ex = ((FileInfo)x.Tag).extension,
                   ey = ((FileInfo)y.Tag).extension;
            return ex.CompareTo(ey);
        }
        public static int compareName(ToolStripItem x, ToolStripItem y) {
            string ex = Path.GetFileNameWithoutExtension(((FileInfo)x.Tag).path),
                   ey = Path.GetFileNameWithoutExtension(((FileInfo)y.Tag).path);
            return ex.CompareTo(ey);
        }

        public static implicit operator Element(string a)
            => a == "separator" ? new Element() { separator = true } : null;
    }
}
