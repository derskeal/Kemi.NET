using System;
using System.Collections.Generic;
using System.Reflection;
using CommandLine;
using KemiNETShared;

namespace CLI.Commander
{
    /*public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
            
        [Option('o', "output", Required = false, HelpText = "Set output to file.")]
        public bool Output { get; set; }
    }*/
        
    
    [Verb(Constants.Commands.Start, HelpText = "Start a process")]
    public class StartOption
    {
        [Value(0, Required = true, HelpText = "Program to run")]
        public string ProgramExecutable { get; set; }
        
        [Value(1, Required = false, HelpText = "Arguments to be passed to program" )]
        public string ProgramArguments { get; set; }

        public string GetVerb()
        {
            return Constants.Commands.Start;
        }
    }
    
    
    [Verb(Constants.Commands.List, HelpText = "List running processes")]
    public class ListOption
    {
        public string GetVerb()
        {
            return Constants.Commands.List;
        }
    }
    
    [Verb(Constants.Commands.Stop, HelpText = "Stop a process")]
    public class StopOption
    {
        [Value(0, Required = true, HelpText = "ID of process to stop")]
        public IList<int> ProgramId { get; set; }     
        
        /*[Value(1, Required = false, HelpText = "Name of process to stop" )]
        public string ProcessName { get; set; }*/

        public string GetVerb()
        {
            return Constants.Commands.Stop;
        }
    }
    
    [Verb(Constants.Commands.Shutdown, HelpText = "Start a process")]
    public class ShutdownOption 
    {

        [Value(0, Required = false, HelpText = "Program to run")]
        public string Program { get; set; }
        
        public string GetVerb()
        {
            return Constants.Commands.Shutdown;
        }
        
    }
    
    
    [Verb(Constants.Commands.Ping, HelpText = "Test the daemon")]
    public class PingOption 
    {
        [Option("tries",Required = false, HelpText = "Number of pings")]
        public int Tries { get; set; }
    }

    [Verb("test", HelpText = "Test the daemon")]
    public class TestOption 
    {
        [Option("tries",Required = false, HelpText = "Number of pings")]
        public int Tries { get; set; }
    }
    
    
    /*[Verb("commit", HelpText = "Record changes to the repository.")]
    public class CommitOptions {
        //commit options here
        /*[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages." )]
        public bool Verbose { get; set; }#1#
    }*/
}