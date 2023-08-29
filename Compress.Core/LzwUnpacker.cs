using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class LzwUnpacker : ICryptoUnpacker
    {
        public LzwUnpacker()
        {
            this.bitReader = new BitReader();
            this.table = new SequenceTable();
            this.oldCode = (ulong)SpecialSeqCodes.Clear;
        }

        public LzwUnpacker(LzwAlgoParams parameters)
            : this()
        {
            this.parameters = parameters;
        }

        public byte[] Unpack(byte[] data)
        {
            return this.Unpack(data, 0, data.Length);
        }

        private CryptoStreamTransporter repestCheck = new CryptoStreamTransporter();

        public byte[] Unpack(byte[] packed, int offset, int length)
        {
            //var table = new SequenceTable();
            if (repestCheck.Repeat == 1)
            {
                table.InitTable();
            }

            var OutFile = new List<byte>();

            //var bitReader = new BitReader();
            bitReader.PutBytes(packed, offset, length);

            //ulong old_code = (ulong)SpecialSeqCodes.Clear;

            while (bitReader.TryRead(table.NextBitLength, out ulong code))
            {
                if (table.ContainsKey(code))
                {
                    OutFile.AddRange(table.GetByte(table.GetSequence(code)));

                    if (oldCode != (ulong)SpecialSeqCodes.Clear)
                    {
                        table.AddSequence(table.GetSequence(oldCode), table.GetFirst(table.GetSequence(code)));
                    }

                    oldCode = code;
                }
                else
                {
                    if (oldCode != (ulong)SpecialSeqCodes.Clear)
                    {
                        string OutString = table.AddSequence(table.CreateKey(table.GetSequence(oldCode),
                            table.GetFirst(table.GetSequence(oldCode))));

                        OutFile.AddRange(table.GetByte(OutString));
                    }

                    oldCode = code;
                }

                if (table.NextBitLength == parameters.MaxCodeBitCount)
                {
                    table.InitTable();
                    this.oldCode = (ulong)SpecialSeqCodes.Clear;
                }
            }

            return OutFile.ToArray();
        }

        public ICryptoUnpacker Clean()
        {
            bitReader.Clear();
            oldCode = 256;
            table.table = null;
            table.bitCount = 9;
            return this;
        }

        //private LzwAlgoParams parameters = LzwAlgoParams.Default;
        private LzwAlgoParams parameters;
        private BitReader bitReader;
        private SequenceTable table;
        private ulong oldCode;
    }
}
