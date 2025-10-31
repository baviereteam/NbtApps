using NbtTools.Geography;
using System.Collections.Generic;

namespace NbtTools
{
    public class QueryResult<T>
    {
        private readonly T data;
        private readonly ICollection<Chunk> unreadableChunks;

        public T Result { get => data; }
        public ICollection<Chunk> UnreadableChunks { get => unreadableChunks; }

        /// <summary>
        /// Indicates whether the query was able to successfully collect data from every required chunk.
        /// </summary>
        public bool IsComplete => UnreadableChunks.Count == 0;

        public QueryResult(T data, ICollection<Chunk> unreadableChunks)
        {
            this.data = data;
            this.unreadableChunks = unreadableChunks;
        }
    }
}
