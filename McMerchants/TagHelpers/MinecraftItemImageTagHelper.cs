using Microsoft.AspNetCore.Razor.TagHelpers;
using NbtTools.Items;

namespace McMerchants.TagHelpers
{
    [HtmlTargetElement("minecraft-item-image")]
    public class MinecraftItemImageTagHelper : TagHelper
    {
        public string For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var texture = ItemIconProvider.GetIconFor(For);

            output.TagName = "img";
            output.Attributes.SetAttribute("src", $"/textures/{texture}");
        }
    }
}
