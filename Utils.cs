using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SaintsFieldSourceParser
{
    public static class Utils
    {
        public static bool Debug = false;
        private static string _tempFolderPath;
        public static void DebugToFile(string toWrite, [CallerLineNumber] int lineNumber = 0)
        {
            if (!Debug)
            {
                return;
            }

            // #if DEBUG
            if (string.IsNullOrEmpty(_tempFolderPath))
            {
                _tempFolderPath = Path.GetTempPath();
            }

            // const string filePath = @"C:\Users\tyler\AppData\Local\Temp\SaintsDebug.txt";
            string tempFilePath = Path.Combine(_tempFolderPath, "SaintsFieldSourceParser.txt");
            //tempFilePath = "/tmp/SaintsDebug.txt";
            try
            {
                using (FileStream fs = new FileStream(tempFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.WriteLine($"[{lineNumber}] {toWrite}");
                }
            }
            catch (Exception)
            {
                //
            }
            // #endif
        }
    }
}
