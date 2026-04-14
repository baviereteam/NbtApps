using McMerchantsLib.Models.Bom;
using McMerchantsLib.Models.Database;
using DbBom = McMerchantsLib.Models.Database.Bom;
using McMerchantsLib.Stock;
using NbtTools.Database;
using NbtTools.Items;
using McMerchants.Database;
using NbtTools.Geography;

namespace McMerchantsLib.Bom
{
    public class BomService
    {
        private readonly NbtDbContext _nbtContext;
        private readonly McMerchantsDbContext _mcmerchantsContext;
        private readonly StockService _stockService;
        private readonly StoredItemService _storedItemService;

        public BomService(NbtDbContext nbtContext, McMerchantsDbContext mcmContext, StockService stockService, StoredItemService storedItemService)
        {
            _nbtContext = nbtContext;
            _mcmerchantsContext = mcmContext;
            _stockService = stockService;
            _storedItemService = storedItemService;
        }

        /// <summary>
        /// Imports the lines of a CSV BOM into the database.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rawBom"></param>
        /// <param name="replaceId">If provided, the BOM entry with that ID is deleted (case of "retry" import after fixing broken lines)</param>
        /// <returns></returns>
        public BomImportResultDTO ImportBom(string name, string rawBom, int? replaceId = null)
        {
            DbBom? bom = null;
            if (replaceId.HasValue)
            {
                try
                {
                    bom = _mcmerchantsContext.Boms.Single(bom => bom.Id == replaceId.Value);
                    _mcmerchantsContext.Boms.Remove(bom);
                    _mcmerchantsContext.SaveChanges();
                }
                catch
                {
                    // we'll just create a new BOM
                }
            }

            if (bom == null)
            {
                bom = new DbBom()
                {
                    Name = name,
                    Items = new List<BomItem>()
                };
            }
            
            var rejectedLines = new List<Tuple<string, string>>();

            var rawBomLines = rawBom.Split('\n').Skip(1); // ignore header line

            foreach (string rawLine in rawBomLines)
            {
                try
                {
                    // ignore line breaks or trailing lines
                    if (rawLine == "")
                    {
                        continue;
                    }

                    string[] rawData = rawLine.Split(",");

                    // Check the item exists.
                    var item = GetItemFromName(rawData[0].Trim('"'));
                    if (item == null)
                    {
                        throw new InvalidDataException($"{rawData[0]} is not a known item.");
                    }

                    var bomLine = new BomItem()
                    {
	                    ItemName = item.Name,
	                    RequiredQuantity = int.Parse(rawData[1].Trim('"')),
	                    Bom = bom
                    };
                    bom.Items.Add(bomLine);
                }
                catch (Exception e)
                {
                    rejectedLines.Add(new Tuple<string, string>(rawLine, e.Message));
                }
            }

            _mcmerchantsContext.Boms.Add(bom);
            _mcmerchantsContext.SaveChanges();

            return new BomImportResultDTO()
            {
                Bom = bom,
                UnreadableLines = rejectedLines
            };
        }

        /// <summary>
        /// Returns the lines of a given BOM, hydrated with the Searchables matching the items.
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public EnrichedBomDTO GetItemsOf(DbBom bom)
        {
	        var results = new List<EnrichedBomItem>();
	        foreach (var rawItem in bom.Items.OrderBy(item => item.ItemName))
	        {
		        var item = GetItemFromName(rawItem.ItemName);
		        if (item == null)
		        {
			        throw new InvalidDataException($"{rawItem.ItemName} is not a known item.");
		        }

		        results.Add(new EnrichedBomItem(rawItem, item));
	        }

	        return new EnrichedBomDTO()
	        {
		        Items = results,
		        IsComplete = true // there's no stock query here, so it's always complete.
	        };
        }

        public EnrichedBomDTO GetAvailabilityOf(DbBom bom, Cuboid workzone)
        {
            var dto = new EnrichedBomDTO();

            var bomEntries = GetItemsAndSearchablesOf(bom);
            var searchedItems = bomEntries.Keys;

            // search in registered locations
            var searchResults = _stockService.GetStockOf(searchedItems);
            dto.IsComplete = searchResults.IsComplete;

            // search in workzone (if provided)
            CuboidItemsSearchResult? workzoneQuery = null;
            if (workzone != null)
            {
                workzoneQuery = _storedItemService.FindStoredItems(searchedItems, workzone);
                dto.IsComplete &= workzoneQuery.IsComplete;
            }

            foreach (var result in searchResults.Results.Where(result => bomEntries.ContainsKey(result.Key)))
            {
                bomEntries[result.Key].StoredQuantities = result.Value;
                if (workzoneQuery != null && workzoneQuery.Results.ContainsKey(result.Key))
                {
                    var countsForItem = workzoneQuery.Results[result.Key].Values;
                    bomEntries[result.Key].WorkzoneQuantity = countsForItem.Aggregate(0, (sum, current) => sum + current);
                }
            }

            dto.Items = bomEntries.Values;
            return dto;
        }

        private Dictionary<Searchable, EnrichedBomItem> GetItemsAndSearchablesOf(DbBom bom)
        {
            var results = new Dictionary<Searchable, EnrichedBomItem>();
            foreach (var rawItem in bom.Items.OrderBy(item => item.ItemName))
            {
                var searchable = GetItemFromName(rawItem.ItemName);
                if (searchable == null)
                {
                    throw new InvalidDataException($"{rawItem.ItemName} is not a known item.");
                }
                results.Add(searchable, new EnrichedBomItem(rawItem, searchable));
            }

            return results;
        }

        private Item? GetItemFromName(string name)
        {
            return _nbtContext.Items.Single(item => item.Name.ToLower() == name.ToLower());
        }
    }
}
