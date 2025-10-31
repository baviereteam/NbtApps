using System;

namespace NbtTools.Nbt
{
    public class UnreadableChunkException : Exception
    {
        public UnreadableChunkException(string message) : base(message) { }
        public UnreadableChunkException(string message, Exception innerException): base(message, innerException) { }

        public long ChunkId;
    }
}
