using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Epsic.Info3e.Mays.Seeders
{
    public static class SeedDataApplicationImages
    {
        public static void SeedImages(IWebHostEnvironment environment)
        {
            var seedDirectory = $"{environment.WebRootPath}\\AssetsSeeds\\";
            var targetDirectory = $"{environment.WebRootPath}\\Assets\\";
            // Can't seed
            if (!Directory.Exists(seedDirectory) || Directory.Exists(targetDirectory)) return;

            Directory.Move(seedDirectory, targetDirectory);
        }
    }
}
