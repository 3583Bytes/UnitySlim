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
            //Default Path for Win Editor Files
            var ending = @"\Unity\Editor\Editor.log";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                //Default Path for Mac Editor Files
                ending = @"/Library/Logs/Unity/Editor.log";
            }
            
            //Get Current App Data Folder (which has a user name in it)
            var fileName = Utils.AppDataFolder() + ending;
            var limiter = 10;

            if (args.Length > 0)
            {
                //Display Help
                if (args[0] == "-h" || args[0] == "-help")
                {
                    Console.WriteLine("Usage: UnitySlim [filepath] [limiter]");
                    Console.WriteLine();
                    Console.WriteLine("filepath : Path to the Editor.log file created by Unity at build time.");
                    Console.WriteLine("limiter  : Number of assets to display sorted by size desc");
                    Console.WriteLine();
                    return;
                }
                else
                {
                    fileName = args[0];
                }
            }
            if (args.Length > 1)
            {
                limiter = int.Parse(args[1]);
            }
            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No Arguments Found using default values");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (!File.Exists(fileName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Invalid Path: " + fileName);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
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
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Summary:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
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
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine("Top " + limiter + " Assets By Size:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("To exit press any key ...");
            Console.ReadLine();
        }
    }
}
