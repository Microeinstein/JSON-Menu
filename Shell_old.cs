using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Menu {
    class Shell {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1;    // 'Small icon

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public static Image ExtractIcon(string file, int number = 0, bool largeIcon = false) {
            IntPtr large;
            IntPtr small;
            Icon i = null;
            ExtractIconEx(file, number, out large, out small, 1);
            try {
                i = Icon.FromHandle(largeIcon ? large : small);
            } catch {
                i = null;
            }
            return i != null ? i.ToBitmap() : null;
        }
        public static IntPtr getResource(String path, uint type) {
            SHFILEINFO shinfo = new SHFILEINFO();
            return SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), type);
        }
    }
}
