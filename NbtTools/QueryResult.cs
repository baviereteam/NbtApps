using NbtTools.Geography;
using System.Collections.Generic;

namespace NbtTools
{
    public class QueryResult<T>
    {
        private readonly ICollection<T> data;
        private readonly ICollection<Chunk> unreadableChunks;

        public ICollection<T> Result { get => data; }
        public ICollection<Chunk> UnreadableChunks { get => unreadableChunks; }

        /// <summary>
        /// Indicates whether the query was able to successfully collect data from every required chunk.
        /// </summary>
        public bool IsComplete => UnreadableChunks.Count == 0;

        public QueryResult()
        {
            data = new List<T>();
            unreadableChunks = new List<Chunk>();
        }
        public QueryResult(ICollection<T> data, ICollection<Chunk> unreadableChunks)
        {
            this.data = data;
            this.unreadableChunks = unreadableChunks;
        }

        public void AddRange(QueryResult<T> source)
        {
            foreach(var item in source.Result)
            {
                data.Add(item);
            }
            foreach(var chunk in source.UnreadableChunks)
            {
                unreadableChunks.Add(chunk);
            }
        }
    }
}
