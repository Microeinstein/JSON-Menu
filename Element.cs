using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static Menu.Functions;

namespace Menu {
    class Element {
        List<string> int_files;
        string int_text, int_icon, int_path, int_args, int_workdir, int_folder;
        int maxElem = 31;
        int depth = 0;

        public bool separator { get; set; }
        public bool showHiddenFiles { get; set; }
        public bool showHiddenFolders { get; set; }
        public bool showOnlyFiles { get; set; }
        public bool showOnlyFolders { get; set; }
        public bool sortByName { get; set; }
        public string text {
            get { return int_text; }
            set { int_text = Environment.ExpandEnvironmentVariables(value); }
        }
        public string icon {
            get { return int_icon; }
            set { int_icon = Environment.ExpandEnvironmentVariables(value); }
        }
        public string path {
            get { return int_path; }
            set { int_path = Environment.ExpandEnvironmentVariables(value); }
        }
        public string args {
            get { return int_args; }
            set { int_args = Environment.ExpandEnvironmentVariables(value); }
        }
        public string workDir {
            get { return int_workdir; }
            set { int_workdir = Environment.ExpandEnvironmentVariables(value); }
        }
        public string folder {
            get { return int_folder; }
            set { int_folder = Environment.ExpandEnvironmentVariables(value); }
        }
        public int maxDepth = -1;
        public List<string> mask {
            get { return int_files; }
            set {
                int_files = new List<string>();
                foreach (var s in value)
                    int_files.Add(Environment.ExpandEnvironmentVariables(s));
            }
        }
               List<string> noMask = new List<string>(new string[] { "*" });
        public List<Element> items = new List<Element>();

        public bool isText { get { return text != null && text != ""; } }
        public bool isIcon { get { return icon != null && icon != ""; } }
        public bool isPath { get { return path != null && path != ""; } }
        public bool isArgs { get { return args != null && args != ""; } }
        public bool isWorkdir { get { return workDir != null && workDir != ""; } }
        public bool isFolder { get { return folder != null && folder != ""; } }
        public bool isMask { get { return mask != null && mask.Count > 0; } }

        public ToolStripItem toMenuItem() {
            ToolStripItem it;

            if (isFolder) {                                                     //Automatic folder list
                ToolStripMenuItem item = new ToolStripMenuItem();
                string itemT = folder == Directory.GetDirectoryRoot(folder) ?
                               folder :
                               Path.GetFileName(folder);
                item.Text = isText ? text : itemT;
                item.Text = item.Text.Replace("&", "&&");
                item.Image = isIcon ? getCustomImage(path) : GetIcon(folder);
                item.Tag = new string[] { Path.GetFileName(folder), folder, "", Path.GetDirectoryName(folder) };
                item.MouseUp += Context.itemClick;

                addTree(folder, ref item);
                it = item;
            } else if (!separator) {                                            //Defined menu-item listing
                Image im = new Bitmap(1, 1);

                if (isIcon)                                                     //Icon
                    im = getCustomImage(icon);
                else if (isPath)
                    im = ExtractIcon(path);

                if (isPath) {                                                   //Execution
                    it = new ToolStripMenuItem(text, im);
                    it.MouseUp += Context.itemClick;
                    it.Tag = new string[] { "", path, args, workDir };
                } else {
                    it = new ToolStripMenuItem(text, im);
                    it.Tag = null;
                }
                if (items.Count > 0)
                    foreach (Element subi in items) { ((ToolStripMenuItem)it).DropDownItems.Add(subi.toMenuItem()); }

                it = moreMenu((ToolStripMenuItem)it);
            } else {                                                            //Separator
                it = new ToolStripSeparator();
                it.Tag = null;
            }
            return it;
        }
        void addTree(string folder, ref ToolStripMenuItem menu) {
            if (!showOnlyFiles)
                addFolders(folder, depth, ref menu);
            if (!showOnlyFolders)
                addFiles(folder, ref menu);
            menu = sortMenu(menu);
            menu = moreMenu(menu);
        }
        void addFolders(string folder, int depth, ref ToolStripMenuItem menu) {
            depth++;

            try {
                foreach (string subfolder in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly)) {
                    ToolStripMenuItem folderItem = new ToolStripMenuItem();

                    if (maxDepth == -1 || depth < maxDepth) {
                        addFolders(subfolder, depth, ref folderItem);
                        if (!showOnlyFolders)
                            addFiles(subfolder, ref folderItem);
                    }

                    if (showHiddenFolders || !isHidden(subfolder)) {
                        folderItem.Text = Path.GetFileName(subfolder).Replace("&", "&&");
                        folderItem.Image = GetIcon(subfolder);
                        folderItem.MouseUp += Context.itemClick;
                        folderItem.Name = "folder";
                        folderItem.Tag = new string[] { Path.GetFileName(subfolder), subfolder, "", Path.GetDirectoryName(subfolder) };

                        menu.DropDownItems.Add(folderItem);
                    }
                }
            } catch (Exception) { }

            depth--;
        }
        void addFiles(string folder, ref ToolStripMenuItem menu) {
            foreach (string type in isMask ? mask : noMask) {
                foreach (string file in Directory.GetFiles(folder, type)) {
                    if (showHiddenFiles || !isHidden(file)) {
                        ToolStripMenuItem fileItem = new ToolStripMenuItem();
                        int special = 0;
                        fileItem.Image = getImage(file, out special);
                        fileItem.MouseUp += Context.itemClick;
                        fileItem.Name = "file";
                        string[] lnkinfo;

                        switch (Path.GetExtension(file)) {
                            case ".lnk":
                                fileItem.Text = Path.GetFileNameWithoutExtension(file).Replace("&", "&&");
                                lnkinfo = GetShortcutInfo(file);
                                if (special == 1)
                                    lnkinfo[1] = Path.Combine(lnkinfo[3], Path.GetFileName(lnkinfo[1]));
                                if (special == 2)
                                    lnkinfo[1] = special2(lnkinfo[1]);
                                fileItem.Tag = lnkinfo;
                                break;
                            case ".url":
                                fileItem.Text = Path.GetFileNameWithoutExtension(file).Replace("&", "&&");
                                lnkinfo = GetUrlInfo(file);
                                fileItem.Tag = lnkinfo;
                                break;
                            default:
                                fileItem.Text = Path.GetFileName(file).Replace("&", "&&");
                                fileItem.Tag = new string[] { Path.GetFileName(file), file, "", Path.GetDirectoryName(file) };
                                break;
                        }

                        
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
                    sList.Sort((x, y) => compareExt(x, y));                   //Sort files by extension
                    foreach (ToolStripItem item in sList) {             //Split each extension in a different key
                        string ext = Path.GetExtension(((string[])item.Tag)[0]);
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
                } else {
                    sList.Sort((x, y) => compareName(x, y));
                }
            }

            return sList;
        }
        ToolStripMenuItem moreMenu(ToolStripMenuItem menu) {
            if (menu.HasDropDownItems) {
                var menuList = new List<ToolStripItem>();
                foreach (ToolStripItem item in menu.DropDownItems) {
                    if (item is ToolStripMenuItem)
                        menuList.Add(moreMenu((ToolStripMenuItem)item));
                    else
                        menuList.Add(item);
                }

                var listes = new List<List<ToolStripItem>>();
                while (menuList.Count > maxElem) {
                    var newList = new List<ToolStripItem>();
                    newList.AddRange(menuList.GetRange(0, maxElem));
                    listes.Add(newList);
                    menuList.RemoveRange(0, maxElem);
                }
                var lastList = new List<ToolStripItem>();
                lastList.AddRange(menuList);
                listes.Add(lastList);
                menuList.Clear();

                listes.Reverse();
                foreach (List<ToolStripItem> iterList in listes.GetRange(1, listes.Count - 1)) {
                    var mo = more();
                    var prev = listes.IndexOf(iterList) - 1;

                    ((ToolStripMenuItem)mo[1]).DropDownItems.AddRange(listes[prev].ToArray());
                    iterList.AddRange(mo);
                }
                listes.Reverse();

                menu.DropDownItems.Clear();
                menu.DropDownItems.AddRange(listes[0].ToArray());
            }

            return menu;
        }
        ToolStripItem[] more() {
            return new ToolStripItem[] {
                new ToolStripSeparator(),
                new ToolStripMenuItem("More...", null) };
        }

        int compareExt(ToolStripItem x, ToolStripItem y) {
            string ex = Path.GetExtension(((string[])x.Tag)[0]),
                   ey = Path.GetExtension(((string[])y.Tag)[0]);
            return ex.CompareTo(ey);
        }
        int compareName(ToolStripItem x, ToolStripItem y) {
            string ex = Path.GetFileNameWithoutExtension(((string[])x.Tag)[0]),
                   ey = Path.GetFileNameWithoutExtension(((string[])y.Tag)[0]);
            return ex.CompareTo(ey);
        }

        public static implicit operator Element(string a) {
            return a == "separator" ? new Element() { separator = true } : null;
        }
    }
}
