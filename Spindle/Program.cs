using Mono.Cecil;
using Spindle.Enums;
using Spindle.IO;
using Spindle.Patches;
using Spindle.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Spindle
{
    internal class Program
    {
        private static string _gameAssemblyFilename;
        private static string _bootstrapAssemblyFilename;
        private static string _requestedPatchName;

        private static ModuleDefinition _gameAssemblyDefinition;
        private static ModuleDefinition _bootstrapAssemblyDefinition;

        private static Patcher _patcher;

        internal static void Main(string[] args)
        {
            WriteStartupHeader();

            if (!IsValidSyntax(args))
            {
                ColoredOutput.WriteInformation($"Usage: {GetExecutingFileName()} <-t (--target) Assembly-CSharp.dll> [options]");
                ColoredOutput.WriteInformation("  Options:");
                ColoredOutput.WriteInformation("    -t [--target]+: Specify the target Assembly-CSharp.dll you want to patch.");
                ColoredOutput.WriteInformation("    -s [--source]+: Specify the source DLL you want to cross-reference.");
                ColoredOutput.WriteInformation("    -p [--patch]+:  Run only patch with the specified name.");
                ErrorHandler.TerminateWithError("Invalid syntax provided.", TerminationReason.InvalidSyntax);
            }

            ParseArguments(args);

            if (string.IsNullOrEmpty(_gameAssemblyFilename))
                ErrorHandler.TerminateWithError("Target DLL name not specified.", TerminationReason.TargetDllNotProvided);
            if ((args.Contains("-p") || args.Contains("--patch")) && string.IsNullOrEmpty(_requestedPatchName))
                ErrorHandler.TerminateWithError("Patch name not specified.", TerminationReason.PatchNameNotProvided);
            if ((args.Contains("-s") || args.Contains("--source")) && string.IsNullOrEmpty(_bootstrapAssemblyFilename))
                ErrorHandler.TerminateWithError("Source DLL name not specified.", TerminationReason.SourceDllNotProvided);

            if (!DistanceFileExists())
                ErrorHandler.TerminateWithError("Specified TARGET DLL not found.", TerminationReason.TargetDllNonexistant);

            if (!BootstrapFileExists() && (args.Contains("-s") || args.Contains("--source")))
                ErrorHandler.TerminateWithError("Specified SOURCE DLL not found.", TerminationReason.SourceDllNonexistant);

            CreateBackup();
            PreparePatches();
            RunPatches();

            ModuleWriter.SavePatchedFile(_gameAssemblyDefinition, _gameAssemblyFilename, false);

            _patcher.AddPatch(new DecapsulationPatch());
            _patcher.RunSpecific("Decapsulation");

            var devDllFileName = $"{Path.GetFileNameWithoutExtension(_gameAssemblyFilename)}.dev.dll";
            ModuleWriter.SavePatchedFile(_gameAssemblyDefinition, devDllFileName, true);
            ColoredOutput.WriteSuccess($"Saved decapsulated development DLL to {devDllFileName}");

            ColoredOutput.WriteSuccess("Patch process completed.");
        }

        private static void WriteStartupHeader()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"Centrifuge Spindle for Unity Engine. Version {version.Major}.{version.Minor}.{version.Build}.{version.Revision}");
            Console.WriteLine("------------------------------------------");
            Console.ResetColor();
        }

        private static bool IsValidSyntax(ICollection<string> args)
        {
            return args.Count >= 1 && (args.Contains("-t") || args.Contains("--target"));
        }

        private static void ParseArguments(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if ((args[i] == "-s" || args[i] == "--source") && (i + 1) < args.Length)
                {
                    _bootstrapAssemblyFilename = args[i + 1];
                    i++;
                }

                if ((args[i] == "-t" || args[i] == "--target") && (i + 1) < args.Length)
                {
                    _gameAssemblyFilename = args[i + 1];
                    i++;
                }

                if ((args[i] == "-p" || args[i] == "--patch") && (i + 1) < args.Length)
                {
                    _requestedPatchName = args[i + 1];
                    i++;
                }
            }
        }

        private static string GetExecutingFileName()
        {
            return Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        }

        private static bool DistanceFileExists()
        {
            return File.Exists(_gameAssemblyFilename);
        }

        private static bool BootstrapFileExists()
        {
            return File.Exists(_bootstrapAssemblyFilename);
        }

        private static void CreateBackup()
        {
            if (!File.Exists($"{_gameAssemblyFilename}.backup"))
            {
                ColoredOutput.WriteInformation("Performing a backup...");
                File.Copy($"{_gameAssemblyFilename}", $"{_gameAssemblyFilename}.backup");
            }
        }

        private static void PreparePatches()
        {
            ColoredOutput.WriteInformation("Preparing patches...");

            _gameAssemblyDefinition = ModuleLoader.LoadGameModule(_gameAssemblyFilename);

            if (!string.IsNullOrEmpty(_bootstrapAssemblyFilename))
            {
                _bootstrapAssemblyDefinition = ModuleLoader.LoadBootstrapModule(_bootstrapAssemblyFilename);
            }
            _patcher = new Patcher(_bootstrapAssemblyDefinition, _gameAssemblyDefinition);
            _patcher.AddPatch(new CentrifugeInitPatch());
        }

        private static void RunPatches()
        {
            if (string.IsNullOrEmpty(_requestedPatchName))
            {
                ColoredOutput.WriteInformation("Running all patches...");
                _patcher.RunAll();
            }
            else
            {
                ColoredOutput.WriteInformation($"Running the requested patch: '{_requestedPatchName}'");
                _patcher.RunSpecific(_requestedPatchName);
            }
        }
    }
}
