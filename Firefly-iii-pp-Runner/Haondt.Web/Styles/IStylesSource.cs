namespace Haondt.Web.Styles
{
    public interface IStylesSource
    {
        public int Priority { get; }
        public Task<string> GetStylesAsync();
    }
}
