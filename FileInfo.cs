using Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Micro.Menu.Core;

namespace Micro.Menu {
    public class FileInfo {
        public const string internalReq = "InternetShortcut";

        public string description {
            get {
                if (!string.IsNullOrWhiteSpace(_desc))
                    return _desc;
                else {
                    var p = isShortcut && extension == ".lnk" ?
                                destination :
                                extension == ".exe" ?
                                    path : null;
                    if (p == null || !File.Exists(p))
                        return null;
                    var fvi = FileVersionInfo.GetVersionInfo(p);
                    return string.IsNullOrWhiteSpace(fvi.FileDescription) ?
                               string.IsNullOrWhiteSpace(fvi.ProductName) ?
                                   null :
                                   fvi.ProductName :
                               fvi.FileDescription;
                }
            }
        }
        public string finalPath => isShortcut ? destination : path;
        public string parentDir => Directory.GetParent(finalPath).FullName;
        public string finalWorkDir => string.IsNullOrWhiteSpace(workingDir) ? parentDir : workingDir;
        public bool hasIcon => icon != null;

        public string name, path, extension, destination, destExt, arguments, workingDir, iconPath;
        public int iconIndex;
        public bool isShortcut, isDirectory;
        public Image icon;
        string _desc;
        
        public FileInfo(string filePath, string args = null, string workDir = null, string name = null, Image icon = null) {
            if (!File.Exists(filePath) && !(isDirectory = Directory.Exists(filePath)))
                throw new FileNotFoundException();
            path = filePath;
            extension = Path.GetExtension(filePath).ToLower();
            arguments = args;
            workingDir = workDir;
            resolveLink();
            this.name = name ?? Path.GetFileNameWithoutExtension(filePath).Replace("&", "&&");
            this.icon = icon ?? getImage();
        }
        void resolveLink() {
            if (!isDirectory) {
                isShortcut = (extension == ".lnk" || extension == ".url");
                switch (extension) {
                    case ".lnk":
                        string lnkPath = path.Substring(0, path.LastIndexOf(@"\"));
                        string lnkName = path.Substring(path.LastIndexOf(@"\") + 1);
                        if (!lnkName.EndsWith(".lnk"))
                            lnkName += ".lnk";

                        Folder lnkFolder = shell.NameSpace(lnkPath);
                        FolderItem lnkItem = lnkFolder.Items().Item(lnkName);
                        if (lnkItem == null || !lnkItem.IsLink)
                            return;

                        try {
                            ShellLinkObject lnk = (ShellLinkObject)lnkItem.GetLink;
                            destination = HandleWoW(lnk.Path);
                            destExt = Path.GetExtension(destination).ToLower();
                            arguments = arguments ?? lnk.Arguments;
                            workingDir = workingDir ?? lnk.WorkingDirectory;
                            try { _desc = lnk.Description; } catch { }
                            iconIndex = lnk.GetIconLocation(out iconPath);
                        } catch {
                            isShortcut = false;
                        }
                        break;

                    case ".url":
                        destination = IniReadValue(path, internalReq, "URL");
                        iconPath = IniReadValue(path, internalReq, "IconFile");
                        int.TryParse(IniReadValue(path, internalReq, "IconIndex"), out iconIndex);
                        break;
                }
            }
        }
        Image getImage() {
            for (int way = 1; way <= 5; way++) {
                switch (way) {
                    case 1 when !string.IsNullOrWhiteSpace(iconPath):
                        try { return ExtractImage(iconPath, iconIndex); } catch { }
                        break;
                    case 2 when (extension == ".url" || extension == ".lnk") && destExt == ".exe":
                        try { return ExtractImage(destination); } catch {
                            if (extension != ".url")
                                return ExtractImage(@"%systemroot%\system32\imageres.dll", 11);
                        }
                        break;
                    case 4:
                        if (isDirectory(path))
                            return GetIcon(path);
                        else
                            try { return ExtractImage(path); } catch { }
                        break;
                    case 5:
                        return GetIcon(path);
                }
            }
            return null;
        }

        public static string HandleWoW(string path) {
            string np;
            if (isWoW && path.StartsWith(programfiles) && !File.Exists(path))
                if (File.Exists(np = path.Replace(programfiles, programw6432)))
                    return np;
            return path;
        }
    }
}
