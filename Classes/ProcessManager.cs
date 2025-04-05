using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarZInjector.Classes
{
    public static class ProcessManager
    {
        public static Process GetTargetProcessFromExeName(string exeName)
        {
            if (exeName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                exeName = exeName[..^4];
            }
            var processes = Process.GetProcessesByName(exeName);
            return processes.FirstOrDefault(p => !p.HasExited && p.Responding)!;
        }

        public static bool IsProcessRunning(string processName)
        {
            if (processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                processName = processName[..^4];
            }
            return Process.GetProcessesByName(processName).Length > 0;
        }
    }
}
