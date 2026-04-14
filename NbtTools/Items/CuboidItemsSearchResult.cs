using NbtTools.Geography;
using System.Collections.Generic;

using QuantitiesByPosition = System.Collections.Generic.IDictionary<NbtTools.Geography.Point, int>;

namespace NbtTools.Items
{
    public class CuboidItemsSearchResult
    {
        public IDictionary<Searchable, QuantitiesByPosition> Results { get; set; }

        public ICollection<Chunk> UnreadableChunks { get; internal set; }

        /// <summary>
        /// Indicates whether the query was able to successfully collect data from every required chunk.
        /// </summary>
        public bool IsComplete => UnreadableChunks.Count == 0;

        public CuboidItemsSearchResult()
        {
            Results = new Dictionary<Searchable, QuantitiesByPosition>();
        }

        public void Add(Point place, Searchable searchedItem, int quantity)
        {
            if (!Results.ContainsKey(searchedItem))
            {
                Results.Add(searchedItem, new Dictionary<Point, int>());
            }

            Results[searchedItem].AddOrIncrement(place, quantity);
        }
    }
}
