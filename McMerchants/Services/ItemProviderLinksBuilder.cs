using Humanizer;
using McMerchants.Models.Database;
using Microsoft.Extensions.Configuration;

namespace McMerchants.Services
{
    public class ItemProviderLinksBuilder
    {
        public bool CanGenerateMapLinks { get; private set; }
        public string MapLinkTitle { get; private set; }
        public string CustomLinkTitle { get; private set; }

        private readonly string webMapPattern = null;

        public ItemProviderLinksBuilder(IConfiguration configuration)
        {
            MapLinkTitle = configuration["Options:WebmapLinkTitle"]?.ToString() ?? "Map";
            CustomLinkTitle = configuration["Options:CustomLinkTitle"]?.ToString() ?? "Web";

            var pattern = configuration["Options:WebmapUrlPattern"];
            CanGenerateMapLinks = !string.IsNullOrWhiteSpace(pattern);

            if (CanGenerateMapLinks)
            {
                // Must match the order used in GetWebMapUrl()
                webMapPattern = pattern
                    .Replace("{DIMENSION}", "{0}")
                    .Replace("{X}", "{1}")
                    .Replace("{Y}", "{2}")
                    .Replace("{Z}", "{3}");
            }
        }

        public string GetMapUrlFor(ItemProviderRegion place)
        {
            return webMapPattern.FormatWith(
                place.Dimension,
                place.StartX + (place.EndX - place.StartX) / 2,
                place.StartY + (place.EndY - place.StartY) / 2,
                place.StartZ + (place.EndZ - place.StartZ) / 2
            );
        }
    }
}
