using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class ExternalInternalPath
    {
        public string InternalPath { get; set; }

        public string ExternalPath { get; set; }

        public ItemType Type { get; set; }
    }

    public class FileHelper
    {
        public FileHelper(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public List<ExternalInternalPath> CheckAndGetPaths(string currentPath, string packagePath, List<string> names)
        {
            var rootedPaths = names.Select(name => Path.IsPathRooted(name) ? name : Path.Combine(currentPath, name));

            var res = new List<ExternalInternalPath>();

            this.ProcessEntries(packagePath, rootedPaths, res);

            return res;
        }

        private void ProcessEntries(string internalPath, IEnumerable<string> paths, List<ExternalInternalPath> res)
        {
            foreach (var entry in paths)
            {
                var path = new ExternalInternalPath
                {
                    ExternalPath = entry,
                    InternalPath = Path.Combine(internalPath, Path.GetFileName(entry)),
                };

                if (this.fileSystem.IsDirectory(entry))
                {
                    this.ProcessEntries(path.InternalPath, this.fileSystem.GetChildItems(entry), res);
                    path.Type = ItemType.Directory;
                }
                else
                {
                    path.Type = ItemType.File;
                }

                // todo: implement checking of differency of internal path

                res.Add(path);
            };
        }

        private IFileSystem fileSystem;
    }
}
