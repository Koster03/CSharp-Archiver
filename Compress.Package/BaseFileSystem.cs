using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public abstract class BaseFileSystem : IFileSystem
    {
        public abstract void CreateDirectory(string path);
        public abstract void Delete(string path);
        public abstract bool DirectoryExists(string path);
        public abstract bool FileExists(string path);
        public abstract IList<string> GetChildItems(string path);
        public abstract long GetFileLength(string path);
        public abstract DateTime GetLastWriteTimeUtc(string path);
        public abstract bool IsDirectory(string path);
        public abstract void Move(string from, string to);
        public abstract Stream Open(string path, FileMode mode);

        public void CreateTextFile(string path, string content)
        {
            using (var stream = this.Open(path, FileMode.OpenOrCreate))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(content);
            }
        }

        public string ReadTextFile(string path)
        {
            using (var stream = this.Open(path, FileMode.Open))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
