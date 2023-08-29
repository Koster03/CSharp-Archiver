using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compress.Core
{
    public class LzwStreamWriter : Stream
    {
        public LzwStreamWriter(Stream inner)
        {
            this.inner = inner;
            this.packer = new LzwPacker();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // todo: implement
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            this.inner.Flush();
        }

        public override void Close()
        {
            var packed = this.packer.GetLastSegment();
            if (packed.Length > 0)
                inner.Write(packed, 0, packed.Length);

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
            get { return false; }
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
                throw new NotSupportedException();
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
        private ICryptoPacker packer;
    }
}
