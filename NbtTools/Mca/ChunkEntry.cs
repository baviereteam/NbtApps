using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtTools.Mca
{
    public class ChunkEntry
    {
        public int Id { get; private set; }
        public int Length { get; set; }
        public int CompressionType { get; set; }
        public byte[] Data { get; set; }

        public ChunkEntry(int id)
        {
            Id = id;
        }
    }
}
