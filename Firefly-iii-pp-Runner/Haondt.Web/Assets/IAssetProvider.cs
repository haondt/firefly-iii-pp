using DotNext;

namespace Haondt.Web.Assets
{
    public interface IAssetProvider
    {
        public Task<Result<byte[]>> GetAssetAsync(string assetPath);

    }
}