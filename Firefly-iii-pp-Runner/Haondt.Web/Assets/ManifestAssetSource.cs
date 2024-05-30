using DotNext;
using System.Reflection;

namespace Haondt.Web.Assets
{
    public class ManifestAssetSource(Assembly assembly) : IAssetSource
    {
        private readonly Lazy<HashSet<string>> _paths = new (() => assembly.GetManifestResourceNames().ToHashSet());
        private readonly string? _assemblyPrefix = assembly.GetName().Name;
        private readonly Assembly _assembly = assembly;

        public async Task<Result<byte[]>> GetAssetAsync(string assetPath)
        {
            var fullPath = $"{_assemblyPrefix}.{assetPath}";
            if (!_paths.Value.Contains(fullPath))
                return new(new FileNotFoundException(fullPath));
            using var stream = _assembly.GetManifestResourceStream(fullPath);
            if (stream == null)
                return new(new FileNotFoundException(fullPath));
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return new(memoryStream.ToArray());
        }
    }
}
