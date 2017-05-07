using System;
using System.Windows.Forms;
using static Micro.Menu.Core;

namespace Micro.Menu {
    static class Program {
        [STAThread]
        static void Main() {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1] == startupArg) {
                startupLaunch = !startupLaunch;
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Context());
        }
    }
}
