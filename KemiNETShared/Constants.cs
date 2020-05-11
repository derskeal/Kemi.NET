namespace KemiNETShared
{
    public class Constants
    {
        public const string ServerProcessName = "kemi";
        public const string DefaultHostname = "localhost";
        public const int DefaultPort = 11000;
        public const char StringSeparator = ';';

        public class Commands
        {
            public const string Ping = "ping";
            public const string Start = "start";
            public const string List = "list";
            public const string Stop = "stop";
            public const string Shutdown = "shutdown";
        }
    }
}