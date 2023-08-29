using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    class SequenceTable
    {
        public Dictionary<ulong, string> table; // Код строки - строка

        public Dictionary<ulong, ulong> HashedTable = new Dictionary<ulong, ulong>(); // Хэш строки - код строки

        public int bitCount = 9;

        public void InitTable()
        {
            table = new Dictionary<ulong, string>();

            for (int i = 0; i <= 0xFF; i++)
            {
                this.AddSequence(((ulong)SpecialSeqCodes.Clear).ToString(), (byte)i);
            }

            this.AddToTable(((ulong)SpecialSeqCodes.NotFound).ToString());
            this.AddToTable(((ulong)SpecialSeqCodes.Eof).ToString());
        }

        public string AddSequence(string previousSec, byte ch)
        {
            string key = this.CreateKey(previousSec, ch);
            return this.AddToTable(key);
        }

        private string AddToTable(string key)
        {
            table.Add((ulong)table.Count, key);
            return key;
        }

        public string AddPlz(string previousSec, byte ch)
        {
            ulong key = GetHash(CreateKey(previousSec, ch));
            HashedTable.Add(key, (ulong)HashedTable.Count);

            return CreateKey(previousSec, ch);
        }

        public void Hesh()
        {
            ulong code = 0;

            foreach (var item in table.Values)
            {
                HashedTable.Add(GetHash(item), code++);
            }

        }

        public Dictionary<ulong, ulong> p_pow = new Dictionary<ulong, ulong>();

        public void InitPpow()
        {
            uint p = 31;

            p_pow.Add(0, 1);

            for (uint i = 1; i < 10000; ++i)
                p_pow.Add(i, p_pow[i - 1] * p);
        }
        private ulong GetHash(string item)
        {
            ulong hash = 0;

            for (int j = 0; j < item.Length; ++j)
            {
                hash += (ulong)(item[j] - 'a' + 1) * p_pow[(ulong)j];
            }

            return hash;
        }
        
        public string FindPlz(string previousSecCode, byte ch)
        {
            if (previousSecCode == ((ulong)SpecialSeqCodes.Clear).ToString())
                return ch.ToString();


            if (HashedTable.TryGetValue(GetHash(previousSecCode + ' ' + ch.ToString()), out ulong val))
            {
                return table[val];
            }
            else
            {
                return ((ulong)SpecialSeqCodes.NotFound).ToString();
            }

            //if (HashedTable.ContainsKey(GetHash(previousSecCode + ' ' + ch.ToString())))
            //    return previousSecCode + ' ' + ch.ToString();
        }

        public string AddSequence(string previousSecCode)
        {
            return this.AddToTable(previousSecCode);
        }

        public string AddSequence(string previousSec, string curSeq)
        {
            return this.AddToTable(previousSec + ' ' + curSeq);
        }

        public ulong GetCode(string seq)
        {
            return this.table.FirstOrDefault(x => x.Value == seq).Key;
        }

        public string GetFirst(string seq)
        {
            var str = "";
            for (int i = 0; i < seq.Length; i++)
            {
                if (seq[i] == ' ')
                    break;

                str += seq[i];

                if (str.Length == 3)
                    break;
            }

            return str;
        }

        private string CreateKey(string previousSecCode, byte ch)
        {
            if (previousSecCode == ((ulong)SpecialSeqCodes.Clear).ToString())
                return ch.ToString();

            return previousSecCode + ' ' + ch.ToString();
        }

        public string CreateKey(string previousSec, string curSec)
        {
            return previousSec + ' ' + curSec;
        }

        public int CurrentBitLength => (this.table.Count >= (int)Math.Pow(2, bitCount)) ? ++bitCount : bitCount;

        public int NextBitLength => (this.table.Count >= (int)Math.Pow(2, bitCount) - 1) ? ++bitCount : bitCount;

        public string FindSequence(string previousSecCode, byte ch)
        {
            if (previousSecCode == ((ulong)SpecialSeqCodes.Clear).ToString())
                return ch.ToString();

            if (table.ContainsValue(previousSecCode + ' ' + ch.ToString()))
                return previousSecCode + ' ' + ch.ToString();

            return ((ulong)SpecialSeqCodes.NotFound).ToString();
        }

        public string GetSequence(ulong code)
        {
            return this.table[code];
        }

        public byte[] GetByte(string seq)
        {
            var strMass = seq.Split(' ');
            return strMass.Select(st => Convert.ToByte(st)).ToArray();
        }

        public bool ContainsKey(ulong code)
        {
            if (this.table.ContainsKey(code))
                return true;
            return false;
        }

        public void UpdateTable(int bitLenght)
        {
            if (bitLenght == LzwAlgoParams.Default.MaxCodeBitCount)
            {
                this.InitTable();
            }
        }
    }
}
