using Microsoft.Extensions.Options;

namespace Haondt.Web.Services
{
    public class AssetProvider(IWebHostEnvironment env, IOptions<AssetSettings> options)
    {
        public bool TryGetAsset(string path, out byte[] content)
        {
            var pathsToCheck = new List<string>
            {
                Path.Combine(env.ContentRootPath, "wwwroot", path),
            };

            if (!string.IsNullOrEmpty(options.Value.BasePath))
                pathsToCheck.Add(Path.Combine(options.Value.BasePath, path));


            foreach (var pathToCheck in pathsToCheck)
            {
                if (File.Exists(pathToCheck))
                {
                    content = File.ReadAllBytes(pathToCheck);
                    return true;
                }
            }

            content = [];
            return false;
        }
    }
}
