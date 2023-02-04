using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.Models.Postman
{
    public class Item
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public List<Event> Event { get; set; } = new List<Event>();

    }
}
