using System;
using System.IO;
using System.Collections.Generic;
using Slim.Model;
using Slim.Util;
using System.Runtime.InteropServices;

namespace Slim
{
    class Program
    {
        static void Main(string[] args)
        {
            var ending = @"\Unity\Editor\Editor.log";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                ending = @"/Library/Logs/Unity/Editor.log";
            }
            

            var fileName = Utils.AppDataFolder() + ending;
            var limiter = 10;

            if (args.Length > 0)
            {
                fileName = args[0];
            }
            if (args.Length > 1)
            {
                limiter = int.Parse(args[1]);
            }
            if (args.Length == 0)
            {
                Console.WriteLine("No Arguments Found using default values");
            }

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Error: Invalid Path: " + fileName);
              
            }

            Console.WriteLine("Found Editor Log: " + fileName);
            Console.WriteLine();

            LogParser parser = new LogParser(fileName);

            //Parse Editor Log to get Results
            parser.LoadFile().Wait();

            List<SummaryItem> summaryData = new List<SummaryItem>();

            //Display Summary
            foreach (LogItem item in parser.Result)
            {
                var tempItem = new SummaryItem { ItemType = Utils.GetRootFolder(item.ItemPath), ItemSize = item.ItemSize };

                if (!summaryData.Contains(tempItem))
                {
                    summaryData.Add(new SummaryItem { ItemType = Utils.GetRootFolder(item.ItemPath), ItemSize = item.ItemSize });
                }
                else
                {
                    summaryData.Find(x => x.ItemType.Contains(tempItem.ItemType)).ItemSize += tempItem.ItemSize;
                }

            }

            //Display Summary
            Console.WriteLine("Summary:");
            Console.WriteLine();

            foreach (SummaryItem item in summaryData)
            {
                if (item.ItemSize >= 1000)
                {
                    Console.WriteLine(item.ItemType + " " + Math.Round(item.ItemSize / 1000, 2) + " mb");
                }
                else
                {
                    Console.WriteLine(item.ItemType + " " + Math.Round(item.ItemSize, 2) + " kb");
                }

            }

            //Display Results
            Console.WriteLine();
            Console.WriteLine("Top " + limiter + " Assets By Size:");
            Console.WriteLine();

            int count = 0;

            foreach (LogItem item in parser.Result)
            {
                count++;
                if (item.ItemSize >= 1000)
                {
                    Console.WriteLine(Math.Round(item.ItemSize / 1000, 2) + " mb " + item.ItemPercent + "% " + Utils.ShrinkPath(item.ItemPath, 90));
                }
                else
                {
                    Console.WriteLine(Math.Round(item.ItemSize, 2) + " kb " + item.ItemPercent + "% " + Utils.ShrinkPath(item.ItemPath, 90));
                }

                if (count>= limiter)
                {
                    break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press Any Key:");
            Console.ReadLine();
        }
    }
}
