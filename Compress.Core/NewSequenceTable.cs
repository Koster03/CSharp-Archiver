using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class NewSequenceTable
    {
        public Dictionary<ulong, ulong> tableCompress = new Dictionary<ulong, ulong>();
        public Dictionary<ulong, ulong> tableDecompress = new Dictionary<ulong, ulong>();

        private int bitCount = 9;

        //public int CurrentBitLength
        //{
        //    get
        //    {
        //        int maxCount = this.tableCompress.Count >= this.tableDecompress.Count ? (this.tableCompress.Count + 2) : (this.tableDecompress.Count + 3);
        //        return (int)Math.Truncate(Math.Log(maxCount) / Math.Log(2)) + 1;
        //    }
        //}

        public int CurrentBitLength => (this.tableCompress.Count >= (int)Math.Pow(2, bitCount)) ? ++bitCount : bitCount;

        public int NextBitLength
        {
            get { return CurrentBitLength + 1; }
        }

        public ulong AddSequence(ulong previousSecCode, byte ch)
        {
            return (previousSecCode << 8) ^ ch;
        }

        public ulong GetCode(ulong SecCode)
        {
            return tableCompress.FirstOrDefault(x => x.Value == SecCode).Key;
        }

        public ulong FindSequence(ulong previousSecCode, byte ch)
        {
            ulong seq = ((previousSecCode << 8) ^ ch);
            if (tableCompress.TryGetValue(seq, out ulong code))
            {
                return code;
            }
            else
            {
                return ulong.MaxValue;
            }

        }

        public byte[] GetSequence(ulong code)
        {
            ulong prefix = code >> 8;
            ulong ch = code & 0xFF;
            var list = new List<byte>
            {
                (byte)ch
            };

            while (prefix != (ulong)SpecialSeqCodes.Clear)
            {
                ulong seq = tableDecompress[prefix];
                list.Add((byte)(seq & 0xFF));
                prefix = seq >> 8;
            }

            list.Reverse();

            return list.ToArray();
        }

        public ulong GetCodeDecomp(ulong SecCode)
        {
            return tableDecompress.FirstOrDefault(x => x.Value == SecCode).Key;
        }

        public void Init()
        {
            this.tableCompress.Clear();
            for (int i = 0; i < 256; i++)
            {
                this.tableCompress.Add((ulong)(((int)SpecialSeqCodes.Clear << 8) ^ i), (ulong)i);// старшие 8 - префикс, младшие - символ
            }

            this.tableDecompress.Clear();
            for (int i = 0; i < 256; i++)
            {
                this.tableDecompress.Add((ulong)i, (ulong)(((int)SpecialSeqCodes.Clear << 8) ^ i));// старшие 8 - префикс, младшие - символ
            }
        }
    }
}
