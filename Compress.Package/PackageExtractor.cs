using System;
using Compress.Core;
using System.IO;
using System.Collections.Generic;

namespace Compress.Package
{
    public class PackageExtractor : IPackageExtractor
    {
        public PackageExtractor(IFileSystem fileSystem, ICryptoFactory cryptoFactory)
        {
            _fileSystem = fileSystem;
            this.cryptoFactory = cryptoFactory;
        }
        public void ExtractFileTo(Stream packageStream, ItemHeader item, string pathTo, Func<string, bool> shouldOverwrite, Action<FileProcessingEventArgs> fileProcessing)
        {
            bool answer = true;
            switch (item)
            {
                case FileHeader fh:
                    {
                        if (_fileSystem.FileExists(Path.Combine(pathTo, fh.Path)))
                        {
                            answer = shouldOverwrite(Path.Combine(pathTo, Path.GetFileName(item.Path)));
                        }

                        break;
                    }
                case DirectoryHeader dh:
                    {
                        if (_fileSystem.DirectoryExists(Path.Combine(pathTo, dh.Path)))
                        {
                            answer = shouldOverwrite(Path.Combine(pathTo, Path.GetFileName(item.Path)));
                        }

                        break;
                    }
            }

            //if (answer)
            if (true)
            {
                switch (item)
                {
                    case FileHeader fh:
                        {
                            ExtractFile(packageStream, Path.Combine(pathTo, item.Path), fh, fileProcessing);
                            break;
                        }
                    case DirectoryHeader dh:
                        {
                            var lst = new List<FileHeader>();
                            Children(new List<ItemHeader> { dh }, lst);

                            lst.ForEach(a => ExtractFile(packageStream, Path.Combine(pathTo, a.Path), a, fileProcessing));
                            break;
                        }
                }
            }
        }

        private void ExtractFile(Stream packageStream, string pathTo, FileHeader item, Action<FileProcessingEventArgs> fileProcessing)
        {
            using (var output = _fileSystem.Open(pathTo, FileMode.Create))
            {
                packageStream.Position = item.StartOffset;

                this.transporter.DataProcessed += (_, args) =>
                {
                    var e = new FileProcessingEventArgs
                    {
                        FileName = Path.GetFileName(item.Path),
                        Type = FileProcessingType.Unpack,
                        Total = args.Total,
                        TotalProcessed = args.TotalProcessed,
                    };

                    fileProcessing(e);
                };

                transporter.Unpack(packageStream, output, item.PackedLength, cryptoFactory.CreateUnpacker());
            }
        }

        private void Children(List<ItemHeader> items, List<FileHeader> children)
        {
            foreach (var item in items)
            {
                if (item is DirectoryHeader)
                {
                    var t = item as DirectoryHeader;
                    Children(t.Items, children);
                }
                if (item is FileHeader)
                {
                    children.Add(item as FileHeader);
                }
            }
        }

        private IFileSystem _fileSystem;
        private readonly ICryptoFactory cryptoFactory;

        private CryptoStreamTransporter transporter = new CryptoStreamTransporter();
    }
}