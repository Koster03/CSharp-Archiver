using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    class LzwPackerOnNewTable : ICryptoPacker
    {
        private LzwAlgoParams parameters = LzwAlgoParams.Default;
        private ulong currentSeq;
        private BitWriter bitWriter;
        private NewSequenceTable table2;
        public LzwPackerOnNewTable()
        {
            this.bitWriter = new BitWriter();
            this.table2 = new NewSequenceTable();
            this.currentSeq = (int)SpecialSeqCodes.Clear;
        }

        public ICryptoPacker Clean()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public byte[] GetLastSegment()
        {
            //bitWriter.Write(table2.GetCode(currentSeq), table2.CurrentBitLength);
            return bitWriter.GetLastBytes(table2.CurrentBitLength);
        }

        public byte[] Pack(byte[] data)
        {
            var firstSegment = this.Pack(data, 0, data.Length);
            var lastSegment = this.GetLastSegment();

            // return lastSegment;

            bitWriter.Clear();

            if (lastSegment.Length == 0)
                return firstSegment;
            else
                return firstSegment.Concat(lastSegment).ToArray();
        }

        public byte[] Pack(byte[] data, int offset, int length)
        {
            var table2 = new NewSequenceTable();
            table2.Init();
            //var bitWriter = new BitWriter();

            //string currentSeq = ((ulong)SpecialSeqCodes.Clear).ToString();

            for (int i = offset; i < length; i++)
            {
                ulong newSeq = table2.FindSequence(currentSeq, data[i]);

                if (newSeq != ulong.MaxValue)
                {
                    currentSeq = newSeq;
                }
                else
                {
                    bitWriter.Write(currentSeq, table2.CurrentBitLength);
                    table2.AddSequence(currentSeq, data[i]);
                    currentSeq = table2.GetCode(currentSeq);
                }
            }

            //foreach (var ch in data)
            //{
            //    string newSeq = table.FindSequence(currentSeq, ch);

            //    if (newSeq != ((ulong)SpecialSeqCodes.NotFound).ToString())
            //    {
            //        currentSeq = newSeq;
            //    }
            //    else
            //    {
            //        bitWriter.Write(table.GetCode(currentSeq), table.CurrentBitLength);
            //        table.AddSequence(currentSeq, ch);
            //        currentSeq = ch.ToString();
            //    }

            //    if (table.CurrentBitLength == this.parameters.MaxCodeBitCount)
            //    {
            //        table.InitTable();
            //    }
            //}

            //bitWriter.Write(table.GetCode(currentSeq), table.CurrentBitLength);
            //return bitWriter.GetBytes();
            //lastCode = table.GetCode(currentSeq);
            // return bitWriter.GetAllBytes();
            return bitWriter.GetRoundBytes();
        }
    }
}
