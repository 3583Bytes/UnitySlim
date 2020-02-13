using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Slim.Model;

namespace Slim
{
    public static class ResultBuilder
    {
        public static async Task<List<LogItem>> FromStream(Stream stream)
        {
            List<LogItem> files;

            using (var reader = new StreamReader(stream))
            {
                //Read the raw input data and make a list of LogItems.
                files = new List<LogItem>();
                var foundSizeInfo = false;
                string line;

                while (!reader.EndOfStream)
                {
                    line = await reader.ReadLineAsync();

                    if (line.Trim().Contains(LogStartKeyword))
                    {
                        // This is the line before the size breakdown
                        foundSizeInfo = true;
                        continue;
                    }

                    if (!foundSizeInfo)
                    {
                        continue;
                    }

                    LogItem data;
                    if (ParseDetailsFromLine(line, out data))
                    {
                        files.Add(data);
                    }
                    else
                    {
                        break;
                    }
                }
                // Sort the list of files by file size.
                files.Sort(new LogItem());
            }

            return files;
        }

        /// <summary>
        ///     Convert Unity editor log line into metadata to process.
        /// </summary>
        /// <remarks>
        ///     Line Looks Like: " {size} {unit} {percent}% {path}/{file}"
        /// </remarks>
        private static bool ParseDetailsFromLine(string line, out LogItem data)
        {
            try
            {
                line = line.Trim();
                var size = line.Substring(0, line.IndexOf(' '));
                var unit = line.Substring(line.IndexOf(' ') + 1, 2);
                var percent = float.Parse(line.Substring(line.IndexOf('%') - 3, 3));
                var kilobytes = float.Parse(size, CultureInfo.InvariantCulture);
                switch (unit)
                {
                    case "mb":
                        kilobytes *= 1000;
                        break;
                }

                var path = line.Substring(line.IndexOf("% ", StringComparison.Ordinal) + 2);
                data = new LogItem(path, kilobytes, percent);
                return true;
            }
            catch
            {
                data = new LogItem(string.Empty, -1, 0f);
                return false;
            }
        }

        internal static string LogStartKeyword
        {
            get
            {
                return "Used Assets and files from the Resources folder, sorted by uncompressed size:";
            }
        }
    }
}
