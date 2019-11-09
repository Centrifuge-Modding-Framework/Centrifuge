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
using System.Security.Cryptography;

namespace Spindle
{
    internal class Program
    {
        private static string _gameAssemblyFilename;
        private static string _bootstrapAssemblyFilename;
        private static string _requestedPatchName;

        private static bool _generateHashFile;
        private static bool _decapOnly;

        private static ModuleDefinition _gameAssemblyDefinition;
        private static ModuleDefinition _bootstrapAssemblyDefinition;

        private static Patcher _patcher;

        internal static void Main(string[] args)
        {
            WriteStartupHeader();

            if (!IsValidSyntax(args))
            {
                ColoredOutput.WriteInformation($"Usage: {GetExecutingFileName()} <(-t | --target) target_assembly.dll> [options]");
                ColoredOutput.WriteInformation("  Options:");
                ColoredOutput.WriteInformation("    -t [--target]+: Specify the target assembly you want to patch.");
                ColoredOutput.WriteInformation("    -s [--source]+: Specify the source DLL you want to source the init code from.");
                ColoredOutput.WriteInformation("    -p [--patch]+:  Run only the patch with the specified name.");
                ColoredOutput.WriteInformation("    -h [--hash]: Generate a .md5 file of the patched assembly.");
                ColoredOutput.WriteInformation("    -d [--decap-only]: Only decapsulate the target DLL. Invalidates -s -p and -h.");

                ErrorHandler.TerminateWithError("Invalid syntax provided.", TerminationReason.InvalidSyntax);
            }

            ParseArguments(args);

            if (string.IsNullOrEmpty(_gameAssemblyFilename))
                ErrorHandler.TerminateWithError("Target DLL name not specified.", TerminationReason.TargetDllNotProvided);
            if ((args.Contains("-p") || args.Contains("--patch")) && string.IsNullOrEmpty(_requestedPatchName))
                ErrorHandler.TerminateWithError("Patch name not specified.", TerminationReason.PatchNameNotProvided);
            if ((args.Contains("-s") || args.Contains("--source")) && string.IsNullOrEmpty(_bootstrapAssemblyFilename))
                ErrorHandler.TerminateWithError("Source DLL name not specified.", TerminationReason.SourceDllNotProvided);

            if (!BootstrapFileExists() && (args.Contains("-s") || args.Contains("--source")))
                ErrorHandler.TerminateWithError("Specified SOURCE DLL not found.", TerminationReason.SourceDllNonexistant);

            if (!GameFileExists())
                ErrorHandler.TerminateWithError("Specified TARGET DLL not found.", TerminationReason.TargetDllNonexistant);

            PreparePatches();

            if (!_decapOnly)
            {
                CreateBackup();
                RunPatches();

                ModuleWriter.SavePatchedFile(_gameAssemblyDefinition, _gameAssemblyFilename, false);

                if (_generateHashFile)
                    GenerateHashFile();
            }
            else
            {
                ColoredOutput.WriteInformation("Attention! Decap-only run requested.");
            }

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

                if (args[i] == "-d" || args[i] == "--decap-only")
                {
                    _decapOnly = true;
                }

                if (args[i] == "-h" || args[i] == "--hash")
                {
                    _generateHashFile = true;
                }
            }
        }

        private static string GetExecutingFileName()
        {
            return Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        }

        private static bool GameFileExists()
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

        private static void GenerateHashFile()
        {
            var md5Path = Path.Combine(
                Path.GetDirectoryName(_gameAssemblyFilename),
                $"{Path.GetFileNameWithoutExtension(_gameAssemblyFilename)}.md5"
            );

            using var md5 = MD5.Create();
            using var stream = File.OpenRead(_gameAssemblyFilename);
            using var sw = new StreamWriter(md5Path);

            var hash = md5.ComputeHash(stream);
            var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            sw.Write(hashString);
        }
    }
}
