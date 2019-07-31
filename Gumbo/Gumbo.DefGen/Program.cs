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
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            var appSettings = configuration.GetSection("AppSettings");

            Environment.SetEnvironmentVariable("PATH", appSettings.GetValue<string>("VCBinPath"));
            var libFilePath = appSettings.GetValue<string>("LibFilePath");
            var libName = appSettings.GetValue<string>("LibName");
            var outputDefFilePath = appSettings.GetValue<string>("OutputDefFilePath");

            var exportedNames = GetExportableNames(libFilePath);
            GenerateDefinitionFile(libName, outputDefFilePath, exportedNames);
        }

        static IEnumerable<string> GetExportableNames(string libFilePath)
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
                .Select(x => new { Address = x[0], Name = x[1].Substring(1) })
                .Where(x => !x.Name.StartsWith("?"))
                .Select(x => x.Name)
                .ToList();
        }

        static void GenerateDefinitionFile(string libName, string outputDefFilePath, IEnumerable<string> exportedNames)
        {
            var defs = new List<string>
            {
                $"LIBRARY   {libName}",
                "EXPORTS"
            };
            defs.AddRange(exportedNames.Select((x, i) => $"   {x} @{i + 1}"));
            File.WriteAllLines(outputDefFilePath, defs);
        }
    }
}
