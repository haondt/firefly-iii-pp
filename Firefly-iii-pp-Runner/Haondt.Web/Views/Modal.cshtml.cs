using Haondt.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Haondt.Web.Views
{
    public class ModalModel : IPageModel
    {
        public required PageEntry Content { get; set; }
        public bool AllowClickOut { get; set; } = true;
    }
}
