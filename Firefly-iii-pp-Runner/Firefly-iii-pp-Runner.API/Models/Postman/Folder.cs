using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.API.Models.Postman
{
    public class Folder : Item
    {
        public List<Item> Item { get; set; } = new List<Item>();
    }
}
