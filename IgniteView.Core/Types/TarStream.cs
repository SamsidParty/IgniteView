using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class TarStream : Stream
    {
        public TarEntry InnerEntry;
        public Stream InnerStream;

        public TarStream(TarEntry entry, Stream innerStream)
        {
            InnerEntry = entry;
            InnerStream = innerStream;
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => InnerEntry.Length;
        public override long Position { get; set; }
        public override void Flush() { }
        public override void SetLength(long value) { }
        public override void Write(byte[] buffer, int offset, int count) { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = (int)Math.Clamp(count, 0, InnerEntry.Length - Position); // Prevent reading past the region of the entry
            InnerStream.Seek((int)InnerEntry.DataOffset + (int)Position, SeekOrigin.Begin);
            count = InnerStream.Read(buffer, offset, count);
            Position += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
            {
                Position = offset;
            }
            else if (origin == SeekOrigin.Current) 
            {
                 Position += offset;
            }
            else // SeekOrigin.End
            {
                Position = Length - offset;
            }

            return Position;
        }

        public override void Close()
        {
            InnerStream.Close();
        }

        protected override void Dispose(bool disposing)
        {
            InnerStream.Dispose();
        }
    }
}
