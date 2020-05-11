using System;
using System.Collections.Generic;

namespace CLI.Managers
{
    public class PrettyPrintManager
    {
        public static void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }
        
        public static void PrintWithTabs(string[] textArray, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            
            string text = String.Empty;
            foreach (var singleText in textArray)
            {
                text += singleText + "\t";
            }
            Console.WriteLine(text);
        }
        
        public static void PrintTable(List<string> headers, List<string[]> records, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            
            string text = String.Empty;
            foreach (var singleText in records)
            {
                text += singleText + "\t";
            }
            Console.WriteLine(text);
        }
        
    }
}