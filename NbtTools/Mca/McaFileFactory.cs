using Microsoft.Extensions.Configuration;
using System.IO;

namespace NbtTools.Mca
{
    public class McaFileFactory
    {
        private readonly IConfiguration Configuration;

        public McaFileFactory(IConfiguration config) { 
            Configuration = config;
        }

        private string GetPathForDimension(string dimension)
        {
            return Configuration[$"MapPaths:{dimension}"].ToString();
        }

        private McaFile GetFile(string type, string dimension, string path)
        {
            return new McaFile(
                Path.Combine(GetPathForDimension(dimension), type, path)
            );
        }

        public McaFile GetEntitiesFile(string dimension, string path)
        {
            return GetFile("entities", dimension, path);
        }

        public McaFile GetRegionFile(string dimension, string path) {
            return GetFile("region", dimension, path);
        }
    }
}
