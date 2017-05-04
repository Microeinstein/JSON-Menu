using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Micro.Menu.Core;

namespace Micro.Menu {
    public class NirPack {
        const string general = "General";
        public string Name;
        public int SoftwareCount, GroupCount;
        public NirGroup[] Groups;

        public NirPack(string path) {
            SoftwareCount = int.Parse(IniReadValue(path, general, "SoftwareCount"));
            GroupCount = int.Parse(IniReadValue(path, general, "GroupCount"));
            Name = IniReadValue(path, general, "Name");
            Groups = new NirGroup[GroupCount];

            for (int g = 0; g < GroupCount; g++) {
                Groups[g] = new NirGroup(
                    IniReadValue(path, $"Group{g}", "name"),
                    IniReadValue(path, $"Group{g}", "ShowAll") == "1"
                );
            }
            for (int s = 0; s < SoftwareCount; s++) {
                var grp = Groups[int.Parse(IniReadValue(path, $"Software{s}", "group"))];
                var soft = new NirExe(
                    grp,
                    IniReadValue(path, $"Software{s}", "AppName"),
                    getAbsolutePath(Directory.GetParent(path).FullName, IniReadValue(path, $"Software{s}", "exe")),
                    getAbsolutePath(Directory.GetParent(path).FullName, IniReadValue(path, $"Software{s}", "exe64")),
                    IniReadValue(path, $"Software{s}", "shortDesc"),
                    "", //IniReadValue(path, $"Software{s}", "LongDesc"),
                    "", //IniReadValue(path, $"Software{s}", "url"),
                    ""  //getAbsolutePath(Directory.GetParent(path).FullName, IniReadValue(path, $"Software{s}", "help"))
                );
                if (string.IsNullOrWhiteSpace(soft.Name))
                    soft.Name = FileVersionInfo.GetVersionInfo(soft.BestExe).ProductName;
                grp.Add(soft);
            }
            foreach (var g1 in Groups) {
                if (g1.ShowAll) {
                    foreach (var g2 in Groups) {
                        if (g1 != g2)
                            g1.AddRange(g2);
                    }
                }
                g1.Sort((a, b) => a.Name.CompareTo(b.Name));
            }
        }
    }
    public class NirGroup : List<NirExe> {
        public string Name;
        public bool ShowAll;

        public NirGroup(string name, bool showAll) {
            Name = name;
            ShowAll = showAll;
        }
    }
    public class NirExe {
        public string BestExe => is64Bit && File.Exists(Exe64) ? Exe64 : Exe;
        public string Exe, Exe64, Name, ShortDesc, LongDesc, Help, Url;
        public NirGroup Group;

        public NirExe(NirGroup group, string appName, string exe, string exe64 = "", string shortDesc = "", string longDesc = "", string url = "", string help = "") {
            Group = group;
            Exe = exe;
            Exe64 = exe64;
            Name = appName;
            ShortDesc = shortDesc;
            LongDesc = longDesc;
            Url = url;
        }
    }
}
