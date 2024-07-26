using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McMerchants.Tasks
{
    public class TextureAtlasToCssConverter : IStartupTask
    {
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment Environment;
        private readonly ILogger<TextureAtlasToCssConverter> Logger;
        private const string WEBROOT_IMG_PATH = "img/atlas.png";
        private const string WEBROOT_CSS_PATH = "css/atlas.css";
        private readonly char[] SPLIT_TABLE = new char[] { '/', '\t' };
        private readonly Dictionary<string, string> Substitutions = new Dictionary<string, string>()
        {
            { "quartz_block_side", "quartz_block" },
            { "compass_00", "compass" },
            { "clock_32", "clock" },
            { "crossbow_standby", "crossbow" },
            { "pumpkin_side", "pumpkin" },
            { "melon_side", "melon" },

            // Glass panes use the regular glass texture (because their texture is just a glass "bar", from the side / top)
            { "glass_pane_top", "glass" },
            { "black_stained_glass_pane_top",       "black_stained_glass" },
            { "blue_stained_glass_pane_top",        "blue_stained_glass" },
            { "brown_stained_glass_pane_top",       "brown_stained_glass" },
            { "cyan_stained_glass_pane_top",        "cyan_stained_glass" },
            { "gray_stained_glass_pane_top",        "gray_stained_glass" },
            { "green_stained_glass_pane_top",       "green_stained_glass" },
            { "light_blue_stained_glass_pane_top",  "light_blue_stained_glass" },
            { "light_gray_stained_glass_pane_top",  "light_gray_stained_glass" },
            { "lime_stained_glass_pane_top",        "lime_stained_glass" },
            { "magenta_stained_glass_pane_top",     "magenta_stained_glass" },
            { "orange_stained_glass_pane_top",      "orange_stained_glass" },
            { "pink_stained_glass_pane_top",        "pink_stained_glass" },
            { "purple_stained_glass_pane_top",      "purple_stained_glass" },
            { "red_stained_glass_pane_top",         "red_stained_glass" },
            { "white_stained_glass_pane_top",       "white_stained_glass" },
            { "yellow_stained_glass_pane_top",      "yellow_stained_glass" },
        };

        public TextureAtlasToCssConverter(IConfiguration configuration, IWebHostEnvironment environment, ILogger<TextureAtlasToCssConverter> logger) 
        {
            Configuration = configuration;
            Environment = environment;
            Logger = logger;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // don't do this if the files already exist
            if (!NeedsToRun())
            {
                Logger.LogInformation("Atlas files found.");
                return Task.CompletedTask;
            }

            try
            {
                Logger.LogInformation("Atlas files not found, generating...");
                var css = GenerateCSS();
                WriteCSS(css);
                CopyImage();

                Logger.LogInformation("Atlas files generated.");
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                throw new Exception("Cannot generate the texture atlas", e);
            }
        }

        private bool NeedsToRun()
        {
            return !(
                File.Exists(Path.Combine(Environment.WebRootPath, WEBROOT_CSS_PATH))
                && File.Exists(Path.Combine(Environment.WebRootPath, WEBROOT_IMG_PATH))
            );
        }
        private string GenerateCSS()
        {
            var descriptorPath = Configuration["TextureAtlasPaths:Descriptor"];
            string[] entries = File.ReadAllLines(descriptorPath);
            return ParseDescriptorEntries(entries);
        }

        private string ParseDescriptorEntries(string[] entries)
        {
            var sb = new StringBuilder(".sprite{height:16px;width:16px;background-image:url('/img/atlas.png');}");

            foreach (string entry in entries)
            {
                /*
                 * minecraft:block/acacia_door_bottom	x=96	y=64	w=16	h=16
                 * 0: minecraft:block
                 * 1: acacia_door_bottom
                 * 2: x=96
                 * 3: y=64
                 * 4: w=16
                 * 5: h=16
                 */
                var infos = entry.Split(SPLIT_TABLE);

                switch (infos[0])
                {
                    case "minecraft:block":
                    case "minecraft:item":
                        var name = Substitutions.ContainsKey(infos[1]) ? Substitutions[infos[1]] : infos[1];

                        sb
                            .Append($"[data-item='minecraft:{name}']{{")
                            .Append($"background-position: -{infos[2].Substring(2)}px -{infos[3].Substring(2)}px;")
                            .Append("}");
                        break;

                    default:
                        // not a block or an item, so it's not needed
                        continue;
                }
            }

            return sb.ToString();
        }

        private void WriteCSS(string css)
        {
            File.WriteAllText(Path.Combine(Environment.WebRootPath, WEBROOT_CSS_PATH), css);
        }

        private void CopyImage()
        {
            File.Copy(Configuration["TextureAtlasPaths:Atlas"], Path.Combine(Environment.WebRootPath, WEBROOT_IMG_PATH), true);
        }
    }
}
