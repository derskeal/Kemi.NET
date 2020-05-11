using System.Collections.Generic;

namespace KemiNETShared.Requests
{
    public class Request
    {
        public string Command { get; }
        
        // for the start command, the first item in the array is the executable name
        public List<string> Arguments { get; }

        public List<string> Options { get; }

        public List<string> Flags { get; }

        public Request(string command, List<string> arguments = null, List<string> options = null, List<string> flags = null)
        {
            Command = command;
            Arguments = arguments ?? new List<string>();
            Options = options ?? new List<string>();;
            Flags = flags ?? new List<string>();;
        }
    }


    public class Request2
    {
        public string Type { get; }
        
        public string Message { get; }

        public Request2(string type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}