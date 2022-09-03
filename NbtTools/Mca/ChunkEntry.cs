using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtTools.Mca
{
    public class ChunkEntry
    {
        /// <summary>
        /// ID of the chunk.
        /// It is equal to its header offset, and to  (4 * ((x & 31) + (z & 31) * 32)).
        /// </summary>
        public long Id { get; private set; }
        public int Length { get; set; }
        public int CompressionType { get; set; }
        public byte[] Data { get; set; }

        public ChunkEntry(long id)
        {
            Id = id;
        }
    }
}
