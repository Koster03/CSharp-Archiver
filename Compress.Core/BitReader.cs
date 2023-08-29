using System;
using System.Collections.Generic;
using System.Linq;

namespace Compress.Core
{
    public class BitReader
    {
        private List<bool> bitMass = new List<bool>();

        public void PutBytes(byte[] data, int offset, int length)
        {
            var TmpbitMass = this.setMassBits(data.ToList(), offset, length);
            bitMass.AddRange(TmpbitMass);
        }

        public void Clear()
        {
            bitMass.Clear();
        }

        private List<bool> setMassBits(List<byte> arr, int offset, int length)
        {
            var listBits = new List<bool>();

            for (int i = offset; i < length; i++)
            {
                var tmp = Enumerable.Range(0, 8)
                .Select(bq => (arr[i] >> bq) & 1)
                .Reverse()
                .ToList();

                for (int j = 0; j < tmp.Count; j++)
                {
                    if (tmp[j] == 1)
                        listBits.Add(true);
                    else
                        listBits.Add(false);
                }
            }

            return listBits;
        }
        private void SetBit(List<byte> arr, int pos, int whatByte, bool what)
        {
            if (what)
                arr[whatByte] |= (byte)(1 << 7 - pos);
            else
                arr[whatByte] &= (byte)(0xFF >> pos + 1);

        }

        public bool TryRead(int bitCount, out ulong code)
        {
            if (this.bitMass.Count < bitCount)
            {
                code = 0;
                return false;
            }

            var o = BitConverter.GetBytes((ulong)0).ToList();

            int wtByte = 0, pos = 7, cor = 1;

            bitCount = bitCount > 64 ? 64: bitCount;

            for (int i = bitCount - 1; i >= 0; i--)
            {

                if (i < (bitCount - cor * 8))
                {
                    cor++;
                    wtByte++;
                    pos = 7;
                }

                SetBit(o, pos, wtByte, bitMass[i]);

                bitMass.RemoveAt(i);
                pos--;
            }

            code = ConvertToLong(o);
            return true;
        }

        private static ulong ConvertToLong(List<byte> arr)
        {
            ulong num = 0;

            for (int i = 7; i >= 0; --i)
            {
                num = (num << 8) | arr[i];
            }

            return num;
        }
    }
}
