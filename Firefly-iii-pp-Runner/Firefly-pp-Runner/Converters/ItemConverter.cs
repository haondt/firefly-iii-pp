using Firefly_iii_pp_Runner.Models.Postman;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Firefly_iii_pp_Runner.Converters
{
    public class ItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Item).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            Item obj = jObject.ContainsKey("request")
                ? new Request() : new Folder();
            serializer.Populate(jObject.CreateReader(), obj);
            return obj;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
