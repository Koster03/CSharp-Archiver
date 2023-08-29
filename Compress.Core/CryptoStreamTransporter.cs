using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Compress.Core
{
    public class CryptoStreamTransporter : ICryptoStreamTransporter
    {
        public event EventHandler<DataProcessedEventArgs> DataProcessed;

        private byte[] CopyToBuffer(Stream input)
        {

            byte[] buff = new byte[size];

            input.Read(buff, 0, buff.Length);

            return buff;
        }

        private void packFirstSegment(Stream input, Stream output, long length, ICryptoPacker packer)
        {
            byte[] buffer = CopyToBuffer(input);

            byte[] t = packer.Pack(buffer, 0, (int)length);

            output.Write(t, 0, t.Length);
        }

        private void packLastSegment(Stream output, ICryptoPacker packer)
        {
            byte[] packed = packer.GetLastSegment();
            if (packed.Length > 0)
                output.Write(packed, 0, packed.Length);
        }

        public void Pack(Stream input, Stream output, long length, ICryptoPacker packer)
        {
            Console.Write("Packing... ");
            var progress = new ProgressBar();

            var all = input.Length / size;
            if (input.Length % size != 0)
                all++;

            if (input.Length <= size)
            {
                packFirstSegment(input, output, length, packer);
                packLastSegment(output, packer);
                progress.Report((double)1 / 1);
                Thread.Sleep(125);
            }
            else
            {
                while (repeat * size <= input.Length)
                {
                    var prevPos = input.Position;

                    packFirstSegment(input, output, size, packer);

                    var startFile = input.Position;

                    DataProcessed?.Invoke(this, new DataProcessedEventArgs
                    {
                        Total = length,
                        TotalProcessed = input.Position - startFile,
                        ProcessedNow = input.Position - prevPos,
                    });

                    progress.Report((double)repeat / all);

                    repeat++;
                }

                int count = (int)input.Length - (repeat - 1) * size;

                if (count != 0)
                {
                    packFirstSegment(input, output, count, packer);
                    packLastSegment(output, packer);
                }

                progress.Report((double)repeat / all);
                Thread.Sleep(125);
            }
           // output.di
            //output.Close();
            Console.WriteLine("Done.");
            repeat = 1;
        }
        private void unpackSegment(Stream input, Stream output, long length, ICryptoUnpacker unpacker)
        {
            byte[] buffer = CopyToBuffer(input);
            byte[] unpuckBuffer = unpacker.Unpack(buffer, 0, (int)length);
            output.Write(unpuckBuffer, 0, unpuckBuffer.Length);
        }

        public void Unpack(Stream input, Stream output, long length, ICryptoUnpacker unpacker)
        {
            //Console.Write("Unpacking... ");
            //var progress = new ProgressBar();

           // DataProcessed?.Invoke(this, new DataProcessedEventArgs());

            var all = input.Length / size;
            if (input.Length % size != 0)
                all++;

            long startFile = input.Position;

            if (input.Length <= size)
            {
                var prevPos = input.Position;
                unpackSegment(input, output, length, unpacker);

                DataProcessed?.Invoke(this, new DataProcessedEventArgs
                {
                    Total = length,
                    TotalProcessed = input.Position - startFile,
                    ProcessedNow = input.Position - prevPos,
                });

                //progress.Report((double)repeat / all);
                //Thread.Sleep(125);
            }
            else
            {
                while (size * repeat <= input.Length)
                {
                    unpackSegment(input, output, size, unpacker);
                    //progress.Report((double)repeat / all);
                    repeat++;
                }

                int count = (int)input.Length - (repeat - 1) * size;

                if (count != 0)
                {
                    unpackSegment(input, output, count, unpacker);
                    //progress.Report((double)repeat / all);
                    //Thread.Sleep(125);
                }
            }
        }

        private static int size = 32768;
        private static int repeat = 1;

        public int Repeat { get => repeat; }
    }
}
