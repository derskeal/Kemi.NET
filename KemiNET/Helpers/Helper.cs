using System;
using System.IO;

namespace KemiNET.Helpers
{
    public class Helper
    {
        public static string GetLogFile(string filename)
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string programHomeDir = Path.Combine(homeDir, ".keminet");
            string logDir = Path.Combine(programHomeDir, "logs");
            return $"{logDir}/{filename}";
        }
    }
}