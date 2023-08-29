using Compress.Core;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace Compress.Test
{
    public class LzwPackUnpackTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BookTest()
        {
            var data = new byte[] { 45, 55, 55, 151, 55, 55, 55 };

            var packed = new LzwPacker().Pack(data);

            var codes = BinaryUtility.ConvertToUlong(packed);

            var unpacked = new LzwUnpacker().Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void BasicCodesTest()
        {
            var data = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();

            var packed = new LzwPacker().Pack(data);

            var unpacked = new LzwUnpacker().Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void RepeatBasicCodesTest()
        {
            var data = Enumerable.Range(0, 1000000).Select(i => (byte)i).ToArray();

            var packed = new LzwPacker().Pack(data);

            var unpacked = new LzwUnpacker().Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void LinearCodesTest()
        {
            var data = Enumerable.Range(0, 100000).SelectMany(i => BitConverter.GetBytes(i)).ToArray();

            string path = @"E:\Files\";
            var transporter = new CryptoStreamTransporter();
            var packer = new LzwPacker(new LzwAlgoParams { MaxCodeBitCount = 20 });

            using (var input = new FileStream(path + "TestFile2.fb2", FileMode.Open))
            using (var output = new FileStream(path + "outputFileName.z", FileMode.Create))
            {
                transporter.Pack(input, output, input.Length, packer);
            }

            using (var input = new FileStream(path + "outputFileName.z", FileMode.Open))
            using (var output = new FileStream(path + "hmm.fb2", FileMode.Create))
            {
                transporter.Unpack(input, output, input.Length, new LzwUnpacker());
            }

            var packed = new LzwPacker().Pack(data);

            var unpacked = new LzwUnpacker().Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void StepsCodesTest()
        {
            var data = Enumerable.Range(0, 100000).SelectMany(i => Enumerable.Range(1, 10).Select(j =>(byte)i)).ToArray();

            var packed = new LzwPacker().Pack(data);

            var unpacked = new LzwUnpacker().Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void RepeatCodesTest()
        {
            var data = Enumerable.Range(0, 1000).SelectMany(i => Enumerable.Range(1, 10).Select(j => (byte)j)).ToArray();

            var packed = new LzwPacker(new LzwAlgoParams { MaxCodeBitCount = 9 }).Pack(data);

            var unpacked = new LzwUnpacker().Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void RandomCheckTest()
        {
            int repeats = 50;
            int minLength = 1;
            int maxLength = 200000;

            for (int i = 0; i < repeats; i++)
            {
                var data = generateRandomBytes(minLength, maxLength);
                var packed = new LzwPacker(new LzwAlgoParams { MaxCodeBitCount = 12 }).Pack(data);
                var unpacked = new LzwUnpacker().Unpack(packed);

                CollectionAssert.AreEqual(data, unpacked);
            }
        }

        private byte[] generateRandomBytes(int minLength, int maxLength)
        {
            var r = new Random();
            var res = new byte[r.Next(minLength, maxLength + 1)];
            for (int i = 0; i < res.Length; i++)
                res[i] = (byte)r.Next(256);

            return res;
        }
    }
}