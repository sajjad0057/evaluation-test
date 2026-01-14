using Serilog;
using System.Diagnostics;
using System.Runtime.InteropServices;


class Program
{
    private static int index = 0;

    static async Task Main()
    {
        string projectRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;

        string logsFolder = Path.Combine(projectRoot, "Logs");

        if (!Directory.Exists(logsFolder))
            Directory.CreateDirectory(logsFolder);

        string logFilePath = Path.Combine(logsFolder, "system-health.log");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                logFilePath,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss} | Index:{Index} | CPU:{Cpu}% | Memory:{Memory}MB{NewLine}"
            )
            .CreateLogger();

        Log.Information("System Health Monitor started");

        while (true)
        {
            index++;

            double cpuUsage = Math.Round(GetSystemCpuUsage(), 2);
            double memoryUsage = Math.Round(GetSystemMemoryUsageInMB(), 2);

            Log.ForContext("Index", index)
               .ForContext("Cpu", cpuUsage)
               .ForContext("Memory", memoryUsage)
               .Information("Metrics recorded");

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }

    private static double GetSystemCpuUsage()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Thread.Sleep(1000);
            return cpuCounter.NextValue();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var cpu1 = File.ReadAllLines("/proc/stat")[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse).ToArray();
            long total1 = cpu1.Sum();
            long idle1 = cpu1[3];

            Thread.Sleep(1000);

            var cpu2 = File.ReadAllLines("/proc/stat")[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse).ToArray();
            long total2 = cpu2.Sum();
            long idle2 = cpu2[3];

            return (total2 - total1 - (idle2 - idle1)) * 100.0 / (total2 - total1);
        }
        else
        {
            throw new PlatformNotSupportedException("CPU usage not implemented for this OS");
        }
    }

    private static double GetSystemMemoryUsageInMB()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            GlobalMemoryStatusEx(ref memStatus);
            ulong used = memStatus.ullTotalPhys - memStatus.ullAvailPhys;
            return used / (1024.0 * 1024.0);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            string[] lines = File.ReadAllLines("/proc/meminfo");
            ulong total = ulong.Parse(lines[0].Split(':')[1].Trim().Split(' ')[0]);
            ulong available = ulong.Parse(lines[2].Split(':')[1].Trim().Split(' ')[0]);
            ulong used = total - available;
            return used / 1024.0; // MB
        }
        else
        {
            throw new PlatformNotSupportedException("Memory usage not implemented for this OS");
        }
    }

    #region Windows P/Invoke MEMORYSTATUSEX
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
    #endregion
}
