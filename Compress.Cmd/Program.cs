using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compress.Package;
using Compress.Core;
using System.Reflection;

namespace Compress.Cmd
{
    public class Program
    {
        // common format of utility call:
        // Compress.Cmd {command} {package name} {option} {path}
        // {command} {package name} are required and are always first 2 arguments
        // there could be multiple options and multiple paths
        // as a {path} is meant any argument after first two {command} and {package name} arguments which starts not from "-"
        // {option} is started from "-"
        // options can be included to list of args in any order after {command} and {package name} arguments
        //
        // list of commands:
        //
        // "a": add all (external) {path} to {package name}
        // {package name} could exist, in this case files are added to (or replaced in) existing package
        // otherwise {package name} is created
        // if {package name} has no extension .pkg extension is meant
        // {path} should be valid external path (in file system) to file or directory
        // if it is relative then it should be relative to current path
        // possibly option are
        // "k{max key lenght}" means maximal length in bits of key for LZW algo, length can not be lesser than 9
        // "i{internal directory name} means internal path to directory inside package to which {path} are added
        // 
        // examples:
        // 1) Compress.Cmd a package file1.txt dir1
        //  file file1.txt and directory dir1 are added to package.pkg package
        //  (take a look that .pkg is omitted)
        //
        // 2) Compress.Cmd a package.pkg file1.txt -t18 file2.txt
        //  files file1.txt and file2.txt are added to package.pkg package with max key length 18 bits
        //
        // 3) Compress.Cmd a package.pkg -idir file1.txt 
        //  file1.txt is added to package.pkg into internal directory dir/
        //
        //
        // "x": extract all (internal) {path} from {package name}
        // {package name} must exist
        // if {package name} has no extension .pkg extension is meant
        // {path} should be valid internal path inside package to file or directory
        // possibly option are
        // "e{external directory name} means external path to directory to which paths are extracted
        //   if it doesn't exist it should be created
        // "f" means that all answers to overwrite questions are "yes", therefore user should not be asked about it
        // 
        // examples:
        // 1) Compress.Cmd x package file1.txt dir1
        //  file file1.txt and directory dir1 are extracted from package.pkg package
        //  (take a look that .pkg is omitted)
        //  if some file already exists then user is asked about overwriting this file
        //
        // 2) Compress.Cmd a package.pkg file1.txt -f file2.txt
        //  files file1.txt and file2.txt are extracted from package.pkg,
        //  if these files already exist then they are overwritten without questions
        //
        // 3) Compress.Cmd x package.pkg -ec:/dir file1.txt 
        //  file1.txt is extracted from package.pkg into external directory c:/dir/
        //
        //
        // "d": delete all (internal) {path} from {package name}
        // {package name} must exist
        // if {package name} has no extension .pkg extension is meant
        // {path} should be valid internal path inside package to file or directory
        // 
        // examples:
        // 1) Compress.Cmd d package file1.txt dir1
        //  file file1.txt and directory dir1 are deleted from package.pkg package
        //  (take a look that .pkg is omitted)
        //
        //
        // "h": usage will be shown
        //

        private static FileSystem _fileSystem = new FileSystem();

        //private static PackageFile _packege = new PackageFactory
        //{
        //    Packer = new LzwPacker(new LzwAlgoParams() { MaxCodeBitCount = 20 }),
        //    Unpacker = new LzwUnpacker(new LzwAlgoParams() { MaxCodeBitCount = 20 }),
        //    FileSystem = new FileSystem()
        //}.CreatePackage();

        private static PackageFactory _factory = new PackageFactory();

        private static PackageFile _packege = _factory.CreatePackage();



        //private static PackageFile _packege;
        static void Main(string[] args)
        {
            string utilityName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().FullName).Split(',')[0];

           // var t = _factory.CreatePackage();

            _packege.FileProcessing += (_, argss) =>
            {
                var progress = new ProgressBar();

                Console.Clear();

                if (argss.Type == FileProcessingType.Unpack)
                {
                    Console.Write("Extracting {0}...", argss.FileName);
                }
                else if (argss.Type == FileProcessingType.Pack)
                {
                    Console.Write("Packing {0}...", argss.FileName);
                }

                progress.Report(argss.TotalProcessed / argss.Total);

                System.Threading.Thread.Sleep(125);
            };

            if (args.Length == 0 || args[0] == "h")
            {
                PrintHelp(utilityName);
                return;
            }

            if (args.Length < 2)
            {
                Console.WriteLine($"Format of utility usage is '{utilityName} (command) (package_name) [option] [path]'. Type '{utilityName} h' for help.");
            }

            string packagePath = args[1];
            if (string.IsNullOrEmpty(Path.GetExtension(packagePath)))
                packagePath += ".pkg";

            var options = GetOptions(args.Skip(2)).ToList();
            var paths = GetPaths(args.Skip(2)).ToList();


            switch (args[0])
            {
                case "a":
                    {
                        ExecuteAddCommand(packagePath, options, paths);
                        break;
                    }
                case "x":
                    {
                        ExecuteExtractCommand(packagePath, options, paths);
                        break;
                    }
                case "d":
                    {
                        ExecuteDeleteCommand(packagePath, paths);
                        break;
                    }
                case "h":
                    PrintHelp(utilityName);
                    break;
            }

        }

        private static void ExecuteAddCommand(string packagePath, List<Option> options, List<string> paths)
        {
            _fileSystem.Delete(Path.Combine(@"E:\Files\", packagePath));

            var factory = new PackageFactory();

            var pkeg = factory.CreatePackage();

            for (int i = 0; i < paths.Count; i++)
            {
                paths[i] = Path.Combine(@"E:\Files\", paths[i]);

                if (_fileSystem.IsDirectory(paths[i]))
                {
                    if (!_fileSystem.DirectoryExists(paths[i]))
                    {
                        Console.WriteLine("Directory {0} not exist!", Path.GetDirectoryName(paths[i]));
                        paths.Remove(paths[i]);
                        continue;
                    }
                }
                else
                {
                    if(!_fileSystem.FileExists(paths[i]))
                    {
                        Console.WriteLine("File {0} not exist!", Path.GetFileName(paths[i]));
                        paths.Remove(paths[i]);
                        continue;
                    }
                }
            }

            string where = "";

            foreach (var option in options)
            {
                switch (option.Name)
                {
                    case "-k":
                        {
                            if (int.TryParse(option.Value, out int keyLength))
                            {
                                factory.SetPackerFactory(new PackerFactory(keyLength));
                            }
                            else
                            {
                                Console.WriteLine("Incorrect option value");
                            }
                            break;
                            //maxCodeBitCount = Convert.ToInt32(option.Value);
                        }
                    case "-i":
                        where = option.Value;
                        break;
                }
            }

            //if (maxCodeBitCount != 20)
            //{
            //    _packege = new PackageFactory
            //    {
            //        Packer = new LzwPacker(new LzwAlgoParams() { MaxCodeBitCount = maxCodeBitCount }),
            //        Unpacker = new LzwUnpacker(new LzwAlgoParams() { MaxCodeBitCount = maxCodeBitCount }),
            //        FileSystem = new FileSystem()
            //    }.CreatePackage();
            //}

            _packege.Open(Path.Combine(@"E:\Files\", packagePath));
            _packege.Add(paths, where);
        }

        private static void ExecuteExtractCommand(string packagePath, List<Option> options, List<string> paths)
        {
            _packege.Open(Path.Combine(@"E:\Files\", packagePath));
            string where = @"E:\Files\ext";

            bool ans = false;

            foreach (var option in options)
            {
                switch (option.Name)
                {
                    case "-e":
                        where = option.Value;
                        break;
                    case "-f":
                        ans = true;
                        break;
                }
            }

            if (ans)
            {
                _packege.AskToOverwrite += (_, argss) =>
                {
                    argss.Overwrite = true;
                };
            }
            else
            {
                _packege.AskToOverwrite += (_, argss) =>
                {
                    Console.Write("{0} already exist. Want to overwrite it? ", argss.Path);
                    var an = Console.ReadKey();

                    switch (an.Key)
                    {
                        case ConsoleKey.Y:
                            {
                                argss.Overwrite = true;
                                break;
                            }
                        case ConsoleKey.N:
                            {
                                argss.Overwrite = false;
                                break;
                            }
                    }
                    Console.WriteLine();
                };
            }

            _packege.Extract(paths, where);
        }

        private static void ExecuteDeleteCommand(string packagePath, List<string> paths)
        {
            _packege.Open(Path.Combine(@"E:\Files\", packagePath));

            _packege.Delete(paths);
        }

        private class Option
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        private static IEnumerable<Option> GetOptions(IEnumerable<string> args)
        {
            List<Option> options = new List<Option>();

            foreach (var arg in args)
            {
                if (arg[0] == '-')
                {
                    options.Add(new Option
                    {
                        Name = arg.Substring(0, 2),
                        Value = arg.Substring(2, arg.Length - 2),
                    });
                }
            }

            return options;
        }

        private static IEnumerable<string> GetPaths(IEnumerable<string> args)
        {
            return args.Where(arg => arg[0] != '-');
        }

        private static string MakePathRoot(string path)
        {
            return Path.IsPathRooted(path)
                    ? path
                    : Path.Combine(Directory.GetCurrentDirectory(), path);
        }

        private static void PrintHelp(string utilityName)
        {
            Console.WriteLine("Common format of utility call:");
            Console.WriteLine($"{utilityName} (command) (package name) [option] [path]");
            Console.WriteLine("(command) (package name) are required and are always first 2 arguments");
            Console.WriteLine("there could be multiple options and multiple paths");
            Console.WriteLine("as a (path) is meant any argument after first two (command) and (package name) arguments which starts not from '-'");
            Console.WriteLine("(option) is started from '-'");
            Console.WriteLine("options can be included to list of args in any order after (command) and (package name) arguments");
            Console.WriteLine();
            Console.WriteLine("list of commands:");
            Console.WriteLine();
            Console.WriteLine("'a': add all (external) (path) to (package name)");
            Console.WriteLine("(package name) could exist, in this case files are added to (or replaced in) existing package");
            Console.WriteLine("otherwise (package name) is created");
            Console.WriteLine("if (package name) has no extension .pkg extension is meant");
            Console.WriteLine("(path) should be valid external path (in file system) to file or directory");
            Console.WriteLine("if it is relative then it should be relative to current path");
            Console.WriteLine("possibly option are");
            Console.WriteLine("'k(max key lenght)' means maximal length in bits of key for LZW algo, length can not be lesser than 9");
            Console.WriteLine("'i(internal directory name) means internal path to directory inside package to which (path) are added");
            Console.WriteLine("");
            Console.WriteLine("examples:");
            Console.WriteLine($"1) {utilityName} a package file1.txt dir1");
            Console.WriteLine(" file file1.txt and directory dir1 are added to package.pkg package");
            Console.WriteLine(" (take a look that .pkg is omitted)");
            Console.WriteLine();
            Console.WriteLine($"2) {utilityName} a package.pkg file1.txt -t18 file2.txt");
            Console.WriteLine(" files file1.txt and file2.txt are added to package.pkg package with max key length 18 bits");
            Console.WriteLine();
            Console.WriteLine($"3) {utilityName} a package.pkg -idir file1.txt ");
            Console.WriteLine(" file1.txt is added to package.pkg into internal directory dir/");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("'x': extract all (internal) (path) from (package name)");
            Console.WriteLine("(package name) must exist");
            Console.WriteLine("if (package name) has no extension .pkg extension is meant");
            Console.WriteLine("(path) should be valid internal path inside package to file or directory");
            Console.WriteLine("possibly option are");
            Console.WriteLine("'e(external directory name) means external path to directory to which paths are extracted");
            Console.WriteLine("  if it doesn't exist it should be created");
            Console.WriteLine("'f' means that all answers to overwrite questions are 'yes', therefore user should not be asked about it");
            Console.WriteLine("");
            Console.WriteLine("examples:");
            Console.WriteLine($"1) {utilityName} x package file1.txt dir1");
            Console.WriteLine(" file file1.txt and directory dir1 are extracted from package.pkg package");
            Console.WriteLine(" (take a look that .pkg is omitted)");
            Console.WriteLine(" if some file already exists then user is asked about overwriting this file");
            Console.WriteLine();
            Console.WriteLine($"2) {utilityName} a package.pkg file1.txt -f file2.txt");
            Console.WriteLine(" files file1.txt and file2.txt are extracted from package.pkg,");
            Console.WriteLine(" if these files already exist then they are overwritten without questions");
            Console.WriteLine();
            Console.WriteLine($"3) {utilityName} x package.pkg -ec:/dir file1.txt ");
            Console.WriteLine(" file1.txt is extracted from package.pkg into external directory c:/dir/");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("'d': delete all (internal) (path) from (package name)");
            Console.WriteLine("(package name) must exist");
            Console.WriteLine("if (package name) has no extension .pkg extension is meant");
            Console.WriteLine("(path) should be valid internal path inside package to file or directory");
            Console.WriteLine("");
            Console.WriteLine("examples:");
            Console.WriteLine($"1) {utilityName} d package file1.txt dir1");
            Console.WriteLine(" file file1.txt and directory dir1 are deleted from package.pkg package");
            Console.WriteLine(" (take a look that .pkg is omitted)");
            Console.WriteLine();
        }
    }
}