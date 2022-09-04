using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace McMerchants
{
    public class MinecraftIdLocalizer : IStringLocalizer
    {
        private IDictionary<string, LocalizedString> cache = new Dictionary<string, LocalizedString>();

        LocalizedString IStringLocalizer.this[string name] => LocalizeId(name);

        LocalizedString IStringLocalizer.this[string name, params object[] arguments] => throw new NotImplementedException();

        IEnumerable<LocalizedString> IStringLocalizer.GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        private LocalizedString LocalizeId(string id)
        {
            if (cache.ContainsKey(id))
            {
                return cache[id];
            }

            var translation = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                id
                    .Replace("minecraft:", "")
                    .Replace("_", " ")
            );

            var result = new LocalizedString(id, translation);
            cache.Add(id, result);
            return result;
        }
    }
}
