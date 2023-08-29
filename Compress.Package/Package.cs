using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compress.Core;

namespace Compress.Package
{
    public class PackageFile
    {
        public PackageFile(IFileSystem fileSystem, IPackageWriter packageWriter, IPackageExtractor packageExtractor)
        {
            this.fileSystem = fileSystem;
            this.packageWriter = packageWriter;
            this.packageExtractor = packageExtractor;
        }

        // anchor package into external @filePath
        // if it exists then header should be read
        public void Open(string filePath)
        {
            this.packagePath = filePath;
            var hs = new HeaderSerializator();
            using (Stream input = fileSystem.Open(filePath, FileMode.OpenOrCreate))
            {
                this.header = hs.Load(input);
                this.header.RootDirectory = DirectoryHeader.FromDirectory(filePath);
                this.header.ReconstructInternalRelations(input.Position);
            }
        }

        // create directory inside package
        // @path is an internal path to dir which can contains several un-existing directories
        public void CreateDirectory(string path)
        {
            var tasks = new List<PackageWriterTask>();
            var pts = new List<PackTask>();
            var cts = new List<CopyTask>();
            pts.Add(new PackTask("", path, ItemType.Directory));

            foreach (var item in Items)
            {
                cts.Add(new CopyTask(item));
            }

            tasks.AddRange(cts);
            tasks.AddRange(pts);

            ApplyChanges(tasks);
        }

        // add several external @paths into package to internal package @packagePath
        public void Add(List<string> paths, string packagePath = "")
        {
            var tasks = new List<PackageWriterTask>();
            List<CopyTask> cts = new List<CopyTask>();
            List<PackTask> pts = new List<PackTask>();

            Children(paths, pts, packagePath);

            if (Items.Count != 0)
            {
                var lst = pts.Select(a => a.Path.InternalPath).ToList();

                foreach (var item in Items)
                {
                    if (!lst.Contains(item.Path))
                    {
                        cts.Add(new CopyTask(item));
                    }
                }
            }

            tasks.AddRange(cts);
            tasks.AddRange(pts);

            ApplyChanges(tasks);
        }

        private void Children(List<string> paths, List<PackTask> pts, string parent)
        {
            parent = parent.Contains('\\') || parent == "" ? parent : parent + '\\';
            foreach (var path in paths)
            {
                if (fileSystem.IsDirectory(path))
                {
                    var t = fileSystem.GetChildItems(path).ToList();
                    parent += Path.GetFileName(path) + '\\';
                    Children(t, pts, parent);

                    var tmpPar = parent.Split('\\').ToList();
                    tmpPar.RemoveAt(tmpPar.Count - 2);
                    parent = string.Join("\\", tmpPar.ToArray());
                }
                if (!fileSystem.IsDirectory(path))
                {
                    pts.Add(new PackTask(path, parent + Path.GetFileName(path), ItemType.File));
                }
                else
                {
                    pts.Add(new PackTask(path, parent + Path.GetFileName(path), ItemType.Directory));
                }
            }
        }

        private void Children(List<ItemHeader> items, List<ItemHeader> children)
        {
            foreach (var item in items)
            {
                if (item is DirectoryHeader)
                {
                    var t = item as DirectoryHeader;
                    Children(t.Items, children);
                }

                children.Add(item);
            }
        }

        // apply tasks by creation of new package
        private void ApplyChanges(List<PackageWriterTask> tasks)
        {
            this.header = packageWriter.Write(this.packagePath.Insert(this.packagePath.Length - 4, "v2"), this.packagePath, tasks, _ => this.FileProcessing(this, _));
        }

        // delete several internal @paths 
        public void Delete(List<string> paths)
        {
            List<CopyTask> cts = new List<CopyTask>();

            var lst = paths.Select(a => a).ToList();
            var lr = new List<ItemHeader>();

            foreach (var item in Items)
            {
                if (lst.Contains(item.Path))
                {
                    if (item is DirectoryHeader)
                    {
                        Children(new List<ItemHeader> { item }, lr);
                    }
                    continue;
                }

                cts.Add(new CopyTask(item));
            }


            foreach (var l in lr)
            {
                if (cts.Select(a => a.Item).Contains(l))
                {
                    cts.Remove(cts.Where(a => a.Item == l).First());
                }
            }

            ApplyChanges(new List<PackageWriterTask>(cts));
        }

        // extract internal @paths to external @pathTo
        public void Extract(List<string> paths, string pathTo)
        {
            using (var input = fileSystem.Open(packagePath, FileMode.Open))
            {
                foreach (var path in paths)
                {
                    packageExtractor.ExtractFileTo(input, Items
                        .Where(it => it.Path == path).First(), pathTo, 
                        extPath => {
                            var e = new AskToOverwriteEventArgs { Path = extPath };
                            this.AskToOverwrite(this, e);
                            return e.Overwrite;
                        },
                        _ => this.FileProcessing(this, _));
                }
            }
        }

        public IList<ItemHeader> Items
        {
            get => this.header.Items.Select(i => i).ToList();
        }

        public DirectoryHeader RootDirectory
        {
            get => this.header.RootDirectory;
        }

        public event EventHandler<AskToOverwriteEventArgs> AskToOverwrite = delegate { };

        public event EventHandler<FileProcessingEventArgs> FileProcessing = delegate { };

        private string packagePath;
        private PackageHeader header = new PackageHeader();
        private readonly IFileSystem fileSystem;
        private readonly IPackageWriter packageWriter;
        private readonly IPackageExtractor packageExtractor;
    }
}