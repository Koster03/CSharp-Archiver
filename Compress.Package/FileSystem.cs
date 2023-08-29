using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Compress.Package
{
    public class FileSystem : BaseFileSystem
    {
        public override void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public override void Delete(string path)
        {
            if (this.IsDirectory(path))
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
            }
        }

        private void CreateFile(string path)
        {
            var dirPath = Path.GetDirectoryName(path);
            this.CreateDirectory(dirPath);
        }

        public override bool DirectoryExists(string path)
        {
            return Directory.Exists(path) ? true : false;
        }

        public override bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public override IList<string> GetChildItems(string path)
        {
            return Directory.GetFiles(path).Concat(Directory.GetDirectories(path)).ToList();
        }

        public override long GetFileLength(string path)
        {
            FileInfo fileInf = new FileInfo(path);
            return fileInf.Length;
        }

        public override DateTime GetLastWriteTimeUtc(string path)
        {
            if (IsDirectory(path))
            {
                return Directory.GetLastWriteTimeUtc(path);
            }

            return File.GetLastWriteTimeUtc(path);
        }

        public override bool IsDirectory(string path)
        {
            return Directory.Exists(PathHelper.RemoveEndSeparator(path)) ? true : false;
        }

        public override void Move(string from, string to)
        {
            if (FileExists(to))
            {
                throw new FileNotFoundException("File {to} already exist");
            }
            File.Move(from, to);
        }

        public override Stream Open(string path, FileMode mode)
        {
            if (!this.FileExists(path))
                CreateFile(path);

            return File.Open(path, mode);
        }
    }
}
