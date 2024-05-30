namespace Haondt.Web.Styles
{
    public interface IStylesSource
    {
        public Task<string> GetStylesAsync();
    }
}
