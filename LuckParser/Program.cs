using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;

namespace LuckParser
{
    static class Program {
        public static string Version = "1.8";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            List<string> logFiles = new List<string>();

            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            if (args.Length > 0)
            {

                for (var i = 0; i < args.Length; i++) {
                    if (args[i] == "-h") {
                        PrintHelp();
                        return 0;
                    }

                    if (args[i] == "-c") {
                        if (args.Length - i >= 1) {
                            Properties.Settings.LoadFrom(File.OpenText(args[++i]));
                        } else {
                            PrintHelp();
                            return 0;
                        }
                    } else if (args[i].StartsWith("--")) {
                        int eq = args[i].IndexOf('=');
                        string setting, val;
                        if (eq > 0) {
                            setting = args[i].Substring(2, eq - 2);
                            val = args[i].Substring(eq + 1);
                        } else {
                            setting = args[i].Substring(2);
                            val = "True";
                        }
                        if (args.Length - i >= 1 && Properties.Settings.HasSetting(setting)) {
                            Properties.Settings.Default.Set(setting, val);
                        } else {
                            PrintHelp();
                            return 0;
                        }
                    } else {
                        logFiles.Add(args[i]);
                    }
                }
            }
            if (logFiles.Count > 0)
            {
                // Use the application through console 
                new ConsoleProgram(logFiles);
                return 0;
            }

            PrintHelp();
            return 0;
        }

        private static void PrintHelp() {
            Console.WriteLine("GuildWars2EliteInsights.exe [arguments] [logs...]");
            Console.WriteLine("");
            Console.WriteLine("-c [config path] : use another config file");
            Console.WriteLine("-p : disable windows specific functions");
            Console.WriteLine("-h : help");
            Console.WriteLine("");
            foreach (var setting in Properties.Settings.EnumSettings()) {
                Console.WriteLine($"--{setting} [{Properties.Settings.Default.Get(setting)}]");
            }
        }
    }
}
