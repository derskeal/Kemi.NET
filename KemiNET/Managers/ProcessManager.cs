using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KemiNET.Helpers;
using KemiNETShared.Responses;
using Newtonsoft.Json;
using Serilog;

namespace KemiNET.Managers
{
    public struct ProcessData
    {
        public int systemProcessId;

        public string processName;
    }
    
    public class ProcessManager
    {
        private List<int> runningProcesses;
        private Dictionary<int, int> runningProcesses2;
        
        private readonly SocketManager socketManager;

        public ProcessManager(SocketManager socketManager)
        {
            this.socketManager = socketManager;
            runningProcesses = new List<int>();
        }
        
        public void CreateProcess(List<string> processArguments)
        {
            if (processArguments.Count < 1)
            {
                socketManager.SendResponse(new
                {
                    status = "failed",
                    error = "No parameters provided"
                });
            };

            string executableFilename = processArguments[0];
            processArguments.Remove(processArguments[0]);
            string executableArguments = string.Empty;

            foreach (var arg in processArguments)
            {
                executableArguments += arg + " ";
            }
            try
            {
                string logFilename = $"{executableFilename}.txt";
                var log = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(Helper.GetLogFile(logFilename), rollingInterval: RollingInterval.Day)
                    .CreateLogger();
                
                
                
                Process newProcess = new Process();
                newProcess.StartInfo.FileName = executableFilename;
                newProcess.StartInfo.Arguments = executableArguments;
                newProcess.StartInfo.UseShellExecute = false;
                newProcess.StartInfo.CreateNoWindow = true;
                newProcess.StartInfo.RedirectStandardOutput = true;
                newProcess.StartInfo.RedirectStandardError = true;
                newProcess.EnableRaisingEvents = true;
                
                newProcess.OutputDataReceived += (sender, args) => {
                    log.Information(args.Data);
                };

                newProcess.Start();
                newProcess.BeginOutputReadLine();
                
                runningProcesses.Add(newProcess.Id);
                
                ProcessData processData = new ProcessData();
                processData.processName = "Example name";
                processData.systemProcessId = newProcess.Id;
                
                
                
                Log.Information($"Successfully started new app - {executableFilename} with arguments {executableArguments}");
                
                var response = new
                {
                    status = "success",
                    appId = runningProcesses.Count,
                    appName = $"app0{runningProcesses.Count}"
                };

                socketManager.SendResponse(JsonConvert.SerializeObject(response));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            
        }
        
        public void Shutdown()
        {
            foreach (int processId in runningProcesses)
            {
                Process.GetProcessById(processId).Kill();
            }
            Log.Information("Shutdown Successful.");
            Environment.Exit(0);
        }

        public void ListProcesses()
        {
            var runningProcessesDetails = new List<object>();
            List<string[]> runningProcessesData = new List<string[]>();
            
            foreach (int processId in runningProcesses)
            {
                
                string[] processProps = {runningProcesses.IndexOf(processId).ToString(), "name", processId.ToString()};
                
                runningProcessesData.Add(processProps);
                runningProcessesDetails.Add(processId);
            }
            
            var response = new
            {
                status = "success",
                processes = runningProcessesData
            };
            
            IEnumerable<string> columns = new[] {"appID", "appName", "processNumber"};
            var res = new ListCommandResponse(ResponseCode.SUCCESS, columns, runningProcessesData);
            
            socketManager.SendResponse(JsonConvert.SerializeObject(res));
        }
        
        public void StopProcesses(List<string> processesStringList)
        {
            var statusDict = new Dictionary<string, string>();

            foreach (string processIdString in processesStringList)
            {
                if (int.TryParse(processIdString, out var processId))
                {
                    if (runningProcesses.Contains(processId))
                    {
                        Process.GetProcessById(processId).Kill();
                        runningProcesses.Remove(processId);
                        statusDict.Add(processId.ToString(), $"Successfully stopped {processId}");
                    }
                    else 
                    {
                        statusDict.Add(processId.ToString(), $"Process {processId} not found");
                    }
                }
                else
                {
                    statusDict.Add(processId.ToString(), "Invalid process id");
                }
            }
            
            
            var response = new
            {
                status = "success",
                data = statusDict
            };
            
            socketManager.SendResponse(JsonConvert.SerializeObject(response));
        }
    }
}