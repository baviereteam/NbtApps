using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
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
                        sb
                            .Append($"[data-item='minecraft:{infos[1]}']{{")
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
