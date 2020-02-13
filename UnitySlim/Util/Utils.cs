using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Slim.Util
{
    public static class Utils
    {

        /// <summary>
        /// Parses a string and rerturns a shorter version
        /// </summary>
        /// <param name="absolutepath">The path to compress</param>
        /// <param name="limit">The maximum length</param>
        /// <param name="delimiter">The character(s) to use to imply incompleteness</param>
        /// <returns></returns>
        public static string ShrinkPath(string absolutepath, int limit, string delimiter = "…")
        {
            //no path provided
            if (string.IsNullOrEmpty(absolutepath))
            {
                return "";
            }

            var fileName = Path.GetFileName(absolutepath);
            int fileNameLen = fileName.Length;
            int pathLen = absolutepath.Length;
            var dir = absolutepath.Substring(0, pathLen - fileNameLen);

            int delimLen = delimiter.Length;
            int idealMinLen = fileNameLen + delimLen;

            var slash = (absolutepath.IndexOf("/") > -1 ? "/" : "\\");

            //less than the minimum amt
            if (limit < ((2 * delimLen) + 1))
            {
                return "";
            }

            //fullpath
            if (limit >= pathLen)
            {
                return absolutepath;
            }

            //file name condensing
            if (limit < idealMinLen)
            {
                return delimiter + fileName.Substring(0, (limit - (2 * delimLen))) + delimiter;
            }

            //whole name only, no folder structure shown
            if (limit == idealMinLen)
            {
                return delimiter + fileName;
            }

            return dir.Substring(0, (limit - (idealMinLen + 1))) + delimiter + slash + fileName;
        }

        public static string AppDataFolder()
        {
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //var userPath = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "LOCALAPPDATA" : "Home");
            
            return userPath.Replace(@".local/share", "");
        }

        public static string GetRootFolder(string path)
        {
            while (true)
            {
                string temp = Path.GetDirectoryName(path);
                if (String.IsNullOrEmpty(temp))
                    break;
                path = temp;
            }
            return path;
        }
    }
}
