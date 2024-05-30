
using Microsoft.Extensions.Options;

namespace Haondt.Web.Styles
{
    public class ColorsStylesSource(IOptions<ColorSettings> colorOptions) : IStylesSource
    {
        private readonly ColorSettings _colorSettings = colorOptions.Value;

        public Task<string> GetStylesAsync()
        {
            var colorsCss = ":root {\n";
            colorsCss += string.Join('\n', new List<string>
            {
                $"    --color-dark-bg: {_colorSettings.DarkBackground};",
                $"    --color-bright-bg: {_colorSettings.BrightBackground};",
                $"    --color-dark-fg: {_colorSettings.DarkForeground};",
                $"    --color-bright-fg: {_colorSettings.BrightForeground};",
                $"    --color-accent: {_colorSettings.Accent};",
                $"    --color-negative: {_colorSettings.Negative};",
                $"    --color-positive: {_colorSettings.Positive};",
            });
            colorsCss += "\n}\n";
            return Task.FromResult(colorsCss);
        }
    }
}
