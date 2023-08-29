using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class BitWriter
    {
        private List<byte> arr = new List<byte>();
        private int bitPos = 0;

        public void Clear()
        {
            bitPos = 0;
            arr.Clear();
        }
        public void ClearRound()
        {
            bitPos = bitPos - 8 * (bitPos / 8);
            arr.RemoveRange(0, arr.Count - 1);
        }

        public byte[] GetBytes()
        {
            var tmpArr = arr.Select(a => a).ToList();
            Clear();
            return tmpArr.ToArray();
        }

        private void SetBit(int pos, bool what)
        {
            //++countBit;
            var posByte = pos / 8;
            var posBit = pos % 8;

            while (pos >= 8 * this.arr.Count)
                this.arr.Add(0);

            if (what)
                this.arr[posByte] |= (byte)(1 << 7 - posBit);
            else
                this.arr[posByte] &= (byte)(0xFF << 7 - posBit);

        }

        public void Write(ulong code, int bitCount)
        {
            var tm = Enumerable.Range(0, bitCount)
                .Select(bq => ((ulong)code >> bq) & 1)
                .Reverse()
                .ToList();

            for (int i = 0; i < tm.Count; i++)
            {
                this.SetBit(this.bitPos, Convert.ToBoolean(tm[i]));
                this.bitPos++;
            }
        }

        public byte[] GetRoundBytes()
        {
            if (bitPos % 8 == 0)
            {
                return GetBytes();
            }
            else
            {
                List<byte> t = this.arr.Select(a => a).ToList();
                t.RemoveAt(t.Count-1);
                //kolvoByte = t.Count;
                ClearRound();
                return t.ToArray();
            }
        }

        public byte[] GetLastBytes(int bitLength)
        {
            //if (kolvoByte == 0)
            //{
            //    int corr = 2;

            //    while (corr * 8 < bitLength - 1)
            //    {
            //        corr++;
            //    }

            //    return arr.Skip(arr.Count - corr).ToArray();
            //}

            //return arr.Skip(kolvoByte).ToArray();

            return arr.ToArray();
        }

        public byte[] GetAllBytes()
        {
            return this.arr.ToArray();
        }
    }
}
