using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CLI.Managers;
using CommandLine;
using KemiNETShared;
using KemiNETShared.Requests;
using KemiNETShared.Responses;
using Newtonsoft.Json;
using Serilog;

namespace CLI.Commander
{
    public static class Actions
    {
        public static int RunVerb(StartOption option)
        {
            //build arguments message
            List<string> message = new List<string>
            {
                option.ProgramExecutable, option.ProgramArguments
            };
            
            // send to server
            Request request = new Request(option.GetVerb(), message);
            string res = KemiD.SendRequest(request);
            Console.WriteLine(res);
            return 0;
        }
        
        public static int RunVerb(ListOption option)
        {
            // send to server
            Request request = new Request(option.GetVerb());
            string res = KemiD.SendRequest(request);
            Console.WriteLine("Before pretty printing");
            Console.WriteLine(res);

            ListCommandResponse listCommandResponse = JsonConvert.DeserializeObject<ListCommandResponse>(res);
            
            
            PrettyPrintManager.PrintTable(listCommandResponse.TableHeaders.ToList(), listCommandResponse.TableRows, ConsoleColor.Red);
            
            // TODO implement pretty-printing
            return 0;
        }
        
        public static int RunVerb(StopOption option)
        {
            IEnumerable<string> ids = option.ProgramId.Select(id => id.ToString());
            
            List<string> idList = new List<string>(ids);
            //get the id and send it
            // send to server
            Request request = new Request(option.GetVerb(), idList);
            string res = KemiD.SendRequest(request);
            Console.WriteLine(res);
            return 0;
        }

        public static int RunVerb(PingOption t)
        {
            Console.WriteLine("Ping only");
            if (KemiD.Ping())
            {
                Log.Information("Pinged");
                return 0;
            }
            
            KemiD.Start();
            
            if (KemiD.Ping())
            {
                Log.Information("Pinged");
                return 0;
            }

            Log.Information("Unable to start server. Failing gracefully...lol.");
            return 1;

        }
        
        public static int RunVerb(TestOption t)
        {
            Console.WriteLine("Ping 2 why");
            if (KemiD.Ping())
            {
                Log.Information("Pinged2");
                return 0;
            }
            
            KemiD.Start();
            
            if (KemiD.Ping())
            {
                Log.Information("Pinged2");
                return 0;
            }

            Log.Information("2.Unable to start server. Failing gracefully...lol.");
            return 1;

        }
        
        public static int RunVerb(ShutdownOption shutdownOption)
        {
            // send to server
            Request request = new Request(Constants.Commands.Shutdown);
            KemiD.Shutdown(request);
            return 0;
        }

        public static int HandleErrors(IEnumerable<Error> errors)
        {
            foreach (var error in errors.OfType<NamedError>())
            {
                Log.Error(error.NameInfo.ShortName);
                Log.Error(error.NameInfo.LongName);
                Log.Information(string.Empty);
            }
            return 1;
        }

    }
}