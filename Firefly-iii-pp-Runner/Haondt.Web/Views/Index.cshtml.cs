using Haondt.Web.Pages;

namespace Haondt.Web.Views
{
    public class IndexModel : IPageModel
    {
        public required string Title { get; set; }
        public required PageEntry NavigationBar { get; set; }
        public required PageEntry Content { get; set; }
    }
}
