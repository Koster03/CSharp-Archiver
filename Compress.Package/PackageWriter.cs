using System;
using System.Collections.Generic;
using Compress.Core;
using System.IO;
using System.Linq;

namespace Compress.Package
{
    public class PackageWriter : IPackageWriter
    {
        public PackageWriter(IFileSystem fileSystem, ICryptoFactory packerFactory)
        {
            _fileSystem = fileSystem;
            this.packerFactory = packerFactory;
        }

        public PackageWriter(IFileSystem fileSystem, ICryptoPacker packer)
        {
            _fileSystem = fileSystem;
            //this.parametrs = parametrs;

            _packer = packer;

            if (this.parametrs == null)
            {
                this.parametrs = LzwAlgoParams.Default;
            }
        }
        public PackageHeader Write(string newPackageName, string oldPackageName, List<PackageWriterTask> tasks, Action<FileProcessingEventArgs> fileProcessing)
        {
            PackageHeader newHeader = new PackageHeader();
            newHeader.RootDirectory = DirectoryHeader.FromDirectory(oldPackageName);
            var transporter = new CryptoStreamTransporter();
            Stream inp = null;

            foreach (var task in tasks)
            {
                switch (task)
                {
                    case CopyTask copy:
                        {
                            if (copy.Item is DirectoryHeader)
                            {
                                var t = copy.Item as DirectoryHeader;
                                t.Items.Clear();
                            }
                            
                            newHeader.Items.Add(copy.Item.Clone());
                            break;
                        }
                    case PackTask pack:
                        {
                            if (pack.Path.Type == ItemType.Directory)
                            {
                                if (pack.Path.ExternalPath != string.Empty)
                                {
                                    pack.OutputItem = new DirectoryHeader
                                    {
                                        Path = pack.Path.InternalPath,
                                        Modified = _fileSystem.GetLastWriteTimeUtc(pack.Path.ExternalPath),
                                    };
                                }
                                else
                                {
                                    pack.OutputItem = new DirectoryHeader
                                    {
                                        Path = pack.Path.InternalPath,
                                        Modified = DateTime.UtcNow,
                                    };
                                }

                                newHeader.Items.Add(pack.OutputItem);
                            }
                            else
                            {
                                using (var packingFile = _fileSystem.Open(pack.Path.ExternalPath, FileMode.Open))
                                {
                                    pack.OutputItem = new FileHeader
                                    {
                                        Path = pack.Path.InternalPath,
                                        Modified = _fileSystem.GetLastWriteTimeUtc(pack.Path.ExternalPath),
                                        PackedLength = 0,
                                        UnpackedLength = packingFile.Length,
                                        StartOffset = 0,
                                    };
                                }

                                newHeader.Items.Add(pack.OutputItem);
                            }

                            break;
                        }
                }
            }

            MemoryStream ms = new MemoryStream();

            long plz;

            _serializator.Save(ms, newHeader);

            using (var output = _fileSystem.Open(newPackageName, FileMode.Create))
            {
                long startContent = ms.Position;

                plz = startContent;

                ms.Position = 0;
                ms.CopyTo(output, (int)ms.Length);

                try
                {
                    if (tasks.Any(task => task is CopyTask))
                    {
                        inp = _fileSystem.Open(oldPackageName, FileMode.Open);
                    }

                    int i = 0;

                    foreach (var task in tasks)
                    {
                        switch (task)
                        {
                            case CopyTask copy:
                                {
                                    if (copy.Item is DirectoryHeader)
                                    {
                                        i++;
                                        continue;
                                    }
                                    var item = copy.Item as FileHeader;

                                    var it = (FileHeader)newHeader.Items[i++];

                                    it.StartOffset = startContent;
                                    inp.Position = (int)item.StartOffset;

                                    //inp.Position = startContent;
                                    CopyStream(inp, output, (int)item.PackedLength);

                                    startContent += item.PackedLength;
                                    break;
                                }
                            case PackTask pack:
                                {
                                    if (pack.OutputItem is DirectoryHeader)
                                    {
                                        continue;
                                    }
                                    using (var packingFile = _fileSystem.Open(pack.Path.ExternalPath, FileMode.Open))
                                    {
                                        var st = output.Length;
                                        var w = MemberwiseClone();

                                        transporter.DataProcessed += (_, args) =>
                                        {
                                            var e = new FileProcessingEventArgs
                                            {
                                                FileName = Path.GetFileName(pack.Path.InternalPath),
                                                Type = FileProcessingType.Pack,
                                                Total = args.Total,
                                                TotalProcessed = args.TotalProcessed,
                                            };

                                            fileProcessing(e);
                                        };


                                       transporter.Pack(packingFile, output, packingFile.Length, this.packerFactory.CreatePacker());

                                        var item = pack.OutputItem as FileHeader;
                                        if (item.PackedLength == 0)
                                        {
                                            item.PackedLength = output.Length - st;
                                            //item.StartOffset = startContent;
                                        }

                                        startContent += item.PackedLength;

                                        break;
                                    }
                                }
                        }
                        output.Flush();
                    }
                }
                finally
                {
                    inp?.Dispose();
                }

                newHeader.ReconstructInternalRelations(plz);

                MemoryStream streamHeader = new MemoryStream();
                _serializator.Save(streamHeader, newHeader);

                output.Position = 0;
                output.Write(streamHeader.ToArray(), 0, (int)streamHeader.Length);
            }

            _fileSystem.Delete(oldPackageName);
            _fileSystem.Move(newPackageName, oldPackageName);

            return newHeader;
        }

        private void CopyStream(Stream input, Stream output, int bytes)
        {
            byte[] buffer = new byte[32768];
            int read;
            while (bytes > 0 &&
                   (read = input.Read(buffer, 0, Math.Min(buffer.Length, bytes))) > 0)
            {
                output.Write(buffer, 0, read);
                bytes -= read;
            }
        }

        private IFileSystem _fileSystem;
        private readonly ICryptoFactory packerFactory;
        private HeaderSerializator _serializator = new HeaderSerializator();
        private LzwAlgoParams parametrs = LzwAlgoParams.Default;

        private ICryptoPacker _packer;
    }
}