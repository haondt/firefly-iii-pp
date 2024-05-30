using DotNext;
using DotNext.Threading;
using Haondt.Web.Assets;
using System.Reflection;
using System.Text;

namespace Haondt.Web.Styles
{
    public class ManifestAssetStylesSource(Assembly assembly, string stylesPath) : IStylesSource
    {
        private readonly Task<string> _stylesTask = CreateStylesTask(assembly, stylesPath);
        private static async Task<string> CreateStylesTask(Assembly assembly, string stylesPath)
        {
            var assetSource = new ManifestAssetSource(assembly);
            var asset = await assetSource.GetAssetAsync(stylesPath);
            return Encoding.Default.GetString(asset.Value);
        }

        public Task<string> GetStylesAsync() => _stylesTask;
    }
}
