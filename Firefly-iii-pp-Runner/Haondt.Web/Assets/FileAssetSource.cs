using DotNext;
using Microsoft.Extensions.Options;

namespace Haondt.Web.Assets
{
    public class FileAssetSource(string basePath) : IAssetSource
    {
        public async Task<Result<byte[]>> GetAssetAsync(string assetPath)
        {
            var fullPath = Path.Combine(basePath, assetPath);
            if (File.Exists(fullPath))
                return new(await File.ReadAllBytesAsync(fullPath));
            return new(new FileNotFoundException(assetPath));
        }
    }
}
