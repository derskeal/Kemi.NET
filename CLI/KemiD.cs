using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using CLI.Managers;
using KemiNETShared;
using KemiNETShared.Requests;
using KemiNETShared.Responses;
using Newtonsoft.Json;
using Serilog;

namespace CLI
{
    public class KemiD
    {
        public static bool IsRunning()
        {
            return Ping();
        }

        public static bool Ping()
        {
            Request request = new Request(Constants.Commands.Ping);
            
            Log.Information(JsonConvert.SerializeObject(request));
            try
            {
                string response = SocketConnectionManager.SendToServer(JsonConvert.SerializeObject(request));

                PingResponse pingResponse = JsonConvert.DeserializeObject<PingResponse>(response);

                return pingResponse.Status == "ready";
            }
            catch (Exception exception)
            {
                //Log.Error(exception);
                return false;
            }

        }

        public static string Shutdown(Request request)
        {
            try
            {
                string res = SocketConnectionManager.SendToServer(JsonConvert.SerializeObject(request));
                return res;
            }
            catch (SocketException socketException)
            {
                return "Daemon is not running";
            }
        }

        public static string SendRequest(Request request)
        {
            Log.Information(JsonConvert.SerializeObject(request));
            int maxTry = 0;
            while (maxTry <= 1)
            {
                try
                {
                    string res = SocketConnectionManager.SendToServer(JsonConvert.SerializeObject(request));
                    return res;
                }
                catch (SocketException socketException)
                {
                    maxTry++;
                    if (socketException.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        Start();
                    }
                }
            }
            
            Log.Error("Unable to reach Kemi Daemon");
            return "Unable to reach Kemi Daemon";





            /*//check (by name) if server is running first
            // if not start it
            if (!IsRunning())
            {
                Start();
            }

            if (IsRunning())
            {
                Log.Information(JsonConvert.SerializeObject(request));
                return SocketConnectionManager.SendToServer(JsonConvert.SerializeObject(request));
            }

            return "Unable to send request";*/
        }

        public static bool Start()
        {
            using (var executableProcess = new Process())
            {
                try
                {
                    executableProcess.StartInfo.UseShellExecute = false;
                    executableProcess.StartInfo.FileName = "mono";
                    executableProcess.StartInfo.Arguments = "KemiNET.exe";
                    executableProcess.StartInfo.RedirectStandardOutput = false;
                    executableProcess.StartInfo.RedirectStandardError = false;
                    executableProcess.StartInfo.CreateNoWindow = true;
                    
                    executableProcess.Start();


                    /*if (!executableProcess.Responding)
                    {
                        throw new Exception("KemiD start failed!");
                    }*/
                    //todo send a "readiness probe (😉)" to the server
                    Thread.Sleep(2000);
                    Log.Information("KemiD started.");
                    return true;


                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    return false;
                }
                
            }
        }
    }
}