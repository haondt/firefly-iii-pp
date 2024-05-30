namespace Haondt.Web.Styles
{
    public interface IStylesProvider
    {
        public Task<string> GetStylesAsync();
    }
}
