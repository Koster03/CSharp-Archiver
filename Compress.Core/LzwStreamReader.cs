using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compress.Core
{
    public class LzwStreamReader : Stream
    {
        public LzwStreamReader(Stream inner)
        {
            this.inner = inner;
            this.unpacker = new LzwUnpacker();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //return 0;
            int e;
            if (cache == null && !check)
            {
                e = inner.Read(buffer, offset, count);

                var unpuckBuffer = unpacker.Unpack(buffer, offset, e);

                if (e == unpuckBuffer.Length)
                {
                    for (int i = 0; i < unpuckBuffer.Length; i++)
                    {
                        buffer[i] = unpuckBuffer[i];
                    }
                }
                else if (e < unpuckBuffer.Length)
                {
                    for (int i = 0; i < e; i++)
                    {
                        buffer[i] = unpuckBuffer[i];
                    }

                    cache = new byte[unpuckBuffer.Length - e];

                    int j = 0;

                    for (int i = e; i < unpuckBuffer.Length; i++)
                    {
                        cache[j] = unpuckBuffer[i];
                        j++;
                    }
                }
            }
            else
            {
                if (cache == null)
                    e = 0;
                else
                {
                    e = cache.Length;
                    check = true;

                    for (int i = 0; i < cache.Length; i++)
                    {
                        buffer[i] = cache[i];
                    }

                    cache = null;
                }
            }

            
            //Stream tmpStream;
            //tmpStream.Write(unpuckBuffer, offset, unpuckBuffer.Length);
            //inner.Read(buffer, offset, unpuckBuffer.Length);
            //inner.WriteAsync(t, offset, t.Length);
            return  e;
            // todo: implement
        }

        public override void Flush()
        {
            this.inner.Flush();
        }

        public override void Close()
        {
            this.inner.Close();
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            inner.Dispose();
            base.Dispose(disposing);
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get
            {
                return inner.Length;
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        private Stream inner;
        private byte[] cache;
        private ICryptoUnpacker unpacker;
        private bool check = false;
    }
}
