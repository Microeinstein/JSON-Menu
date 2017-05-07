using Microsoft.Win32;
using Shell32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Micro.Menu.uKernel32;
using static Micro.Menu.uOthers;
using static Micro.Menu.uShell32;
using static Micro.Menu.uUser32;

namespace Micro.Menu {
    public static class Core {
        public const int toolStripItemHeight = 23;
        public const string regLocation = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                            regName = "JSONMenu",
                            startupArg = "toggleStartup";
        public static readonly string programfiles = Environment.GetEnvironmentVariable("programfiles"),
                                      programw6432 = Environment.GetEnvironmentVariable("programw6432"),
                                      execPath = $"\"{Application.ExecutablePath.Replace("\"", "")}\"",
                                      defaultMenu = Encoding.Default.GetString(Properties.Resources.defaultMenu);
        public static readonly bool is64Bit = Environment.Is64BitOperatingSystem,
                                    isWoW = is64Bit && !Environment.Is64BitProcess,
                                    isElevated = WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);
        public static Shell shell = new Shell();
        public static Regex rIconSplit = new Regex(@"^((""?)[^""]+?\2)(?:,(-?\d+))?$");
        public static Screen mouseScreen => Screen.FromPoint(Control.MousePosition);
        public static RegistryKey regUserRun = Registry.CurrentUser.OpenSubKey(regLocation, isElevated);
        public static int maxElements => mouseScreen.WorkingArea.Height / toolStripItemHeight;
        public static bool startupLaunch {
            get => (string)regUserRun.GetValue(regName, "") == execPath;
            set {
                if (isElevated) {
                    if (value)
                        regUserRun.SetValue(regName, execPath, RegistryValueKind.String);
                    else
                        regUserRun.DeleteValue(regName, false);
                } else
                    try { Process.Start(new ProcessStartInfo(execPath, startupArg) { Verb = "runas" }); } catch { }
            }
        }
        public enum IconSize {
            Large = 0,
            Small = 1
        }

        public static bool isHidden(string path) => File.GetAttributes(path).HasFlag(FileAttributes.Hidden);
        public static bool isDirectory(string path) => File.GetAttributes(path).HasFlag(FileAttributes.Directory);

        ///<summary>Call ExtractImage by splitting <paramref name="icon"/> by comma</summary>
        public static Image getCustomImage(string icon) {
            var grps = rIconSplit.Match(icon).Groups;
            var path = grps[1].Value;

            if (!string.IsNullOrEmpty(grps[3].Value))
                return ExtractImage(path, int.Parse(grps[3].Value));
            else
                return ExtractImage(path);
        }
        public static Image GetIcon(string path, IconSize size = IconSize.Small, bool linkOverlay = false) {
            Image img = null;
            try {
                uint flags = SHGFI_ICON
                                | (linkOverlay ? SHGFI_LINKOVERLAY : 0)
                                | (IconSize.Small == size ? SHGFI_SMALLICON : SHGFI_LARGEICON);
                SHFILEINFO ShFI = new SHFILEINFO();
                SHGetFileInfo(path, 0, ref ShFI, (uint)Marshal.SizeOf(ShFI), flags);
                img = ((Icon)Icon.FromHandle(ShFI.hIcon).Clone()).ToBitmap();
                DestroyIcon(ShFI.hIcon);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return img;
        }
        public static Image ExtractImage(string file, int num = 0, bool largeIcon = false) {
            Image img = null;
            Icon[] ico = ExtractIcons(file, out IntPtr[] ptr, num);
            img = (largeIcon ? null : ico[0]?.ToBitmap())
               ?? (largeIcon ? ico[1]?.ToBitmap() : null);
            DestroyIcon(ptr[0]);
            DestroyIcon(ptr[1]);
            return img;
        }
        public static Icon ExtractIcon(string file, int num = 0, bool largeIcon = false) {
            Icon ic = null;
            Icon[] ico = ExtractIcons(file, out IntPtr[] ptr, num);
            ic = (largeIcon ? ico[1] : ico[0])?.Clone() as Icon;
            DestroyIcon(ptr[0]);
            DestroyIcon(ptr[1]);
            return ic;
        }
        public static Icon[] ExtractIcons(string file, int num = 0) {
            Icon[] ico = ExtractIcons(file, out IntPtr[] ptr, num);
            ico[0] = ico[0]?.Clone() as Icon;
            ico[1] = ico[1]?.Clone() as Icon;
            DestroyIcon(ptr[0]);
            DestroyIcon(ptr[1]);
            return ico;
        }
        static Icon[] ExtractIcons(string file, out IntPtr[] ptr, int num = 0) {
            IntPtr large = IntPtr.Zero,
                   small = IntPtr.Zero;
            Icon icoS = null,
                 icoL = null;

            if (num == -1) {
                int count = 0;
                Exception exx = null;

                while (count < 5 || (icoS != null && icoL != null)) {
                    ExtractIconEx(file, num, out large, out small, 1);
                    try {
                        icoS = Icon.FromHandle(small);
                        icoL = Icon.FromHandle(large);
                    } catch (Exception ex) {
                        icoS = icoL = null;
                        exx = ex;
                    }
                    count++;
                }

                if (count == 5 && exx != null && (icoS == null || icoL == null))
                    throw exx;
            } else {
                ExtractIconEx(file, num, out large, out small, 1);
                try {
                    icoS = Icon.FromHandle(small);
                    icoL = Icon.FromHandle(large);
                } catch (Exception ex) {
                    icoS = icoL = null;
                    throw ex;
                }
            }

            ptr = new[] { small, large };
            return new[] { icoS, icoL };
        }
        
        public static string getAbsolutePath(string root, string relative) {
            if (!Path.IsPathRooted(relative))
                return Path.Combine(root, relative);
            else
                return relative;
        }
        public static string GetFileTypeDescription(string fileNameOrExtension) {
            SHFILEINFO ShFI = new SHFILEINFO();
            if (SHGetFileInfo(
                    fileNameOrExtension,
                    FILE_ATTRIBUTE_NORMAL,
                    ref ShFI,
                    (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                    SHGFI_USEFILEATTRIBUTES | SHGFI_TYPENAME) != IntPtr.Zero)
                return ShFI.szTypeName;
            return null;
        }
        public static string IniReadValue(string path, string Section, string Key) {
            StringBuilder temp = new StringBuilder(65535);
            GetPrivateProfileString(Section, Key, "", temp, 65535, path);
            return temp.ToString();
        }
        public static bool IsCUI(string path) {
            if (!File.Exists(path) || Path.GetExtension(path) != ".exe")
                return false;
            IMAGE_NT_HEADERS? headers = null;
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read)) {
                try {
                    var map = CreateFileMapping(file.SafeFileHandle.DangerousGetHandle(), IntPtr.Zero, FILE_PAGE_READONLY, 0, 0, null);
                    var remap = MapViewOfFile(map, FILE_MAP_READ, 0, 0, 0);
                    headers = (IMAGE_NT_HEADERS)Marshal.PtrToStructure(ImageNtHeader(remap), typeof(IMAGE_NT_HEADERS));
                } catch { }
                file.Close();
            }
            return headers != null && headers?.OptionalHeader.Subsystem == IMAGE_SUBSYSTEM.WINDOWS_CUI;
        }
        public static string EscapeCMD(string cmd) {
            return cmd
                .Replace("^", "^^")
                .Replace("\"", "\"\"")
                .Replace("\\", "^\\")
                .Replace("&", "^&")
                .Replace("|", "^|")
                .Replace(">", "^>")
                .Replace("<", "^<");
        }
    }

    public static class uShell32 {
        #region Shell32 Constants
        public const int MAX_PATH = 256;
        public const uint BIF_RETURNONLYFSDIRS   = 0x0001,
                          BIF_DONTGOBELOWDOMAIN  = 0x0002,
                          BIF_STATUSTEXT         = 0x0004,
                          BIF_RETURNFSANCESTORS  = 0x0008,
                          BIF_EDITBOX            = 0x0010,
                          BIF_VALIDATE           = 0x0020,
                          BIF_NEWDIALOGSTYLE     = 0x0040,
                          BIF_USENEWUI           = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX),
                          BIF_BROWSEINCLUDEURLS  = 0x0080,
                          BIF_BROWSEFORCOMPUTER  = 0x1000,
                          BIF_BROWSEFORPRINTER   = 0x2000,
                          BIF_BROWSEINCLUDEFILES = 0x4000,
                          BIF_SHAREABLE          = 0x8000,
                          SHGFI_LARGEICON         = 0x00000,  // get large icon
                          SHGFI_SMALLICON         = 0x00001,  // get small icon
                          SHGFI_OPENICON          = 0x00002,  // get open icon
                          SHGFI_SHELLICONSIZE     = 0x00004,  // get shell size icon
                          SHGFI_PIDL              = 0x00008,  // pszPath is a pidl
                          SHGFI_USEFILEATTRIBUTES = 0x00010,  // use passed dwFileAttribute
                          SHGFI_ADDOVERLAYS       = 0x00020,  // apply the appropriate overlays
                          SHGFI_OVERLAYINDEX      = 0x00040,  // Get the index of the overlay
                          SHGFI_UNKNOWN_ONE       = 0x00080,
                          SHGFI_ICON              = 0x00100,  // get icon
                          SHGFI_DISPLAYNAME       = 0x00200,  // get display name
                          SHGFI_TYPENAME          = 0x00400,  // get type name
                          SHGFI_ATTRIBUTES        = 0x00800,  // get attributes
                          SHGFI_ICONLOCATION      = 0x01000,  // get icon location
                          SHGFI_EXETYPE           = 0x02000,  // return exe type
                          SHGFI_SYSICONINDEX      = 0x04000,  // get system icon index
                          SHGFI_LINKOVERLAY       = 0x08000,  // put a link overlay on icon
                          SHGFI_SELECTED          = 0x10000,  // show icon in selected state
                          SHGFI_ATTR_SPECIFIED    = 0x20000,  // get only specified attributes
                          SHGFI_UNKNOWN_TWO       = 0x40000,
                          SHGFI_UNKNOWN_THREE     = 0x80000,
                          FILE_ATTRIBUTE_DIRECTORY = 0x00010,
                          FILE_ATTRIBUTE_NORMAL    = 0x00080;
        #endregion

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
    public static class uKernel32 {
        public static uint FILE_PAGE_EXECUTE_READ      = 0x20,
                           FILE_PAGE_EXECUTE_READWRITE = 0x40,
                           FILE_PAGE_EXECUTE_WRITECOPY = 0x80,
                           FILE_PAGE_READONLY          = 0x02,
                           FILE_PAGE_READWRITE         = 0x04,
                           FILE_PAGE_WRITECOPY         = 0x08,
                           STANDARD_RIGHTS_REQUIRED   = 0xF0000,
                           SECTION_QUERY                = 0x01,
                           SECTION_MAP_WRITE            = 0x02,
                           SECTION_MAP_READ             = 0x04,
                           SECTION_MAP_EXECUTE          = 0x08,
                           SECTION_EXTEND_SIZE          = 0x10,
                           SECTION_MAP_EXECUTE_EXPLICIT = 0x20,
                           SECTION_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED
                                              | SECTION_QUERY
                                              | SECTION_MAP_WRITE
                                              | SECTION_MAP_READ
                                              | SECTION_MAP_EXECUTE
                                              | SECTION_EXTEND_SIZE,
                           FILE_MAP_ALL_ACCESS = SECTION_ALL_ACCESS,
                           FILE_MAP_READ       = SECTION_MAP_READ,
                           FILE_MAP_WRITE      = SECTION_MAP_WRITE,
                           FILE_MAP_COPY       = 0x01;

        [DllImport("kernel32.dll")]
        public static extern long WritePrivateProfileString(
            string section,
            string key,
            string val,
            string filePath);

        [DllImport("kernel32.dll")]
        public static extern uint GetPrivateProfileString(
            string section,
            string key,
            string def,
            StringBuilder retVal,
            uint size,
            string filePath);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateFileMapping(
            IntPtr hFile,
            IntPtr lpAttributes,
            uint flProtect,
            uint dwMaximumSizeHigh,
            uint dwMaximumSizeLow,
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject,
            uint dwDesiredAccess,
            uint dwFileOffsetHigh,
            uint dwFileOffsetLow,
            uint dwNumberOfBytesToMap);

    }
    public static class uUser32 {
        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
    }
    public static class uOthers {
        public enum IMAGE_SUBSYSTEM : short {
            UNKNOWN = 0,
            NATIVE = 1,
            WINDOWS_GUI = 2,
            WINDOWS_CUI = 3,
            OS2_CUI = 5,
            POSIX_CUI = 7,
            WINDOWS_CE_GUI = 9,
            EFI_APPLICATION = 10,
            EFI_BOOT_SERVICE_DRIVER = 11,
            EFI_RUNTIME_DRIVER = 12,
            EFI_ROM = 13,
            XBOX = 14,
            WINDOWS_BOOT_APPLICATION = 16
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_FILE_HEADER {
            public short Machine;
            public short NumberOfSections;
            public int TimeDateStamp;
            public int PointerToSymbolTable;
            public int NumberOfSymbols;
            public short SizeOfOptionalHeader;
            public short Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_OPTIONAL_HEADER {
            public short Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public int SizeOfCode;
            public int SizeOfInitializedData;
            public int SizeOfUninitializedData;
            public int AddressOfEntryPoint;
            public int BaseOfCode;
            public int BaseOfData;
            public int ImageBase;
            public int SectionAlignment;
            public int FileAlignment;
            public short MajorOperatingSystemVersion;
            public short MinorOperatingSystemVersion;
            public short MajorImageVersion;
            public short MinorImageVersion;
            public short MajorSubsystemVersion;
            public short MinorSubsystemVersion;
            public int Win32VersionValue;
            public int SizeOfImage;
            public int SizeOfHeaders;
            public int CheckSum;
            [MarshalAs(UnmanagedType.I2)]
            public IMAGE_SUBSYSTEM Subsystem;
            public short DllCharacteristics;
            public int SizeOfStackReserve;
            public int SizeOfStackCommit;
            public int SizeOfHeapReserve;
            public int SizeOfHeapCommit;
            public int LoaderFlags;
            public int NumberOfRvaAndSizes;
            public IntPtr DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_NT_HEADERS {
            public int Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER OptionalHeader;
        }

        [DllImport("Dbghelp.dll")]
        public static extern IntPtr ImageNtHeader(IntPtr ImageBase);
    }
}