using Shell32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Menu.uKernel32;
using static Menu.uShell32;
using static Menu.uUser32;

namespace Menu {
    public static class Functions {
        public enum IconSize {
            Large = 0,
            Small = 1
        }
        public enum FolderType {
            Open = 0,
            Closed = 1
        }

        public static string special2(string path) {
            bool cond1 = path.StartsWith(@"C:\Program Files (x86)"),
                 cond2 = path.StartsWith(@"C:\Program Files");
            string respa =
                cond1 ? path.Replace(@"C:\Program Files (x86)", @"C:\Program Files") :
                cond2 ? path.Replace(@"C:\Program Files", @"C:\Program Files (x86)") :
                        "";
            return respa;
        }
        public static bool isHidden(string path) {
            return File.GetAttributes(path).HasFlag(FileAttributes.Hidden);
        }
        public static Image getCustomImage(string icon) {
            string[] ip = icon.Split(',');

            if (ip.Length == 2)
                return ExtractIcon(ip[0], int.Parse(ip[1]));
            else
                return ExtractIcon(ip[0]);
        }
        public static Image getImage(string path, out int special) {
            special = 0;
            string resolv = "";
            string[] ic;
            int num = 0;
            Image ret = null;

            switch (Path.GetExtension(path)) {
                case ".lnk":
                    ic = GetShortcutInfo(path);
                    resolv = ic[5];
                    num = int.Parse(ic[6]);
                    try {
                        if (resolv != "")
                            ret = ExtractIcon(resolv, num);
                        else {
                            resolv = ic[1];
                            if (File.GetAttributes(resolv).HasFlag(FileAttributes.Directory))
                                ret = GetIcon(resolv);
                            else
                                ret = ExtractIcon(resolv);
                        }
                    } catch (Exception) { }

                    if (Path.GetExtension(resolv) == ".exe" && ret == null) {
                        try {
                            ret = ExtractIcon(Path.Combine(ic[3], Path.GetFileName(ic[1])));
                            special = 1;
                        } catch (Exception) {
                            try {
                                var sp2 = special2(ic[1]);
                                if (sp2 != "") {
                                    ret = ExtractIcon(sp2);
                                    special = 2;
                                }
                            } catch (Exception) { }
                        }
                    }
                    if (Path.GetExtension(resolv) == ".exe" && ret == null) {
                        ret = ExtractIcon(@"%systemroot%\system32\imageres.dll", 11);
                    }
                    break;
                case ".url":
                    ic = GetUrlInfo(path);
                    resolv = ic[5];
                    num = int.Parse(ic[6]);
                    try {
                        if (resolv != "")
                            ret = ExtractIcon(resolv, num);
                        else {
                            resolv = ic[1];
                            ret = ExtractIcon(resolv);
                        }
                    } catch (Exception) { }
                    break;
                default:
                    ret = GetIcon(path);
                    break;
            }

            return ret;
        }

        public static Image GetIcon(string path, IconSize size = IconSize.Small, bool linkOverlay = false) {
            Image img = null;
            try {
                uint flags = SHGFI_ICON;
                if (linkOverlay) flags |= SHGFI_LINKOVERLAY;
                if (IconSize.Small == size) { flags |= SHGFI_SMALLICON; }
                else { flags |= SHGFI_LARGEICON; }
                SHFILEINFO shfi = new SHFILEINFO();

                SHGetFileInfo(path, 0, ref shfi, (uint)Marshal.SizeOf(shfi), flags);
                Icon ico = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
                img = ico.ToBitmap();
                DestroyIcon(shfi.hIcon);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return img;
        }
        public static Image ExtractIcon(string file, int num = 0, bool largeIcon = false) {
            IntPtr large = (IntPtr)0,
                   small = (IntPtr)0;
            Icon i = null;
            Image img = null;

            if (num == -1) {
                int count = 0;
                Exception exx = null;

                while (count < 5 || i != null) {
                    ExtractIconEx(file, num, out large, out small, 1);
                    try { i = Icon.FromHandle(largeIcon ? large : small); }
                    catch (Exception ex) { i = null; exx = ex; }
                    count++;
                }

                if (count == 5 && exx != null && i == null)
                    throw exx;
            } else {
                ExtractIconEx(file, num, out large, out small, 1);
                try { i = Icon.FromHandle(largeIcon ? large : small); }
                catch (Exception ex) { i = null; throw ex; }
            }

            if (i != null) {
                img = i.ToBitmap();
                DestroyIcon(largeIcon ? large : small);
            }
            return img;
        }

        /// <summary>
        /// Returns [Input string, Path, Arguments, Working Directory, Description, Icon location, Icon index]
        /// </summary>
        public static string[] GetShortcutInfo(string link) {
            try {
                Shell32.Shell shell = new Shell32.Shell();

                string lnkPath = link.Substring(0, link.LastIndexOf("\\"));
                string lnkName = link.Substring(link.LastIndexOf("\\") + 1);
                if (!lnkName.EndsWith(".lnk"))
                    lnkName += ".lnk";

                Folder lnkFolder = shell.NameSpace(lnkPath);
                FolderItem lnkItem = lnkFolder.Items().Item(lnkName);

                if (lnkItem == null || !lnkItem.IsLink)
                    return new string[] { };
                
                ShellLinkObject lnk = (ShellLinkObject)lnkItem.GetLink;
                string loc = "";
                int num;
                num = lnk.GetIconLocation(out loc);
                return new string[] { link /*lnkItem.Name*/, lnk.Path, lnk.Arguments, lnk.WorkingDirectory, lnk.Description, loc, num.ToString() };
            }
            catch (Exception ex) {
                return new string[] { ex.Message };
            }
        }
        public static string GetFileTypeDescription(string fileNameOrExtension) {
            SHFILEINFO shfi = new SHFILEINFO();
            if (IntPtr.Zero != SHGetFileInfo(
                                fileNameOrExtension,
                                FILE_ATTRIBUTE_NORMAL,
                                ref shfi,
                                (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                                SHGFI_USEFILEATTRIBUTES | SHGFI_TYPENAME)) {
                return shfi.szTypeName;
            }
            return null;
        }
        public static string[] GetUrlInfo(string url) {
            string sec = "InternetShortcut";
            string path = IniReadValue(url, sec, "URL"),
                   icon = IniReadValue(url, sec, "IconFile"),
                   icon_ind = IniReadValue(url, sec, "IconIndex");
            return new string[] { url, path, "", "", "", icon, icon_ind };
        }
        public static string IniReadValue(string path, string Section, string Key) {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, path);
            return temp.ToString();
        }
    }

    public class uShell32 {

        public const int MAX_PATH = 256;
        [StructLayout(LayoutKind.Sequential)]
        public struct SHITEMID {
            public ushort cb;
            [MarshalAs(UnmanagedType.LPArray)]
            public byte[] abID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ITEMIDLIST {
            public SHITEMID mkid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BROWSEINFO {
            public IntPtr hwndOwner;
            public IntPtr pidlRoot;
            public IntPtr pszDisplayName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszTitle;
            public uint ulFlags;
            public IntPtr lpfn;
            public int lParam;
            public IntPtr iImage;
        }
        
        public const uint BIF_RETURNONLYFSDIRS      = 0x0001;
        public const uint BIF_DONTGOBELOWDOMAIN     = 0x0002;
        public const uint BIF_STATUSTEXT            = 0x0004;
        public const uint BIF_RETURNFSANCESTORS     = 0x0008;
        public const uint BIF_EDITBOX               = 0x0010;
        public const uint BIF_VALIDATE              = 0x0020;
        public const uint BIF_NEWDIALOGSTYLE        = 0x0040;
        public const uint BIF_USENEWUI              = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
        public const uint BIF_BROWSEINCLUDEURLS     = 0x0080;
        public const uint BIF_BROWSEFORCOMPUTER     = 0x1000;
        public const uint BIF_BROWSEFORPRINTER      = 0x2000;
        public const uint BIF_BROWSEINCLUDEFILES    = 0x4000;
        public const uint BIF_SHAREABLE             = 0x8000;

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO {
            public const int NAMESIZE = 80;
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAMESIZE)]
            public string szTypeName;
        };

        public const uint SHGFI_LARGEICON           = 0x00000;  // get large icon
        public const uint SHGFI_SMALLICON           = 0x00001;  // get small icon
        public const uint SHGFI_OPENICON            = 0x00002;  // get open icon
        public const uint SHGFI_SHELLICONSIZE       = 0x00004;  // get shell size icon
        public const uint SHGFI_PIDL                = 0x00008;  // pszPath is a pidl
        public const uint SHGFI_USEFILEATTRIBUTES   = 0x00010;  // use passed dwFileAttribute
        public const uint SHGFI_ADDOVERLAYS         = 0x00020;  // apply the appropriate overlays
        public const uint SHGFI_OVERLAYINDEX        = 0x00040;  // Get the index of the overlay
        public const uint SHGFI_UNKNOWN_ONE         = 0x00080;  //
        public const uint SHGFI_ICON                = 0x00100;  // get icon
        public const uint SHGFI_DISPLAYNAME         = 0x00200;  // get display name
        public const uint SHGFI_TYPENAME            = 0x00400;  // get type name
        public const uint SHGFI_ATTRIBUTES          = 0x00800;  // get attributes
        public const uint SHGFI_ICONLOCATION        = 0x01000;  // get icon location
        public const uint SHGFI_EXETYPE             = 0x02000;  // return exe type
        public const uint SHGFI_SYSICONINDEX        = 0x04000;  // get system icon index
        public const uint SHGFI_LINKOVERLAY         = 0x08000;  // put a link overlay on icon
        public const uint SHGFI_SELECTED            = 0x10000;  // show icon in selected state
        public const uint SHGFI_ATTR_SPECIFIED      = 0x20000;  // get only specified attributes
        public const uint SHGFI_UNKNOWN_TWO         = 0x40000;  //
        public const uint SHGFI_UNKNOWN_THREE       = 0x80000;  //

        public const uint FILE_ATTRIBUTE_DIRECTORY  = 0x00010;
        public const uint FILE_ATTRIBUTE_NORMAL     = 0x00080;

        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags);

        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int ExtractIconEx(
            string sFile,
            int iIndex,
            out IntPtr piLargeVersion,
            out IntPtr piSmallVersion,
            int amountIcons);

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIcon(
            IntPtr hInst,
            string lpszExeFileName,
            int nIconIndex);
    }
    public class uKernel32 {
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
    public class uUser32 {
        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
    }
}