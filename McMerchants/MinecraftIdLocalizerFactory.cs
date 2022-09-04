using Microsoft.Extensions.Localization;
using System;

namespace McMerchants
{
    public class MinecraftIdLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            return new MinecraftIdLocalizer();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new MinecraftIdLocalizer();
        }
    }
}
