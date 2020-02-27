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
            //Set Default Values, used if no args are passed
            string fileName = Utils.GetDefaultFilePath();
            int limiter = 20;

            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No Arguments Found using default values");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }

            foreach (string arg in args)
            {
                //TryParse overwrites to 0 on failure.
                int tmp;
                //Check if parameter is the limiter (int)
                bool success = Int32.TryParse(args[0], out tmp);

                if (success)
                {
                    limiter = tmp;
                }
                //Not limiter maybe its -h for help
                else
                {
                    //Display Help
                    if (arg == "-h" || arg == "-help")
                    {
                        Console.WriteLine("Usage: UnitySlim [filepath] [limiter]");
                        Console.WriteLine();
                        Console.WriteLine("Unity Game Engine Build Size Analyzer.");
                        Console.WriteLine();
                        Console.WriteLine("filepath : Path to the Editor.log file created by Unity at build time.");
                        Console.WriteLine("limiter  : Number of assets to display sorted by size desc");
                        Console.WriteLine();
                        return;
                    }
                    else
                    {
                        //Maybe it is a file
                        if (File.Exists(arg))
                        {
                            fileName = arg;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Warning:" +arg + "is not a valid argument");
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
                

            }

            // We still want to check if file exists in case we are using a default path
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

                if (count >= limiter)
                {
                    break;
                }
            }
            
        }

    }
}
