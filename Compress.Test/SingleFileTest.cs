using Compress.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compress.Test
{
    class SingleFileTest
    {
        [Test]
        public void WriteReadTestFile1Test()
        {
            using (var input = new FileStream(GetPathFor(@"TestData\TestFile1.txt"), FileMode.Open))
            using (var output = new FileStream(GetPathFor(@"TestData\TestFile1.txt.z"), FileMode.Create))
            using (var ls = new LzwStreamWriter(output))
            {
                CopyStream(input, ls);
            }

            using (var input = new FileStream(GetPathFor(@"TestData\TestFile1.txt.z"), FileMode.Open))
            using (var ls = new LzwStreamReader(input))
            using (var output = new FileStream(GetPathFor(@"TestData\TestFile1.unpacked.txt"), FileMode.Create))
            {
                CopyStream(ls, output);
            }
        }

        [Test]
        public void WriteReadTestFile2Test()
        {
            using (var input = new FileStream(GetPathFor(@"TestData\TestFile2.fb2"), FileMode.Open))
            using (var output = new FileStream(GetPathFor(@"TestData\TestFile2.fb2.z"), FileMode.Create))
            using (var ls = new LzwStreamWriter(output))
            {
                CopyStream(input, ls);
            }

            using (var input = new FileStream(GetPathFor(@"TestData\TestFile2.fb2.z"), FileMode.Open))
            using (var ls = new LzwStreamReader(input))
            using (var output = new FileStream(GetPathFor(@"TestData\TestFile2.unpacked.fb2"), FileMode.Create))
            {
                CopyStream(ls, output);
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        private string GetPathFor(string name)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, name);
        }
    }
}
