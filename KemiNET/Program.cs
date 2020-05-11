using System;
using System.IO;
using CommandLine;
using KemiNET.Services;
using Serilog;
using Topshelf;
using Topshelf.Logging;


namespace KemiNET
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string programHomeDir = Path.Combine(homeDir, ".keminet");
            string logDir = Path.Combine(programHomeDir, "logs");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"{logDir}/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            Log.Information("Hello, world!");
            
            HostLogger.UseLogger(new SerilogLogWriterFactory.SerilogHostLoggerConfigurator(Log.Logger));
            HostFactory.Run(host =>
            {
                host.UseLinuxIfAvailable();
                host.Service<MainService>(service =>
                {
                    host.SetServiceName("Kemi.NET");
                    host.SetDisplayName("Kemi Process Manager");
                    const string desc = "This service is a process manager for apps. Kill-Execute-Manage-Inspect";
                    host.SetDescription(desc);
                    host.StartAutomatically();

                    service.ConstructUsing(name => new MainService());
                    service.WhenStarted(tc => tc.Start());
                    service.WhenStopped(tc => tc.Stop());

                });
                host.UseSerilog();

                try
                {
                    host.RunAsLocalSystem();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error Starting up Service. Reason: {ex.Message}");
                }

            });

        }
    }
}