using Firefly_iii_pp_Runner.API.Models.Postman.Enums;

namespace Firefly_iii_pp_Runner.API.Models.Postman
{
    public class Event
    {
        public EventTypeEnum Listen { get; set; }
        public Script Script { get; set; }
    }

    public class Script
    {
        public ScriptTypeEnum Type { get; set; }
        public List<string> Exec { get; set; }
        public string Id { get; set; }
    }
}
