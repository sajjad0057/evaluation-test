using Serilog;
using System.Diagnostics;


class Program
{
    private static int index = 0;

    static async Task Main()
    {
        string projectRoot =
            Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;

        string logsDirectory = Path.Combine(projectRoot, "Logs");

 
        if (!Directory.Exists(logsDirectory))
            Directory.CreateDirectory(logsDirectory);
        

        string logFilePath = Path.Combine(logsDirectory, "system-health.log");

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

            double cpuUsage = Math.Round(GetCpuUsage(), 2);
            double memoryUsage = Math.Round(GetMemoryUsageInMB(), 2);

            Log.ForContext("Index", index)
               .ForContext("Cpu", cpuUsage)
               .ForContext("Memory", memoryUsage)
               .Information("Metrics recorded");

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }

    private static double GetCpuUsage()
    {
        var startTime = DateTime.UtcNow;
        var startCpu = Process.GetCurrentProcess().TotalProcessorTime;

        Task.Delay(1000).Wait();

        var endTime = DateTime.UtcNow;
        var endCpu = Process.GetCurrentProcess().TotalProcessorTime;

        return ((endCpu - startCpu).TotalMilliseconds /
               (Environment.ProcessorCount *
               (endTime - startTime).TotalMilliseconds)) * 100;
    }

    private static double GetMemoryUsageInMB()
    {
        return Process.GetCurrentProcess().WorkingSet64 / (1024.0 * 1024.0);
    }
}
