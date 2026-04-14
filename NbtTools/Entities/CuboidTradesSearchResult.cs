using NbtTools.Entities.Trading;
using NbtTools.Geography;
using NbtTools.Items;
using System.Collections.Generic;

namespace NbtTools.Entities
{
    public class CuboidTradesSearchResult
    {
        public IDictionary<Searchable, ICollection<Trade>> Results { get; set; }

        public ICollection<Chunk> UnreadableChunks { get; internal set; }

        /// <summary>
        /// Indicates whether the query was able to successfully collect data from every required chunk.
        /// </summary>
        public bool IsComplete => UnreadableChunks.Count == 0;

        public CuboidTradesSearchResult()
        {
            Results = new Dictionary<Searchable, ICollection<Trade>>();
        }

        public void Add(Searchable searchedItem, Trade trade)
        {
            if (!Results.ContainsKey(searchedItem))
            {
                Results.Add(searchedItem, new List<Trade>());
            }

            Results[searchedItem].Add(trade);
        }
    }
}
