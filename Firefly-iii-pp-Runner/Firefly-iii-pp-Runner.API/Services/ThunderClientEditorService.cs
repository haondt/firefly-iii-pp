using Firefly_iii_pp_Runner.API.Converters;
using Firefly_iii_pp_Runner.API.Models.ThunderClient;
using Firefly_iii_pp_Runner.API.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Firefly_iii_pp_Runner.API.Services
{
    public class ThunderClientEditorService
    {
        private readonly ThunderClientEditorSettings _settings;
        private JsonSerializerSettings _serializerSettings;

        public ThunderClientEditorService(IOptions<ThunderClientEditorSettings> options)
        {
            _settings = options.Value;
            if (!Directory.Exists(_settings.Path))
                throw new Exception($"Path does not exist: {_settings.Path}");

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new ItemConverter()
                },
                MissingMemberHandling = MissingMemberHandling.Error,
            };
        }

        public void ConvertPostmanJson(string json)
        {
            var pm = JsonConvert.DeserializeObject<Models.Postman.Collection>(json, _serializerSettings);
            ;
        }

        public async Task<int> GetClientCount()
        {
            return (await LoadClients()).Count();
        }

        private async  Task<List<Client>> LoadClients()
        {
            var filePath = Path.Combine(_settings.Path, _settings.ClientFile);
            if (!File.Exists(filePath))
            {
                throw new Exception($"File does not exist: {filePath}");
            }
            
            using (var reader = new StreamReader(filePath))
            {
                return JsonConvert.DeserializeObject<List<Client>>(await reader.ReadToEndAsync(), _serializerSettings);
            }
        }
        private async  Task<List<Collection>> LoadCollections()
        {
            var filePath = Path.Combine(_settings.Path, _settings.CollectionFile);
            if (!File.Exists(filePath))
            {
                throw new Exception($"File does not exist: {filePath}");
            }
            
            using (var reader = new StreamReader(filePath))
            {
                return JsonConvert.DeserializeObject<List<Collection>>(await reader.ReadToEndAsync(), _serializerSettings);
            }
        }
    }
}
