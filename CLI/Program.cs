using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using CommandLine;
using CLI.Commander;
using Serilog;

namespace CLI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            return Parser.Default.ParseArguments<PingOption, StartOption, ListOption, StopOption, ShutdownOption>(args)
                .MapResult(
                    (PingOption opts) => Actions.RunVerb(opts),
                    (StartOption opts) => Actions.RunVerb(opts),
                    (ListOption opts) => Actions.RunVerb(opts),
                    (StopOption opts) => Actions.RunVerb(opts),
                    (ShutdownOption opts) => Actions.RunVerb(opts),
                    Actions.HandleErrors);
        }
        
        
    }
}