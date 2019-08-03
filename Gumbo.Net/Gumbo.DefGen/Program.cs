using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Gumbo.DefGen
{
    class Program
    {
        /// <summary>
        /// This is a .DEF file generator to produce a .DLL with exports.
        /// Without it PInvokes won't work.
        /// Setps:
        /// Build .LIB. 
        /// Put it in a project. 
        /// Generate .DEF file. 
        /// Set .DEF file path in Project properties -> Linker -> Input -> Module Definition File.
        /// Build .DLL instead of .LIB.
        /// </summary>
        /// <param name="args"></param>
        static int Main(string[] args)
        {
            var r = ParseArgs(args, out var bitness, out var libFile);
            if (r != 0)
                return r;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            var appSettings = configuration.GetSection("AppSettings");
            var vcBinPath = appSettings.GetValue<string>("VCBinPath");
            Environment.SetEnvironmentVariable("PATH", vcBinPath);
            Console.WriteLine($"VCBINPATH: {vcBinPath}");
            Console.WriteLine($"BUILD: {libFile}");
            MakeDefFile(libFile, bitness == "x86" ? 1 : 0);
            return r;
        }

        static int ParseArgs(string[] args, out string bitness, out string libFile)
        {
            bitness = null;
            libFile = null;
            if (args.Length != 2)
            {
                Console.WriteLine(@"HELP
Gumbo.DefGen x86 [path to lib]
Gumbo.DefGen x64 [path to lib]
");
                return 1;
            }
            bitness = args[0];
            if (bitness != "x86" && bitness != "x64")
            {
                Console.WriteLine("NO FILE");
                return 1;
            }
            libFile = args[1];
            if (!File.Exists(libFile))
            {
                Console.WriteLine("NO FILE");
                return 1;
            }
            return 0;
        }

        static void MakeDefFile(string libFile, int clip)
        {
            var library = Path.GetFileNameWithoutExtension(libFile);
            var defFile = Path.ChangeExtension(libFile, ".def");
            var exportedNames = GetExportableNames(libFile, clip)
                .Where(x => x != "_vfprintf_l");
            GenerateDefinitionFile(library, defFile, exportedNames);
        }

        static IEnumerable<string> GetExportableNames(string libFilePath, int clip)
        {
            var linkermemberFileName = Path.GetTempFileName();
            var args = $@"/LINKERMEMBER:2 /OUT:""{linkermemberFileName}"" ""{libFilePath}""";
            Process.Start("dumpbin.exe", args).WaitForExit();
            var lines = File.ReadAllLines(linkermemberFileName);
            File.Delete(linkermemberFileName);
            return lines
                .SkipWhile(x => !x.Contains("public symbols"))
                .Skip(2)
                .TakeWhile(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().Split(' '))
                .Select(x => new { Address = x[0], Name = x[1] })
                .Where(x => !x.Name.StartsWith("?") && !x.Name.StartsWith("__xmm@"))
                .Select(x => x.Name.Substring(clip))
                .ToList();
        }

        static void GenerateDefinitionFile(string library, string defFile, IEnumerable<string> exportedNames)
        {
            var defs = new List<string>
            {
                $"LIBRARY   {library}",
                "EXPORTS"
            };
            defs.AddRange(exportedNames.Select((x, i) => $"   {x} @{i + 1}"));
            File.WriteAllLines(defFile, defs);
        }
    }
}
