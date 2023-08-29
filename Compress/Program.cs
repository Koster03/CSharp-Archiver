using Compress.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Compress
{
    class Program
    {
        static void Main(string[] args)
        {
            // if there is no arguments or -h command is passed then show help message and quit

            if (args.Length == 0 || args[0] == "-h")
            {
                PrintHelp();
                return;
            }

            if (args.Length == 3)
            {
                if (args[0] == "-p")
                {
                    PackFile(args[1], args[2]);
                }
                else if (args[0] == "-u")
                {
                    UnpackFile(args[1], args[2]);
                }
                else
                {
                    PrintHelp();
                }
            }

            if (args.Length == 2)
            {
                if (args[0] == "-u")
                {
                    string unpackFile = "";
                    string str1 = args[1];
                    if (str1[str1.Length - 1] == 'z' && str1[str1.Length - 2] == '.')
                    {
                        unpackFile += args[1].Remove(args[1].Length - 2);
                    }

                    else
                    {
                        unpackFile += args[1] + ".unpack";
                    }

                    UnpackFile(args[1], unpackFile);
                }
                else
                {
                    PackFile(args[0], args[1]);
                }
            }

            if (args.Length == 1)
            {
                string outFile = args[0] + ".z";
                PackFile(args[0], outFile);
            }
        }

        private static void PackFile(string inputFileName, string outputFileName)
        {
            string path = @"E:\Files\";

            var transporter = new CryptoStreamTransporter();
            var packer = new LzwPacker();

            using (var input = new FileStream(path + inputFileName, FileMode.Open))
            using (var output = new FileStream(path + outputFileName, FileMode.Create))
            {
                transporter.Pack(input, output, input.Length, packer);
            }
        }

        private static void UnpackFile(string inputFileName, string outputFileName)
        {
            string path = @"E:\Files\";

            var transporter = new CryptoStreamTransporter();
            LzwUnpacker unp = new LzwUnpacker();

            using (var input = new FileStream(path + inputFileName, FileMode.Open))
            using (var output = new FileStream(path + outputFileName, FileMode.Create))
            {
                transporter.Unpack(input, output, input.Length, unp);
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Incorrect data entry");
        }
    }
}
