using System.Collections.Generic;

namespace NbtTools.Items
{
    public class ItemIconProvider
    {
        // { "a", "b" }
        private static IDictionary<string, string> specificIcons = new Dictionary<string, string>()
        {
            
        };

        /// <summary>
        /// Gets the name of the icon for this item in the item/ directory of the default Minecraft textures.
        /// </summary>
        /// <param name="item">An item ID, starting with the minecraft: prefix or not.</param>
        /// <returns></returns>
        public static string GetIconFor(string item)
        {
            if (specificIcons.ContainsKey(item))
            {
                return specificIcons[item];
            }
            else
            {
                return item.Replace("minecraft:", "") + ".png";
            }
        }
    }
}
