using System;
using System.Collections;
using System.IO;

namespace wikipedia_metric
{
    internal static class FileManager
    {
        private static readonly Logger Logger;

        static FileManager()
        {
            Logger = new Logger(nameof(FileManager));
        }

        // Opens StreamWriter for a given path
        public static StreamWriter GetStreamWriter(string filePath)
        {
            try
            {
                return new StreamWriter(filePath);
            }
            catch (Exception)
            {
                Logger.Error("An error occured during opening the StreamWriter for a given path: " + filePath);
                throw;
            }
        }

        // Opens StreamWriter for a given path
        public static StreamReader GetStreamReader(string filePath)
        {
            try
            {
                return new StreamReader(filePath);
            }
            catch (Exception)
            {
                Logger.Error("An error occured during opening the StreamReader for a given path: " + filePath);
                throw;
            }
        }

        // Returns an IEnumerable of a file lines at a given path
        public static IEnumerable GetLines(string filePath)
        {
            try
            {
                return File.ReadLines(filePath);
            }
            catch (Exception)
            {
                Logger.Error("An error occured during reading the file: " + filePath);
                throw;
            }
        }
    }
}