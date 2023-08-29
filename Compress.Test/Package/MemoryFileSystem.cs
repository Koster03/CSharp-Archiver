using Compress.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Test.Package
{
    public class MemoryFileSystem : BaseFileSystem
    {
        abstract class MemoryItem
        {
            public string Path { get; set; }

            public MemoryDirectory Parent { get; set; }

            public DateTime Modified { get; set; } = DateTime.UtcNow;
        }

        class MemoryDirectory : MemoryItem
        {
            public MemoryDirectory(string path, MemoryDirectory parent)
            {
                this.Path = PathHelper.RemoveEndSeparator(path);
                this.Parent = parent;
                if (parent != null)
                    parent.Children.Add(this);
            }

            public List<MemoryItem> Children { get; set; } = new List<MemoryItem>();
        }

        class MemoryFile : MemoryItem
        {
            public MemoryFile(string path, MemoryDirectory parent)
            {
                this.Path = path;
                this.Parent = parent;
                parent.Children.Add(this);
            }

            public MemoryStream Stream { get; set; } = new MemoryStream();
        }

        public MemoryFileSystem(string rootPath)
        {
            if (!Path.IsPathRooted(rootPath))
                throw new ArgumentException($"Path {rootPath} is not rooted");

            this.RootDirectory = new MemoryDirectory(rootPath, null);
            this.AddItem(this.RootDirectory);
        }

        public override void CreateDirectory(string path)
        {
            path = this.CheckAndRootPath(path);

            foreach (string dir in this.GetPathComponents(path).Reverse())
            {
                if (this.FileExists(dir))
                    throw new ArgumentException($"Directory {path} can not be created because file {dir} exists");

                if (!this.DirectoryExists(dir))
                {
                    var parentDir = this.GetRequiredItem(Path.GetDirectoryName(dir)) as MemoryDirectory;
                    var newDirectory = new MemoryDirectory(dir, parentDir);
                    this.AddItem(newDirectory);
                }
            }
        }

        public override void Delete(string path)
        {
            path = this.CheckAndRootPath(path);

            if (this.FileExists(path) || this.DirectoryExists(path))
            {
                var item = this.GetRequiredItem(path);
                if (item.Parent == null)
                    throw new ArgumentException("Root directory can not be deleted");

                item.Parent.Children.Remove(item);
                this.RemoveItem(item);
            }
        }

        public override bool DirectoryExists(string path)
        {
            path = this.CheckAndRootPath(path);

            var item = this.GetItem(path);
            return item != null && item is MemoryDirectory;
        }

        public override bool FileExists(string path)
        {
            path = this.CheckAndRootPath(path);

            var item = this.GetItem(path);
            return item != null && item is MemoryFile;
        }

        public override IList<string> GetChildItems(string path)
        {
            path = this.CheckAndRootPath(path);

            if (!this.DirectoryExists(path))
                throw new FileNotFoundException("Directory {path} doesn't exist");

            var dir = this.GetItem(path) as MemoryDirectory;
            return dir.Children.Select(i => i.Path).ToList();
        }

        public override long GetFileLength(string path)
        {
            path = this.CheckAndRootPath(path);

            if (!this.FileExists(path))
                throw new FileNotFoundException("File {path} doesn't exist");

            var file = this.GetItem(path) as MemoryFile;
            return file.Stream.ToArray().Length;
        }

        public override DateTime GetLastWriteTimeUtc(string path)
        {
            path = this.CheckAndRootPath(path);

            if (!this.FileExists(path) && !this.DirectoryExists(path))
                throw new FileNotFoundException("Item {path} doesn't exist");

            var item = this.GetItem(path);
            return item.Modified;
        }

        public override bool IsDirectory(string path)
        {
            path = this.CheckAndRootPath(path);

            if (!this.FileExists(path) && !this.DirectoryExists(path))
                throw new FileNotFoundException("Item {path} doesn't exist");

            return this.DirectoryExists(path);
        }

        public override void Move(string from, string to)
        {
            from = this.CheckAndRootPath(from);
            to = this.CheckAndRootPath(to);

            if (this.FileExists(to) || this.DirectoryExists(to))
                return;

            if (!this.FileExists(from) && !this.DirectoryExists(from))
                return;

            var item = this.GetItem(from);
            this.Delete(from);
            if (item is MemoryDirectory)
                this.CreateDirectory(to);
            else if (item is MemoryFile file)
            {
                var toDir = Path.GetDirectoryName(to);
                this.CreateDirectory(toDir);
                var dir = this.GetItem(toDir) as MemoryDirectory;
                var newFile = new MemoryFile(to, dir)
                {
                    Stream = file.Stream,
                };
                this.AddItem(newFile);
            }
        }

        public override Stream Open(string path, FileMode mode)
        {
            path = this.CheckAndRootPath(path);

            MemoryFile file;
            switch (mode)
            {
                case FileMode.Append:
                    {
                        if (!this.FileExists(path))
                            throw new FileNotFoundException("File {path} doesn't exist");

                        file = this.GetItem(path) as MemoryFile;
                        file.Stream = new MemoryStream(file.Stream.ToArray());
                        file.Stream.Position = file.Stream.Length;
                        break;
                    }

                case FileMode.Create:
                    {
                        if (this.FileExists(path))
                        {
                            file = this.GetItem(path) as MemoryFile;
                            file.Stream = new MemoryStream();
                        }
                        else
                        {
                            file = CreateFile(path);
                        }
                        break;
                    }

                case FileMode.CreateNew:
                    {
                        if (this.FileExists(path))
                            throw new FileNotFoundException("File {path} already exists");

                        file = CreateFile(path);
                        break;
                    }

                case FileMode.Open:
                    {
                        if (!this.FileExists(path))
                            throw new FileNotFoundException("File {path} doesn't exist");

                        file = this.GetItem(path) as MemoryFile;
                        file.Stream = new MemoryStream(file.Stream.ToArray());
                        file.Stream.Position = 0;
                        break;
                    }

                default:
                case FileMode.OpenOrCreate:
                    {
                        if (!this.FileExists(path))
                            file = CreateFile(path);
                        else
                        {
                            file = this.GetItem(path) as MemoryFile;
                            file.Stream = new MemoryStream(file.Stream.ToArray());
                            file.Stream.Position = 0;
                        }
                        break;
                    }

                case FileMode.Truncate:
                    {
                        if (!this.FileExists(path))
                            throw new FileNotFoundException("File {path} doesn't exist");

                        file = this.GetItem(path) as MemoryFile;
                        file.Stream = new MemoryStream();
                        break;
                    }
            }

            return file.Stream;
        }

        private MemoryFile CreateFile(string path)
        {
            var dirPath = Path.GetDirectoryName(path);
            this.CreateDirectory(dirPath);
            var dir = this.GetItem(dirPath) as MemoryDirectory;
            var file = new MemoryFile(path, dir);
            this.AddItem(file);

            return file;
        }

        private string CheckAndRootPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                if (!PathHelper.IsBaseOf(this.RootDirectory.Path, path))
                {
                    throw new ArgumentException($"Path {path} is not a child path of root memory directory {this.RootDirectory.Path}");
                }
            }
            else
            {
                path = Path.Combine(this.RootDirectory.Path, path);
            }

            return PathHelper.RemoveEndSeparator(path);
        }

        private IEnumerable<string> GetPathComponents(string path)
        {
            while (path != this.RootDirectory.Path)
            {
                yield return path;
                path = Path.GetDirectoryName(path);
            }
        }

        private MemoryItem GetItem(string path)
        {
            if (this.itemDictionary.TryGetValue(path, out var item))
                return item;

            return null;
        }

        private MemoryItem GetRequiredItem(string path)
        {
            var item = this.GetItem(path);
            if (item == null)
                   throw new KeyNotFoundException("There is no item with path {path}");

            return item;
        }

        private void AddItem(MemoryItem item)
        {
            this.itemDictionary.Add(item.Path, item);
        }

        private void RemoveItem(MemoryItem item)
        {
            this.itemDictionary.Remove(item.Path);
            if (item is MemoryDirectory dir)
                foreach (var child in dir.Children)
                    this.RemoveItem(child);
        }
        
        private MemoryDirectory RootDirectory { get; }
        private Dictionary<string, MemoryItem> itemDictionary = new Dictionary<string, MemoryItem>();
    }
}
