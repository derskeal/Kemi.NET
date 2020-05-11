using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using KemiNET.Helpers;
using KemiNET.Managers;
using KemiNETShared;
using KemiNETShared.Requests;
using Newtonsoft.Json;
using Serilog;
using Topshelf;

namespace KemiNET.Services
{
    public class MainService
    {
        private SocketManager socketManager;
        private ProcessManager processManager;
        public void Start()
        {
            Log.Debug("Testing wantoo wantoo");
            socketManager = new SocketManager();
            processManager = new ProcessManager(socketManager);
            
            socketManager.StartListening(ReceiveSocketData);
        }
        
        public void Stop()
        {
            socketManager.StopListening();
            Environment.Exit(0);
        }

        public void ReceiveSocketData(string content)
        {
            Request request = JsonConvert.DeserializeObject<Request>(content);
            Log.Information($"Received command '{request.Command}'");

            FireAction(request);
            
        }

        private void FireAction(Request request)
        {
            try
            {
                switch (request.Command)
                {
                    case Constants.Commands.Ping:
                        Log.Information("Ping detected");
                        RespondToPing();
                        break;
                    case Constants.Commands.Start:
                        Log.Information("Start command detected");
                        Log.Information($"{request.Arguments[0]}, {request.Arguments[1]}");
                        processManager.CreateProcess(request.Arguments);
                        break;
                    case Constants.Commands.List:
                        Log.Information("List command detected");
                        processManager.ListProcesses();
                        break;
                    case Constants.Commands.Stop:
                        Log.Information("Stop command detected");
                        processManager.StopProcesses(request.Arguments);
                        break;
                    case Constants.Commands.Shutdown:
                        Log.Information("Shutdown command detected");
                        processManager.Shutdown();
                        break;
                    default:
                        Log.Error("Invalid command!");
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Source);
                Log.Error(e.ToString());
            }
            
        }

        public void RespondToPing()
        {
            var response = new
            {
                status = "ready",
                version = "0.1.0"
            };

            socketManager.SendResponse(JsonConvert.SerializeObject(response));
        }

        
        
        
    }
}