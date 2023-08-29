using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

using Compress.Core;

namespace Compress.Test
{
    class BitReaderWriterTest
    {

        [Test]
        public void BitWriterTest1()
        {
            var bitWriter = new BitWriter();
            bitWriter.Write(0, 3);
            bitWriter.Write(0b1111111, 7);
            var res = bitWriter.GetAllBytes();
        }

        private ulong GetAllBitsFor(int bitCount)
        {
            ulong res = 0;
            for (int i = 0; i < bitCount; i++)
                res |= 1ul << i;

            return res;
        }

        [Test]
        public void BitWriterTest2()
        {
            var bitWriter = new BitWriter();
            bitWriter.Write(0, 1);
            bitWriter.Write(GetAllBitsFor(24), 24);
            var res = bitWriter.GetAllBytes();
        }

        [Test]
        public void BitWriterTest3()
        {
            var bitWriter = new BitWriter();
            bitWriter.Write(0, 3);
            bitWriter.Write(GetAllBitsFor(37), 37);
            var res = bitWriter.GetAllBytes();
        }

        [Test]
        public void BitWriterTest4()
        {
            var bitWriter = new BitWriter();
            bitWriter.Write(GetAllBitsFor(16), 16);
            var res = bitWriter.GetAllBytes();
        }


        [Test]
        public void BitWriterTest5()
        {
            var bitWriter = new BitWriter();
            bitWriter.Write(0, 1);
            bitWriter.Write(GetAllBitsFor(2), 2);
            bitWriter.Write(GetAllBitsFor(2), 2);
            bitWriter.Write(GetAllBitsFor(2), 2);
            bitWriter.Write(GetAllBitsFor(2), 2);
            var res = bitWriter.GetAllBytes();
        }

        [Test]
        public void BitReaderTest1()
        {
            //var bitReader = new BitReader();
            //bitReader.PutBytes(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, });
            //ulong l = 0;
            //bitReader.TryRead(1, out l);
            //bitReader.TryRead(1, out l);
            //bitReader.TryRead(2, out l);
            //bitReader.TryRead(2, out l);
            //bitReader.TryRead(4, out l);
            //bitReader.TryRead(4, out l);
            //bitReader.TryRead(6, out l);
            //bitReader.TryRead(6, out l);
            //bitReader.TryRead(8, out l);
            //bitReader.TryRead(8, out l);
        }

        [Test]
        public void BitReaderTest2()
        {
            ////var bitReader = new BitReader();
            ////bitReader.PutBytes(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, });
            ////ulong l = 0;
            ////bitReader.TryRead(2, out l);
            ////Assert.AreEqual(l, GetAllBitsFor(2));
            ////bitReader.TryRead(14, out l);
            ////Assert.AreEqual(l, GetAllBitsFor(14));
            ////bitReader.TryRead(14, out l);
            ////Assert.AreEqual(l, GetAllBitsFor(14));
            ////bitReader.TryRead(44, out l);
            ////Assert.AreEqual(l, GetAllBitsFor(44));
        }


        [Test]
        public void BitReaderTest3()
        {
            //var bitReader = new BitReader();
            //bitReader.PutBytes(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, });
            //ulong l = 0;
            //bitReader.TryRead(74, out l);
            //Assert.AreEqual(l, GetAllBitsFor(74));
            //bool b = bitReader.TryRead(74, out l);
            //Assert.AreEqual(l, 0);
            //Assert.AreEqual(b, false);
        }
    }
}
