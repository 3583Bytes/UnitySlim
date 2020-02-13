using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Slim.Model;

namespace Slim
{
    internal class LogParser
    {
        /// <summary>
        ///     Path to the editor log file.
        /// </summary>
        private readonly string fileName;
        /// <summary>
        ///    Result
        /// </summary>
        public List<LogItem> Result;

        /// <summary>
        ///   Constructor
        /// </summary>
        public LogParser (string FileName)
        {
            fileName = FileName;
        }
        /// <summary>
        ///   Load Editor File and Build Results
        /// </summary>
        public async Task LoadFile()
        {
            using (var stream = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Result = await ResultBuilder.FromStream(stream);
            }
        }

    }
}
