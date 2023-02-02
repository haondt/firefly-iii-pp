namespace Firefly_iii_pp_Runner.API.Models.Postman
{
    public class Event
    {
        public string Listen { get; set; }
        public Script Script { get; set; }
    }

    public class Script
    {
        public string Type { get; set; }
        public List<string> Exec { get; set; }
        public string Id { get; set; }
    }
}
