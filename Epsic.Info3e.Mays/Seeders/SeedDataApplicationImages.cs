using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Epsic.Info3e.Mays.Seeders
{
    public static class SeedDataApplicationImages
    {
        public static void SeedImages(IWebHostEnvironment environment)
        {
            var postSeedDirectory = $"{environment.WebRootPath}\\AssetsSeeds\\";
            var postTargetDirectory = $"{environment.WebRootPath}\\Assets\\";
            var avatarSeedDirectory = $"{environment.WebRootPath}\\AvatarsSeeds\\";
            var avatarTargetDirectory = $"{environment.WebRootPath}\\Avatars\\";

            if (Directory.Exists(postSeedDirectory) && !Directory.Exists(postTargetDirectory))
            {
                Directory.Move(postSeedDirectory, postTargetDirectory);
            }

            if (Directory.Exists(avatarSeedDirectory) && !Directory.Exists(avatarTargetDirectory))
            {
                Directory.Move(avatarSeedDirectory, avatarTargetDirectory);
            }
        }
    }
}
