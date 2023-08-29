using System.Linq;

namespace Compress.Core
{
    public class LzwPacker : ICryptoPacker
    {
        public LzwPacker()
        {
            this.bitWriter = new BitWriter();
            this.table = new SequenceTable();
            //tbl = new NewSequenceTable();
            this.currentSeq = ((ulong)SpecialSeqCodes.Clear).ToString();
        }

        public LzwPacker(LzwAlgoParams parameters)
            : this()
        {
            this.parameters = parameters;
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

        private CryptoStreamTransporter repestCheck = new CryptoStreamTransporter();
        public byte[] Pack(byte[] data, int offset, int length)
        {
            //var tabley = new SequenceTable();

            if (repestCheck.Repeat == 1)
            {
                table.InitTable();
                table.p_pow.Clear();
                table.InitPpow();
                table.Hesh();
            }

            for (int i = offset; i < length; i++)
            {
                //string newSeq = table.FindSequence(currentSeq, data[i]);

                string newSeq = table.FindPlz(currentSeq, data[i]);

                if (newSeq != ((ulong)SpecialSeqCodes.NotFound).ToString())
                {
                    currentSeq = newSeq;
                }
                else
                {
                    bitWriter.Write(table.GetCode(currentSeq), table.CurrentBitLength);
                    table.AddSequence(currentSeq, data[i]);
                    table.AddPlz(currentSeq, data[i]);
                    currentSeq = data[i].ToString();
                }

                if (table.CurrentBitLength == this.parameters.MaxCodeBitCount)
                {
                    table.InitTable();
                }
            }

            
            //var tbl = new NewSequenceTable();

            //if (repestCheck.Repeat == 1)
            //{
            //    tbl.Init();
            //}

            //currentSeq = (ulong)SpecialSeqCodes.Clear;


            //for (int i = offset; i < length; i++)
            //{
            //    ulong newSeq = tbl.FindSequence(currentSeq, data[i]);

            //    if (newSeq != ulong.MaxValue)
            //    {
            //        currentSeq = tbl.AddSequence(currentSeq, data[i]);
            //    }
            //    else
            //    {
            //        bitWriter.Write(currentSeq, tbl.CurrentBitLength);

            //        currentSeq = tbl.AddSequence(currentSeq, data[i]);
            //        tbl.tableCompress.Add(currentSeq, data[i]);
            //        currentSeq = (ulong)data[i];
            //    }

            //    if (tbl.CurrentBitLength == this.parameters.MaxCodeBitCount)
            //    {
            //        tbl.Init();
            //    }
            //}


            return bitWriter.GetRoundBytes();
        }

        public byte[] GetLastSegment()
        {
            bitWriter.Write(table.GetCode(currentSeq), table.CurrentBitLength);
            return bitWriter.GetLastBytes(table.CurrentBitLength);

            //bitWriter.Write(currentSeq, tbl.CurrentBitLength);
            //return bitWriter.GetLastBytes(tbl.CurrentBitLength);
        }

        public ICryptoPacker Clean()
        {
            this.table.table = null;
            this.table.HashedTable.Clear();
            table.p_pow.Clear();
            this.table.bitCount = 9;
            bitWriter.Clear();
            currentSeq = "256";
            return this;
        }

        //private LzwAlgoParams parameters = LzwAlgoParams.Default;
        private LzwAlgoParams parameters;
        private string currentSeq;
        //private ulong currentSeq;
        private BitWriter bitWriter;
        private SequenceTable table;
        //private NewSequenceTable tbl;
    }
}
