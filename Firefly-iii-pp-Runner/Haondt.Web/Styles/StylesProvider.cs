using Microsoft.Extensions.Options;

namespace Haondt.Web.Styles
{
    public class StylesProvider(IEnumerable<IStylesSource> stylesSources) : IStylesProvider
    {
        private readonly Task<string> _stylesTask = CreateStylesTask(stylesSources);
        private static async Task<string> CreateStylesTask(IEnumerable<IStylesSource> stylesSources)
        {
            var styles = await Task.WhenAll(stylesSources
                .OrderBy(s => s.Priority)
                .Select(s => s.GetStylesAsync()));
            return string.Join('\n', styles);
        }
        public Task<string> GetStylesAsync() => _stylesTask;
    }
}
