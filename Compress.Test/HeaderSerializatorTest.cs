using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Compress.Package;
using NUnit.Framework;

namespace Compress.Test
{
    class HeaderSerializatorTest
    {
        [Test]
        public void WriteReadTestFile1Test()
        {
            var header = new PackageHeader
            {
                Items = new List<ItemHeader>()
                {
                    new FileHeader
                    {
                        Path = @"SomeFileName.txt",
                        PackedLength = 18832,
                        UnpackedLength = 67274,
                        Modified = new DateTime(2008, 5, 1, 8, 30, 52),
                    },
                    new DirectoryHeader
                    {
                        Path = @"Folder",
                        Modified = new DateTime(2009, 10, 20, 14, 45, 1),
                    },
                    new FileHeader
                    {
                        Path = @"Folder\Another file.fb2",
                        PackedLength = 103447,
                        UnpackedLength = 33458,
                    },
                    new DirectoryHeader
                    {
                        Path = @"Folder\Papka",
                        Modified = new DateTime(2020, 2, 20, 14, 12, 0),
                    },
                    new FileHeader
                    {
                        Path = @"Folder\Caption.jpg",
                        PackedLength = 54377,
                        UnpackedLength = 52959,
                    },
                    new FileHeader
                    {
                        Path = @"Folder\Papka\TestFile.txt",
                        PackedLength = 25000,
                        UnpackedLength = 30000,
                    },
                },
            };

            using (var ms = new MemoryStream())
            {
                var serializator = new HeaderSerializator();
                serializator.Save(ms, header);
                ms.Position = 0;
                var loadedHeader = serializator.Load(ms);

                AreEqualByJson(header, loadedHeader);
            }
        }
            
        public static void AreEqualByJson(object expected, object actual)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var expectedJson = serializer.Serialize(expected);
            var actualJson = serializer.Serialize(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}
